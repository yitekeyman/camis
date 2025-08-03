import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ProjectApiService} from '../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-mnes-pr-new-request',
  templateUrl: 'mnes-pr-new-request.component.html'
})
export class MnesPrNewRequestComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;
  plan: any;

  constructor(
    private router: Router,
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
  }

  ngOnInit(): void {
    this.ar.queryParams.subscribe(params => {
      this.farmId = params.farmId;

      this.api.getFarm(this.farmId).subscribe(farm => {
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.plan = plan;
          this.loading = false;
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  goToReports(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`/mne-supervisor/plan/${planId}/reports`);
  }

  async requestProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E expert (optional):');
    if (message === null) {
      return
    }

    const body = this.plan;
    this.keyCase.PascalCase(body);

    this.loading = true;
    dialog.loading();
    this.projectApi.requestNewProgressReport(body, message).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-supervisor/dashboard`))
      .then(() => dialog.success('The progress report has been approved successfully.'))
      .catch(err => {
        this.keyCase.camelCase(body);
        this.loading = false;
        return dialog.error(err);
      });
  }

}
