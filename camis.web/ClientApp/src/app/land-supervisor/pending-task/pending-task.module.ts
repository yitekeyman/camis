import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PendingTaskRoutingModule } from './pending-task-routing.module';
import { MainShellModule } from '../../_shared/main-shell/main-shell.module';
import { LandBankModule } from '../../_shared/land-bank/land-bank.module';
import { DocumentModule } from '../../_shared/document/document.module';

import { PendingTaskComponent } from './pending-task/pending-task.component';
import { TaskDefaultComponent } from './task-default/task-default.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import { DetailTableComponent } from './detail-table/detail-table.component';
import { CamisMapModule } from '../../_shared/camismap/camismap.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PendingTaskRoutingModule,
    MainShellModule,
    CamisMapModule,
    LandBankModule,
    DocumentModule
  ],
  declarations: [
    PendingTaskComponent,
    TaskDefaultComponent,
    TaskDetailComponent,
    DetailTableComponent
  ]
})
export class PendingTaskModule { }
