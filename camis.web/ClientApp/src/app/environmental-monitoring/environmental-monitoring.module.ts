import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {EnvironmentalMonitoringComponent} from "./environmental-monitoring.component";
import {EnvironmentalMonitoringRoutingModule} from "./environmental-monitoring-routing.module";

@NgModule({
  declarations: [],
  imports: [CommonModule,
    FormsModule,
    ReactiveFormsModule,
    EnvironmentalMonitoringComponent,
    EnvironmentalMonitoringRoutingModule
  ],
  providers:[],
  exports: [EnvironmentalMonitoringComponent],
})

export class EnvironmentalMonitoringModule{

}
