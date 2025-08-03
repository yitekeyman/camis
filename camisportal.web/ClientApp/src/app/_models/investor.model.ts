import {interceptingHandler} from "@angular/common/http/src/module";

export interface InvestorModel {

    Name:string;
    Nationality:string;
    TypeId:number;
    OriginId:number;
    Capital:string;
    Phone:string;
    Email:string;
    AddressId:any;
    Registrations:Registration[];
}
export interface Registration {

    TypeId:number;
    TypeName:string;
    AuthorityId:number;
    AuthorityName:string;
    RegistrationNumber:string;
    Document:RegDocument
}

export interface RegDocument {

    File:string;
    Mimetype:string;
    FileName:string;
    Date:any;
    Ref:string;
    Note:string;
    Type:number;
    
}



export interface Application{
    invProfile:any;
    proposalDocument:ProposalDocument[];
    contactAddress:any;
    promoID:string;
    promotionUnitId:string;
    proposalAbstract:string;
    investmentTypes:number[];
    proposedCapital:number;
    applicationTime:any;
}

export interface ProposalDocument {
    data: string;
    mime: string;

    docRef: string;
    documentType: number;
}

export interface ContactPerson {
    id:string;
    name:string;
    phone:string;
    email:string;
}

export interface InvestorApplication {
    application:any;
    submittedDate:any;
    deadline:any;
    title:string;
    status:string;
    promoID:string;
    investorID:string;
}

export interface Applicants {
    submittedDate:any;
    investorName:string;
    nationality:string;
    investmentType:number[];
    capital:number;
    contact:any;
    promoID:string;
    investorID:string
    promotionUnitId:string;
}