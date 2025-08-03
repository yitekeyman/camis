import {Component, Input, OnInit} from "@angular/core";
import {ActivatedRoute} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {ListServices} from "../../_services/list.services";
import {Application} from "../../_models/investor.model";
import swal from "sweetalert2";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";
import {InvestorServices} from "../../_services/investor.services";

declare var $:any;
@Component({
    selector:'app-inv-profile-detail',
    templateUrl:'./investorProfileDetail.component.html'
})
export class InvestorProfileDetailComponent implements OnInit{
    @Input('investorID') investorID:string;

    @Input('promotionUnitID')promotionUnitID:string;
    @Input('userName')userName:string;

    public application:Application;
    public invProfile:any;
    investmentTypes:any[]=[];
    investorType:any;
    investorOrgin:any;
    authority:any[]=[];
    registration:any[]=[];

    investmentTypeList:any[]=[];
    investorTypeList:any[]=[];
    investorOrgionList:any[]=[];
    authorityList:any[]=[];
    registrationList:any[]=[];
    investmentType:any[]=[];

    loading:boolean=false;

    constructor(public activatedRoute:ActivatedRoute, public bidServices:BidServices, public listServices:ListServices, public investorServices:InvestorServices){}
    ngOnInit(){
        this.loading=true;
        
        this.getApplication();
    }

    getApplication(){

        if(this.promotionUnitID!=null && this.investorID!=null){
            this.bidServices.getApplication(this.promotionUnitID, this.investorID).subscribe(res=>{
                this.application=res;
                this.invProfile=JSON.parse(this.application.invProfile.defaultProfile);
                this.getApplicationList();
            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });
        }
        else{
            this.investorServices.getInvestor(this.userName).subscribe(user=>{
                this.invProfile=user.defaultProfile;
                this.getApplicationList();
            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            })
        }
       
    }

    getApplicationList(){
        this.listServices.getOperationType().subscribe(data=>{
            this.investorTypeList=data;
            this.prepareInvestorType();
        });
        this.listServices.getInvestorOrigin().subscribe(data=>{
            this.investorOrgionList=data;
            this.prepareInvestorOrigin();
        });

        this.listServices.getAuthority().subscribe(data=>{
            this.authorityList=data;
            this.listServices.getRegistrationType().subscribe(data2=>{
                this.registrationList=data2;
                this.prepareAuthority();
            });
        });
    this.loading=false;

    }
    prepareInvestorType(){
        for(const invType of this.investorTypeList){
            if(invType['id'] == this.invProfile.TypeId){
                this.investorType=invType['name'];
            }
        }
    }

    prepareInvestorOrigin(){
        for(const origin of this.investorOrgionList){
            if(origin['id'] == this.invProfile.OriginId){
                this.investorOrgin=origin['name'];
            }
        }
    }

    prepareAuthority(){
        for(const authority of this.invProfile.Registrations){
            for(const authorityList of this.authorityList){
                if(authority['AuthorityId'] == authorityList['id']){
                    for(const registrationList of this.registrationList){
                        if(authority['TypeId']==registrationList['id']){
                            authority['AuthorityId']=authorityList['name'];
                            authority['TypeId']=registrationList['name']
                            this.authority.push(authority);
                        }
                    }
                }
            }

        }
    }

    prepareInvestment(){
        for(const investment of this.application['investmentTypes']){
            for(const invTypeList of this.investmentTypeList){
                if(investment === invTypeList['id']){
                    this.investmentType.push(invTypeList['name']);
                }
            }
        }
    }

    openDoc(investorID:string, index:number){
        const overrideFilePath=`${configs.url}Investor/GetRegistrationDoc?investorId=${investorID}&index=${index}`;
        $("#view-document-dialog").modal("show");
        $("#docIframe").attr("src", overrideFilePath);
    }
    
    openProDoc(promID:string, investorID:string, index:number){
        const overrideFilePath=`${configs.url}Bid/GetProposalDocument?promId=${promID}&investorId=${investorID}&index=${index}`;
        $("#view-document-dialog").modal("show");
        $("#docIframe").attr("src", overrideFilePath);
    }
    
}