import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {LandAdminRoutingModule} from './land-admin-routing.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';

import {LandAdminComponent} from './land-admin/land-admin.component';
import {LaDashboardComponent} from './la-dashboard/la-dashboard.component';
import {LaLandSelectionComponent} from './la-land-selection/la-land-selection.component';
import {LaFarmsComponent} from './la-farms/la-farms.component';
import { ReportModule } from '../_shared/report/report.module';
// import { ReportComponent } from '../_shared/report/report.component';
import { GenerateReportComponent } from '../_shared/report/generate-report/generate-report.component';
// import { ReportComponent } from './report/report.component';

@NgModule({
  declarations: [
    LandAdminComponent,
    LaDashboardComponent,
    LaLandSelectionComponent,
    LaFarmsComponent,
    // GenerateReportComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LandAdminRoutingModule,

    DashboardModule,
    FarmModule,
    MainShellModule,
    ReportModule
  ],
  providers: []
})
export class LandAdminModule { }
