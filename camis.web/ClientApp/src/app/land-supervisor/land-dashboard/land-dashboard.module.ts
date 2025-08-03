import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { CommonModule } from '@angular/common';

import { LandDashboardRoutingModule } from './land-dashboard-routing.module';
import { MainShellModule } from '../../_shared/main-shell/main-shell.module';
import { LandBankModule } from '../../_shared/land-bank/land-bank.module';

import { LandDashboardComponent } from './land-dashboard/land-dashboard.component';
import { DefaultDashboardComponent } from './default-dashboard/default-dashboard.component';
import { ReportModule } from '../../_shared/report/report.module';


@NgModule({
  imports: [
    CommonModule,
    LandDashboardRoutingModule,
    MainShellModule,
    FormsModule,
    ReactiveFormsModule,
    LandBankModule,
    ReportModule
  ],
  declarations: [
    LandDashboardComponent,
    DefaultDashboardComponent
  ]
})
export class LandDashboardModule { }
