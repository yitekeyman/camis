import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AdminComponent} from './admin.component';
import {AdminDashboardComponent} from './AdminDashboard/adminDashboard.component';
import {UserComponent} from './user/user.component';
import {EmployeeComponent} from './EmployeeManagement/employee.component';

const routes: Routes = [{
  path: '',
  component: AdminComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: AdminDashboardComponent },
    { path: 'users', component: UserComponent },
    { path: 'employee', component: EmployeeComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
