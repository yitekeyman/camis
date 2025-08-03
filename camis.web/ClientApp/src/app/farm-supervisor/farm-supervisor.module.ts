import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {FarmSupervisorRoutingModule} from './farm-supervisor-routing.module';
import {AddressModule} from '../_shared/address/address.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';
import {DocumentModule} from '../_shared/document/document.module';

import {FarmSupervisorComponent} from './farm-supervisor/farm-supervisor.component';
import {FsDashboardComponent} from './fs-dashboard/fs-dashboard.component';
import {FsFarmDeletionComponent} from './fs-farm-deletion/fs-farm-deletion.component';
import {FsFarmListComponent} from './fs-farm-list/fs-farm-list.component';
import {FsFarmModificationComponent} from './fs-farm-modification/fs-farm-modification.component';
import {FsFarmRegistrationComponent} from './fs-farm-registration/fs-farm-registration.component';
import {FsFarmViewComponent} from './fs-farm-view/fs-farm-view.component';
import {FsUpdatePlanComponent} from './fs-update-plan/fs-update-plan.component';

@NgModule({
  declarations: [
    FarmSupervisorComponent,
    FsDashboardComponent,
    FsFarmDeletionComponent,
    FsFarmListComponent,
    FsFarmModificationComponent,
    FsFarmRegistrationComponent,
    FsFarmViewComponent,
    FsUpdatePlanComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FarmSupervisorRoutingModule,

    AddressModule,
    DashboardModule,
    FarmModule,
    MainShellModule,
    ProjectModule,
    DocumentModule,
  ],
  providers: [
  ]
})
export class FarmSupervisorModule { }
