import {RouterModule, Routes} from "@angular/router";
import {LandBankComponent} from "./land-bank.component";
import {NgModule} from "@angular/core";

const routes: Routes = [{
  path: '',
  component: LandBankComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'search-land' },
    { path: 'search-land', loadComponent: () => import('./userManagement/userManagement.component').then((c) =>c.UserManagementComponent) },
    { path: 'activity-log', loadComponent: ()=>import('./activityLog/activityLog.component').then((c) =>c.ActivityLogComponent) },
    {path:'activity-template', loadComponent:()=>import('./configurationAdmin/ca-activity-templates/ca-activity-templates.component').then((c) =>c.CaActivityTemplatesComponent) },

  ]
}];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LandBankRoutingModule {}
