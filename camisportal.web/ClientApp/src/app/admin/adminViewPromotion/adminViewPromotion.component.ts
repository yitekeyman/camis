import {Component, Input, OnInit} from "@angular/core";
import {BidServices} from "../../_services/bid.services";
import {PromotionUnit} from "../../_models/bid.model";
import {Http} from "@angular/http";
import {HttpHeaders} from "@angular/common/http";
import {Applicants, Application} from "../../_models/investor.model";
import {InvestorServices} from "../../_services/investor.services";
import swal from "sweetalert2";
import {Router} from "@angular/router";

declare var $:any;

@Component({
    selector:'app-view-promotion',
    templateUrl:'./adminViewPromotion.component.html'
})

export class AdminViewPromotionComponent implements OnInit{
    promotionUnit:PromotionUnit;
    @Input() public promotionID:string;
    public documentURL:any;
    appQuery:any;
    application:Application[];
    applicants:Applicants;
    public applicantList:any[]=[];
    public applicantSearchResult:any[];
    constructor(public bidService:BidServices, public investorServices:InvestorServices, public router:Router){

    }
    ngOnInit(){
        if(this.promotionID!=''){
            this.bidService.getPromotion(this.promotionID).subscribe(res=>{
                this.promotionUnit=res;
                let file=new Blob([this.promotionUnit.documents[0].data],{type:this.promotionUnit.documents[0].mime});
                this.documentURL=URL.createObjectURL(file);
            })
        }

        this.appQuery={
            promoID:this.promotionID,
            investorID:''
        };
        
        this.applicants={
            submittedDate:null,
            investorName:'',
            nationality:'',
            investmentType:[],
            capital:null,
            contact:null,
            promoID:'',
            investorID:'',
            promotionUnitId:''
        };

        this.getApplication();
    }

    getApplication(){
        this.investorServices.searchApplication(this.appQuery).subscribe(res=>{
            this.application=res.result;
            for(const appList of this.application){
                const investor=JSON.parse(appList.invProfile.defaultProfile);
                this.applicants.investorName=investor.Name;
                this.applicants.nationality=investor.Nationality;
                this.applicants.investorID=appList.invProfile.id;
                this.applicants.submittedDate=appList.applicationTime;
                this.applicants.investmentType=appList.investmentTypes;
                this.applicants.contact=appList.contactAddress;
                this.applicants.capital=appList.proposedCapital;
                this.applicants.promoID=appList.promoID;
                this.applicants.promotionUnitId=appList.promotionUnitId;
                this.applicantList.push(this.applicants);
            }
        },e=>{
            swal({
                type:'error',
                title:'Oops..',
                text:e &&(e.message|| JSON.stringify(e))||'Unknown error'
            })
        })
    }
    
   evaluateBid(prom_ID:string, investor_ID:string){
       $('#view-promotion-dialog').modal('hide');
        this.router.navigate([`admin/evaluate/${prom_ID}/${investor_ID}`]);
   }
   
    
}