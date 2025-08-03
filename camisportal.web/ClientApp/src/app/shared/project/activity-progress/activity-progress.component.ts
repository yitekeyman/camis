import {AfterViewInit, Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ProjectServices} from "../../../_services/project.services";
import {InvestorServices} from "../../../_services/investor.services";

@Component({
    selector: 'app-activity-progress',
    templateUrl: 'activity-progress.component.html',
    styleUrls: ['activity-progress.component.css']
})
export class ActivityProgressComponent implements OnInit, AfterViewInit{

    @Input('overridePercent') overridePercent: number;

    @Input('activity') activity: any;
    @Input('reportTime') reportTime: number;
    @Input('activityId') activityId:string;
    @Input('promotionUnitId') promotionUnitId:string;
    @Output('calc') calculatedNotification = new EventEmitter<number>();

    percent = 0;

    constructor (public api:ProjectServices, public investorService:InvestorServices) {
    }

    ngOnInit(){
        this.percent=0;
        if (!this.activity.id) { return; }

        if (this.overridePercent != undefined) {
            this.percent = this.overridePercent;
            this.calculatedNotification.emit(this.percent);
            return;
        }

        this.investorService.calculatedProgress(this.activityId, this.promotionUnitId, this.reportTime).subscribe(progress => {
           
                    this.percent = progress.value;
                    this.calculatedNotification.emit(this.percent);
            
       });
    }
    ngAfterViewInit(){
        this.percent=0;
        if (!this.activity.id) { return; }

        if (this.overridePercent != undefined) {
            this.percent = this.overridePercent;
            this.calculatedNotification.emit(this.percent);
            return;
        }
    }

}
