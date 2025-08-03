import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {FarmClerkRoutingModule} from './farm-clerk-routing.module';
import {AddressModule} from '../_shared/address/address.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {DocumentModule} from '../_shared/document/document.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';

import {FarmClerkComponent} from './farm-clerk/farm-clerk.component';
import {FcDashboardComponent} from './fc-dashboard/fc-dashboard.component';
import {FcFarmManagementComponent} from './fc-farm-management/fc-farm-management.component';
import {FcFarmModificationComponent} from './fc-farm-modification/fc-farm-modification.component';
import {FcFarmRegistrationComponent} from './fc-farm-registration/fc-farm-registration.component';
import {FcFarmViewComponent} from './fc-farm-view/fc-farm-view.component';
import {FcProjectDetailRegistrationComponent} from './fc-farm-registration/fc-project-detail-registration/fc-project-detail-registration.component';
import {FcFarmOperatorViewComponent} from "./fc-farm-operator-view/fc-farm-operator-view.component";
import {FcUpdatePlanComponent} from './fc-update-plan/fc-update-plan.component';

@NgModule({
  declarations: [
    FarmClerkComponent,
    FcDashboardComponent,
    FcFarmManagementComponent,
    FcFarmModificationComponent,
    FcFarmRegistrationComponent,
    FcFarmViewComponent,
    FcProjectDetailRegistrationComponent,
    FcFarmOperatorViewComponent,
    FcUpdatePlanComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FarmClerkRoutingModule,

    AddressModule,
    DashboardModule,
    DocumentModule,
    FarmModule,
    MainShellModule,
    ProjectModule,
  ],
  providers: [
  ]
})
export class FarmClerkModule { }
