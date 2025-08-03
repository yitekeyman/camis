import {Component, OnInit, Output, Input, EventEmitter, ViewChild} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import { } from '@types/googlemaps';
import {ListServices} from "../../_services/list.services";
import {AuthServices} from "../../_services/auth.services";

declare var imageViewer:any;
declare var $:any;
@Component({
    selector:'app-bid-detail',
    templateUrl:'./bidDetail.component.html'
})
export class BidDetailComponent implements OnInit{
    public userRole:number|null;
    public promotionID:string;
    public isApply:boolean=false;
    public investor_id:string='';
   
    constructor(public activatedRoute:ActivatedRoute, public router:Router, public bidService:BidServices, public listServices:ListServices, public authServices:AuthServices){}
    
    ngOnInit():void{
       
        this.userRole=JSON.parse(<string>localStorage.getItem("role"));
        this.promotionID=this.activatedRoute.snapshot.params['prom_id'];
    }
    
}