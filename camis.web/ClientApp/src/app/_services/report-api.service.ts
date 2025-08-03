import { QueryEncoder } from '@angular/http';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import {ApiService} from './api.service';
import { ReportType ,Region, ReportResponseModel, ReportRequestModel} from '../_shared/report/report.model';

@Injectable()
export class ReportAPIService {

  reportTypeUrl = '../assets/data/ReportType.json';
  regionListUrl = '../assets/data/RegionList.json';



  private qs: QueryEncoder;

  constructor(private api: ApiService, private http: HttpClient) {
    this.qs = new QueryEncoder();
  }

  // get all the land detail informtions
  getReportTypes(): Observable<ReportType[]> {
    return this.http.get<ReportType[]>(this.reportTypeUrl);
  }

  getRegionLists() : Observable<Region[]>{
      return this.http.get<Region[]>(this.regionListUrl)
  }

  getAllFarms() : Observable<Region[]>{
    return this.api.get("report/GetAllFarms");
  }

  getReport(data : ReportRequestModel) : Observable<string>{
      return this.api.post("report/GetReport",data, { responseType : 'string' as 'json'});
  }

  downloadReport(data : ReportResponseModel) : Observable<Blob>{
    return this.api.post("report/Download",data,{ responseType : 'blob' as 'json'});
  }

  getAllRegions() : Observable<any[]>{
    return this.api.get("report/GetAllRegions");
  }

  getZones(regionid : string) : Observable<any[]>{
    var path =  `report/GetZones?regionid=${regionid}`;
    return this.api.get(path);
  }

  getWoredas(zoneid : string) : Observable<any[]> {
    var path = `report/GetWoredas?zoneid=${zoneid}`;
    return this.api.get(path);
  }



  

}
