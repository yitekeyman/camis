import {Component, OnInit} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {IdocumentPriview, PromotionDocModel, PromotionUnit} from "../../_models/bid.model";
import {InvestorServices} from "../../_services/investor.services";
import {ListServices} from "../../_services/list.services";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";
declare var $:any;
@Component({
    selector:'app-bid-detail',
    templateUrl:'./investorBidDetail.component.html'
})
export class InvestorBidDetailComponent implements OnInit{
    public userRole:number|null;
    public promotionID:string;
    public username:string;
    public isApply:boolean=false;
    public investor_id:any;
 
    constructor(public activatedRoute:ActivatedRoute, public router:Router, public bidService:BidServices, public invService:InvestorServices, public listServices:ListServices){}
    
    ngOnInit():void{
        this.userRole=JSON.parse(<string>localStorage.getItem("role"));
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        this.promotionID=this.activatedRoute.snapshot.params['prom_id'];
        this.getApplication();
        
    }

 
    
    getApplication(){
        this.invService.getInvestor(this.username).subscribe(res=>{
            this.investor_id=res.id;
            
        });
        
    }
}