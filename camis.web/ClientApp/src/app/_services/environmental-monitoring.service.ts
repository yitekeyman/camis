import {Injectable} from "@angular/core";
import {ApiService} from "./api.service";

@Injectable()
export class EnvironmentalMonitoringService{
  constructor(private api: ApiService) {

  }

  public DetectChanges(request:any){
    return this.api.post('EnvironmentalMonitoring/DetectChanges', request);
  }
  public ProcessImagery(request:any){
    return this.api.post('EnvironmentalMonitoring/ProcessImagery', request);
  }
  public GetParcelHistory(parcelUPID:string, monthsBack:number){
    return this.api.get(`EnvironmentalMonitoring/GetParcelHistory?parcelUpid=${parcelUPID}&monthsBack=${monthsBack}`);
  }
  public GetSpectralIndices(parcelUPID:string, date:any){
    return this.api.get(`EnvironmentalMonitoring/GetSpectralIndices?parcelUpid=${parcelUPID}&date=${date}`);
  }
  public GetSignificantChanges(dateFrom:any, dateTo:any, region:any){
    return this.api.get(`EnvironmentalMonitoring/GetSignificantChanges?startDate${dateFrom}&endDate=${dateTo}&region=${region}`);
  }
  public CheckFloodRisk(geom:string){
    return this.api.post(`EnvironmentalMonitoring/CheckFloodRisk`, geom);
  }
  public CheckDroughtCondition(region:string){
    return this.api.get(`EnvironmentalMonitoring/CheckDroughtCondition?region=${region}`);
  }

  public GetParcelChanges(parcelUPID:string,dateFrom:any, dateTo:any, threshold:any){
    return this.api.get(`EnvironmentalMonitoring/GetParcelChanges?parcelUpid=${parcelUPID}/changes?startDate${dateFrom}&endDate=${dateTo}&threshold=${threshold}`);
  }
  public HealthCheck(){
    return this.api.get(`EnvironmentalMonitoring/HealthCheck`);
  }
}
