import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {MneExpertComponent} from './mne-expert/mne-expert.component';
import {MneeDashboardComponent} from './mnee-dashboard/mnee-dashboard.component';
import {MneeFarmsComponent} from './mnee-farms/mnee-farms.component';
import {MneeFarmViewComponent} from './mnee-farm-view/mnee-farm-view.component';
import {MneePrRequestedComponent} from './mnee-pr-requested/mnee-pr-requested.component';
import {MneePrAcceptedComponent} from './mnee-pr-accepted/mnee-pr-accepted.component';
import {MneePrSurveyingComponent} from './mnee-pr-surveying/mnee-pr-surveying.component';
import {MneePrReviewingComponent} from './mnee-pr-reviewing/mnee-pr-reviewing.component';
import {MneeReportsComponent} from './mnee-reports/mnee-reports.component';
import {MneeReportViewComponent} from './mnee-report-view/mnee-report-view.component';

const routes: Route[] = [{
  path: '',
  component: MneExpertComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: MneeDashboardComponent },
    { path: 'farms', component: MneeFarmsComponent },
    { path: 'farm/:farmId', component: MneeFarmViewComponent },
    { path: 'pr/:workflowId/requested', component: MneePrRequestedComponent },
    { path: 'pr/:workflowId/accepted', component: MneePrAcceptedComponent },
    { path: 'pr/:workflowId/surveying', component: MneePrSurveyingComponent },
    { path: 'pr/:workflowId/reviewing', component: MneePrReviewingComponent },
    { path: 'plan/:planId/reports', component: MneeReportsComponent },
    { path: 'report/:reportId', component: MneeReportViewComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MneExpertRoutingModule { }
