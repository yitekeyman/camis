import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {ProjectModule} from "../../_shared/project/project.module";

@Component({
  selector: 'app-mne-reports',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ProjectModule],
  templateUrl: 'mne-reports.component.html'
})
export class MneReportsComponent implements OnInit {

  planId: string;
  plan: any = {};

  loading = true;

  term = '';
  totalReports = 0;
  reports: any[] = [];
  loginRole = '0';

  constructor(
    private api: ProjectApiService,
    private farmApi: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
    this.loginRole = localStorage.getItem("role");
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.planId = params['planId'];
      this.load(0);

      this.api.getActivityPlan(this.planId).subscribe(plan => {
        this.keyCase.camelCase(plan);
        this.plan = plan
      }, dialog.error);
    }, dialog.error);
  }

  load(skip = this.reports.length, take = 10) {
    dialog.loading();
    return this.api.searchReports(this.planId, this.term, skip, take).subscribe(paginator => {
      this.keyCase.camelCase(paginator);
      this.totalReports = paginator.totalSize;
      this.reports = this.reports.slice(0, skip).concat(paginator.items);
      this.loading = false;
      dialog.close();
    }, dialog.error);
  }


  openDetail(reportId: string) {
    return this.router.navigate([`mne/report/${reportId}`]);
  }

}
