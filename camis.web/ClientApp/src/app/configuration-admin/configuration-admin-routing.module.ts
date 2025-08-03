import {NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';

import {ConfigurationAdminComponent} from './configuration-admin/configuration-admin.component';
import {CaActivityTemplatesComponent} from './ca-activity-templates/ca-activity-templates.component';

const routes: Route[] = [{
  path: '',
  component: ConfigurationAdminComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'activity-templates' },
    { path: 'activity-templates', component: CaActivityTemplatesComponent },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConfigurationAdminRoutingModule { }
