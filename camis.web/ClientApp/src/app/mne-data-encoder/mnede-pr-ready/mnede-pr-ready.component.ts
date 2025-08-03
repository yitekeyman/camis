import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {configs} from '../../app-config';
import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocumentSelectorChangeEvent} from '../../_shared/document/document-selector/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-mnede-pr-ready',
  templateUrl: 'mnede-pr-ready.component.html',
})
export class MnedePrReadyComponent implements OnInit {

  readonly readonlyReporting = false;
  readonly urlPrefix = configs.url;

  loading = true;

  workflowId: string;

  farmId: string;
  farm: any;
  activityPlan: any = {
    reportDocuments: []
  };

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
        this.activityPlan = workItem.data;
        this.keyCase.camelCase(this.activityPlan);

        if (!this.activityPlan.reportDocuments) {
          this.activityPlan.reportDocuments = [];
        }
        const now = Date.now();
        this.activityPlan.reportDate = new Date(
          this.activityPlan.reportDate ? this.activityPlan.reportDate - new Date(now).getTimezoneOffset() * 60000 : now
        ).toISOString().slice(0, 10);

        this.api.getAllActivityStatusTypes().subscribe(types => {
          this.statusTypes = types;
          if (this.statusTypes.length && !this.activityPlan.reportStatusId) {
            this.activityPlan.reportStatusId = this.statusTypes[0].id;
          }
        }, dialog.error);

        this.farmApi.getFarmByActivity(this.activityPlan.rootActivityId).subscribe(farm => {
          this.farmId = farm.id;
          this.farm = farm;
          this.keyCase.camelCase(this.farm);

          this.loading = false;
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  onDocumentSelectorChange(e: IDocumentSelectorChangeEvent) {
    if (e.documents) {
      this.activityPlan.reportDocuments = e.documents;
    }
  }


  async cancelProgressReport(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this workflow? This is irreversible.')) {
      return;
    }

    this.loading = true;
    dialog.loading();
    this.api.cancelProgressReport(this.workflowId, null).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-data-encoder/dashboard`))
      .then(() => dialog.success('The workflow has been cancelled successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async encodeProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E expert (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();

    const body = this.activityPlan;
    body.isAdditional = body.isAdditional == 'true';
    body.reportDate = new Date(body.reportDate).getTime();
    this.keyCase.PascalCase(body);

    this.loading = true;
    dialog.loading();
    this.api.encodeProgressReport(this.workflowId, body, message).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-data-encoder/dashboard`))
      .then(() => dialog.success('Your changes have been sent to the expert successfully.'))
      .catch(err => {
        this.keyCase.camelCase(body);
        this.loading = false;
        return dialog.error(err);
      });
  }

}
