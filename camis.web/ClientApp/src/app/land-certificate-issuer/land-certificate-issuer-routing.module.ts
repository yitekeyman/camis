import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {LandCertificateIssuerComponent} from './land-certificate-issuer/land-certificate-issuer.component';
import {LciCertificationComponent} from './lci-certification/lci-certification.component';
import {LciDashboardComponent} from './lci-dashboard/lci-dashboard.component';
import {LciFarmsComponent} from './lci-farms/lci-farms.component';
import {LciFarmViewComponent} from './lci-farm-view/lci-farm-view.component';

const routes: Route[] = [{
  path: '',
  component: LandCertificateIssuerComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: LciDashboardComponent },
    { path: 'certification/:workflowId', component: LciCertificationComponent },
    { path: 'farms', component: LciFarmsComponent },
    { path: 'farm/:farmId', component: LciFarmViewComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LandCertificateIssuerRoutingModule { }
