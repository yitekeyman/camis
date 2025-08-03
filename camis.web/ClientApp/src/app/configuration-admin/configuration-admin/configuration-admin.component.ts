import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-configuration-admin',
  templateUrl: 'configuration-admin.component.html'
})
export class ConfigurationAdminComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
      route: 'activity-templates',
      title: 'Activity Templates',
      iconStyle: 'fa-leaf'
    },
  ];

  constructor() {
  }

  ngOnInit(): void {
  }

}
