import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MainShellModule } from '../../_shared/main-shell/main-shell.module';
import { NewLandRoutingModule } from './new-land-routing.module';
import { LandBankModule } from '../../_shared/land-bank/land-bank.module';

import { NewLandComponent } from './new-land/new-land.component';
import { NewLandDefaultComponent } from './new-land-default/new-land-default.component';
import { NewLandFormComponent } from './new-land-form/new-land-form.component';

import { CamisMapModule } from '../../_shared/camismap/camismap.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MainShellModule,
    NewLandRoutingModule,
    CamisMapModule,
    LandBankModule
  ],
  declarations: [
    NewLandComponent,
    NewLandDefaultComponent,
    NewLandFormComponent,
  ]
})
export class NewLandModule { }
