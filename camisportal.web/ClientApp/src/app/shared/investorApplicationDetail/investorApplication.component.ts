import {Component, Input, OnInit} from "@angular/core";
import {Application} from "../../_models/investor.model";
import {ActivatedRoute} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {ListServices} from "../../_services/list.services";
import swal from "sweetalert2";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";

declare var $:any;

@Component({
    selector:'investor-application',
    templateUrl:'./investorApplication.component.html'
})
export class InvestorApplicationComponent implements OnInit{

    @Input('investorID') investorID:string;

    @Input('promotionUnitID')promotionUnitID:string;
    public application:Application;
    invesTypeList:any[]=[];
    invesType:any[]=[];

    constructor(public activatedRoute:ActivatedRoute, public bidServices:BidServices, public listServices:ListServices){}
    
    ngOnInit(){
        this.getApplication();
    }

    getApplication(){

        this.bidServices.getApplication(this.promotionUnitID, this.investorID).subscribe(res=>{
            this.application=res;
            this.getApplicationList();
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }
    getApplicationList(){
        this.listServices.getInvetmentType().subscribe(data=>{
            this.invesTypeList=data;
            this.prepareInvestment();
        });
    }
    prepareInvestment(){
        for(const investment of this.application['investmentTypes']){
            for(const invTypeList of this.invesTypeList){
                if(investment === invTypeList['id']){
                    this.invesType.push(invTypeList['name']);
                }
            }
        }
    }

    openProDoc(promID:string, investorID:string, index:number){
        const overrideFilePath=`${configs.url}Bid/GetProposalDocument?promId=${promID}&investorId=${investorID}&index=${index}`;
        $("#view-document-dialog").modal("show");
        $("#docIframe").attr("src", overrideFilePath);
    }
    
}