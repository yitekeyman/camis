import {Injectable} from "@angular/core";
import {ApiService} from "./api.services";
import {Router} from "@angular/router";
import {Observable} from "rxjs";
import {QueryEncoder} from '@angular/http';
import {b} from "@angular/core/src/render3";

@Injectable()
export class InvestorServices {
    private qs: QueryEncoder;
    
    constructor(public apiServices:ApiService, public router:Router){
        this.qs = new QueryEncoder();
    }
    
    participateBid(body:any):Observable<any>{
        return this.apiServices.post(`Bid/ParticipateBid`, body);
    }
    searchApplication(body:any){
        return this.apiServices.post(`Bid/SearchApplications`, body);
    }
    registerInvestor(body:any){
        return this.apiServices.post(`Investor/RegisterInvestor`, body);
    }
    
    getInvestor(body:string){
        return this.apiServices.get(`Investor/GetInvestor?username=${body}`);
    }
    updateInvestor(body:any){
        return this.apiServices.post(`Investor/UpdateInvestor`,body);
    }
    getLatestApplication(promoUnitId:string, investorId:string) {
        return this.apiServices.get(`Bid/GetLatestApplication?prom_unit_id=${promoUnitId}&investor_id=${investorId}`);
    }
    getProjects(id:string, promo_Unit_id:string){
        return this.apiServices.get(`Projects/SearchReports?id=${id}&promotionUnitId=${promo_Unit_id}`);
    }
    submitSelfEvaluation(body:any, promo_Unit_id:string){
        return this.apiServices.post(`Projects/SubmitNewProgressReport${promo_Unit_id ?'?promotionUnitId='+ this.qs.encodeValue(promo_Unit_id): ''}`,body);
    }
    calculateResourceProgress(activityId: string, promo_Unit_id:string, reportTime?: number) {
        return this.apiServices.get(`Projects/CalculateResourceProgress?id=${activityId}&promotionUnitId=${promo_Unit_id}&reportTime=${reportTime ?  + this.qs.encodeValue('' + reportTime) : ''}`);
    }

    calculateOutcomeProgress(activityId: string,  promo_Unit_id:string, reportTime?: number) {
        return this.apiServices.get(`Projects/CalculateOutcomeProgress?id=${activityId}&promotionUnitId=${promo_Unit_id}&reportTime=${reportTime ?  + this.qs.encodeValue('' + reportTime) : ''}`);
    }
    getPlanFromRootActivity(activityId: string,  promo_Unit_id:string): Observable<any> {
        return this.apiServices.get(`Projects/PlanFromRootActivity?id=${activityId}&promotionUnitId=${promo_Unit_id}`);
    }
    calculatedProgress(activityId:string, promo_unit_id:string, reportTime?:number){
        return this.apiServices.get(`Projects/CalculateProgress?id=${activityId}&promotionUnitId=${promo_unit_id}&reportTime=${reportTime ?  + this.qs.encodeValue('' + reportTime) : ''}`);
    }
    getInvestorById(invId:string){
        return this.apiServices.get(`Investor/GetInvestorById?investorId=${invId}`);
    }
}