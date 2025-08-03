import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';

@Component({
    selector: 'app-mnede-reports',
    templateUrl: 'mnede-reports.component.html'
})
export class MnedeReportsComponent implements OnInit {

    planId: string;
    plan: any = {};

    loading = true;

    term = '';
    totalReports = 0;
    reports: any[] = [];

    constructor (
        private api: ProjectApiService,
        private farmApi: FarmApiService,
        private router: Router,
        private ar: ActivatedRoute
    ) {
    }

    ngOnInit(): void {
        this.ar.params.subscribe(params => {
            this.planId = params.planId;
            this.load(0);

            this.api.getActivityPlan(this.planId).subscribe(plan => this.plan = plan, dialog.error);
        }, dialog.error);
    }

    load(skip = this.reports.length, take = 10) {
        return this.api.searchReports(this.planId, this.term, skip, take).subscribe(paginator => {
            this.totalReports = paginator.totalSize;
            this.reports = this.reports.slice(0, skip).concat(paginator.items);
            this.loading = false;
        }, dialog.error);
    }


    openDetail(reportId: string): Promise<boolean> {
        return this.router.navigateByUrl(`/mne-data-encoder/report/${reportId}`);
    }

}
