import {Injectable} from "@angular/core";
import {ApiService} from "./api.services";
import {FormBuilder} from "@angular/forms";
import {Router} from "@angular/router";
import {Observable} from "rxjs";

@Injectable()
export class BidServices {
    
    public formFb:FormBuilder;
    
    constructor(public router:Router, public apiService:ApiService){}
    
    registerPromotion(body:any):Observable<any>{
        return this.apiService.post(`Bid/PostPromotion`, body);
    }
    updatePromotion(body:any):Observable<any>{
        return this.apiService.post(`Bid/UpdatePromotion`, body);
    }
    searchBid(body:any){
        return this.apiService.post(`Bid/SearchPromotions`, body);
    }
    getPromotion(body:any){
        return this.apiService.get(`Bid/GetPromotion?prom_id=${body}`);
    }
    approvePromotion(body:any){
        return this.apiService.post(`Bid/ApprovePromotion?promo_id=${body}`, null);
    }
    
    closePromotion(body:any){
        return this.apiService.post(`Bid/ClosePromotion?promo_id=${body}`, null);
    }
    
    cancelPromotion(body:any){
        return this.apiService.post(`Bid/CancelPromotion?promo_id=${body}`,null);
    }
    finishPromotion(body:any){
        return this.apiService.post(`Bid/FinishEvaluation?promo_id=${body}`,null);
    }
    applyForPromotion(body:any) {
        return this.apiService.post(`Bid/ApplyForPromotion`, body)
    }
    getApplication(promUnit_id:string, investor_id:string){
        return this.apiService.get(`Bid/GetApplication?promUnit_id=${promUnit_id}&investor_id=${investor_id}`);
    }
    
    saveEvaluation(body:any){
        return this.apiService.post(`Bid/SubmitEvaluation`, body);
    }
    getLandInformation(body:string){
        return this.apiService.get(`Bid/GetLandData?upin=${body}`);
    }
    
    getLandDetails(promoUnitID:string, promoID:string){
        return this.apiService.get(`Bid/GetLandDataByPromotionId?promotionUnitId=${promoUnitID}&promotionId=${promoID}`);
    }
    getEvaluationData(promID:string, teamID:string, evaluatorUserName:string, investorID:string){
        return this.apiService.get(`Bid/GetEvaluationData?promID=${promID}&teamID=${teamID}&evaluatorUserName=${evaluatorUserName}&investorID=${investorID}`);
    }
    
    getEvaluationPoint(promID:string, teamID:string, evaluatorUserName:string, investorID:string){
        return this.apiService.get(`Bid/GetEvaluationPoint?promID=${promID}&teamID=${teamID}&evaluatorUserName=${evaluatorUserName}&investorID=${investorID}`)
    }
    
    getAllEvaluationPoint(promID:string){
        return this.apiService.get(`Bid/GetAllEvaluationPoint?promID=${promID}`);
    }
    AutoAcceptApplication(promoUnitID:string,promID:string){
        return this.apiService.post(`Bid/AutoAcceptApplication?promUnitId=${promoUnitID}&promID=${promID}`, null);
    }
    showDocument(data:any, mime:any){
        return this.apiService.get(`User/ShowDocument?data=${data}&mime=${mime}`);
    }
    downloadDocument(data:any){
        return this.apiService.get(`Bid/DownloadPromotionDocument?promId=${data}`);
  }
  getEvaluationSummary(promoUnit_Id: any, prom_Id:any) {
    return this.apiService.get(`Bid/GetEvaluationSummary?promUnitId=${promoUnit_Id}&promID=${prom_Id}`);
  }
  getPromotionUnit(promUnitId:any, promId:any){
        return this.apiService.get(`Bid/GetPromotionUnit?promUnit_id=${promUnitId}&prom_id=${promId}`);
  }
  postbondSubmissionDate(prom_id:any, date:any){
        return this.apiService.post(`Bid/PostBondSubmissionDate?promId=${prom_id}&date=${date}`, null);
  }
}
