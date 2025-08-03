import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-land-admin',
  templateUrl: 'land-admin.component.html'
})
export class LandAdminComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
      route: 'dashboard',
      title: 'Pending Tasks',
      iconStyle: 'fa-tasks'
    },
    {
      route: 'farms',
      title: 'Search Farms and Farm Owners',
      iconStyle: 'fa-search'
    },
    {
      route : "report",
      title : "Report",
      iconStyle : "fa-archive"
    }
  ];

  constructor () {
  }

  ngOnInit(): void {
  }

}
