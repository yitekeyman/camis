import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';

import {LandCertificateIssuerRoutingModule} from './land-certificate-issuer-routing.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {DocumentModule} from '../_shared/document/document.module';

import {LandCertificateIssuerComponent} from './land-certificate-issuer/land-certificate-issuer.component';
import {LciCertificationComponent} from './lci-certification/lci-certification.component';
import {LciDashboardComponent} from './lci-dashboard/lci-dashboard.component';
import {LciFarmsComponent} from './lci-farms/lci-farms.component';
import {LciFarmViewComponent} from './lci-farm-view/lci-farm-view.component';
import {FormsModule} from "@angular/forms";

@NgModule({
  declarations: [
    LandCertificateIssuerComponent,
    LciCertificationComponent,
    LciDashboardComponent,
    LciFarmsComponent,
    LciFarmViewComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    LandCertificateIssuerRoutingModule,

    DashboardModule,
    FarmModule,
    MainShellModule,
    DocumentModule,
  ],
  providers: []
})
export class LandCertificateIssuerModule { }
