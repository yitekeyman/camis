import {Injectable} from '@angular/core';
import {QueryEncoder} from '@angular/http';
import {Observable} from 'rxjs/Observable';

import {ApiService} from './api.service';
import {IActivityPlanTemplate} from "../_shared/project/activities/interfaces";

@Injectable()
export class ProjectApiService {

  private qs: QueryEncoder;

  constructor(private api: ApiService) {
    this.qs = new QueryEncoder();
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
    return this.api.get(`Projects/SearchReports/${planId}?term=${this.qs.encodeValue(term)}&skip=${this.qs.encodeValue(skip.toString())}&take=${this.qs.encodeValue(take.toString())}`);
  }


  calculateProgress(activityId: string, reportTime?: number) {
    return this.api.get(`Projects/CalculateProgress/${activityId}${reportTime ? '?reportTime=' + this.qs.encodeValue('' + reportTime) : ''}`);
  }

  calculateResourceProgress(activityId: string, reportTime?: number) {
    return this.api.get(`Projects/CalculateResourceProgress/${activityId}${reportTime ? '?reportTime=' + this.qs.encodeValue('' + reportTime) : ''}`);
  }

  calculateOutcomeProgress(activityId: string, reportTime?: number) {
    return this.api.get(`Projects/CalculateOutcomeProgress/${activityId}${reportTime ? '?reportTime=' + this.qs.encodeValue('' + reportTime) : ''}`);
  }


  getLastWorkItem(workflowId: string): Observable<any> {
    return this.api.get(`Projects/LastWorkItem/${workflowId}`);
  }


  requestNewProgressReport(body: any, message: string | null): Observable<any> {
    return this.api.post(`Projects/RequestNewProgressReport${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  acceptProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/AcceptProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  surveyProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/SurveyProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  surveyedProgressReport(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.post(`Projects/SurveyedProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  encodeProgressReport(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.post(`Projects/EncodeProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  rejectProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/RejectProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  reportProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/ReportProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  cancelProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/CancelProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  approveProgressReport(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Projects/ApproveProgressReport/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }


  requestNewUpdatePlan(body: any, message: string | null): Observable<any> {
    return this.api.put(`Projects/RequestNewUpdatePlan${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  cancelUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Projects/CancelUpdatePlan/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  requestUpdatePlan(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.put(`Projects/RequestUpdatePlan/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  rejectUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Projects/RejectUpdatePlan/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  approveUpdatePlan(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Projects/ApproveUpdatePlan/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
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
