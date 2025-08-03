import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

// import { LandExpertModule } from './land-clerk.module';
import { LandDashboardRoutingModule } from './land-dashboard/land-dashboard-routing.module';


const routes: Route[] = [{
  path: 'land-clerk',
  // component: LandExpertModule,
  children: [
    { path: 'dashboard', component: LandDashboardRoutingModule }
  ]
}];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ],
  exports: [RouterModule],
  declarations: []
})
export class LandClerkRoutingModule { }
