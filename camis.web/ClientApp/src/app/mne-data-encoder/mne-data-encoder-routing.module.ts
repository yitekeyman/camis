import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {MneDataEncoderComponent} from './mne-data-encoder/mne-data-encoder.component';
import {MnedeDashboardComponent} from './mnede-dashboard/mnede-dashboard.component';
import {MnedeFarmViewComponent} from './mnede-farm-view/mnede-farm-view.component';
import {MnedePrReadyComponent} from './mnede-pr-ready/mnede-pr-ready.component';
import {MnedeReportsComponent} from './mnede-reports/mnede-reports.component';
import {MnedeReportViewComponent} from './mnede-report-view/mnede-report-view.component';
import {MnedeFarmsComponent} from './mnede-farms/mnede-farms.component';

const routes: Route[] = [{
  path: '',
  component: MneDataEncoderComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: MnedeDashboardComponent },
    { path: 'farms', component: MnedeFarmsComponent },
    { path: 'farm/:farmId', component: MnedeFarmViewComponent },
    { path: 'pr/:workflowId/ready', component: MnedePrReadyComponent },
    { path: 'plan/:planId/reports', component: MnedeReportsComponent },
    { path: 'report/:reportId', component: MnedeReportViewComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MneDataEncoderRoutingModule { }
