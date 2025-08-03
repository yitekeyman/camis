import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Observable} from 'rxjs/observable';

import {IDashboardStates, IWorkflowOpenEvent} from './interfaces';
import {WorkflowApiService} from '../../../_services/workflow-api.service';
import dialog from '../../dialog';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: 'dashboard.component.html'
})
export class DashboardComponent implements OnInit {

  @Input('customApi')
  customApi: Observable<any>;
  @Input('title')
  title = 'Dashboard';
  @Input('filters')
  filters: IDashboardStates[] = [];

  @Output('open')
  open = new EventEmitter<IWorkflowOpenEvent>();

  loading = true;
  activeWorkflows: any[] = [];
  involvedWorkflows: any[] = [];

  constructor (private workflowApi: WorkflowApiService, private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit(): void {
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
            } else {
              this.involvedWorkflows.push(workflow);
            }
          }
        }
      }
      this.loading = false;
    }, dialog.error);
  }

}
