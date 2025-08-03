import {Router} from '@angular/router';
import {ApiService} from './api.services';
import {Component, Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable} from 'rxjs/observable';

class Json {
    id: number;
    name: string;
}

@Injectable()
export class ListServices {
    monthUrl = '../assets/data/json.json';
    soilTestTypeUrl = '../assets/data/soilTestType.json';
    investmentTypeUrl = '../assets/data/investmentType.json';
    regionUrl = '../assets/data/region.json';
    authorityUrl = '../assets/data/authority.json';
    oprationUrl = '../assets/data/opreationType.json';
    registrationUrl = '../assets/data/registrationType.json';
    investorOriginUrl = '../assets/data/investorOrigin.json';
    statusUrl='../assets/data/status.json';
    roleUrl="../assets/data/role.json";
    accessablityURL="../assets/data/Accessiblity.json";
    agroEcoZoneURL="../assets/data/AgroEchologicalZone.json";
    topographyUrl="../assets/data/Topography.json";
    exisingLandUseUrl="../assets/data/ExistingLandUse.json";
    moistureSourceUrl="../assets/data/MoistureSource.json";
    constructor(public router: Router, public apiService: ApiService , public http: HttpClient) {}

    getMonth(): Observable<Json[]> {
        return this.http.get<Json[]>(this.monthUrl);
    }

    getSoilTest(): Observable<Json[]> {
        return this.http.get<Json[]>(this.soilTestTypeUrl);
    }

    getInvetmentType(): Observable<Json[]> {
        return this.http.get<Json[]>(this.investmentTypeUrl);
    }

    getRegion(): Observable<Json[]> {
        return this.http.get<Json[]>(this.regionUrl);
    }

    getAuthority(): Observable<Json[]> {
        return this.http.get<Json[]>(this.authorityUrl);
    }

    getInvestorOrigin(): Observable<Json[]> {
        return this.http.get<Json[]>(this.investorOriginUrl);
    }

    getOperationType(): Observable<Json[]> {
        return this.http.get<Json[]>(this.oprationUrl);
    }

    getRegistrationType(): Observable<Json[]> {
        return this.http.get<Json[]>(this.registrationUrl);
    }
    getStatus(): Observable<Json[]> {
        return this.http.get<Json[]>(this.statusUrl);
    }
    getRole():Observable<Json[]>{
        return this.http.get<Json[]>(this.roleUrl);
    }
    getAccessbility():Observable<Json[]>{
        return this.http.get<Json[]>(this.accessablityURL);
    }
    getAgroEcoZone():Observable<Json[]>{
        return this.http.get<Json[]>(this.agroEcoZoneURL);
    }
    getTopography():Observable<Json[]>{
        return this.http.get<Json[]>(this.topographyUrl);
    }
    getExistingLandUse():Observable<Json[]>{
        return this.http.get<Json[]>(this.exisingLandUseUrl);
    }
    getMoistureSource():Observable<Json[]>{
        return this.http.get<Json[]>(this.moistureSourceUrl);
    }

}
