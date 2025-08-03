import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocument} from '../../_shared/document/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-mnee-pr-surveying',
  templateUrl: 'mnee-pr-surveying.component.html'
})
export class MneePrSurveyingComponent implements OnInit {

  loading = true;

  workflowId: string;

  farmId: string;
  farm: any;
  plan: any;

  surveyDocs: IDocument[] = [];

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

  async surveyedProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E data encoder (optional):');
    if (message === null) {
      return
    }

    const body = this.plan;
    body.reportDocuments = body.reportDocuments ? body.reportDocuments.concat(this.surveyDocs) : this.surveyDocs;
    this.keyCase.PascalCase(body);

    this.loading = true;
    dialog.loading();
    this.api.surveyedProgressReport(this.workflowId, body, message).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-expert/dashboard`))
      .then(() => dialog.success('Your changes have been sent to the data encoder successfully.'))
      .catch(err => {
        this.keyCase.camelCase(body);
        this.loading = false;
        return dialog.error(err);
      });
  }

}
