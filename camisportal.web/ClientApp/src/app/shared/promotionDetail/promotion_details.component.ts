import {Component, Input, OnInit, ViewChild} from "@angular/core";
import {IdocumentPriview} from "../../_models/bid.model";
import {ActivatedRoute, Router} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {ListServices} from "../../_services/list.services";
import {AuthServices} from "../../_services/auth.services";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";
import swal from "sweetalert2";
import dialog from "../loader/loader_dialog";
import {InvestorServices} from "../../_services/investor.services";
import {e, p, r} from "@angular/core/src/render3";

declare var imageViewer:any;
declare var $:any;

@Component({
    selector:'app-promotion-detail',
    templateUrl:'./promotion_details.component.html'
})
export class PromotionDetailsComponent implements OnInit{
    @Input('promotionID') promotionID:string;
    @Input('investorID') investor_id:string;
    @Input('isApply') isApply:boolean;

    public userRole:number|null;
    public promotion:any;
    public promotionUnit:any;
    public promotionUnit1:any;
    public proUnit:any[]=[];
    public application:any;

    climates: any[] = [];
    soilTests:any[]=[];
    investmentTypes:any[]=[];
    accessablity:any[]=[];
    agroEcoZone:any[]=[];
    regions:any;
    investTypes:any[]=[];
    topography:any[]=[];
    exisingLandUse:any[]=[];
    moistureSource:any[]=[];

    monthesList: any[] = [];
    soilTestsList:any[]=[];
    investmentTypeList:any[]=[];
    regionList:any[]=[];
    images:any[]=[];
    accessablityList:any[]=[];
    agroEcoZoneList:any[]=[];
    topographyList:any[]=[];
    exisingLandUseList:any[]=[];
    moistureSourceList:any[]=[];
    public app:boolean;


    statusList:any[]=[];
    status:any;
    public loading:boolean=false;
    public username:string;
    @Input('document') document: IdocumentPriview;

   /* @ViewChild('gmap') set mapElement(mapel: any) {
        if(mapel)
            this.initMap(mapel);
    };*/
    map: google.maps.Map;
    polygon: any;
    map_center: any;
    map_bound: any;



    constructor(public activatedRoute:ActivatedRoute, public router:Router, public invService:InvestorServices, public bidService:BidServices, public listServices:ListServices, public authServices:AuthServices){}

    ngOnInit(){
        this.userRole=JSON.parse(<string>localStorage.getItem("role"));
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        this.promotionUnit={
            id:'',
            landData:null,
            pictures:null,
            documents:null,
            isApply:false
        };
        this.getPromotion();
    }

