import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LandDashboardRoutingModule } from './land-dashboard-routing.module';
import { ReportModule } from '../../_shared/report/report.module';
import { MainShellModule } from '../../_shared/main-shell/main-shell.module';
import { LandBankModule } from '../../_shared/land-bank/land-bank.module';

import { LandDashboardComponent } from './land-dashboard/land-dashboard.component';
import { DashboardDefaultComponent } from './dashboard-default/dashboard-default.component';

@NgModule({
  imports: [
    CommonModule,
    LandDashboardRoutingModule,
    MainShellModule,
    FormsModule,
    ReactiveFormsModule,
    ReportModule,
    LandBankModule
  ],
  declarations: [
    LandDashboardComponent,
    DashboardDefaultComponent
  ],
})
export class LandDashboardModule { }
