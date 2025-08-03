import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-activity-bar',
    templateUrl: 'activity-bar.component.html',
    styleUrls: ['activity-bar.component.css']
})
export class ActivityBarComponent implements OnInit {

    @Input('activity')
    activity: any = {
        schedules: [],
        activityPlanDetails: [],
        children: []
    };

    now = Date.now();

    @Input('maxTime')
    maxTime = this.now;

    @Input('minTime')
    minTime = this.now;


    constructor () {
    }

    ngOnInit(): void {
    }


    get left(): number {
        const schedule = this.activity.schedules && this.activity.schedules.length && this.activity.schedules[0];
        if (!schedule) { return 0; }

        const val = (schedule.from - this.minTime) / (this.maxTime - this.minTime) * 100;
        return Number.isFinite(val) ? val : 0;
    }

    get width(): number {
        const schedule = this.activity.schedules && this.activity.schedules.length && this.activity.schedules[0];
        if (!schedule) { return 0; }

        const val = (schedule.to - schedule.from) / (this.maxTime - this.minTime) * 100;
        return Number.isFinite(val) ? val : 0;
    }

}
