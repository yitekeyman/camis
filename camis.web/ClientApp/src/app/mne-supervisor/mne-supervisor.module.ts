import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {MneSupervisorRoutingModule} from './mne-supervisor-routing.module';
import {DocumentModule} from '../_shared/document/document.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';

import {MneSupervisorComponent} from './mne-supervisor/mne-supervisor.component';
import {MnesDashboardComponent} from './mnes-dashboard/mnes-dashboard.component';
import {MnesFarmsComponent} from './mnes-farms/mnes-farms.component';
import {MnesPrNewRequestComponent} from './mnes-pr-new-request/mnes-pr-new-request.component';
import {MnesPrReportedComponent} from './mnes-pr-reported/mnes-pr-reported.component';
import {MnesReportsComponent} from './mnes-reports/mnes-reports.component';
import {MnesReportViewComponent} from './mnes-report-view/mnes-report-view.component';

@NgModule({
  declarations: [
    MneSupervisorComponent,
    MnesDashboardComponent,
    MnesFarmsComponent,
    MnesPrNewRequestComponent,
    MnesPrReportedComponent,
    MnesReportsComponent,
    MnesReportViewComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MneSupervisorRoutingModule,

    DashboardModule,
    DocumentModule,
    FarmModule,
    MainShellModule,
    ProjectModule,
  ],
  providers: []
})
export class MneSupervisorModule { }
