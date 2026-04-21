import {Component, Input, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {WorkflowApiService} from "../../_services/workflow-api.service";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import dialog from "../../_shared/dialog";
import {Observable} from "rxjs";
import {IDashboardStates, IWorkflowOpenEvent} from "../../default/pendingTask/interfaces";
import {PagerService} from "../../_services/pager.service";
import {Router} from "@angular/router";
import {FarmApiService} from "../../_services/farm-api.service";

@Component({
  selector: "app-fm-pending-task",
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: 'fm-pending-task.component.html'
})

export class FmPendingTaskComponent implements OnInit {
  @Input('customApi')
  clerkRole = false;
  loginRole = '';
  user = '';
  noItem = false;
  public pager: any = {};
  pagedItems: any[];
  filters: IDashboardStates[] = [
    {type: 1, states: [1]},
    {type: 2, states: [1]},
    {type: 3, states: [1]},
    {type: 11, states: [1]},
  ];
  public activeWorkflows: any = [] = [];
  customApi: Observable<any>;

  constructor(private workflowApi: WorkflowApiService, public keyCase: ObjectKeyCasingService, private pagerService: PagerService, private router: Router, private api: FarmApiService) {
    this.loginRole = localStorage.getItem("role");
    if (this.loginRole === '2') {
      this.user = 'Commercial Farm Registrar';
      this.filters = [
        { type: 1, states: [0] },
        { type: 2, states: [0] },
        { type: 3, states: [0] },
        { type: 11, states: [0] },
      ];
    }
    if (this.loginRole === '3') {
      this.user = 'Commercial Farm Supervisor';
      this.filters = [
        { type: 1, states: [1] },
        { type: 2, states: [1] },
        { type: 3, states: [1] },
        { type: 11, states: [1] },
      ];
    }
    if (this.loginRole === '6') {
      this.user = 'Land Bank Administrator';
      this.filters = [
        {
          type: 10,
          states: [2,3,4],
          asyncMsg$: (e: IWorkflowOpenEvent): Observable<any> => {
            return Observable.create(observer => {
              observer.next('Loading...');

              this.api.getTransferStatus(e.workflowId).subscribe(res => {
                if (res && res.status == -99) {
                  observer.next('Refreshing...');
                  window.location.reload();
                } else {
                  let status = 'Unknown';
                  switch (res.status) {
                    case 0:
                      status = 'Initial';
                      break;
                    case 1:
                      status = 'Waiting For NRLAIS';
                      break;
                    case -2:
                      status = 'Executed';
                      break;
                    case -3:
                      status = 'Cancelled';
                      break;
                  }

                  observer.next('Status: ' + status);
                }
              }, dialog.error);

              return () => {
              };
            });
          }
        },
        {
          type: 7,states:[2,3,4,5,6,7,8],
          asyncMsg$: (e: IWorkflowOpenEvent): Observable<any> => {
            return Observable.create(observer => {
              observer.next('Loading...');
              this.api.getTransferStatus(e.workflowId).subscribe(res => {
                if (res && res.status == -99) {
                  observer.next('Refreshing...');
                  window.location.reload();
                } else {
                  let status = 'Unknown';
                  switch (res.status) {
                    case 0:
                      status = 'Initial';
                      break;
                    case 1:
                      status = 'Started';
                      break;
                    case 2:
                      status = 'Parcel Split Requested';
                      break;
                    case 3:
                      status = 'Waiting For NRLAIS';
                      break;
                    case 4:
                      status = 'Waiting For CMSS';
                      break;
                    case 5:
                      status = 'NRLAIS Approved';
                      break;
                    case 6:
                      status = 'NRLAIS Rejected';
                      break;
                    case 7:
                      status = 'CMSS Done Split';
                      break;
                    case 8:
                      status = 'CMSS Rejected';
                      break;
                    case 11:
                      status = 'Rejected';
                      break;
                    case -2:
                      status = 'Executed';
                      break;
                    case -3:
                      status = 'Approved';
                      break;
                    case -4:
                      status = 'Cancelled';
                      break;
                  }

                  observer.next('Status: ' + status);
                }
              }, dialog.error);

              return () => {
              };
            });
          }
        }
      ];
    }
    if (this.loginRole === '7') {
      this.user = 'Land Bank Certificate Issuer';
      this.filters = [
        {
          type: 10,
          states: [4],
          asyncMsg$: (e: IWorkflowOpenEvent): Observable<any> => {
            return Observable.create(observer => {
              observer.next('Loading...');

              this.api.getTransferStatus(e.workflowId).subscribe(res => {
                if (res && res.status == -99) {
                  observer.next('Refreshing...');
                  window.location.reload();
                } else {
                  let status = 'Unknown';
                  switch (res.status) {
                    case 0:
                      status = 'Initial';
                      break;
                    case 1:
                      status = 'Waiting For NRLAIS';
                      break;
                    case -2:
                      status = 'Executed';
                      break;
                    case -3:
                      status = 'Cancelled';
                      break;
                  }

                  observer.next('Status: ' + status);
                }
              }, dialog.error);

              return () => {
              };
            });
          }
        },
      ];
    }


  }

  ngOnInit() {
    this.getTask();
  }

  getTask() {
    dialog.loading();
    (this.customApi || this.workflowApi.getUserWorkflows()).subscribe(workflows => {
      this.keyCase.camelCase(workflows);
      for (const filter of this.filters) {
        for (const workflow of workflows) {
          if (filter.asyncMsg$) {
            workflow.asyncMsgValue$ = filter.asyncMsg$({
              workflowId: workflow.id,
              workflowTypeId: workflow.typeId,
              currentState: workflow.currentState
            });
          }
          if (workflow.typeId == filter.type) {
            if (filter.states.indexOf(workflow.currentState) >= 0) {
              this.activeWorkflows.push(workflow);
            }
          }

        }
      }
      this.setPage(1);
      dialog.close();
    }, dialog.error);
  }

  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.activeWorkflows.length, page);

    //get the paged items
    this.pagedItems = this.activeWorkflows.slice(this.pager.startIndex, this.pager.endIndex + 1);

  }

  workOnThis(e: IWorkflowOpenEvent) {
    let url: string;
    if (this.loginRole === '2') {
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
    } else if (this.loginRole === '3') {
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
    } else if (this.loginRole === '6') {
      switch (e.workflowTypeId) {
        case 7:
          url=`land-bank/task/parcel-details/${e.workflowId}`;
          break;
        case 10:
          switch (e.currentState) {
            case 2:
              url = `land-bank/task/land-selection/${e.workflowId}`;
              break;
            case 4:
              url = `land-bank/task/certification/${e.workflowId}`;
              break;
          }
          break;
        default:
          url = `default/pending-task`;
      }
    } else if (this.loginRole === '7') {
      switch (e.workflowTypeId) {
        case 10:
          url = `land-bank/task/certification/${e.workflowId}`;
          break;
        default:
          url = `default/pending-task`;
      }
    }

    this.router.navigate([url]);
  }
}
