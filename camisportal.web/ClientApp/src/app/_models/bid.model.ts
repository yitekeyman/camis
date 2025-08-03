import {IDocument} from "../../../../../camis.web/ClientApp/src/app/_shared/document/interfaces";

export interface PromotionModel {
    id:any;
    applyDateFrom:any;
    applyDateTo:any;
    title:string;
    promotionRef:string;
    summery:string;
    description:string;
    region:string;
    status:number;
    postedOn:any;
    physicalAddress:string;
    investmentTypes:number[];
}
export interface PromotionReqModel {
    applyDateFrom:any;
    applyDateTo:any;
    title:string;
    promotionRef:string;
    summery:string;
    description:string;
    region:string;
    status:number;
    postedOn:any;
    physicalAddress:string;
    investmentTypes:number[];
}
export interface PromotionDocModel {
    id:any;
    docRef:string;
    mime:string;
    data:string;
    desc:string;
}

export interface EvaluationTeamMemberModel {
   id:string;
   userName:string;
   weight:number;
}

export interface EvaluationTeamModel{
    id:string;
    teamName:string;
    weight:number;
    members:EvaluationTeamMemberModel[];
    criterion:EvaluationCriterionModel[];
}
export interface EvaluationCriterionModel {
    id:string;
    name:string;
    maxVal:number;
    weight:number;
    valueList:any[];
    cubCriterion:EvaluationCriterionModel[];
}

export interface PromotionUnit {
    id:string;
    landUPIN:string;
    promotion:PromotionModel;
    documents:PromotionDocModel[];
    pictures:PromotionDocModel[];
    evalTeams:EvaluationTeamModel[];
    status:number;
    winnerInvestor;
    
}
export interface PromotionRegUnit {
    landUPIN:string;
    promotion:PromotionModel;
    documents:PromotionDocModel[];
    pictures:PromotionDocModel[];
    evalTeams:EvaluationTeamModel[];

}

export interface SearchResult {
    address:string;
    title:string;
    deadLine:Date;
    landArea:number;
    promotion_id:string;
    regionCode:string;
    regionName:string;
}

export interface IdocumentPriview {
    id:any;
    docRef:string;
    mime:string;
    data:string;
    overrideFilePath?: string;
}

export interface IDocumentListOpenEvent {
    document: IdocumentPriview;
}

export interface Evaluation {
    evaluationTeamID:string;
    evaluatorUserName:string;//automatically set from session
    promoID:string;
    promotionUnitID:string;
    investorID:string;
    result:any[];
}

export interface document {

    file:string;
    mimetype:string;
    filename:string;
    date:any;
    ref:string;
    note:string;
    type:number;


}