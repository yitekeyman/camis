import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-farm-clerk',
  templateUrl: 'farm-clerk.component.html'
})
export class FarmClerkComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
      route: 'dashboard',
      title: 'Pending Tasks',
      iconStyle: 'fa-tasks'
    },
    {
      route: 'farm/management',
      title: 'Search Farms and Farm Owners',
      iconStyle: 'fa-search'
    },
    {
      route: 'farm/registration/new',
      title: 'Commercial Farm Registration',
      iconStyle: 'fa-plus-circle'
    }
  ];

  constructor () {
  }

  ngOnInit(): void {
  }

}
