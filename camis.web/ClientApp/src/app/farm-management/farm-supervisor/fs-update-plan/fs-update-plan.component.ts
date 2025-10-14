import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../../_services/project-api.service';
import dialog from '../../../_shared/dialog';
import {FarmApiService} from '../../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";

@Component({
  selector: 'app-fs-update-plan',
  imports: [CommonModule, ReactiveFormsModule, FarmDetailComponent, FormsModule],
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
    dialog.loading();
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'];

      this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.keyCase.camelCase(workItem);
        const plan = workItem.data;
        this.keyCase.camelCase(plan);

        this.api.getAllActivityStatusTypes().subscribe(types => {this.keyCase.camelCase(types);this.statusTypes = types},dialog.error);

        this.farmApi.getFarmByActivity(plan.rootActivityId).subscribe(farm => {
          this.keyCase.camelCase(farm);
          this.farm = farm;
          this.farm.activityPlan = plan;
          this.keyCase.camelCase(this.farm);

          this.loading = false;
          dialog.close();
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
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
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
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('This plan update request has been approved successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

}
