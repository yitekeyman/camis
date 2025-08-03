import {Component, Input, OnInit, ViewChild} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {InvestorServices} from "../../../_services/investor.services";
import {BidServices} from "../../../_services/bid.services";
import {ListServices} from "../../../_services/list.services";
import {AuthServices} from "../../../_services/auth.services";
import swal from "sweetalert2";
import {IdocumentPriview} from "../../../_models/bid.model";
import {configs} from "../../../../../../../camis.web/ClientApp/src/app/app-config";
declare var $:any;
@Component({
    selector:'app-promo-unit-detail',
    templateUrl:'./promotionUnit_details.component.html'
})
export class  PromotionUnitDetailsComponent implements OnInit{
    @Input('promotionID') promotionID:string;
    @Input('investorID') investor_id:string;
    @Input('promotionUnitID') promotionUnitID:string;

    public promotionUnit:any;

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
    statusList:any[]=[];
    status:any;
    public loading:boolean=false;

    @ViewChild('gmap') set mapElement(mapel: any) {
        if(mapel && this.map!=undefined)
            this.initMap(mapel);
    };
    map: google.maps.Map;
    polygon: any;
    map_center: any;
    map_bound: any;

    constructor(public activatedRoute:ActivatedRoute, public router:Router, public invService:InvestorServices, public bidService:BidServices, public listServices:ListServices, public authServices:AuthServices){}
    ngOnInit(){
        this.getPromotionUnit();
    }
    
    public getPromotionUnit(){
        this.loading=true;
        
        this.bidService.getPromotionUnit(this.promotionUnitID, this.promotionID).subscribe(res=>{
            this.promotionUnit=res;
            this.getLists();
            for(let img of this.promotionUnit.pictures){
                this.images.push('data:' + img.mime + ';base64,' + img.data);
            }
            this.listServices.getMonth().subscribe(m=>{
                this.monthesList=m;
                this.climates=[];
                for(let climate of this.promotionUnit.landData['climate']){
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
                for(let test of this.promotionUnit.landData['soilTests']){
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
                for(let ac of this.promotionUnit.landData['accessablity']){
                    for(const accList of this.accessablityList){
                        if(ac==accList['id']){
                            ac=accList['name'];
                            this.accessablity.push(ac);
                        }
                    }
                }
                this.promotionUnit.landData.accessablity=this.accessablity;
            });
            this.listServices.getAgroEcoZone().subscribe(ag=>{
                this.agroEcoZoneList=ag;
                this.agroEcoZone=[];
                for(let agr of this.promotionUnit.landData['agroEchologyZone']){
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
                for(let el of this.promotionUnit.landData.existLandUse){
                    for(const elList of this.exisingLandUseList){
                        if(el==elList['id']){
                            this.exisingLandUse.push(elList['name']);
                        }
                    }
                }
                this.promotionUnit.landData.existLandUse=this.exisingLandUse;
            });
            this.listServices.getMoistureSource().subscribe(ms=>{
                this.moistureSourceList=ms;
                this.moistureSource=null;
                for(const msList of this.moistureSourceList){
                    if(msList['id']==this.promotionUnit.landData.moistureSource){
                        this.moistureSource=msList['name'];
                    }
                }
                this.promotionUnit.landData.moistureSource=this.moistureSource;
            });
            this.listServices.getInvetmentType().subscribe(inv=>{
                this.investmentTypeList=inv;
                this.investTypes=[];
                for(const invt of this.promotionUnit.landData['investmentType']){
                    for(const invtList of this.investmentTypeList){
                        if(invt == invtList['id']){
                            this.investTypes.push(invtList['name']);
                        }
                    }
                }
                this.promotionUnit.landData.investmentType=this.investTypes;
            });
            this.listServices.getTopography().subscribe(t=>{
                this.topographyList=t;
                this.topography=[];
                for(let top of this.promotionUnit.landData['topography']){
                    for(const topList of this.topographyList){
                        if(top.topographyType==topList['id']){
                            top['topographyType']=topList['name'];
                            this.topography.push(top);
                        }
                    }
                }
                this.promotionUnit.landData.topography=this.topography;
            });
        })
        this.loading=false;
    }

    initMap(mapel: any): void {
        this.map = new google.maps.Map(mapel.nativeElement, {
            zoom: 5,
            center: new google.maps.LatLng(24.886, -70.268),
            mapTypeId: google.maps.MapTypeId.SATELLITE
        });
        this.bidService.getLandDetails(this.promotionUnitID, this.promotionID).subscribe(res => {
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
 
    prepareInvestmentType(){
        for(const investment of this.promotionUnit.promotion['investmentTypes']){
            for(const invTypeList of this.investmentTypeList){
                if(investment === invTypeList['id']){
                    this.investmentTypes.push(invTypeList['name']);
                }
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
    prepareStatus(){
        for (const stList of this.statusList) {
            if (this.promotionUnit.promotion.status === stList['id']) {
                this.status= stList['name'];
            }
        }
    }

    getArea(area:any) {
        return Math.round(area / 10) / 1000 + ' hec';
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
}