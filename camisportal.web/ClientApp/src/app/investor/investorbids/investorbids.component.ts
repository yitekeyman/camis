import {Component, OnInit, Output} from "@angular/core";
import {BidServices} from "../../_services/bid.services";
import {Router} from "@angular/router";
import {InvestorServices} from "../../_services/investor.services";
import swal from "sweetalert2";
import {InvestorApplication} from "../../_models/investor.model";
import {PromotionModel, PromotionUnit} from "../../_models/bid.model";
import {ListServices} from "../../_services/list.services";
declare var imageViewer:any;
declare var PNotify:any;
declare var $:any;
@Component({
    selector:'app-investor-bid',
    templateUrl:'./investorbids.component.html'
})
export class InvestorbidsComponent implements OnInit{


    public userRole:number|null;
    public username:string|'';
    query:any;
    public searchResult:any[];

    public applicationSearchResult:any[];
    appQuery:any;

    investorApplication:any[]=[];

    promotionUnit:PromotionUnit;

    application:any;

    applicationStatus:string;

    promotion:PromotionModel;
    investmentType:any[]=[];
    investmentTypeList:any[]=[];
    statusList:any[]=[];
    status:any;

    loading:boolean=false;
    activeBids:any[]=[];
    outdatedBids:any[]=[];
    public notification:any;
    
    public investorId:string;



    constructor(public bidService:BidServices, public router:Router, public investorServices:InvestorServices, public listServices:ListServices){}

    ngOnInit():void{
        this.userRole = JSON.parse(<string>localStorage.getItem("role"));
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        
       

        this.query={
            region:'',
            states:[]
        };

        this.appQuery={
            promoID:'',
            investorID:''
        };


        this.application={
            application:null,
            submittedDate:null,
            deadline:null,
            title:'',
            status:null,
            promoID:'',
            investorID:'',
            promotionUnitID:'',
            landUpin:'',
            investorName:''
        };
        this.loading=true;
        
        this.getAllApplication();
        
    }

    public getAllBids(){
        this.bidService.searchBid(this.query).subscribe(res=>{
            this.searchResult=res.result;
            for(const sr of this.searchResult){
                if(sr.status == 2){
                    this.activeBids.push(sr);
                }
                else if(sr.status >2){
                    this.outdatedBids.push(sr);
                }
            }
            
        },e=>{
            swal({
                type:'error',
                title:'Oops..',
                text:e &&(e.message|| JSON.stringify(e))||'Unknown error'
            })
        });
       
    }

    public showBidDetails(id:any){
        this.router.navigate([`/investor/bid/${id}`]);
    }

    onImageClick(){
        new imageViewer();
    }

    public getAllApplication(){
        this.investorServices.getInvestor(this.username).subscribe(res=>{
            if(res!=null) {
                this.appQuery.investorID = res.id;
                this.investorId=res.id;


                this.investorServices.searchApplication(this.appQuery).subscribe(res3 => {
                    this.applicationSearchResult = res3.result;

                    for (const app of this.applicationSearchResult) {
                        this.bidService.getPromotionUnit(app.promotionUnitId, app.promoID).subscribe(res2 => {
                            this.promotionUnit = res2;
                            this.investorServices.getInvestorById(this.promotionUnit.winnerInvestor).subscribe(inv=>{

                                if(inv!=null){
                                    let investorProfile=JSON.parse(inv.defaultProfile);
                                    this.application.investorName=investorProfile.Name;
                                }
                                else{
                                    this.application.investorName='';
                                }

                            this.application.promoID = this.promotionUnit.promotion.id;
                            this.application.submittedDate = new Date();
                            this.application.title = this.promotionUnit.promotion.title;
                            this.application.status = this.promotionUnit.status;
                            this.application.deadline = this.promotionUnit.promotion.applyDateTo;
                            this.application.application = app;
                            this.application.investorID = res.id;
                            this.application.promotionUnitID = this.promotionUnit.id;
                            this.application.landUpin = this.promotionUnit.landUPIN;
                            this.application.winnerInvestor=this.promotionUnit.winnerInvestor;
                            this.investorApplication.push(this.application);
                            this.application = {
                                application: null,
                                submittedDate: null,
                                deadline: null,
                                title: '',
                                status: null,
                                promoID: '',
                                investorID: '',
                                promotionUnitID: '',
                                landUpin: '',
                                investorName:''
                            };
                            this.getApplicationList();
                        });
                        });
                      
                    }

                    this.loading=false;
                }, e => {
                    swal({
                        type: 'error', title: 'Oops..', text: e && (e.message || JSON.stringify(e)) || 'Unknown error'
                    })
                })
            }
            else{
                this.loading=false;
               this.notificationCaller();
            }
            this.getAllBids();
        },e=>{
            swal({
                type:'error',
                title:'Oops..',
                text:e &&(e.message|| JSON.stringify(e))||'Unknown error'
            })
        });
   
    }

    public getApplication(promoID:string,promUnit_ID:string, investor_ID:string){
        this.router.navigate([`/investor/inv-app/${promoID}/${promUnit_ID}/${investor_ID}`]);
    }

    getApplicationList(){
        this.listServices.getStatus().subscribe(data=>{
            this.statusList=data;
            this.listServices.getInvetmentType().subscribe(data2=>{
                this.investmentTypeList=data2;
                this.prepareApplicationList();
            })
        })
    }

    prepareApplicationList(){
        for(const app of this.investorApplication){
            for(const st of this.statusList){
                if(app.status===st['id']) {
                    for (const inType of app.application['investmentTypes']) {
                        for (const invT of this.investmentTypeList) {
                            if (inType == invT['id']) {
                                this.investmentType.push(invT['name']);
                            }
                        }
                    }
                    app.status = st['name'];
                    app.application.investmentTypes=this.investmentType;
                }
            }
            this.investmentType=[];
        }
    }
    notificationCaller(){
        $(".ui-pnotify").hide();
        this.notification= new PNotify({
            title: "Notification",
            type: "info",
            text: "Welcome. Your Investor Profile have not register. Please Register Your Profile by clicking on 'PROFILE' menu",
            nonblock: {
                nonblock: true
            },
            addclass: 'dark',
            styling: 'bootstrap3',
            hide: true,
            before_close: function(PNotify) {
                PNotify.update({
                    title: PNotify.options.title + " - Enjoy your Stay",
                    before_close: null
                });

                PNotify.queueRemove();

                return false;
            }
        });
    }
}
