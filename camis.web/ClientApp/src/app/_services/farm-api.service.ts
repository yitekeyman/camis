import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';

import {ApiService} from './api.service';
import {IWaitLandAssignmentRequest} from '../_shared/farm/interfaces';
import {HttpParams} from "@angular/common/http";

@Injectable()
export class FarmApiService {



  constructor(private api: ApiService) {

  }


  getLandByUpin(upin: string): Observable<any> {
    return this.api.get(`LandBank/GetLand`,{params:{upin:upin}});
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
    return this.api.get(`Farms/SearchFarmOperators`,{params:{term:term,skip:skip.toString(),take:take.toString() }});
  }

  searchFarms(term: string, ownerType:number, farmType:number, farmStatus:number,  skip: number, take: number): Observable<any> {
    return this.api.get(`Farms/SearchFarms`,{params:{term:term,ownerType:ownerType.toString(), farmType:farmType.toString(), status:farmStatus.toString(), skip:skip.toString(),take:take.toString() }});
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
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/SaveNewFarmRegistration`, body, {params});
  }

  saveFarmRegistration(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/SaveFarmRegistration/${workflowId}`, body,{params});
  }

  requestNewFarmRegistration(body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RequestNewFarmRegistration`, body,{params});
  }

  cancelFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/CancelFarmRegistration/${workflowId}`, null,{params});
  }

  requestFarmRegistration(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RequestFarmRegistration/${workflowId}`, body,{params});
  }

  rejectFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RejectFarmRegistration/${workflowId}`, null,{params});
  }

  approveFarmRegistration(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/ApproveFarmRegistration/${workflowId}`, null,{params});
  }


  requestNewFarmModification(body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RequestNewFarmModification`, body,{params});
  }

  cancelFarmModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/CancelFarmModification/${workflowId}`, null,{params});
  }

  requestFarmModification(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RequestFarmModification/${workflowId}`, body,{params});
  }

  rejectFarmModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectFarmModification/${workflowId}`, null,{params});
  }

  approveFarmModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveFarmModification/${workflowId}`, null,{params});
  }


  requestNewFarmDeletion(body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RequestNewFarmDeletion`, body,{params});
  }

  rejectFarmDeletion(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectFarmDeletion/${workflowId}`, null,{params});
  }

  approveFarmDeletion(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveFarmDeletion/${workflowId}`, null,{params});
  }


  newWaitLandAssignment(body: IWaitLandAssignmentRequest, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/NewWaitLandAssignment`, body,{params});
  }

  waitLandAssignment(workflowId: string, body: IWaitLandAssignmentRequest, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/WaitLandAssignment/${workflowId}`, body,{params});
  }

  getTransferStatus(workflowId: string): Observable<any> {
    return this.api.get(`Farms/TransferStatus/${workflowId}`);
  }

  certifyLandAssignment(workflowId: string, body: any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/CertifyLandAssignment/${workflowId}`, body,{params});
  }
  getFarmByLandId(id: string): Observable<any> {
    return this.api.get(`Farms/FarmByLandId/${id}`);
  }
  getUPINsWithSplitParts():Observable<any[]> {
    return this.api.get(`Farms/GetUPINsWithSplitParts`);
  }
  getSplitPartsByUpin(upid: string): Observable<any> {
    return this.api.get(`Farms/GetSplitPartsByUpin/${upid}`);
  }
  getFarmStatusTypeList(){
    return this.api.get(`Lookup/FarmStatusType`);
  }
  GetLandRightsByLandIdandFarmId(land_id: string, farm_id: string, part_id: number): Observable<any> {
    return this.api.get(`Farms/GetLandRightsByLandIdandFarmId`,{params:{landId:land_id,farmId:farm_id,partId:part_id.toString() }});
  }
  GetAllLandRightsByLandId(land_id: string): Observable<any> {
    return this.api.get(`Farms/GetAllLandRightsByLandId/${land_id}`);
  }
  GetAllLandRightsByFarmId(farm_id: string): Observable<any> {
    return this.api.get(`Farms/GetAllLandRightsByFarmId/${farm_id}`);
  }
  getUserWorkItem(workflowId: string): Observable<any> {
    return this.api.get(`LandBank/GetUserWorkItem?id=${workflowId}`);
  }
  approveLandAssignment(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveLandAssignment/${workflowId}`, null,{params});
  }
  rejectLandAssignment(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectLandAssignment/${workflowId}`, null,{params});
  }
  getFarmLands(id: string): Observable<any> {
    //return this.api.get(`Farms/GetFarmLandByFarmId/${id}`);
    return this.api.get(`Farms/GetFarmLandByFarmId/${id}`);
  }

  requestContractCancellation(wfid:string, body:any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RequestContractCancellation/${wfid}`, body,{params});
  }
  approveContractCancellation(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveContractCancellation/${workflowId}`, null,{params});
  }
  rejectContractCancellation(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectContractCancellation/${workflowId}`, null,{params});
  }
  cancelContractCancellation(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/CancelContractCancellation/${workflowId}`, null,{params});
  }
  requestContractModification(wfid:string, body:any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RequestContractModification/${wfid}`, body,{params});
  }
  approveContractModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveContractModification/${workflowId}`, null,{params});
  }
  rejectContractModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectContractModification/${workflowId}`, null,{params});
  }
  cancelContractModification(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/CancelContractModification/${workflowId}`, null,{params});
  }
  getAllModificationReasonList(): Observable<any> {
    return this.api.get(`Farms/GetAllModificationReasonList`);
  }

  requestContractWarning(wfid:string, body:any, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.post(`Farms/RequestContractWarning/${wfid}`, body,{params});
  }
  approveContractWarning(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/ApproveContractWarning/${workflowId}`, null,{params});
  }
  rejectContractWarning(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/RejectContractWarning/${workflowId}`, null,{params});
  }
  cancelContractWarning(workflowId: string, message: string | null): Observable<any> {
    let params = new HttpParams();
    if (message){
      params=params.set('description', message);
    }
    return this.api.put(`Farms/CancelContractWarning/${workflowId}`, null,{params});
  }

  GetRightWarning( farm_id: string,land_id: string, part_id: number): Observable<any> {
    return this.api.get(`Farms/GetRightWarning`,{params:{farmId:farm_id,landId:land_id,splitIndex:part_id.toString() }});
  }
}
