import {Component} from "@angular/core";
import {BidServices} from "../../_services/bid.services";
import {Router} from "@angular/router";
import swal from "sweetalert2";
import {ListServices} from "../../_services/list.services";

declare var imageViewer:any;
@Component({
    selector:'app-bids',
    templateUrl:'bids.component.html'
})
export class BidsComponent {
    public userRole:number|null;
    query:any;
    public searchResult:any[];
    statusList:any[]=[];
    status:any;
    promotions:any[]=[];
    
    loading:boolean=false;
    activeBids:any[]=[];
    outdatedBids:any[]=[];
    
    constructor(public bidService:BidServices, public router:Router, public listService:ListServices){}

    ngOnInit():void{
        this.userRole = JSON.parse(<string>localStorage.getItem("role"));

        this.query={
            region:'',
            states:[]
        };
        
        this.getAllBids();
    }

    onImageClick(){
        new imageViewer();
    }
    public getAllBids(){
        this.loading=true;
      this.bidService.searchBid(this.query).subscribe(res=>{
          this.searchResult=res.result;
          for(const sr of this.searchResult){
              if(sr.status == 2){
                  this.activeBids.push(sr);
              }
              else if(sr.status > 2){
                  this.outdatedBids.push(sr);
              }
          }
          this.getLists();
          this.loading=false;
      },e=>{
          swal({
              type:'error',
              title:'Oops..',
              text:e &&(e.message|| JSON.stringify(e))||'Unknown error'
          })
      })
    }
    
    public showBidDetails(id:any){
            this.router.navigate([`/default/bid/${id}`]);
    }
    prepareActive(){
        for(const sRes of this.activeBids){
            for (const stList of this.statusList) {
                if (sRes.status === stList['id']) {
                    sRes['status'] = stList['name'];
                }
            }
        }
    }
    prepareOutdated(){
        for(const sRes of this.outdatedBids){
            for (const stList of this.statusList) {
                if (sRes.status === stList['id']) {
                    sRes['status'] = stList['name'];
                    
                }
            }
        }
    }
    getLists(){
        this.listService.getStatus().subscribe(data=>{
            this.statusList=data;
            this.prepareActive();
            this.prepareOutdated();
        });

    }
}