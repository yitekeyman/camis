import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../../_shared/dashboard/dashboard/interfaces';
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";

@Component({
  selector: 'app-fs-dashboard',
  imports:[CommonModule, ReactiveFormsModule],
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
        url = `farm-management/fs/farm/registration/${e.workflowId}`;
        break;
      case 2:
        url = `farm-management/fs/farm/modification/${e.workflowId}`;
        break;
      case 3:
        url = `farm-management/fs/farm/deletion/${e.workflowId}`;
        break;
      case 11:
        url = `farm-management/fs/plan/update/${e.workflowId}`;
        break;
      default:
        url = `default/pending-task`;
    }

    return this.router.navigate([url]);
  }

}
