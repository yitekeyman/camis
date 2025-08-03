import {Injectable} from '@angular/core';
import {Observable} from 'rxjs/Observable';

import {ApiService} from './api.service';

@Injectable()
export class WorkflowApiService {

  constructor(private api: ApiService) {
  }


  getUserWorkflows(): Observable<any> {
    return this.api.get(`Workflows/UserWorkflows`);
  }


  getWorkItems(workflowId: string) {
    return this.api.get(`Workflows/WorkItems/${workflowId}`);
  }

  getLastWorkItem(workflowId: string) {
    return this.api.get(`Workflows/LastWorkItem/${workflowId}`);
  }

}
