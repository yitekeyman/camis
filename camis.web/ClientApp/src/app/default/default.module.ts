import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {DefaultRoutingModule} from "./default-routing.module";
import {DefaultComponent} from "./default.component";

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DefaultRoutingModule,
    DefaultComponent
  ],
  providers: [],
  exports: [DefaultComponent]
})
export class DefaultModule {
}
