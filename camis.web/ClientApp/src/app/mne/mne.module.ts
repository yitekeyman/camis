import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MneComponent} from "./mne.component";
import {MneRoutingModule} from "./mne-routing.module";

@NgModule({
  imports:[CommonModule, ReactiveFormsModule, FormsModule, MneComponent, MneRoutingModule],
  declarations:[],
  exports: [MneComponent],
  providers:[]
})
export class MneModule {}
