import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {FarmClerkComponent} from './farm-clerk/farm-clerk.component';
import {FcDashboardComponent} from './fc-dashboard/fc-dashboard.component';
import {FcFarmManagementComponent} from './fc-farm-management/fc-farm-management.component';
import {FcFarmModificationComponent} from './fc-farm-modification/fc-farm-modification.component';
import {FcFarmRegistrationComponent} from './fc-farm-registration/fc-farm-registration.component';
import {FcFarmViewComponent} from './fc-farm-view/fc-farm-view.component';
import {FcFarmOperatorViewComponent} from "./fc-farm-operator-view/fc-farm-operator-view.component";
import {FcUpdatePlanComponent} from './fc-update-plan/fc-update-plan.component';

const routes: Route[] = [{
  path: '',
  component: FarmClerkComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: FcDashboardComponent },
    { path: 'farm/management', component: FcFarmManagementComponent },
    { path: 'farm/:farmId/modification/new', component: FcFarmModificationComponent },
    { path: 'farm/modification/:workflowId', component: FcFarmModificationComponent },
    { path: 'farm/registration/new', component: FcFarmRegistrationComponent },
    { path: 'farm/registration/:workflowId', component: FcFarmRegistrationComponent },
    { path: 'farm/:farmId', component: FcFarmViewComponent },
    { path: 'farm-operator/:farmOperatorId', component: FcFarmOperatorViewComponent },
    { path: 'plan/:planId/update', component: FcUpdatePlanComponent },
    { path: 'plan/update/:workflowId', component: FcUpdatePlanComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FarmClerkRoutingModule { }
