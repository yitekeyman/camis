import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocumentSelectorChangeEvent} from '../../_shared/document/document-selector/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-mnes-pr-reported',
  templateUrl: 'mnes-pr-reported.component.html'
})
export class MnesPrReportedComponent implements OnInit {

  readonly readonlyReporting = true;

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
        });

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
      .then(() => this.router.navigateByUrl(`/mne-supervisor/dashboard`))
      .then(() => dialog.success('The workflow has been cancelled successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async approveProgressReport(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this progress report?')) {
      return;
    }

    this.loading = true;
    dialog.loading();
    this.api.approveProgressReport(this.workflowId, null).toPromise()
      .then(() => this.router.navigateByUrl(`/mne-supervisor/dashboard`))
      .then(() => dialog.success('The progress report has been approved successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err);
      });
  }

}
