import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import {AdminDashboardModule} from './AdminDashboard/adminDashboard.module';
import {UserModule} from './user/user.module';

import {AdminComponent} from './admin.component';

@NgModule({
  imports: [
    CommonModule,
    AdminRoutingModule,
    AdminDashboardModule,
    UserModule,
  ],
  declarations: [
    AdminComponent,
  ]
})
export class AdminModule { }
