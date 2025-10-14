import {Route, RouterModule} from "@angular/router";
import {NgModule} from "@angular/core";
import {MneComponent} from "./mne.component";

const routes: Route[] = [{
  path: '',
  component: MneComponent,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class MneRoutingModule{}
