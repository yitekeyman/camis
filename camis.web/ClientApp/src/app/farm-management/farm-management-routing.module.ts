import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {FarmManagementComponent} from "./farm-management.component";
import {FcFarmManagementComponent} from "./farm-clerk/fc-farm-management/fc-farm-management.component";

const routes: Routes = [{
  path: '',
  component: FarmManagementComponent,
  children: [
    {path: '', pathMatch: 'full', redirectTo: 'search-farm'},
    {
      path: 'search-farm',
      loadComponent: () => import('./farm-clerk/fc-farm-management/fc-farm-management.component').then((c) => c.FcFarmManagementComponent)
    },
    {
      path: 'fc/farm/:farmId/modification/new',
      loadComponent: () => import('./farm-clerk/fc-farm-modification/fc-farm-modification.component').then((c) => c.FcFarmModificationComponent)
    },
    {
      path: 'fc/farm/modification/:workflowId',
      loadComponent: () => import('./farm-clerk/fc-farm-modification/fc-farm-modification.component').then((c) => c.FcFarmModificationComponent)
    },
    {
      path: 'fc/farm/registration/new',
      loadComponent: () => import('./farm-clerk/fc-farm-registration/fc-farm-registration.component').then((c) => c.FcFarmRegistrationComponent)
    },
    {
      path: 'fc/farm/registration/:workflowId',
      loadComponent: () => import('./farm-clerk/fc-farm-registration/fc-farm-registration.component').then((c) => c.FcFarmRegistrationComponent)
    },
    {
      path: 'fc/farm/:farmId',
      loadComponent: () => import('./farm-clerk/fc-farm-view/fc-farm-view.component').then((c) => c.FcFarmViewComponent)
    },
    {
      path: 'fc/farm-operator/:farmOperatorId',
      loadComponent: () => import('./farm-clerk/fc-farm-operator-view/fc-farm-operator-view.component').then((c) => c.FcFarmOperatorViewComponent)
    },
    {
      path: 'fc/plan/:planId/update',
      loadComponent: () => import('./farm-clerk/fc-update-plan/fc-update-plan.component').then((c) => c.FcUpdatePlanComponent)
    },
    {
      path: 'fc/plan/update/:workflowId',
      loadComponent: () => import('./farm-clerk/fc-update-plan/fc-update-plan.component').then((c) => c.FcUpdatePlanComponent)
    },
    {
      path: 'fs/farm/registration/:workflowId',
      loadComponent: () => import('./farm-supervisor/fs-farm-registration/fs-farm-registration.component').then((c) => c.FsFarmRegistrationComponent)
    },
    {
      path: 'fs/farm/modification/:workflowId',
      loadComponent: () => import('./farm-supervisor/fs-farm-modification/fs-farm-modification.component').then((c) => c.FsFarmModificationComponent)
    },
    {
      path: 'fs/farm/deletion/:workflowId',
      loadComponent: () => import('./farm-supervisor/fs-farm-deletion/fs-farm-deletion.component').then((c) => c.FsFarmDeletionComponent)
    },
    {
      path: 'fs/farm/:farmId',
      loadComponent: () => import('./farm-supervisor/fs-farm-view/fs-farm-view.component').then((c) => c.FsFarmViewComponent)
    },
    {
      path: 'fs/plan/update/:workflowId',
      loadComponent: () => import('./farm-supervisor/fs-update-plan/fs-update-plan.component').then((c) => c.FsUpdatePlanComponent)
    },
    {
      path: 'task/contract-cancellation/:workflowId',
      loadComponent: () => import('./fm-cancel-contract/cancel-contract.component').then((c) => c.CancelContractComponent)
    },
    {
      path: 'task/contract-renewal/:workflowId',
      loadComponent: () => import('./fm-renew-contract/renew-contract.component').then((c) => c.RenewContractComponent)
    },
    {
      path: 'task/contract-warning/:workflowId',
      loadComponent: () => import('./fm-warning-contract/warning-contract.component').then((c) => c.WarningContractComponent)
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FarmManagementRoutingModule {
}
