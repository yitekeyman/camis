import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

import { PendingTaskComponent } from './pending-task/pending-task.component';
import { TaskDefaultComponent } from './task-default/task-default.component';
import { EditLandComponent } from '../../_shared/land-bank/edit-land/edit-land.component';

const routes: Route[] = [{
   path: '', component: PendingTaskComponent,
   children: [
    { path: '', pathMatch: 'full', redirectTo: 'pending-task' },
    { path: 'pending-task', component: TaskDefaultComponent },
    { path: 'pending-task/edit-land/:wfid', component: EditLandComponent}
  ]
}];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ],
  exports: [RouterModule],
})
export class PendingTaskRoutingModule { }
