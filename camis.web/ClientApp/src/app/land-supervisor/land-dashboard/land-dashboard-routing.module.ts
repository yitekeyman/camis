import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

import { LandDashboardComponent } from './land-dashboard/land-dashboard.component';
import { DefaultDashboardComponent } from './default-dashboard/default-dashboard.component';
import { SearchResultDetailComponent } from '../../_shared/land-bank/search-result-detail/search-result-detail.component';
// import { ReportComponent } from '../../land-admin/report/report.component';
import { GenerateReportComponent } from '../../_shared/report/generate-report/generate-report.component';


const routes: Route[] = [{
   path: '', component: LandDashboardComponent,
   children: [
    { path: '', pathMatch: 'full', redirectTo: 'land-dashboard' },
    { path: 'land-dashboard', component: DefaultDashboardComponent },
    { path: 'land-dashboard/land-detail/:landID', component: SearchResultDetailComponent },
    { path: 'report', component : GenerateReportComponent }

  ]
}];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ],
  exports: [RouterModule],
})
export class LandDashboardRoutingModule { }
