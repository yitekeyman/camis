import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {ConfigurationAdminRoutingModule} from './configuration-admin-routing.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';

import {ConfigurationAdminComponent} from './configuration-admin/configuration-admin.component';
import {CaActivityTemplatesComponent} from './ca-activity-templates/ca-activity-templates.component';

@NgModule({
  declarations: [
    ConfigurationAdminComponent,
    CaActivityTemplatesComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ConfigurationAdminRoutingModule,

    MainShellModule,
    ProjectModule,
  ],
  providers: []
})
export class ConfigurationAdminModule { }
