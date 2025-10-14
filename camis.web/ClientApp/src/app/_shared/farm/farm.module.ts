import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {AddressModule} from '../address/address.module';
import {ProjectModule} from '../project/project.module';
import {DocumentModule} from '../document/document.module';

import {AuthorityRegistrarComponent} from './authority-registrar/authority-registrar.component';
import {FarmDetailComponent} from './farm-detail/farm-detail.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    AuthorityRegistrarComponent,
    FarmDetailComponent,
    AddressModule,
    ProjectModule,
    DocumentModule,
  ],
  exports: [
    AuthorityRegistrarComponent,
    FarmDetailComponent,
  ],
  providers: []
})
export class FarmModule {
}
