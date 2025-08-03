import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-mne-expert',
  templateUrl: 'mne-expert.component.html'
})
export class MneExpertComponent implements OnInit {

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
