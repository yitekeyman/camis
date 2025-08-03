import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

import { LandDashboardComponent } from './land-dashboard/land-dashboard.component';
import { DashboardDefaultComponent } from './dashboard-default/dashboard-default.component';
import { SearchResultDetailComponent } from '../../_shared/land-bank/search-result-detail/search-result-detail.component';


const routes: Route[] = [{
   path: '', component: LandDashboardComponent,
   children: [
    { path: '', pathMatch: 'full', redirectTo: 'land-dashboard' },
    { path: 'land-dashboard', component: DashboardDefaultComponent },
    { path: 'land-dashboard/land-detail/:landID', component: SearchResultDetailComponent }
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
