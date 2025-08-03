import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';

@Component({
  selector: 'app-fs-update-plan',
  templateUrl: 'fs-update-plan.component.html'
})
export class FsUpdatePlanComponent implements OnInit {

  readonly readonlyReporting = true;

  loading = true;

  workflowId: string;
  farm: any;

  statusTypes: any[] = [];

  constructor(
    private router: Router,
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
  ) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.workflowId = params.workflowId;

      this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        const plan = workItem.data;
        this.keyCase.camelCase(plan);

        this.api.getAllActivityStatusTypes().subscribe(types => this.statusTypes = types);

        this.farmApi.getFarmByActivity(plan.rootActivityId).subscribe(farm => {
          this.farm = farm;
          this.farm.activityPlan = plan;
          this.keyCase.camelCase(this.farm);

          this.loading = false;
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  async rejectUpdatePlan(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the clerk (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();

    this.api.rejectUpdatePlan(this.workflowId, message).toPromise()
      .then(() => this.router.navigateByUrl(`/supervisor/dashboard`))
      .then(() => dialog.success('This plan update request has been rejected successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async approveUpdatePlan(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this plan update request?')) {
      return null;
    }

    this.loading = true;
    dialog.loading();
    this.api.approveUpdatePlan(this.workflowId, null).toPromise()
      .then(() => this.router.navigateByUrl(`/supervisor/dashboard`))
      .then(() => dialog.success('This plan update request has been approved successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

}
