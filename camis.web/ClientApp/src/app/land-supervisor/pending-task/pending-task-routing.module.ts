import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

import { PendingTaskComponent } from './pending-task/pending-task.component';
import { TaskDefaultComponent } from './task-default/task-default.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';


const routes: Route[] = [{
   path: '', component: PendingTaskComponent,
   children: [
    { path: '', pathMatch: 'full', redirectTo: 'pending-task' },
    { path: 'pending-task', component: TaskDefaultComponent },
    { path: 'pending-task/task-detail/:wfid', component: TaskDetailComponent}

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
