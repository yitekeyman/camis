import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LandBankModule } from '../_shared/land-bank/land-bank.module';
import { LandDashboardModule } from './land-dashboard/land-dashboard.module';
import { PendingTaskModule } from './pending-task/pending-task.module';

import { LandSupervisorRoutingModule } from './land-supervisor-routing.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LandBankModule,
    LandDashboardModule,
    PendingTaskModule,    
    LandSupervisorRoutingModule
  ],
  declarations: []
})
export class LandSupervisorModule { }
