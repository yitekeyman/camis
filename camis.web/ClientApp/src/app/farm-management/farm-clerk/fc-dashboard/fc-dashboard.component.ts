import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../../_shared/dashboard/dashboard/interfaces';
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {DashboardComponent} from "../../../_shared/dashboard/dashboard/dashboard.component";

@Component({
  selector: 'app-fc-dashboard',
  imports:[CommonModule, ReactiveFormsModule, DashboardComponent],
  templateUrl: 'fc-dashboard.component.html'
})
export class FcDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    { type: 1, states: [0] },
    { type: 2, states: [0] },
    { type: 3, states: [0] },
    { type: 11, states: [0] },
  ];

  constructor(private router: Router) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url: string;
    switch (e.workflowTypeId) {
      case 1:
        url = `farm-management/fc/farm/registration/${e.workflowId}`;
        break;
      case 2:
        url = `farm-management/fc/farm/modification/${e.workflowId}`;
        break;
      case 11:
        url = `farm-management/fc/plan/update/${e.workflowId}`;
        break;
      default:
        url = `default/pending-task`;
    }

    return this.router.navigateByUrl(url);
  }
}
