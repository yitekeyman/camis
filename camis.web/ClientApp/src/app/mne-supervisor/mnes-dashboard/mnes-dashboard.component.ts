import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../_shared/dashboard/dashboard/interfaces';

@Component({
  selector: 'app-mnes-dashboard',
  templateUrl: 'mnes-dashboard.component.html'
})
export class MnesDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    { type: 5, states: [5] },
  ];

  constructor(private router: Router) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url = `mne-supervisor/dashboard`;
    switch (e.workflowTypeId) {
      case 5: // progress report workflow
        url = `mne-supervisor/pr/${e.workflowId}/reported`;
        break;
    }

    return this.router.navigateByUrl(url);
  }

}
