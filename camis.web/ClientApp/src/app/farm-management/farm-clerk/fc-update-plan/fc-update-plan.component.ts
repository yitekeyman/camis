import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import {ProjectApiService} from '../../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import dialog from '../../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {
  FcProjectDetailRegistrationComponent
} from "../fc-farm-registration/fc-project-detail-registration/fc-project-detail-registration.component";
import {DocumentModule} from "../../../_shared/document/document.module";

@Component({
  selector: 'app-fc-update-plan',
  imports:[CommonModule, ReactiveFormsModule, FormsModule, FcProjectDetailRegistrationComponent, DocumentModule],
  templateUrl: 'fc-update-plan.component.html'
})
export class FcUpdatePlanComponent implements OnInit {

  workflowId: string | null = null;

  loading = true;

  plan: any = {
    rootActivity: {
      name: 'Business Plan',
      description: 'Commercial Agriculture Business Plan',
      weight: 1,
      schedules: [],
      activityPlanDetails: [],
      children: []
    },
    documents: []
  };

  constructor (
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
  ) {
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'] ? params['workflowId'] : null;
      this.plan.id = params['planId'];

      if (this.workflowId) { // a started workflow
        this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
          this.keyCase.camelCase(workItem);
          this.setFields(workItem.data);
          this.loading = false;
          dialog.close();
        }, dialog.error);
      } else if (this.plan.id) { // new workflow to be started using the plan ID (this.planId)
        this.api.getActivityPlan(this.plan.id).subscribe(plan => {
          this.keyCase.camelCase(plan);
          this.setFields(plan);
          this.loading = false;
          dialog.close();
        }, dialog.error);
      }
    }, dialog.error);
  }

  private setFields(p: any): void {
    if (!p) { return; }
    this.keyCase.camelCase(p);
    this.plan = p;
  }


  async cancel(): Promise<void> {
    if (this.workflowId) {
      if (!await dialog.confirm('Are you sure you want to cancel this workflow? This is irreversible.')) {
        return;
      }

      this.loading = true;
      dialog.loading();
      this.api.cancelUpdatePlan(this.workflowId, null).toPromise()
        .then(() => dialog.success('The workflow has been cancelled successfully.'))
        .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
        .catch(err => {
          this.loading = false;
          return dialog.error(err)
        });
    } else {
      this.router.navigateByUrl(`default/pending-task`).catch(dialog.error);
    }
  }

  dumpSubmit(e: any): boolean {
    e.preventDefault();
    return false;
  }

  async onSubmit(e: any): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to submit this?')) {
      return;
    }
    this.loading = true;

    this.dumpSubmit(e);

    const message = await dialog.prompt('Enter a message for the supervisor');
    if (message === "") {
      this.loading = false;
      return
    }

    dialog.loading();

    const body: any = this.plan;

    this.keyCase.PascalCase(body);

    const req = this.workflowId ?
      this.api.requestUpdatePlan(this.workflowId, body, message) :
      this.api.requestNewUpdatePlan(body, message);

    req.toPromise()
      .then(()=>dialog.success('Your plan update request has been sent to the supervisor successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });

  }

}
