import { Component, OnInit } from '@angular/core';
import { IMainShellRoute } from '../../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-land-dashboard',
  templateUrl: './land-dashboard.component.html',
  styleUrls: ['./land-dashboard.component.css']
})
export class LandDashboardComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
        route: 'land-dashboard',
        title: 'Search Land Bank',
        iconStyle: 'fa-search'
    },
    {
      route: 'pending-task',
      title: 'Pending Task',
      iconStyle: 'fa-archive'
    }, {
      route: 'report',
      title: "Report",
      iconStyle : "fa-paper"
    }
  ];

  constructor() { }

  ngOnInit() {
  }

}
