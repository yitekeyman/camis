import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from '../../../_shared/dialog';
import {ProjectApiService} from "../../../_services/project-api.service";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {ResetPassComponent} from "../../../admin/userManagement/resetPassword/reset-pass.component";
import {
  ContractCancellationFormComponent
} from "../../fm-cancel-contract/contract-cancellation-form/contract-cancellation-form.component";
import {
  ContractUpdateFormComponent
} from "../../fm-update-contract/contract-update-form/contract-update-form.component";
import {
  ContractWarningFormComponent
} from "../../fm-warning-contract/contract-warning-form/contract-warning-form.component";
import {
  ContractRenewalFormComponent
} from "../../fm-renew-contract/contract-renewal-form/contract-renewal-form.component";

@Component({
  selector: 'app-fc-farm-view',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, ContractCancellationFormComponent, ContractUpdateFormComponent, ContractWarningFormComponent, ContractRenewalFormComponent],
  templateUrl: 'fc-farm-view.component.html'
})
export class FcFarmViewComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;

  plan: any;
  loginRole='0';
  showCancellationForm = false;
  showRenewalForm = false;
  showWarningForm = false;
  showUpdateForm=false;
  landId: string=null;
  workflowId: string=null;
  splitIndex:number=0;
  data=null;
  selectedFarmLand=null;

  constructor (private api: FarmApiService, private projectApi: ProjectApiService, private router: Router, private ar: ActivatedRoute, private keyCase:ObjectKeyCasingService) {
    this.loginRole=localStorage.getItem("role");
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => this.farmId = params['farmId'], dialog.error)
      .add(this.api.getFarm(this.farmId).subscribe(farm => {
        this.keyCase.camelCase(farm);
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.keyCase.camelCase(plan);
          this.plan = plan;
          this.loading = false;
          dialog.close();
        }, dialog.error);
      }, dialog.error));
  }


  newFarmRegistrationStep3(operatorId: string): Promise<boolean> {
    return this.router.navigateByUrl(`farm-management/fc/farm/registration/new?step=3&operatorId=${operatorId}`);
  }

  newFarmModification(farmId: string): Promise<boolean> {
    return this.router.navigateByUrl(`farm-management/fc/farm/${farmId}/modification/new`);
  }

  newUpdatePlan(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`farm-management/fc/plan/${planId}/update`);
  }

  async askDelete(farm: any): Promise<void> {
    const f = await dialog.confirm('Are you sure you want to delete this farm?');
    if (!f) { return; }

    const o = await dialog.confirm('Do you also want to delete this farm\'s owner?');

    const message = await dialog.prompt('Enter a message for the supervisor (optional):');
    if (message === null) {
      await dialog.info('Deletion has been aborted.');
      return
    }


    const req: any = farm;
    if (!o) {
      delete req.operatorId;
      delete req.operator;
    }

    this.loading = true;
    dialog.loading();

    this.api.requestNewFarmDeletion(req, message).subscribe(res => {
      if (res.success) {
        this.router.navigateByUrl(`default/pending-task`).catch(dialog.error);
        return dialog.success('Deletion request has been sent to the supervisor.');
      } else {
        this.loading = false;
        return dialog.error(res.message);
      }
    }, err => {
      this.loading = false;
      return dialog.error(err);
    });
  }
  public CancelContBtnClick(id:any, farmLand:any): void {
    if(id==="cancellation"){
      this.showCancellationForm = true;
    }else if(id==="update"){
      this.showUpdateForm = true;
    }

  }
  public closeForm(close: boolean): void {
    if (close) {
      this.showCancellationForm=false;
      this.showRenewalForm=false;
      this.showWarningForm=false;
      this.showUpdateForm=false;
      this.selectedFarmLand=null;
    }
  }
  public reloadPage(rel:boolean){
    if(rel){
      this.router.navigate(['default/pending-task']).catch(dialog.error);
    }
  }
  public async getFarmLand(data:any):Promise<void>{
    if(data!=null){
      this.selectedFarmLand=data.farmLand;
      if(data.taskFor=='renewal'){
        this.showRenewalForm=true;
      }else if(data.taskFor=='warning'){
        this.showWarningForm=true;
      }

    }else{
      await dialog.error("sorry, i can't find farm land right");
    }
  }
}
