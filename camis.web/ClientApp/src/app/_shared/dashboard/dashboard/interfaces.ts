import {Observable} from 'rxjs/Observable';

export interface IDashboardStates {
  type: number;
  states: number[];
  asyncMsg$?: (e: IWorkflowOpenEvent) => Observable<string>;
}

export interface IWorkflowOpenEvent {
  workflowId: string;
  workflowTypeId: number;
  currentState: number;
}
