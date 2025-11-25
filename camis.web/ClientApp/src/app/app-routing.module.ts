import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {MainShellComponent} from "./theme/layout/mainShell/mainShell.component";
import {DefaultComponent} from "./default/default.component";
import {DefaultModule} from "./default/default.module";
import {EnvironmentalMonitoringModule} from "./environmental-monitoring/environmental-monitoring.module";

const routes: Routes = [
  {path: 'login', loadComponent:()=>import('./login/login.component').then((c)=>c.LoginComponent)},
  {
    path: 'default',
    loadChildren: () => import('./default/default.module').then((c) => c.DefaultModule)
  },
  {path: 'admin', loadChildren: ()=>import('./admin/admin.module').then((c)=>c.AdminModule)},
  {path:'land-bank', loadChildren:()=>import('./land-bank/land-bank.module').then((c)=>c.LandBankModule)},
  {path:'farm-management', loadChildren:()=>import('./farm-management/farm-management.module').then((c)=>c.FarmManagementModule)},
  {path:'mne', loadChildren:()=>import('./mne/mne.module').then((c)=>c.MneModule)},
  {path:'environmental-monitoring', loadChildren:()=>import('./environmental-monitoring/environmental-monitoring.module').then((c)=>c.EnvironmentalMonitoringModule)},
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
