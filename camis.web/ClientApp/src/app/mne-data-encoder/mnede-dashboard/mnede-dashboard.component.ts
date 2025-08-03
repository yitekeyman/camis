import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../_shared/dashboard/dashboard/interfaces';

@Component({
  selector: 'app-mnede-dashboard',
  templateUrl: 'mnede-dashboard.component.html'
})
export class MnedeDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    { type: 5, states: [3] },
  ];

  constructor(private router: Router) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url = `mne-data-encoder/dashboard`;
    switch (e.workflowTypeId) {
      case 5: // progress report workflow
        url = `mne-data-encoder/pr/${e.workflowId}/ready`;
        break;
    }

    return this.router.navigateByUrl(url);
  }

}
