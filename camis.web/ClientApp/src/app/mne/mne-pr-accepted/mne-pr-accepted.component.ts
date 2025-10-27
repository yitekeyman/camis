import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {ProjectApiService} from '../../_services/project-api.service';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";

@Component({
  selector: 'app-mne-pr-accepted',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, FarmDetailComponent],
  templateUrl: 'mne-pr-accepted.component.html'
})
export class MnePrAcceptedComponent implements OnInit {

  loading = true;

  workflowId: string;

  farmId: string;
  farm: any;
  plan: any;
  loginRole = '0';

  constructor(
    private router: Router,
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
    this.loginRole = localStorage.getItem("role");
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'];
      dialog.loading();
      this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.keyCase.camelCase(workItem);
        this.plan = workItem.data;

        this.farmApi.getFarmByActivity(this.plan.rootActivityId).subscribe(farm => {
          this.keyCase.camelCase(farm);
          this.farmId = farm.id;
          this.farm = farm;
          dialog.close();
          this.loading = false;

        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  goToReports(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`/mne/plan/${planId}/reports`);
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

  async surveyProgressReport(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the M&E expert (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();
    this.api.surveyProgressReport(this.workflowId, message).toPromise()
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('The progress report status has been saved successfully.'))
      .catch(err => {
        this.loading = false;
        return dialog.error(err);
      });
  }

}
