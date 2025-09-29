import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ProjectApiService} from '../../../_services/project-api.service';
import {CommonModule} from "@angular/common";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
    selector: 'app-activity-progress',
  imports:[CommonModule],
    templateUrl: 'activity-progress.component.html',
    styleUrls: ['activity-progress.component.css']
})
export class ActivityProgressComponent implements OnInit {

    @Input('overridePercent') overridePercent?: number;

    @Input('activity') activity: any;
    @Input('reportTime') reportTime: number;

    @Output('calc') calculatedNotification = new EventEmitter<number>();

    percent = 0;

    constructor (private api: ProjectApiService, private keyCase:ObjectKeyCasingService) {
    }

    ngOnInit(): void {
        if (!this.activity.id) { return; }

        if (this.overridePercent != undefined) {
            this.percent = this.overridePercent;
            this.calculatedNotification.emit(this.percent);
            return;
        }

        this.api.calculateProgress(this.activity.id, this.reportTime).subscribe(progress => {
          this.keyCase.camelCase(progress);
           this.percent = progress.value;
           this.calculatedNotification.emit(this.percent);
       });
    }

}
