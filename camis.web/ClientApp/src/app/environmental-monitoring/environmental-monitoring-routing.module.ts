import {RouterModule, Routes} from "@angular/router";
import {EnvironmentalMonitoringComponent} from "./environmental-monitoring.component";
import {NgModule} from "@angular/core";
import {ParcelHistoryComponent} from "./parcel-history/parcel-history.component";

const routes: Routes = [{
  path: '',
  component: EnvironmentalMonitoringComponent,
  children: [
    {path: '', pathMatch: 'full', redirectTo: 'change-detection'},
    {path: 'change-detection', loadComponent:()=>import('./change-detection/change-detection.component').then((c) => c.ChangeDetectionComponent)},
    {path:'parcel-history', loadComponent:()=>import('./parcel-history/parcel-history.component').then((c) => c.ParcelHistoryComponent)},
    {path:'risk-monitoring', loadComponent:()=>import('./risk-monitoring/risk-monitoring.component').then((c) => c.RiskMonitoringComponent)},
  ]
}]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EnvironmentalMonitoringRoutingModule{

}
