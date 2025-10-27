import {Route, RouterModule} from "@angular/router";
import {NgModule} from "@angular/core";
import {MneComponent} from "./mne.component";
import {MneFarmViewerComponent} from "./mne-farm-viewer/mne-farm-viewer.component";
import {MneReportViewComponent} from "./mne-report-view/mne-report-view.component";
import {MneReportsComponent} from "./mne-reports/mne-reports.component";
import {MnePrReadyComponent} from "./mne-pr-ready/mne-pr-ready.component";
import {MnePrReportedComponent} from "./mne-pr-reported/mne-pr-reported.component";
import {MnePrNewRequestComponent} from "./mne-pr-new-request/mne-pr-new-request.component";
import {MnePrReviewingComponent} from "./mne-pr-reviewing/mne-pr-reviewing.component";
import {MnePrSurveyingComponent} from "./mne-pr-surveying/mne-pr-surveying.component";
import {MnePrAcceptedComponent} from "./mne-pr-accepted/mne-pr-accepted.component";
import {MnePrRequestedComponent} from "./mne-pr-requested/mne-pr-requested.component";
import {FcFarmManagementComponent} from "../farm-management/farm-clerk/fc-farm-management/fc-farm-management.component";

const routes: Route[] = [{
  path: '',
  component: MneComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'search-farm' },
    {path: 'search-farm', loadComponent:()=>import('./mne-farm-management/mne-farm-management.component').then((c)=>c.MneFarmManagementComponent)},
    {path: 'farm/:farmId', loadComponent:()=>import('./mne-farm-viewer/mne-farm-viewer.component').then((c)=>c.MneFarmViewerComponent)},
    {path:'task/pr/:workflowId/requested', loadComponent:()=>import('./mne-pr-requested/mne-pr-requested.component').then((c)=>c.MnePrRequestedComponent)},
    {path:'task/pr/:workflowId/accepted', loadComponent:()=>import('./mne-pr-accepted/mne-pr-accepted.component').then((c)=>c.MnePrAcceptedComponent)},
    {path:'task/pr/:workflowId/surveying', loadComponent:()=>import('./mne-pr-surveying/mne-pr-surveying.component').then((c)=>c.MnePrSurveyingComponent)},
    {path:'task/pr/:workflowId/reviewing', loadComponent:()=>import('./mne-pr-reviewing/mne-pr-reviewing.component').then((c)=>c.MnePrReviewingComponent)},
    {path:'task/pr/new/request', loadComponent:()=>import('./mne-pr-new-request/mne-pr-new-request.component').then((c)=>c.MnePrNewRequestComponent)},
    {path:'task/pr/:workflowId/reported', loadComponent:()=>import('./mne-pr-reported/mne-pr-reported.component').then((c)=>c.MnePrReportedComponent)},
    {path:'task/pr/:workflowId/ready', loadComponent:()=>import('./mne-pr-ready/mne-pr-ready.component').then((c)=>c.MnePrReadyComponent)},
    {path:'plan/:planId/reports', loadComponent:()=>import('./mne-reports/mne-reports.component').then((c)=>c.MneReportsComponent)},
    {path:'report/:reportId', loadComponent:()=>import('./mne-report-view/mne-report-view.component').then((c)=>c.MneReportViewComponent)},

  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class MneRoutingModule{}
