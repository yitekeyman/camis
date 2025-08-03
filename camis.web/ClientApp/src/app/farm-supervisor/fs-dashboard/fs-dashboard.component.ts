import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../_shared/dashboard/dashboard/interfaces';

@Component({
  selector: 'app-fs-dashboard',
  templateUrl: 'fs-dashboard.component.html'
})
export class FsDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    { type: 1, states: [1] },
    { type: 2, states: [1] },
    { type: 3, states: [1] },
    { type: 11, states: [1] },
  ];

  constructor(private router: Router) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url: string;
    switch (e.workflowTypeId) {
      case 1:
        url = `supervisor/farm/registration/${e.workflowId}`;
        break;
      case 2:
        url = `supervisor/farm/modification/${e.workflowId}`;
        break;
      case 3:
        url = `supervisor/farm/deletion/${e.workflowId}`;
        break;
      case 11:
        url = `supervisor/plan/update/${e.workflowId}`;
        break;
      default:
        url = `supervisor/dashboard`;
    }

    return this.router.navigate([url]);
  }

}
