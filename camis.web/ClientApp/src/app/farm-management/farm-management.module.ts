import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmManagementComponent} from "./farm-management.component";
import {FarmManagementRoutingModule} from "./farm-management-routing.module";

@NgModule({
  imports:[CommonModule, ReactiveFormsModule, FormsModule, FarmManagementComponent, FarmManagementRoutingModule],
  declarations:[],
  exports: [FarmManagementComponent],
  providers:[]
})

export class FarmManagementModule{}
