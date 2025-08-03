import { QueryEncoder } from '@angular/http';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { LandModel, Accessablity, SoilTestType, LandBankWorkItem,
  LandType, SearchResult, Month, AgroEchologicalZone,
  Topography, InvestmentType, MoistureSource, WaterTestParameters, ExistingLandUse,
  GroundWater, SurfaceWater} from '../_shared/land-bank/land.model';

import {ApiService} from './api.service';

@Injectable()
export class LandDataService {

  landDataUrl = '../assets/data/landData.json';
  accessablityUrl = '../assets/data/Accessiblity.json';
  soilTestTypeUrl = '../assets/data/soilTestType.json';
  jsonUrl = '../assets/data/json.json';
  landTypeUrl = '../assets/data/LandType.json';
  agroEchologicalZoneUrl = '../assets/data/AgroEchologicalZone.json';
  TopographyUrl = '../assets/data/Topography.json';
  investmentTypeUrl = '../assets/data/InvestmentType.json';
  moistureSourceUrl = '../assets/data/MoistureSource.json';
  waterTestParametersUrl = '../assets/data/WaterTestParameters.json';
  WaterSourceTypeUrl = '../assets/data/WaterSourceType.json';
  existingLandUseUrl = '../assets/data/ExistingLandUse.json';
  groundWaterUrl = '../assets/data/GroundWater.json';
  surfaceWaterUrl = '../assets/data/SurfaceWater.json';

  private qs: QueryEncoder;

  constructor(private api: ApiService, private http: HttpClient) {
    this.qs = new QueryEncoder();
  }

  // get all the land detail informtions
  getLandData(): Observable<LandModel[]> {
    return this.http.get<LandModel[]>(this.landDataUrl);
  }

  // get all the accessiblity values
  getAccessiblity(): Observable<Accessablity[]> {
    return this.http.get<Accessablity[]>(this.accessablityUrl);
  }

  getAgroEchologicalZone(): Observable<AgroEchologicalZone[]> {
    return this.http.get<AgroEchologicalZone[]>(this.agroEchologicalZoneUrl);
  }

  getTopography(): Observable<Topography[]> {
    return this.http.get<Topography[]>(this.TopographyUrl);
  }

  getsoilTestTypeUrl(): Observable<SoilTestType[]> {
    return this.http.get<SoilTestType[]>(this.soilTestTypeUrl);
  }

  getMoistureSource(): Observable<MoistureSource[]> {
    return this.http.get<MoistureSource[]>(this.moistureSourceUrl);
  }

  getGroundWater(): Observable<GroundWater[]> {
    return this.http.get<GroundWater[]>(this.groundWaterUrl);
  }

  getSurfaceWater(): Observable<SurfaceWater[]> {
    return this.http.get<SurfaceWater[]>(this.surfaceWaterUrl);
  }

  getWaterTestParameters(): Observable<WaterTestParameters[]> {
    return this.http.get<WaterTestParameters[]>(this.waterTestParametersUrl);
  }

  getWaterSourceType(): Observable<WaterTestParameters[]> {
    return this.http.get<WaterTestParameters[]>(this.WaterSourceTypeUrl);
  }

  getExistingLandUse(): Observable<ExistingLandUse[]> {
    return this.http.get<ExistingLandUse[]>(this.existingLandUseUrl);
  }

  getInvestmentType(): Observable<InvestmentType[]> {
    return this.http.get<InvestmentType[]>(this.investmentTypeUrl);
  }

  getjsonUrl(): Observable<Month[]> {
    return this.http.get<Month[]>(this.jsonUrl);
  }

  getLandType(): Observable<LandType[]> {
    return this.http.get<LandType[]>(this.landTypeUrl);
  }
  RequestLandEdit(body: LandModel, wfid: string): Observable<string> {
    return this.api.post(`LandBank/RequestLandRegistration?wfid=${wfid}`, body);
  }
  RequestLandRegistration(body: LandModel): Observable<string> {
    return this.api.post(`LandBank/RequestLandRegistration`, body);
  }

  GetUserWorkItems(): Observable<any> {
    return this.api.get(`LandBank/GetUserWorkItems`);
  }
  GetSplitWorkItem(wfid: string): Observable<any> {
    return this.api.get(`LandBank/GetSplitWorkItem?wfid=${wfid}`);
  }
  GetWorkFlowLand(wfid: string): Observable<any> {
    return this.api.get(`LandBank/GetWorkFlowLand?wfid=${wfid}`);
  }

  ApproveRegistration(wfid: string, note: string | null ): Observable<any> {
    return this.api.post(`LandBank/ApproveRegistration?wfid=${wfid}`, note);
  }

  SearchLand(keyword: any): Observable<any> {
    return this.api.post(`LandBank/SearchLand`, keyword);
  }

  GetLand(landID: string): Observable<any> {
    return this.api.get(`LandBank/GetLand?landID=${landID}&geom=true`);
  }

  RejectRegistrationRequest(wfid: string, note: string): Observable<any> {
    return this.api.post(`LandBank/RejectRegistrationRequest?wfid=${wfid}`, { note });
  }

  CancelRegistrationRequest(wfid: string, note: string): Observable<any> {
    return this.api.post(`LandBank/CancelRegistrationRequest?wfid=${wfid}`, note);
  }

  RequestLandPreparation(request: any): Observable<any> {
    return this.api.post(`LandBank/RequestLandPreparation`, request);
  }

  ApprovePreparation(wfid: string, note: string ): Observable<any> {
    return this.api.post(`LandBank/ApprovePreparation?wfid=${wfid}`, note);
  }

  RejectPreparationRequest(wfid: string, note: string): Observable<any> {
    return this.api.post(`LandBank/RejectPreparationRequest?wfid=${wfid}`, note);
  }

  CancelPreparationRequest(wfid: string, note: string): Observable<any> {
    return this.api.post(`LandBank/CancelPreparationRequest?wfid=${wfid}`, note);
  }

}
