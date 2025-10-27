import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocumentSelectorChangeEvent} from '../../_shared/document/document-selector/interfaces';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {ProjectModule} from "../../_shared/project/project.module";
import {DocumentModule} from "../../_shared/document/document.module";

@Component({
  selector: 'app-mne-pr-reviewing',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ProjectModule, DocumentModule],
  templateUrl: 'mne-pr-reviewing.component.html'
})
export class MnePrReviewingComponent implements OnInit {

  readonly readonlyReporting = true;

  loading = true;

  workflowId: string;

  farmId: string;
  farm: any;
  activityPlan: any = {
    reportDocuments: []
  };

  statusTypes: any[] = [];
  loginRole = '0';

  constructor(
    private router: Router,
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
  ) {
    this.loginRole = localStorage.getItem("role");
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'];
      dialog.loading();
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
          dialog.close();
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
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('The workflow has been cancelled successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async rejectProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E data encoder (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();
    this.api.rejectProgressReport(this.workflowId, message).toPromise()
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('Your have rejected the encoding successfully. The data encoder will receive this again.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err);
      });
  }

  async reportProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E supervisor (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();
    this.api.reportProgressReport(this.workflowId, message).toPromise()
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('The progress has been reported to the supervisor successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err);
      });
  }

}
