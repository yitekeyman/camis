import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {MneExpertRoutingModule} from './mne-expert-routing.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {DocumentModule} from '../_shared/document/document.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';

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

@NgModule({
  declarations: [
    MneExpertComponent,
    MneeDashboardComponent,
    MneeFarmsComponent,
    MneeFarmViewComponent,
    MneePrRequestedComponent,
    MneePrAcceptedComponent,
    MneePrSurveyingComponent,
    MneePrReviewingComponent,
    MneeReportsComponent,
    MneeReportViewComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MneExpertRoutingModule,

    DashboardModule,
    DocumentModule,
    FarmModule,
    MainShellModule,
    ProjectModule,
  ],
  providers: []
})
export class MneExpertModule { }
