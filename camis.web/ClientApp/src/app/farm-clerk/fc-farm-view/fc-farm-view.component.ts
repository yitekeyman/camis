import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';
import {ProjectApiService} from "../../_services/project-api.service";

@Component({
  selector: 'app-fc-farm-view',
  templateUrl: 'fc-farm-view.component.html'
})
export class FcFarmViewComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;

  plan: any;

  constructor (private api: FarmApiService, private projectApi: ProjectApiService, private router: Router, private ar: ActivatedRoute) {}

  ngOnInit(): void {
    this.ar.params.subscribe(params => this.farmId = params.farmId, dialog.error)
      .add(this.api.getFarm(this.farmId).subscribe(farm => {
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.plan = plan;
          this.loading = false;
        }, dialog.error);
      }, dialog.error));
  }


  newFarmRegistrationStep3(operatorId: string): Promise<boolean> {
    return this.router.navigateByUrl(`clerk/farm/registration/new?step=3&operatorId=${operatorId}`);
  }

  newFarmModification(farmId: string): Promise<boolean> {
    return this.router.navigateByUrl(`clerk/farm/${farmId}/modification/new`);
  }

  newUpdatePlan(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`clerk/plan/${planId}/update`);
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
        this.router.navigateByUrl(`clerk/farm/dashboard`).catch(dialog.error);
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

}
