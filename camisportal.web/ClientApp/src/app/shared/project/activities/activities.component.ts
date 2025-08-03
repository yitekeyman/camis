import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {IActivityItemChange} from './activity-item/interfaces';
import {IActivityPlanTemplate} from './interfaces';
import {ProjectServices} from "../../../_services/project.services";

@Component({
  selector: 'app-activities',
  templateUrl: 'activities.component.html',
  styleUrls: ['activities.component.css']
})
export class ActivitiesComponent implements OnInit {

  /* Inputs */
  @Input('activity') activity: any = {
    schedules: [],
    activityPlanDetails: [],
    children: []
  };
  @Input('progressPercent') progressPercent: number;
  @Input('readonlyMode') readonlyMode = true;
  @Input('reportingMode') reportingMode = false;
  @Input('readonlyReporting') readonlyReporting = true;

  /* Outputs */
  @Output('change') change = new EventEmitter<any>();

  now = Date.now();
  maxTime = this.now;
  minTime = this.now;

  progressMeasuringUnits: any[] = [];
  progressVariables?: any[];
  progressVariableTypes: any[] = [];
  statusTypes: any[] = [];
  variableValueLists: any[] = [];
  activityPlanTemplates?: IActivityPlanTemplate[];

  get isReady(): boolean {
    return !!this.progressVariables && !!this.activityPlanTemplates;
  }

  constructor(private api:ProjectServices) {
  }

  ngOnInit(): void {
      this.api.getUnit().subscribe(unit=>this.progressMeasuringUnits=unit);
      this.api.getVariable().subscribe(progressVariables=>{
        this.progressVariables=progressVariables;
          let progressVariableTypes = [];
          progressVariableTypes = this.progressVariables
              .map(variable => {
                  const type = variable.type;
                  const exists = !!progressVariableTypes.find(value => value.id == type.id);
                  return exists ? null : type;
              })
              .filter(value => value != null);
          this.progressVariableTypes = progressVariableTypes;
      });
      
      this.api.getStatus().subscribe(status=>this.statusTypes=status);
      this.api.getValueList().subscribe(list=>this.variableValueLists=list);
      this.api.getPlanTemp().subscribe(temp=>this.activityPlanTemplates=temp);
      
      this.calcTimes();
  }


  get todayLeft(): number {
    const val = (this.now - this.minTime) / (this.maxTime - this.minTime) * 100;
    return Number.isFinite(val) ? val : 0;
  }

  get rowsCount(): number {
    return this._countTimelineRows();
  }

  private _countTimelineRows(activity = this.activity, count = 1): number {
    if (activity.children && activity.children.length) {
      for (const child of activity.children) {
        count = this._countTimelineRows(child, ++count);
      }
    }

    return Number.isFinite(count) ? count : 0;
  }

  get _scheduleStr(): string {
    // normalized milliseconds...
    const total = this.maxTime - this.minTime;
    const time = this.now - this.minTime;
    const done = this.progressPercent * total;

    const days = Math.ceil((done - time) / (1000 * 60 * 60 * 24));

    return days === 0 ?
      `This project is going right on the planned schedule.` :
      (days > 0 ?
        `This project is going ahead of the planned schedule by ${days} days.` :
        `This project is lagging behind the planned schedule by ${Math.abs(days)} days.`);
  }

  calcTimes(activity: any = this.activity, reset = true): void {
    if (reset) {
      this.now = Date.now();
      this.maxTime = this.now;
      this.minTime = this.now;
    }

    if (activity && activity.schedules && Array.isArray(activity.schedules)) {
      for (const schedule of activity.schedules) {
        if (schedule.to > this.maxTime) {
          this.maxTime = schedule.to;
        }
        if (schedule.from < this.minTime) {
          this.minTime = schedule.from;
        }
      }
    }

    if (activity && activity.children && Array.isArray(activity.children)) {
      for (const child of activity.children) {
        this.calcTimes(child, false);
      }
    }
  }


  onChange($event: IActivityItemChange): void {
    if (!$event.activity) {
      return;
    }

    this.calcTimes($event.activity);
    this.activity = $event.activity;
    this.change.emit($event);
  }

}
