import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {ReportDetailComponent} from "../../_shared/project/report-detail/report-detail.component";

@Component({
  selector: 'app-mne-report-view',
  imports: [CommonModule, ReactiveFormsModule, FormsModule,ReportDetailComponent],
  templateUrl: 'mne-report-view.component.html'
})
export class MneReportViewComponent implements OnInit {

  loading = true;

  reportId: string;
  report: any;
  loginRole = '0';

  constructor(private api: ProjectApiService, private router: Router, private ar: ActivatedRoute, private keyCase: ObjectKeyCasingService) {
    this.loginRole = localStorage.getItem("role");
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => this.reportId = params['reportId'], dialog.error)
      .add(this.api.getProgressReport(this.reportId).subscribe(report => {
        this.keyCase.camelCase(report);
        this.report = report;
        this.loading = false;
        dialog.close();
      }, dialog.error));
  }

}
