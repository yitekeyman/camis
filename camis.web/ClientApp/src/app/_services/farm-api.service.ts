import {Injectable} from '@angular/core';
import {QueryEncoder} from '@angular/http';
import {Observable} from 'rxjs/Observable';

import {ApiService} from './api.service';
import {IWaitLandAssignmentRequest} from '../_shared/farm/interfaces';

@Injectable()
export class FarmApiService {

  private qs: QueryEncoder;

  constructor(private api: ApiService) {
    this.qs = new QueryEncoder();
  }


  getLandByUpin(upin: string): Observable<any> {
    return this.api.get(`LandBank/GetLand?upin=${this.qs.encodeValue(upin)}`);
  }


  getAllFarmOperatorTypes(): Observable<any> {
    return this.api.get(`Farms/FarmOperatorTypes`);
  }

  getAllFarmOperatorOrigins() {
    return this.api.get(`Farms/FarmOperatorOrigins`);
  }

  getAllFarmTypes(): Observable<any> {
    return this.api.get(`Farms/FarmTypes`);
  }

  getAllRegistrationAuthorities(): Observable<any> {
    return this.api.get(`Farms/RegistrationAuthorities`);
  }

  getAllRegistrationTypes(): Observable<any> {
    return this.api.get(`Farms/RegistrationTypes`);
  }

  getUPINs(): Observable<any> {
    return this.api.get(`Farms/UPINs`);
  }


  searchFarmOperators(term: string, skip: number, take: number): Observable<any> {
    return this.api.get(`Farms/SearchFarmOperators?term=${this.qs.encodeValue(term)}&skip=${this.qs.encodeValue(skip.toString())}&take=${this.qs.encodeValue(take.toString())}`);
  }

  searchFarms(term: string, skip: number, take: number): Observable<any> {
    return this.api.get(`Farms/SearchFarms?term=${this.qs.encodeValue(term)}&skip=${this.qs.encodeValue(skip.toString())}&take=${this.qs.encodeValue(take.toString())}`);
  }


  getFarmOperator(id: string): Observable<any> {
    return this.api.get(`Farms/FarmOperator/${id}`);
  }

  getFarm(id: string): Observable<any> {
    return this.api.get(`Farms/Farm/${id}`);
  }

  getFarmByActivity(activityId: string): Observable<any> {
    return this.api.get(`Farms/FarmByActivity/${activityId}`);
  }


  getLastWorkItem(workflowId: string): Observable<any> {
    return this.api.get(`Farms/LastWorkItem/${workflowId}`);
  }


  saveNewFarmRegistration(body: any, message: string | null): Observable<any> {
    return this.api.post(`Farms/SaveNewFarmRegistration${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  saveFarmRegistration(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.post(`Farms/SaveFarmRegistration/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  requestNewFarmRegistration(body: any, message: string | null): Observable<any> {
    return this.api.post(`Farms/RequestNewFarmRegistration${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  cancelFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Farms/CancelFarmRegistration/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  requestFarmRegistration(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.post(`Farms/RequestFarmRegistration/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  rejectFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Farms/RejectFarmRegistration/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  approveFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    return this.api.post(`Farms/ApproveFarmRegistration/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }


  requestNewFarmModification(body: any, message: string | null): Observable<any> {
    return this.api.put(`Farms/RequestNewFarmModification${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  cancelFarmModification(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Farms/CancelFarmModification/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  requestFarmModification(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.put(`Farms/RequestFarmModification/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  rejectFarmModification(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Farms/RejectFarmModification/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  approveFarmModification(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Farms/ApproveFarmModification/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }


  requestNewFarmDeletion(body: any, message: string | null): Observable<any> {
    return this.api.put(`Farms/RequestNewFarmDeletion${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  rejectFarmDeletion(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Farms/RejectFarmDeletion/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }

  approveFarmDeletion(workflowId: string, message: string | null): Observable<any> {
    return this.api.put(`Farms/ApproveFarmDeletion/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, null);
  }


  newWaitLandAssignment(body: IWaitLandAssignmentRequest, message: string | null): Observable<any> {
    return this.api.post(`Farms/NewWaitLandAssignment${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  waitLandAssignment(workflowId: string, body: IWaitLandAssignmentRequest, message: string | null): Observable<any> {
    return this.api.post(`Farms/WaitLandAssignment/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }

  getTransferStatus(workflowId: string): Observable<any> {
    return this.api.get(`Farms/TransferStatus/${workflowId}`);
  }

  certifyLandAssignment(workflowId: string, body: any, message: string | null): Observable<any> {
    return this.api.post(`Farms/CertifyLandAssignment/${workflowId}${message ? '?description=' + this.qs.encodeValue(message) : ''}`, body);
  }
}
