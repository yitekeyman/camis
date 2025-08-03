import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ProjectApiService} from '../../../_services/project-api.service';

@Component({
    selector: 'app-activity-progress',
    templateUrl: 'activity-progress.component.html',
    styleUrls: ['activity-progress.component.css']
})
export class ActivityProgressComponent implements OnInit {

    @Input('overridePercent') overridePercent?: number;

    @Input('activity') activity: any;
    @Input('reportTime') reportTime: number;

    @Output('calc') calculatedNotification = new EventEmitter<number>();

    percent = 0;

    constructor (private api: ProjectApiService) {
    }

    ngOnInit(): void {
        if (!this.activity.id) { return; }

        if (this.overridePercent != undefined) {
            this.percent = this.overridePercent;
            this.calculatedNotification.emit(this.percent);
            return;
        }

        this.api.calculateProgress(this.activity.id, this.reportTime).subscribe(progress => {
           this.percent = progress.value;
           this.calculatedNotification.emit(this.percent);
       });
    }

}
