import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {DocumentDetailComponent} from "../../_shared/document/document-detail/document-detail.component";
import {FarmApiService} from "../../_services/farm-api.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {WorkflowApiService} from "../../_services/workflow-api.service";
import {LandDataService} from "../../_services/land-data.service";
import dialog from "../../_shared/dialog";
import {
  ContractCancellationFormComponent
} from "../fm-cancel-contract/contract-cancellation-form/contract-cancellation-form.component";
import {ContractRenewalFormComponent} from "./contract-renewal-form/contract-renewal-form.component";

@Component({
  selector: "app-renew-contract",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, DocumentDetailComponent, ContractRenewalFormComponent],
  templateUrl: "renew-contract.component.html"
})

export class RenewContractComponent implements OnInit {
  loading: boolean = false;
  farmId: string;
  workflowId: string;
  data: any = null;
  renewalData: any = null;
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
  showRenewalForm = false;
  renewalReasonList:any[]=[];
  constructor( private api: FarmApiService,
               private router: Router,
               private ar: ActivatedRoute,
               public keyCase: ObjectKeyCasingService,
               private wfApi: WorkflowApiService,
               private landService: LandDataService) {
    this.loginRole = parseInt(localStorage.getItem('role'), 10);
    this.api.getAllModificationReasonList().subscribe(res=>{
      this.keyCase.camelCase(res);
      this.renewalReasonList=res;
    })

  }

  ngOnInit() {
    dialog.loading();
    this.loading = true;
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'];
      this.wfApi.getLastWorkItem(this.workflowId).subscribe(resp => {
        this.keyCase.camelCase(resp);
        this.renewalData = resp.data;
        if (this.workflowId) {
          this.workItem = resp;
          this.wfApi.getWorkflow(this.workflowId).subscribe(workflow => {
            this.keyCase.camelCase(workflow);
            this.workFlowItem = workflow;
            this.api.getUserWorkItem(this.workflowId).subscribe(uwi => {
              this.keyCase.camelCase(uwi);
              this.userWorkItem = uwi;
              this.api.getFarm(this.renewalData.farmId).subscribe(fr => {
                this.data = fr;
                this.api.getFarmLands(this.renewalData.farmId).subscribe(fl => {
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
  async approveContractRenewalRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this task?')) {
      return;
    }

    const message = await dialog.prompt('Enter a note (optional):');
    dialog.loading();

    this.api.approveContractModification(this.workflowId, message).subscribe(res => {
      dialog.success('Contract Renewal task has been approved successfully.');
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }, dialog.error);
  }
  async rejectContractRenewalRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reject this task?')) {
      return;
    }
    let msgQue = '';
    const message = await dialog.prompt("Enter a rejection note for Farm Data Registrar:");
    if (message === ""){
      await dialog.error("Please enter a note");
      return;
    }
    else{
      dialog.loading();

      this.api.rejectContractModification(this.workflowId, message).subscribe(res => {
        dialog.success('Contract Renewal task has been rejected successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }

  }

  async cancelContractRenewalRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this request?')) {
      return;
    }
    const message = await dialog.prompt("Enter a task cancellation reason");
    if (message === "") {
      await dialog.error("Please enter a note");
      return;
    }else{
      dialog.loading();
      this.api.cancelContractModification(this.workflowId, message).subscribe(res => {
        dialog.success('Contract Renewal task has been cancelled successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }

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
  public renewContBtnClick(): void {
    this.showRenewalForm = true;
  }
  public closeForm(close: boolean): void {
    if (close) {
      this.showRenewalForm=false;
    }
  }
  public reloadPage(rel:boolean){
    if(rel){
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }
  }
}
