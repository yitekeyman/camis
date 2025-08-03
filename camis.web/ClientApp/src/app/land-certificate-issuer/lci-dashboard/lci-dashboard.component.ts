import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {IDashboardStates, IWorkflowOpenEvent} from '../../_shared/dashboard/dashboard/interfaces';
import {FarmApiService} from '../../_services/farm-api.service';
import {Observable} from 'rxjs/Observable';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-lci-dashboard',
  templateUrl: 'lci-dashboard.component.html'
})
export class LciDashboardComponent implements OnInit {

  filters: IDashboardStates[] = [
    {
      type: 10,
      states: [4],
      asyncMsg$: (e: IWorkflowOpenEvent): Observable<any> => {
        return Observable.create(observer => {
          observer.next('Loading...');

          this.api.getTransferStatus(e.workflowId).subscribe(res => {
            if (res && res.status == -99) {
              observer.next('Refreshing...');
              window.location.reload(true);
            } else {
              let status = 'Unknown';
              switch (res.status) {
                case 0: status = 'Initial'; break;
                case 1: status = 'Waiting For NRLAIS'; break;
                case -2: status = 'Executed'; break;
                case -3: status = 'Cancelled'; break;
              }

              observer.next('Status: ' + status);
            }
          }, dialog.error);

          return () => {};
        });
      }
    },
  ];

  constructor(private router: Router, private api: FarmApiService) {
  }

  ngOnInit(): void {
  }

  onWorkflowOpen(e: IWorkflowOpenEvent): Promise<boolean> {
    let url: string;
    switch (e.workflowTypeId) {
      case 10:
        url = `land-certificate-issuer/certification/${e.workflowId}`;
        break;
      default:
        url = `land-certificate-issuer/dashboard`;
    }

    return this.router.navigate([url]);
  }

}
