import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../_shared/dashboard/dashboard/interfaces';

@Component({
  selector: 'app-mnee-dashboard',
  templateUrl: 'mnee-dashboard.component.html'
})
export class MneeDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    { type: 5, states: [0, 1, 2, 4] },
  ];

  constructor(private router: Router) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url = `mne-expert/dashboard`;
    switch (e.workflowTypeId) {
      case 5: // progress report workflow
        switch (e.currentState) {
          case 0:
            url = `mne-expert/pr/${e.workflowId}/requested`;
            break;
          case 1:
            url = `mne-expert/pr/${e.workflowId}/accepted`;
            break;
          case 2:
            url = `mne-expert/pr/${e.workflowId}/surveying`;
            break;
          case 4:
            url = `mne-expert/pr/${e.workflowId}/reviewing`;
            break;
        }
       break;
    }

    return this.router.navigateByUrl(url);
  }

}
