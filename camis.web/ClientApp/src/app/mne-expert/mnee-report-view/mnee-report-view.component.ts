import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';

@Component({
    selector: 'app-mnee-report-view',
    templateUrl: 'mnee-report-view.component.html'
})
export class MneeReportViewComponent implements OnInit {

    loading = true;

    reportId: string;
    report: any;

    constructor (private api: ProjectApiService, private router: Router, private ar: ActivatedRoute) {
    }

    ngOnInit(): void {
        this.ar.params.subscribe(params => this.reportId = params.reportId, dialog.error)
            .add(this.api.getProgressReport(this.reportId).subscribe(report => {
                this.report = report;
                this.loading = false;
            }, dialog.error));
    }

}
