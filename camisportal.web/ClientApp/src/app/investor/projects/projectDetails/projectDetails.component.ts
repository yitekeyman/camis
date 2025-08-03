import {Component, OnInit} from "@angular/core";
import {ProjectServices} from "../../../_services/project.services";
import {ActivatedRoute, Router} from "@angular/router";
import {IActivityItemChange} from "../../../shared/project/activities/activity-item/interfaces";
import {InvestorServices} from "../../../_services/investor.services";
declare var $:any;
@Component({
    selector:'.app-project-detail',
    templateUrl:'./projectDetails.component.html'
})


export class ProjectDetailsComponent implements OnInit{
    public loading:boolean=false;
    public projectList:any[];
    public activityList:any[];
    public reports:any[]=[];
    public projectID:any;
    public project:any;
    public plan:any;
    public report:any=null;
    public investorID:string;
    public promotionUnitID:string;
    progressPercent: number;
    resourceProgresses: any[] = [];
    outcomeProgresses: any[] = [];
    constructor(public projectService:ProjectServices, public activeRoute:ActivatedRoute, public router:Router, public investorServices:InvestorServices){}
    
    ngOnInit(){
        this.loading=true;
        this.promotionUnitID=this.activeRoute.snapshot.params['promoUnitId'];
        this.investorID=this.activeRoute.snapshot.params['investorId'];
        this.getProject();
    }
    
    get
    getProject(){
        this.investorServices.getLatestApplication(this.promotionUnitID, this.investorID).subscribe(pro=>{
            this.project=pro.investment;
            this.plan=pro.activityPlan;
            this.investorServices.getProjects(this.plan.id, this.promotionUnitID).subscribe(rep=>{
                this.reports=rep.items;
            });
            this.loading=false;
        });
       

        /*this.projectService.getProject().subscribe(data=>{
            for(let pro of data){
                if(pro.id==this.projectID){
                    this.project=pro;
                    this.projectService.getActivities().subscribe(data2=>{
                        for(let act of data2){
                            if(pro.activityId==act.rootActivityId){
                                this.plan=act;
                                this.projectService.getReports().subscribe(data3=>{
                                    for(const rep of data3){
                                        if(act.rootActivityId==rep.rootActivityId){
                                            this.reports.push(rep);
                                        }
                                    }
                                })
                            }
                           
                        }
                    })
                    
                }
            }
            this.loading=false;
        })*/
    }

    onRootActivityItemChange($event: IActivityItemChange): void {
        if (!$event.activity) { return; }

        if (!this.plan) { this.plan = {}; }
        this.plan.rootActivity = $event.activity;
    }

    openDetail(report: any) {
        this.report=report;
        $("#view-promotion-dialog").modal('show');
    }

    close(){
        this.report=null;
        this.report.calculatedProgress=null;
    }
    
    evaluate(){
        this.router.navigate([`investor/selfEvaluation/${this.promotionUnitID}/${this.investorID}`]);
    }
}