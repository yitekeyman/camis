import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";
import {DefaultComponent} from "./default.component";
import {DashboardComponent} from "./dashboard/dashboard.component";
import {MainShellComponent} from "../theme/layout/mainShell/mainShell.component";

const routes: Routes = [

  {
    path: '',
    component: DefaultComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then((c) =>c.DashboardComponent),
      },
      {
        path: 'pending-task',
        loadComponent: () => import('./pendingTask/pendingTask.component').then((c) => c.PendingTaskComponent)
      },
      {
        path:'reports',
        loadComponent:()=>import('./../_shared/report/generate-report/generate-report.component').then((c) => c.GenerateReportComponent)
      }
    ]
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DefaultRoutingModule {}
