import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {FarmSupervisorComponent} from './farm-supervisor/farm-supervisor.component';
import {FsDashboardComponent} from './fs-dashboard/fs-dashboard.component';
import {FsFarmDeletionComponent} from './fs-farm-deletion/fs-farm-deletion.component';
import {FsFarmListComponent} from './fs-farm-list/fs-farm-list.component';
import {FsFarmModificationComponent} from './fs-farm-modification/fs-farm-modification.component';
import {FsFarmRegistrationComponent} from './fs-farm-registration/fs-farm-registration.component';
import {FsFarmViewComponent} from './fs-farm-view/fs-farm-view.component';
import {FsUpdatePlanComponent} from './fs-update-plan/fs-update-plan.component';

const routes: Route[] = [{
  path: '',
  component: FarmSupervisorComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: FsDashboardComponent },
    { path: 'farm/list', component: FsFarmListComponent },
    { path: 'farm/registration/:workflowId', component: FsFarmRegistrationComponent, },
    { path: 'farm/modification/:workflowId', component: FsFarmModificationComponent, },
    { path: 'farm/deletion/:workflowId', component: FsFarmDeletionComponent, },
    { path: 'farm/:farmId', component: FsFarmViewComponent },
    { path: 'plan/update/:workflowId', component: FsUpdatePlanComponent, },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FarmSupervisorRoutingModule { }
