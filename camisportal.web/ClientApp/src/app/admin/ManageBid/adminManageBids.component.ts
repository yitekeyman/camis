import {Component, Input, OnInit} from "@angular/core";
import {BidServices} from "../../_services/bid.services";
import {IdocumentPriview, PromotionUnit, SearchResult} from "../../_models/bid.model";
import {Router} from "@angular/router";
import swal from "sweetalert2";
import {InvestorServices} from "../../_services/investor.services";
import {Applicants, Application} from "../../_models/investor.model";
import {ListServices} from "../../_services/list.services";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";
import dialog from "../../shared/loader/loader_dialog";

declare var $:any;
@Component({
    selector:'app-bid-management',
    templateUrl:'./adminManageBids.component.html'
})
export class AdminManageBidsComponent implements OnInit{

    query:any;
    public searchResult:any[]=[];
    public selectedPromotion:string;
    public promotionID:string;
    promotionUnit:PromotionUnit;
    appQuery:any;
    application:Application[];
    applicants:Applicants;
    public applicantList:any[]=[];
    public applicantSearchResult:any[];
    statusList:any[]=[];
    status:any;
    promotion:any[]=[];
    regionList:any[]=[];
    regions:any;
    selectedRegion:any;
    
    public role:any;
    public loading:boolean=false;
    public applyDateTo:string=null;
    public updateProm:any=null;

    constructor(public bidService:BidServices, public router:Router, public investorServices:InvestorServices, public listService:ListServices) {

    }

    ngOnInit():void{
        this.loading=true;
        this.selectedRegion=JSON.parse(localStorage.getItem('region'));
        this.role=localStorage.getItem('role');
            this.query={
            region:'',
            states:[]
        };

        this.getAllPromotions();


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

    }

    public getAllPromotions(){
        this.bidService.searchBid(this.query).subscribe(res=>{
            
            if(this.selectedRegion !='99'){
                for(const pro of res.result){
                    if(this.selectedRegion==pro.regionCode){
                        this.searchResult.push(pro);
                    }
                }
            }
            else{
                this.searchResult=res.result;
            }
            this.getLists();
            this.loading=false;
        });
    }

  

    public editPromotion(promUnit_Id:string, promotion_Id:string):void{
        this.router.navigate([`admin/editPro/${promUnit_Id}/${promotion_Id}`]);
    }
    
    public addPromotionUnit(promotion_Id:string):void{
        this.router.navigate([`admin/addPro/${promotion_Id}/addUnit`]);
    }

