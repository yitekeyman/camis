import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import {AddressOutputComponent} from './address-output/address-output.component';
import {AddressSelectorComponent} from './address-selector/address-selector.component';
import {FormsModule} from "@angular/forms";

@NgModule({
  declarations: [

  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    AddressOutputComponent,
    AddressSelectorComponent,
  ],
  exports: [
    AddressOutputComponent,
    AddressSelectorComponent,
  ],
  providers: []
})
export class AddressModule { }
