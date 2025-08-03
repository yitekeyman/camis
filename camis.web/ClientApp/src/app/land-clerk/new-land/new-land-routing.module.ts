import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {Route, RouterModule} from '@angular/router';

import { NewLandComponent } from './new-land/new-land.component';
import { NewLandDefaultComponent } from './new-land-default/new-land-default.component';
import { NewLandFormComponent } from './new-land-form/new-land-form.component';


const routes: Route[] = [{
   path: '', component: NewLandComponent,
   children: [
    { path: '', pathMatch: 'full', redirectTo: 'new-land' },
    { path: 'new-land', component: NewLandDefaultComponent },
    { path: 'new-land/new-land-form', component: NewLandFormComponent }
  ]
}];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ],
  exports: [RouterModule],
})
export class NewLandRoutingModule { }
