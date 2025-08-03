import { Component, OnInit } from '@angular/core';
import { IMainShellRoute } from '../../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-land-dashboard',
  templateUrl: './land-dashboard.component.html'
})
export class LandDashboardComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
        route: 'land-dashboard',
        title: 'Search Land Bank',
        iconStyle: 'fa-search'
    },
    {
      route: 'new-land',
      title: 'Land Profile Registration',
      iconStyle: 'fa fa-file'
    },
    {
      route: 'pending-task',
      title: 'Pending tasks',
      iconStyle: 'fa fa-archive'
    }
  ];

  constructor() { }

  ngOnInit(): void {
  }

}
