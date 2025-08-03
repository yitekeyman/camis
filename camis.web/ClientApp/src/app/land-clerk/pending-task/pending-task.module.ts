import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MainShellModule } from '../../_shared/main-shell/main-shell.module';
import { PendingTaskRoutingModule } from './pending-task-routing.module';
import { LandBankModule } from '../../_shared/land-bank/land-bank.module';

import { PendingTaskComponent } from './pending-task/pending-task.component';
import { TaskDefaultComponent } from './task-default/task-default.component';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PendingTaskRoutingModule,
    MainShellModule,
    LandBankModule
  ],
  declarations: [
    PendingTaskComponent,
    TaskDefaultComponent,
  ]
})
export class PendingTaskModule { }
