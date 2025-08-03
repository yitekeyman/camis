import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-farm-supervisor',
  templateUrl: 'farm-supervisor.component.html'
})
export class FarmSupervisorComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
      route: 'dashboard',
      title: 'Pending Tasks',
      iconStyle: 'fa-tasks'
    },
    {
      route: 'farm/list',
      title: 'Search Farms and Farm Owners',
      iconStyle: 'fa-search'
    }
  ];

  constructor () {
  }

  ngOnInit(): void {
  }

}
