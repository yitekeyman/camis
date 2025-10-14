import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {LandBankRoutingModule} from "./land-bank-routing.module";
import {LandBankComponent} from "./land-bank.component";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LandBankRoutingModule,
    LandBankComponent,

  ],
  declarations: [],
  providers:[],
  exports: [LandBankComponent]
})

export class LandBankModule {}
