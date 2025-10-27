import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ProjectApiService} from '../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";

@Component({
  selector: 'app-mne-pr-new-request',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent],
  templateUrl: 'mne-pr-new-request.component.html'
})
export class MnePrNewRequestComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;
  plan: any;
  loginRole = '0';

  constructor(
    private router: Router,
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
    this.loginRole = localStorage.getItem("role");
  }

  ngOnInit(): void {
    this.ar.queryParams.subscribe(params => {
      this.farmId = params['farmId'];
      dialog.loading();
      this.api.getFarm(this.farmId).subscribe(farm => {
        this.keyCase.camelCase(farm);
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.keyCase.camelCase(plan);
          this.plan = plan;
          this.loading = false;
          dialog.close();
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }


  goToReports(planId: string): Promise<boolean> {
    return this.router.navigateByUrl(`/mne/plan/${planId}/reports`);
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
      .then(() => this.router.navigateByUrl(`/default/pending-task`))
      .then(() => dialog.success('The progress report has been approved successfully.'))
      .catch(err => {
        this.keyCase.camelCase(body);
        this.loading = false;
        return dialog.error(err);
      });
  }

}
