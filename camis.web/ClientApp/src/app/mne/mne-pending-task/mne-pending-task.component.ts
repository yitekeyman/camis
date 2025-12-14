import {Component, Input, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {IDashboardStates, IWorkflowOpenEvent} from "../../default/pendingTask/interfaces";
import {Observable} from "rxjs";
import {WorkflowApiService} from "../../_services/workflow-api.service";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {PagerService} from "../../_services/pager.service";
import {Router} from "@angular/router";
import {FarmApiService} from "../../_services/farm-api.service";
import dialog from "../../_shared/dialog";

@Component({
  selector: "app-mne-pending-task",
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: "./mne-pending-task.component.html",
})
export class MnePendingTaskComponent implements OnInit {
  @Input('customApi')
  clerkRole = false;
  loginRole = '0';
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

  constructor(private workflowApi: WorkflowApiService, private keyCase: ObjectKeyCasingService, private pagerService: PagerService, private router: Router, private api: FarmApiService) {
    this.loginRole = localStorage.getItem("role");
    if (this.loginRole === '8') {
      this.user = 'M&E Expert';
      this.filters = [
        {
          type: 5, states: [0, 1, 2, 4]
        }
      ]
    }
    if (this.loginRole === '9') {
      this.user = 'M&E Supervisor';
      this.filters = [
        {type: 5, states: [5]}
      ];
    }
    if (this.loginRole === '10') {
      this.user = 'M&E Data Encoder';
      this.filters = [
        {type: 5, states: [3]}
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
    if (this.loginRole === '8') {
      switch (e.workflowTypeId) {
        case 5:
          switch (e.currentState) {
            case 0:
              url = `mne/task/pr/${e.workflowId}/requested`;
              break;
            case 1:
              url = `mne/task/pr/${e.workflowId}/accepted`;
              break;
            case 2:
              url = `mne/task/pr/${e.workflowId}/surveying`;
              break;
            case 4:
              url = `mne/task/pr/${e.workflowId}/reviewing`;
              break;
          }
          break;
        default:
          url = `default/pending-task`;
      }
    } else if (this.loginRole === '9') {
      switch (e.workflowTypeId) {
        case 5:
          url = `mne/task/pr/${e.workflowId}/reported`;
          break;
        default:
          url = `default/pending-task`;
      }
    } else if (this.loginRole === '10') {
      switch (e.workflowTypeId) {
        case 5:
          url = `mne/task/pr/${e.workflowId}/ready`;
          break;
        default:
          url = `default/pending-task`;
      }
    }

    this.router.navigate([url]);
  }
}
