import {Component, Input, OnInit} from "@angular/core";
import {ProjectServices} from "../../../_services/project.services";
import swal from "sweetalert2";
import {InvestorServices} from "../../../_services/investor.services";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
    selector:'app-report-details',
    templateUrl:'./reportDetails.component.html'
})
export class ReportDetailsComponent implements OnInit{
    
    @Input('report') report:any;
    @Input('promotionUnitId') promotionUnitId:string;

    public plan: any;
    public resourceProgresses: any[] = [];
    public outcomeProgresses: any[] = [];


    public now = Date.now();
    public maxTime = this.now;
    public minTime = this.now;
    public status:string;
    
    constructor(public projectService:ProjectServices, public investorServices:InvestorServices, public keyCase:ObjectKeyCasingService){}
    ngOnInit(){
        this.keyCase.camelCase(this.report);
        this._plugInReportDetails(this.report.rootActivity, this.report.activityStatuses, this.report.variableProgresses);
        this.calcTimes();
        this.investorServices.calculateResourceProgress(this.report.rootActivityId, this.promotionUnitId, this.report.reportTime).subscribe(
            resourceProgresses => this.resourceProgresses = resourceProgresses);
        this.investorServices.calculateOutcomeProgress(this.report.rootActivityId, this.promotionUnitId, this.report.reportTime).subscribe(
            outcomeProgresses => this.outcomeProgresses = outcomeProgresses);

        this.investorServices.getPlanFromRootActivity(this.report.rootActivityId, this.promotionUnitId).subscribe(plan => {
            this.plan = plan;
            this.calcTimes();
            this.scheduleStr();
        });
        
    }
    scheduleStr() {
        // normalized milliseconds...
        const total = this.maxTime - this.minTime;
        const time = this.now - this.minTime;
        const done = this.plan.calculatedProgress * total;

        const days = Math.ceil((done - time) / (1000 * 60 * 60 * 24));

        this.status= days === 0 ?
            `On Time` :
            (days > 0 ?
                `Ahead by ${days} days.` :
                `Lagging by ${Math.abs(days)} days.`);
    }

    calcTimes(activity: any = this.report.rootActivity, reset = true): void {
        if (reset) {
            this.now = Date.now();
            this.maxTime = this.now;
            this.minTime = this.now;
        }

        if (activity && activity.schedules && Array.isArray(activity.schedules)) {
            for (const schedule of activity.schedules) {
                if (schedule.to > this.maxTime) { this.maxTime = schedule.to; }
                if (schedule.from < this.minTime) { this.minTime = schedule.from; }
            }
        }

        if (activity && activity.children && Array.isArray(activity.children)) {
            for (const child of activity.children) {
                this.calcTimes(child, false);
            }
        }
    }

    private _plugInReportDetails(activity: any, activityStatuses: any[], variableProgresses: any[]): void {
        if (!activity || !activityStatuses || !variableProgresses) {
            swal({
                type: 'error', title: 'Oops...', text:'Missing parameters for ReportDetailComponent._plugInReportDetails'
            });
            return;
        }

        for (const as of activityStatuses) {
            if (as.activityId == activity.id) {
                activity.progressStatusId = as.statusId;
            }
        }

        for (const detail of activity.activityPlanDetails) {
            for (const vp of variableProgresses) {
                if (vp.activityId == activity.id && vp.variableId == detail.variableId) {
                    detail.progress = vp.progress;
                }
            }
        }

        for (const child of activity.children) {
            this._plugInReportDetails(child, activityStatuses, variableProgresses);
        }
        
        
    }
 

    
   
    
}