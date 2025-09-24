import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {MainShellComponent} from "./theme/layout/mainShell/mainShell.component";

const routes: Routes = [
  {path: 'login', loadComponent:()=>import('./login/login.component').then((c)=>c.LoginComponent)},
  {
    path: '',
    component: MainShellComponent,
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/default/default.component').then((c) => c.DefaultComponent)
      },
      {
        path: 'pending-task',
        loadComponent: () => import('./pendingTask/pendingTask.component').then((c) => c.PendingTaskComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
