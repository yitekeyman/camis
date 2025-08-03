import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';

const routes: Route[] = [
  { path: 'admin', loadChildren: 'app/admin/admin.module#AdminModule' },
  { path: 'configuration-admin', loadChildren: 'app/configuration-admin/configuration-admin.module#ConfigurationAdminModule' },
  { path: 'clerk', loadChildren: 'app/farm-clerk/farm-clerk.module#FarmClerkModule' },
  { path: 'supervisor', loadChildren: 'app/farm-supervisor/farm-supervisor.module#FarmSupervisorModule' },
  { path: 'land-clerk', loadChildren: 'app/land-clerk/land-clerk.module#LandClerkModule' },
  { path: 'land-supervisor', loadChildren: 'app/land-supervisor/land-supervisor.module#LandSupervisorModule' },
  { path: 'land-admin', loadChildren: 'app/land-admin/land-admin.module#LandAdminModule' },
  {
    path: 'land-certificate-issuer',
    loadChildren: 'app/land-certificate-issuer/land-certificate-issuer.module#LandCertificateIssuerModule'
  },
  { path: 'login', loadChildren: 'app/login/login.module#LoginModule' },
  { path: 'mne-data-encoder', loadChildren: 'app/mne-data-encoder/mne-data-encoder.module#MneDataEncoderModule' },
  { path: 'mne-expert', loadChildren: 'app/mne-expert/mne-expert.module#MneExpertModule' },
  { path: 'mne-supervisor', loadChildren: 'app/mne-supervisor/mne-supervisor.module#MneSupervisorModule' },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forRoot(routes),
  ],
  exports: [RouterModule],
  declarations: []
})
export class AppRoutingModule { }
