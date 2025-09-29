import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AdminComponent} from './admin.component';
import {CaActivityTemplatesComponent} from "./configurationAdmin/ca-activity-templates/ca-activity-templates.component";

const routes: Routes = [{
  path: '',
  component: AdminComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'user-management' },
    { path: 'user-management', loadComponent: () => import('./userManagement/userManagement.component').then((c) =>c.UserManagementComponent) },
    { path: 'activity-log', loadComponent: ()=>import('./activityLog/activityLog.component').then((c) =>c.ActivityLogComponent) },
    {path:'activity-template', loadComponent:()=>import('./configurationAdmin/ca-activity-templates/ca-activity-templates.component').then((c) =>c.CaActivityTemplatesComponent) },

  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
