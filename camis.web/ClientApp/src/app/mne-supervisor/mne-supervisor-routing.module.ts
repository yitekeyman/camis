import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {MneSupervisorComponent} from './mne-supervisor/mne-supervisor.component';
import {MnesFarmsComponent} from './mnes-farms/mnes-farms.component';
import {MnesDashboardComponent} from './mnes-dashboard/mnes-dashboard.component';
import {MnesPrNewRequestComponent} from './mnes-pr-new-request/mnes-pr-new-request.component';
import {MnesPrReportedComponent} from './mnes-pr-reported/mnes-pr-reported.component';
import {MnesReportsComponent} from './mnes-reports/mnes-reports.component';
import {MnesReportViewComponent} from './mnes-report-view/mnes-report-view.component';

const routes: Route[] = [{
  path: '',
  component: MneSupervisorComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: MnesDashboardComponent },
    { path: 'farms', component: MnesFarmsComponent },
    { path: 'pr/new/request', component: MnesPrNewRequestComponent },
    { path: 'pr/:workflowId/reported', component: MnesPrReportedComponent },
    { path: 'plan/:planId/reports', component: MnesReportsComponent },
    { path: 'report/:reportId', component: MnesReportViewComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MneSupervisorRoutingModule { }
