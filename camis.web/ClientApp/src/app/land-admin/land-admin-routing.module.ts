import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {LandAdminComponent} from './land-admin/land-admin.component';
import {LaDashboardComponent} from './la-dashboard/la-dashboard.component';
import {LaLandSelectionComponent} from './la-land-selection/la-land-selection.component';
import {LaFarmsComponent} from './la-farms/la-farms.component';
import { ReportComponent } from '../_shared/report/report.component';
import { GenerateReportComponent } from '../_shared/report/generate-report/generate-report.component';
// import { ReportComponent } from './report/report.component';

const routes: Route[] = [{
  path: '',
  component: LandAdminComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: LaDashboardComponent },
    { path: 'land-selection/new/:farmId', component: LaLandSelectionComponent },
    { path: 'land-selection/:workflowId', component: LaLandSelectionComponent },
    { path: 'farms', component: LaFarmsComponent },
    { path: 'report', component : GenerateReportComponent }

  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LandAdminRoutingModule { }
