import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LandClerkRoutingModule } from './land-clerk-routing.module';
import { LandBankModule } from '../_shared/land-bank/land-bank.module';

import { LandDashboardModule } from './land-dashboard/land-dashboard.module';
import { PendingTaskModule } from './pending-task/pending-task.module';
import { NewLandModule } from './new-land/new-land.module';


@NgModule({
  imports: [
    CommonModule,
    LandClerkRoutingModule,
    LandBankModule,
    LandDashboardModule,
    PendingTaskModule,
    NewLandModule,
  ],
  declarations: []
})
export class LandClerkModule { }
