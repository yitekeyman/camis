import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';

import {ApiService} from './api.service';
import {IActivityPlanTemplate} from "../_shared/project/activities/interfaces";
import {HttpParams} from "@angular/common/http";

@Injectable()
export class ProjectApiService {
  constructor(private api: ApiService) {
  }

  getAllActivityProgressMeasuringUnits(): Observable<any> {
    return this.api.get(`Projects/ActivityProgressMeasuringUnits`);
  }

  getAllActivityProgressVariables(): Observable<any> {
    return this.api.get(`Projects/ActivityProgressVariables`);
  }

  getAllActivityProgressVariableTypes(): Observable<any> {
    return this.api.get(`Projects/ActivityProgressVariableTypes`);
  }

  getAllActivityStatusTypes(): Observable<any> {
    return this.api.get(`Projects/ActivityStatusTypes`);
  }

  getAllActivityVariableValueLists(): Observable<any> {
    return this.api.get(`Projects/ActivityVariableValueLists`);
  }

  getAllActivityTags(): Observable<string[]> {
    return this.api.get(`Projects/ActivityTags`);
  }

  getAllActivityPlanDetailTags(): Observable<string[]> {
    return this.api.get(`Projects/ActivityPlanDetailTags`);
  }


  getActivityPlan(id: string): Observable<any> {
    return this.api.get(`Projects/ActivityPlan/${id}`);
  }

  getPlanFromRootActivity(activityId: string): Observable<any> {
    return this.api.get(`Projects/PlanFromRootActivity/${activityId}`);
  }

  getActivity(id: string): Observable<any> {
    return this.api.get(`Projects/Activity/${id}`);
  }

  getProgressReport(id: string): Observable<any> {
    return this.api.get(`Projects/ProgressReport/${id}`);
  }


  searchReports(planId: string, term: string, skip: number, take: number): Observable<any> {
    return this.api.get(`Projects/SearchReports/${planId}`,{params:{term:term,skip:skip.toString(),take:take.toString() }});
  }


  calculateProgress(activityId: string, reportTime?: number) {
    let params = new HttpParams();
    if (reportTime){
      params=params.set('reportTime', reportTime.toString());
    }
    return this.api.get(`Projects/CalculateProgress/${activityId}`,{params});
  }

  calculateResourceProgress(activityId: string, reportTime?: number) {
    let params = new HttpParams();
    if (reportTime){
      params=params.set('reportTime', reportTime.toString());
    }
    return this.api.get(`Projects/CalculateResourceProgress/${activityId}`,{params});
  }

  calculateOutcomeProgress(activityId: string, reportTime?: number) {
    let params = new HttpParams();
    if (reportTime){
      params=params.set('reportTime', reportTime.toString());
    }
    return this.api.get(`Projects/CalculateOutcomeProgress/${activityId}`,{params});
  }


  getLastWorkItem(workflowId: string): Observable<any> {
    return this.api.get(`Projects/LastWorkItem/${workflowId}`);
  }


  requestNewProgressReport(body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/RequestNewProgressReport`, body,{params});
  }

  acceptProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/AcceptProgressReport/${workflowId}`, null,{params});
  }

  surveyProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/SurveyProgressReport/${workflowId}`, null,{params});
  }

  surveyedProgressReport(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/SurveyedProgressReport/${workflowId}`, body,{params});
  }

  encodeProgressReport(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/EncodeProgressReport/${workflowId}`, body,{params});
  }

  rejectProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/RejectProgressReport/${workflowId}`, null,{params});
  }

  reportProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/ReportProgressReport/${workflowId}`, null,{params});
  }

  cancelProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/CancelProgressReport/${workflowId}`, null,{params});
  }

  approveProgressReport(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Projects/ApproveProgressReport/${workflowId}`, null,{params});
  }


  requestNewUpdatePlan(body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Projects/RequestNewUpdatePlan`, body,{params});
  }

  cancelUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Projects/CancelUpdatePlan/${workflowId}`, null,{params});
  }

  requestUpdatePlan(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Projects/RequestUpdatePlan/${workflowId}`, body,{params});
  }

  rejectUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Projects/RejectUpdatePlan/${workflowId}`, null,{params});
  }

  approveUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Projects/ApproveUpdatePlan/${workflowId}`, null,{params});
  }


  createActivityPlanTemplates(body: IActivityPlanTemplate): Observable<IActivityPlanTemplate> {
    return this.api.post(`Projects/ActivityPlanTemplate`, body);
  }

  getAllActivityPlanTemplates(): Observable<IActivityPlanTemplate[]> {
    return this.api.get(`Projects/ActivityPlanTemplates`);
  }

  updateActivityPlanTemplates(id: string, body: IActivityPlanTemplate): Observable<IActivityPlanTemplate> {
    return this.api.put(`Projects/ActivityPlanTemplate/${id}`, body);
  }

  deleteActivityPlanTemplates(id: string): Observable<IActivityPlanTemplate> {
    return this.api.delete(`Projects/ActivityPlanTemplate/${id}`);
  }
}