    public viewPromotion(promUnit_Id:string, prom_id):void
    {
        dialog.loading();

        this.selectedPromotion=prom_id;
        

        if(this.selectedPromotion!=''){
            this.applicantList=[];
            this.bidService.getPromotionUnit(promUnit_Id, prom_id).subscribe(res=>{
                this.promotionUnit=res;
                this.getProList();
                //this.openDoc(this.promotionUnit.documents[0], this.promotionUnit.promotion.id, this.promotionUnit.id);
            });


            this.appQuery={
                promoID:prom_id,
                investorID:'',
                promoUnitID:promUnit_Id
            };

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
                }
               
                dialog.close();
                
                $('#view-promotion-dialog').modal('show');
            },e=>{
                swal({
                    type:'error',
                    title:'Oops..',
                    text:e &&(e.message|| JSON.stringify(e))||'Unknown error'
                })
            })
        }
    }

    evaluateBid(promoID:string, promUnit_ID:string, investor_ID:string){
        $('#view-promotion-dialog').modal('hide');
        this.router.navigate([`admin/evaluate/${promoID}/${promUnit_ID}/${investor_ID}`]);
    }
    public closePromotion(promotion_Id:string):void{
        swal({
            title: 'Are you sure?',
            text: "You want to close this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Close!'
        }).then((result) => {
            if (result.value) {
                this.bidService.closePromotion(promotion_Id).subscribe(res=>{

                    swal("Success!", "You Have Successfully Close Promotion", "success").then(value => {
                        this.promotion=[];
                        this.getAllPromotions();
                    })


                },e=> {

                    swal({
                        type: 'error', title: 'Oops...', text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                    });

                });
            }
        })
    }

    public approvePromotion(promotion_id:string){
        swal({
            title: 'Are you sure?',
            text: "You want to approve this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Approve!'
        }).then((result) => {
            if (result.value) {
                this.bidService.approvePromotion(promotion_id).subscribe(res=> {
                    swal("Success!", "You Have Successfully Approve Promotion and Its Ready for Public", "success").then(value => {
                        this.promotion=[];
                        this.getAllPromotions();
                    });
                },e=>{
                    swal({
                        type: 'error',
                        title: 'Oops...',
                        text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                    });

                });
            }
        })
    }

    public cancelPromotion(promotion_Id:string):void{
        swal({
            title: 'Are you sure?',
            text: "You want to close this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Cancel!'
        }).then((result) => {
            if (result.value) {
                this.bidService.cancelPromotion(promotion_Id).subscribe(res=> {
                    swal("Success!", "You Have Successfully Cancel Promotion", "success").then(value => {
                        this.promotion=[];
                        this.getAllPromotions();
                    });
                },e=>{
                    swal({
                        type: 'error',
                        title: 'Oops...',
                        text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                    });

                });
            }
        })
    }
    
    public evaluationDetails(promotion_id:string, promoUnit_id:string, investor_id:string):void{
        $('#view-promotion-dialog').modal('hide');
        this.router.navigate([`admin/eva-details/${promotion_id}/${promoUnit_id}/${investor_id}`]);
    }
    public invEvaluationDetails(promotion_id:string, promoUnit_id:string){
        $('#view-promotion-dialog').modal('hide');
        this.router.navigate([`admin/invEva-details/${promotion_id}/${promoUnit_id}`]);
    }
    public autoAcceptPromotion(promotion_id:string, promoUnit_id:string) {
        $('#view-promotion-dialog').modal('hide');
        this.router.navigate([`admin/invEva-details/${promotion_id}/${promoUnit_id}`]);
    }
    public finishPromotionEvaluation(promotion_Id:string):void{
        swal({
            title: 'Are you sure?',
            text: "You want to finish this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Finish!'
        }).then((result) => {
            if (result.value) {
                this.bidService.finishPromotion(promotion_Id).subscribe(res=>{

                    swal("Success!", "You Have Successfully Finish Evaluation", "success").then(value => {
                        this.promotion=[];
                        this.getAllPromotions();
                    })


                },e=> {

                    swal({
                        type: 'error', title: 'Oops...', text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                    });

                });
            }
        })
    }

    getLists(){
        this.listService.getStatus().subscribe(data=>{
            this.statusList=data;
            this.prepareStatuss();
        });
        this.loading=false;
        
    }
    getProList(){
        this.listService.getRegion().subscribe(data=>{
            this.regionList=data;
            this.prepareRegion();
        });
        this.listService.getStatus().subscribe(data=>{
            this.statusList=data;
            this.prepareStatus();
        });
    }
    prepareStatuss(){
        for(const sRes of this.searchResult){
            for (const stList of this.statusList) {
                if (sRes.status === stList['id']) {
                    sRes['status'] = stList['name'];
                    this.promotion.push(sRes);
                }
            }
        }
    }

    prepareStatus(){
            for (const stList of this.statusList) {
                if (this.promotionUnit.promotion.status === stList['id']) {
                    this.status= stList['name'];
                }
            }
    }
    prepareRegion(){
        for(const region of this.regionList){
            if(region['code']===this.promotionUnit.promotion.region){
                this.regions=region['Name'];
            }
        }
    }
    openDoc(doc:IdocumentPriview, promID:string, promoUnitID:string){
        doc.overrideFilePath=`${configs.url}Bid/GetPromotionDocument?promId=${promID}&promUnitId=${promoUnitID}`;
        $("#docIframe").attr("src", doc.overrideFilePath);
    }
    downloadDoc(doc:IdocumentPriview, promID:string, promoUnitID:string){
        doc.overrideFilePath=`${configs.url}Bid/GetPromotionDocument?promId=${promID}&promUnitId=${promoUnitID}`;
        window.open(doc.overrideFilePath, '_blank');
    }
    
    showFormSubmissionDate(prom:any){
        this.updateProm=prom;
        $("#postBond_dialog").modal('show');
    }
    postBondSubmissionDate(){
        if(this.applyDateTo!=null){
            this.bidService.postbondSubmissionDate(this.updateProm.promotion_id, this.applyDateTo).subscribe(res=>{
                swal("Success!", "You have successfully post_bond submission date", "success").then(value => {
                    this.promotion=[];
                    this.getAllPromotions();
                    this.updateProm=null;
                    $("#postBond_dialog").modal('hide');
                })
            },e=> {

                swal({
                    type: 'error', title: 'Oops...', text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                });

            })
        } 
    }
}
