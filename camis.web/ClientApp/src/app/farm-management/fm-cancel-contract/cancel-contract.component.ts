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
import {ContractCancellationFormComponent} from "./contract-cancellation-form/contract-cancellation-form.component";

@Component({
  selector: "app-contract-cancellation",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, DocumentDetailComponent, ContractCancellationFormComponent],
  templateUrl: "cancel-contract.component.html"
})

export class CancelContractComponent implements OnInit {

  loading: boolean = false;
  farmId: string;
  workflowId: string;
  data: any = null;
  cancellationData: any = null;
  public loginRole = 0;
  workItem: any = null;
  workFlowItem: any = null;
  userWorkItem: any = null;
  farmLands: any[]=[];
  state= [{ id: 1, name: 'Filing' }, { id: 2, name: 'Request Sent' }, { id: 3, name: 'Task Rejected' }, {
    id: -2,
    name: 'Task Approved'
  }, { id: -3, name: 'Task Cancelled' }];
  rightType = [
    { id: 1, name: 'Lease From State' },
    { id: 2, name: 'Lease From Private' },
    { id: 3, name: 'Private' },
    { id: 4, name: 'Contract Farming' },
    { id: 5, name: 'Sub-lease' },
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
        this.cancellationData = resp.data;
        if (this.workflowId) {
          this.workItem = resp;
          this.wfApi.getWorkflow(this.workflowId).subscribe(workflow => {
            this.keyCase.camelCase(workflow);
            this.workFlowItem = workflow;
            this.api.getUserWorkItem(this.workflowId).subscribe(uwi => {
              this.keyCase.camelCase(uwi);
              this.userWorkItem = uwi;
              this.api.getFarm(this.cancellationData.id).subscribe(fr => {
                this.data = fr;
                this.api.getFarmLands(this.cancellationData.id).subscribe(fl => {
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


  async approveContractCancellationRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this task?')) {
      return;
    }

    const message = await dialog.prompt('Enter a note (optional):');
    dialog.loading();

    this.api.approveContractCancellation(this.workflowId, message).subscribe(res => {
      dialog.success('Contract Cancellation task has been approved successfully.');
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }, dialog.error);
  }

  async rejectContractCancellationRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reject this task?')) {
      return;
    }
    let msgQue = '';
    const message = await dialog.prompt("Enter a rejection note for Farm Data Registrar:");
    if (message === null)
      return;

    dialog.loading();

    this.api.rejectContractCancellation(this.workflowId, message).subscribe(res => {
      dialog.success('Contract Cancellation task has been rejected successfully.');
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }, dialog.error);

  }

  async cancelContractCancellationRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this request?')) {
      return;
    }
    const message = await dialog.prompt("Enter a task cancellation reason");
    if (message === null) {
      return;
    }
    dialog.loading();
    this.api.cancelContractCancellation(this.workflowId, message).subscribe(res => {
      dialog.success('Contract Cancellation task has been cancelled successfully.');
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }, dialog.error);
  }

  getRightType(id: number): string {
    const type = this.rightType.find(t => t.id === id);
    return type ? type.name : null;
  }
  getLandR(id: string, sIndex:number) {
    return this.farmLands.find(t => t.landId === id && t.splitIndex===sIndex);
  }
  getArea(a:number) {
    return Math.round(a / 10) / 1000 + ' ha';
  }
  public CancelContBtnClick(): void {
    this.showCancellationForm = true;
  }
  public closeForm(close: boolean): void {
    if (close) {
      this.showCancellationForm=false;
    }
  }
  public reloadPage(rel:boolean){
    if(rel){
      window.location.reload();
    }
  }
}
