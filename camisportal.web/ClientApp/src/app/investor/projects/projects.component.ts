import {Component, OnInit} from "@angular/core";
import {Router} from "@angular/router";
import {ProjectServices} from "../../_services/project.services";
import {InvestorServices} from "../../_services/investor.services";

@Component({
    selector:'.app_projects',
    templateUrl:'./projects.component.html'
})

export class ProjectsComponent implements OnInit{
    public loading:boolean=false;
    public projects:any[]=[];
    public activitiesPlan:any[]=[];
    public invQuery:any;
    public username:string;
    public applicationSearchResult:any;
    
    constructor(public router:Router, public projectService:ProjectServices, public investorService:InvestorServices){}
    
    ngOnInit(){
        this.loading=true;
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        this.getProjects();
        this.invQuery={
            promoID:'',
            investorID:'',
            promoUnitID:''
        }
    }
    
    showProjectDetails(promoUnitId:string, investorId:string){
        this.router.navigate([`investor/projectDetails/${promoUnitId}/${investorId}`]);
    }
    
    getProjects(){
        this.investorService.getInvestor(this.username).subscribe(inv=>{
            this.invQuery.investorID=inv.id;
            this.investorService.searchApplication(this.invQuery).subscribe(app=>{
                this.applicationSearchResult=app.result;
                for(let plan of this.applicationSearchResult){
                    if(plan.investment!=null){
                        this.investorService.getLatestApplication(plan.promotionUnitId, plan.invProfile.id).subscribe(pro=>{
                           if(pro.isApproved==true)
                                this.projects.push(pro);
                        });
                       
                    }
                }
                this.loading=false;
            });
        })
       
    }
    
}