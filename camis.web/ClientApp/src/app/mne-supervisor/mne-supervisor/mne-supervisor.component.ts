import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-mne-supervisor',
  templateUrl: 'mne-supervisor.component.html'
})
export class MneSupervisorComponent implements OnInit {

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
  ];

  constructor() {
  }

  ngOnInit(): void {
  }

}
