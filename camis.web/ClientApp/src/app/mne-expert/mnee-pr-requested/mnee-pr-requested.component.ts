import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import dialog from '../../_shared/dialog';

@Component({
    selector: 'app-mnee-pr-requested',
    templateUrl: 'mnee-pr-requested.component.html'
})
export class MneePrRequestedComponent implements OnInit {

  loading = true;

  workflowId: string;

  farmId: string;
  farm: any;
  plan: any;

  constructor(
    private router: Router,
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.workflowId = params.workflowId;

      this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.plan = workItem.data;
        this.keyCase.camelCase(this.plan);

        this.farmApi.getFarmByActivity(this.plan.rootActivityId).subscribe(farm => {
          this.farmId = farm.id;
          this.farm = farm;
          this.keyCase.camelCase(this.farm);

          this.loading = false;
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  goToReports(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`/mne-expert/plan/${planId}/reports`);
  }

  async cancelProgressReport(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this workflow? This is irreversible.')) {
      return;
    }

    this.loading = true;
    dialog.loading();
    this.api.cancelProgressReport(this.workflowId, null).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-expert/dashboard`))
      .then(() => dialog.success('The workflow has been cancelled successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async acceptProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E expert (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();
    this.api.acceptProgressReport(this.workflowId, message).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-expert/dashboard`))
      .then(() => dialog.success('The progress report status has been saved successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err);
      });
  }

}
