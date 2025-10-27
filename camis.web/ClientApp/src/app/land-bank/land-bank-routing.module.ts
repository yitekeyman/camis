import {RouterModule, Routes} from "@angular/router";
import {LandBankComponent} from "./land-bank.component";
import {NgModule} from "@angular/core";
import {LbCertificationComponent} from "./lb-certification/lb-certification.component";
import {GenerateReportComponent} from "../_shared/report/generate-report/generate-report.component";

const routes: Routes = [{
  path: '',
  component: LandBankComponent,
  children: [
    {path: '', pathMatch: 'full', redirectTo: 'search-land'},
    {
      path: 'search-parcel',
      loadComponent: () => import('./search-land/search-land-bank.component').then((c) => c.SearchLandBankComponent)
    },
    {
      path: 'register-parcel',
      loadComponent: () => import('./register-land/register-land-bank.component').then((c) => c.RegisterLandBankComponent)
    },
    {
      path: 'edit-parcel-info/:id',
      loadComponent: () =>import('./register-land/register-land-bank.component').then((c) => c.RegisterLandBankComponent)
    },
    {
      path: 'parcel-details/:landID',
      loadComponent:()=>import('./land-details/land-details.component').then((c) => c.LandDetailsComponent)
    },
    {
      path: 'task/parcel-details/:wfid', loadComponent:()=>import('./land-task/land-task.component').then((c) => c.LandTaskComponent)
    },
    {
      path: 'task/edit-parcel-info/:wfid',
      loadComponent: () =>import('./register-land/edit-land/edit-land.component').then((c) => c.EditLandComponent)
    },
    {
      path: 'task/land-selection/new/:farmId',
      loadComponent: () =>import('./land-selection/land-selection.component').then((c) => c.LandSelectionComponent)
    },
    {
      path: 'task/land-selection/:workflowId',
      loadComponent: () =>import('./land-selection/land-selection.component').then((c) => c.LandSelectionComponent)
    },
    {
      path: 'task/certification/:workflowId',
      loadComponent: () =>import('./lb-certification/lb-certification.component').then((c) => c.LbCertificationComponent)
    },

  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LandBankRoutingModule {
}
