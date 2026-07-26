import {Component, OnInit, ViewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";
import {FarmApiService} from "../../_services/farm-api.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {WorkflowApiService} from "../../_services/workflow-api.service";
import {LandDataService} from "../../_services/land-data.service";
import dialog from "../../_shared/dialog";
import {DocumentDetailComponent} from "../../_shared/document/document-detail/document-detail.component";
import {ContractWarningFormComponent} from "./contract-warning-form/contract-warning-form.component";

@Component({
  selector: "app-contract-warning",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, DocumentDetailComponent, ContractWarningFormComponent],
  templateUrl: "warning-contract.component.html"
})

export class WarningContractComponent implements OnInit {

  loading: boolean = false;
  farmId: string;
  workflowId: string;
  data: any = null;
  warningData: any = null;
  public loginRole = 0;
  workItem: any = null;
  workFlowItem: any = null;
  userWorkItem: any = null;
  farmLands: any[] = [];
  state = [{id: 1, name: 'Filing'}, {id: 2, name: 'Request Sent'}, {id: 3, name: 'Task Rejected'}, {
    id: -2,
    name: 'Task Approved'
  }, {id: -3, name: 'Task Cancelled'}];
  rightType = [
    {id: 1, name: 'Lease From State'},
    {id: 2, name: 'Lease From Private'},
    {id: 3, name: 'Private'},
    {id: 4, name: 'Contract Farming'},
    {id: 5, name: 'Sub-lease'},
  ];
  showCancellationForm = false;

  constructor(
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    public keyCase: ObjectKeyCasingService,
    private wfApi: WorkflowApiService,
    private landService: LandDataService) {
    this.loginRole = parseInt(localStorage.getItem('role'), 10);
  }

  ngOnInit() {
    dialog.loading();
    this.loading = true;
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'];
      this.wfApi.getLastWorkItem(this.workflowId).subscribe(resp => {
        this.keyCase.camelCase(resp);
        this.warningData = resp.data;
        if (this.workflowId) {
          this.workItem = resp;
          this.wfApi.getWorkflow(this.workflowId).subscribe(workflow => {
            this.keyCase.camelCase(workflow);
            this.workFlowItem = workflow;
            this.api.getUserWorkItem(this.workflowId).subscribe(uwi => {
              this.keyCase.camelCase(uwi);
              this.userWorkItem = uwi;
              this.api.getFarm(this.warningData.farmId).subscribe(fr => {
                this.data = fr;
                this.api.getFarmLands(this.warningData.farmId).subscribe(fl => {
                  this.keyCase.camelCase(fl);
                  this.farmLands = fl;
                })
                this.loading = false;
                dialog.close()
              }, dialog.error)
            }, dialog.error)
          }, dialog.error);
        }
      }, dialog.error);
    }, dialog.error);

  }

  getTaskState(id: number) {
    const type = this.state.find(t => t.id === id);
    return type ? type.name : null;
  }


  async approveContractWarningRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this task?')) {
      return;
    }

    const message = await dialog.prompt('Enter a note (optional):');
    dialog.loading();

    this.api.approveContractWarning(this.workflowId, message).toPromise()
      .then(() => dialog.success('Contract warning registration task has been approved successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        dialog.close();
        return dialog.error(err)
      });
  }

  async rejectContractWarningRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reject this task?')) {
      return;
    }
    const message = await dialog.prompt("Enter a rejection note for Farm Data Registrar:");
    if (message === "") {
      await dialog.error("Please enter note")
      return;
    } else {
      dialog.loading();

      this.api.rejectContractWarning(this.workflowId, message).toPromise()
        .then(() => dialog.success('Contract Warning task has been rejected successfully.'))
        .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
        .catch(err => {
          dialog.close();
          return dialog.error(err)
        });
    }

  }

  async cancelContractWarningRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this request?')) {
      return;
    }
    const message = await dialog.prompt("Enter a task cancellation reason");
    if (message === "") {
      await dialog.error("Please enter note")
      return;
    } else {
      dialog.loading();
      this.api.cancelContractWarning(this.workflowId, message).toPromise()
        .then(() => dialog.success('Contract warning registration task has been cancelled successfully.'))
        .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
        .catch(err => {
          dialog.close();
          return dialog.error(err)
        });
    }
  }

  getRightType(id: number): string {
    const type = this.rightType.find(t => t.id === id);
    return type ? type.name : null;
  }

  getLandR(id: string, sIndex: number) {
    return this.farmLands.find(t => t.landId === id && t.splitIndex === sIndex);
  }

  getArea(a: number) {
    return Math.round(a / 10) / 1000 + ' ha';
  }

  public CancelContBtnClick(): void {
    this.showCancellationForm = true;
  }

  public closeForm(close: boolean): void {
    if (close) {
      this.showCancellationForm = false;
    }
  }

  public reloadPage(rel: boolean) {
    if (rel) {
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }
  }
}
