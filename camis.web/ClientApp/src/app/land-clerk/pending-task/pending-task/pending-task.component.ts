import { Component, OnInit } from '@angular/core';
import { IMainShellRoute } from '../../../_shared/main-shell/interfaces';


@Component({
  selector: 'app-pending-task',
  templateUrl: './pending-task.component.html',
  styleUrls: ['./pending-task.component.css']
})
export class PendingTaskComponent implements OnInit {

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
    title: 'Pending task',
    iconStyle: 'fa-archive'
  }
  ];

  constructor() { }

  ngOnInit() {
  }

}