    onImageClick(){
        new imageViewer();
    }
    getPromotion(){
        this.loading=true;
        this.bidService.getPromotion(this.promotionID).subscribe(res=>{

            this.promotion=res;
            this.getLists();
            for(let pro of this.promotion.promotionUnit) {

              
                this.promotionUnit.landData=pro.landData;
                this.images=[];
                for(let img of pro.pictures){
                    this.images.push('data:' + img.mime + ';base64,' + img.data);
                }
                this.listServices.getMonth().subscribe(m=>{
                    this.monthesList=m;
                    this.climates=[];
                    for(let climate of pro.landData['climate']){
                        for (const climateList of this.monthesList) {
                            if (climate.month === climateList['id']) {
                                climate['month'] = climateList['name'];
                                this.climates.push(climate);
                            }
                        }
                    }
                    this.promotionUnit.landData.climate=this.climates;
                });

                this.listServices.getSoilTest().subscribe(s=>{
                    this.soilTestsList=s;
                    this.soilTests=[];
                    for(let test of pro.landData['soilTests']){
                        for(const testList of this.soilTestsList){
                            if(test.testType===testList['id']){
                                test['testType']=testList['name'];
                                this.soilTests.push(test);
                            }
                        }
                    }
                    this.promotionUnit.landData.soilTests=this.soilTests;
                });
                this.listServices.getAccessbility().subscribe(a=>{
                    this.accessablityList=a;
                    this.accessablity=[];
                    for(let ac of pro.landData['accessablity']){
                        for(const accList of this.accessablityList){
                            if(ac==accList['id']){
                                ac=accList['name'];
                                this.accessablity.push(ac);
                            }
                        }
                    }
                    pro.landData.accessablity=this.accessablity;
                });
                this.listServices.getAgroEcoZone().subscribe(ag=>{
                    this.agroEcoZoneList=ag;
                    this.agroEcoZone=[];
                    for(let agr of pro.landData['agroEchologyZone']){
                        for(const agrList of this.agroEcoZoneList){
                            if(agr.agroType==agrList['id']){
                                agr['agroType']=agrList['name'];
                                this.agroEcoZone.push(agr);
                            }
                        }
                    }
                    this.promotionUnit.landData.agroEchologyZone=this.agroEcoZone;
                });
                this.listServices.getExistingLandUse().subscribe(elu=>{
                    this.exisingLandUseList=elu;
                    this.exisingLandUse=[];
                    for(let el of pro.landData.existLandUse){
                        for(const elList of this.exisingLandUseList){
                            if(el==elList['id']){
                                this.exisingLandUse.push(elList['name']);
                            }
                        }
                    }
                    pro.landData.existLandUse=this.exisingLandUse;
                });
                this.listServices.getMoistureSource().subscribe(ms=>{
                    this.moistureSourceList=ms;
                    this.moistureSource=null;
                    for(const msList of this.moistureSourceList){
                        if(msList['id']==pro.landData.moistureSource){
                            this.moistureSource=msList['name'];
                        }
                    }
                    pro.landData.moistureSource=this.moistureSource;
                });
                this.listServices.getInvetmentType().subscribe(inv=>{
                    this.investmentTypeList=inv;
                    this.investTypes=[];
                    for(const invt of pro.landData['investmentType']){
                        for(const invtList of this.investmentTypeList){
                            if(invt == invtList['id']){
                                this.investTypes.push(invtList['name']);
                            }
                        }
                    }
                    pro.landData.investmentType=this.investTypes;
                });
                this.listServices.getTopography().subscribe(t=>{
                    this.topographyList=t;
                    this.topography=[];
                    for(let top of pro.landData['topography']){
                        for(const topList of this.topographyList){
                            if(top.topographyType==topList['id']){
                                top['topographyType']=topList['name'];
                                this.topography.push(top);
                            }
                        }
                    }
                    this.promotionUnit.landData.topography=this.topography;
                });
                
              
                this.promotionUnit.pictures=this.images;
                this.promotionUnit.documents=pro.documents;
                this.promotionUnit.id=pro.id;
              
                this.proUnit.push(this.promotionUnit);
                this.promotionUnit={
                    id:'',
                    landData:[],
                    pictures:null,
                    documents:null,
                    isApply:false
                        
                };
            }
            this.loading=false;
        }, e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }


   /* initMap(mapel: any): void {
        this.map = new google.maps.Map(mapel.nativeElement, {
            zoom: 5,
            center: new google.maps.LatLng(24.886, -70.268),
            mapTypeId: google.maps.MapTypeId.SATELLITE
        });
        this.bidService.getLandDetails(this.promotionID).subscribe(res => {
            if (this.map && res.polygon && res.polygon.length > 0) {
                // Define the LatLng coordinates for the polygon's path.

                this.map_center = new google.maps.LatLng(res.location.lat, res.location.lng);
                // Construct the polygon.
                this.polygon = [];
                for (var p in res.polygon) {
                    var mp = new google.maps.Polygon({
                        paths: res.polygon[0],
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: '#FF0000',
                        fillOpacity: 0.1
                    });
                    this.polygon.push(mp);
                }
                this.map_bound = new google.maps.LatLngBounds(new google.maps.LatLng(res.bottomLeft.lat, res.bottomLeft.lng),
                    new google.maps.LatLng(res.topRight.lat, res.topRight.lng));

                if (this.polygon) {
                    for (var i  in this.polygon)
                        this.polygon[i].setMap(this.map);
                }
                //if (this.map_center)
                //  this.map.setCenter(this.map_center);
                if (this.map_bound)
                    this.map.fitBounds(this.map_bound);

            }
        }, e => {
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });

    }*/
    public participateBid(prom_id:any, promUnit_id:any){
        if(this.userRole==1){
            if(this.investor_id!=null) {
                this.bidService.getApplication(promUnit_id, this.investor_id).subscribe(res => {
                    this.application = res;
                    if (this.application != null && promUnit_id == this.application.promotionUnitId) {
                        this.router.navigate([`/investor/inv-app/${prom_id}/${promUnit_id}/${this.investor_id}`]);
                    } else {
                        this.router.navigate([`investor/bid-reg/${prom_id}/${promUnit_id}`]);
                    }
                });
            }else{
                this.router.navigate([`investor/bid-reg/${prom_id}/${promUnit_id}`]);
            }
           
        }
        else{
            this.router.navigate([`default/login`]);
            localStorage.setItem('routerLink', JSON.stringify(`investor/bid-reg/${prom_id}/${promUnit_id}`));
        }
    }

    public seeApplication(promoID:string,promUnit_ID:string, investor_ID:string){
        this.router.navigate([`/investor/inv-app/${promoID}/${promUnit_ID}/${investor_ID}`]);
    }


    getUnitLists(){
        this.listServices.getMonth().subscribe(data=>{
            this.monthesList=data;
            this.prepareMonth();
            this.promotionUnit1.climates=this.climates;
        });

        this.listServices.getSoilTest().subscribe(data=>{
            this.soilTestsList=data;
            this.prepareSoilTest();
            this.promotionUnit1.soilTests=this.soilTests;
        });

    }
    getLists(){

        this.listServices.getInvetmentType().subscribe(data=>{
            this.investmentTypeList=data;
            this.prepareInvestmentType();
        });

        this.listServices.getRegion().subscribe(data=>{
            this.regionList=data;
            this.prepareRegion();
        });

        this.listServices.getStatus().subscribe(data=>{
            this.statusList=data;
            this.prepareStatus();
        })
    }
    prepareMonth(){

    }

    prepareSoilTest(){

    }

    prepareInvestmentType(){
        for(const investment of this.promotion['investmentTypes']){
            for(const invTypeList of this.investmentTypeList){
                if(investment === invTypeList['id']){
                    this.investmentTypes.push(invTypeList['name']);
                }
            }
        }
    }

    prepareRegion(){
        for(const region of this.regionList){
            if(region['code']===this.promotion.region){
                this.regions=region['Name'];
            }
        }
    }
    prepareStatus(){
        for (const stList of this.statusList) {
            if (this.promotion.status === stList['id']) {
                this.status= stList['name'];
            }
        }
    }

    getLink(doc:IdocumentPriview, promID:string): string {
        if (doc.overrideFilePath) {
            return doc.overrideFilePath;
        }
        return doc.data && doc.mime ?
            `javascript:;` :
            `${configs.url}Bid/GetPromotionDocument?promId=${promID}`;
    }

    openDoc(e:any, doc:IdocumentPriview, promID:string, promUnitID:string){

        let mimeType=doc.mime.split('/').pop();

        if(mimeType == 'pdf' || mimeType=='png' || mimeType=='jpg' || mimeType=='jpeg'){
            doc.overrideFilePath=`${configs.url}Bid/GetPromotionDocument?promId=${promID}&promUnitId=${promUnitID}`;
            $("#view-document-dialog").modal("show");
            $("#docIframe").attr("src", doc.overrideFilePath);
        }
        else{
            doc.overrideFilePath= `${configs.url}Bid/GetPromotionDocument?promId=${promID}`;
            window.open(doc.overrideFilePath);
        }

    }


    downloadDoc(e:any, doc:IdocumentPriview, promID:string){
        doc.overrideFilePath= `${configs.url}Bid/DownloadPromotionDocument?promId=${promID}`;
        window.open(doc.overrideFilePath,'_blank');
    }

    getArea(area:any) {
        return Math.round(area / 10) / 1000 + ' hec';
    }
    
    getApp(promUnitID:string){
            this.bidService.getApplication(promUnitID,this.investor_id).subscribe(invApp=>{
                this.application=invApp;
                if(this.application!=null && promUnitID==this.application.promotionUnitId){
                   this.promotionUnit.isApply=true;
                }else {
                    this.promotionUnit.isApply = false;
                }
                
            });
    }
   
}