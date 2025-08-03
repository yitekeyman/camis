import {Component, Input, OnInit} from '@angular/core';

import {ProjectApiService} from '../../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import dialog from '../../dialog';

@Component({
  selector: 'app-report-detail',
  templateUrl: 'report-detail.component.html',
})
export class ReportDetailComponent implements OnInit {

  loading = true;

  @Input('report') report: any;
  plan: any;


  resourceProgresses: any[] = [];
  outcomeProgresses: any[] = [];


  now = Date.now();
  maxTime = this.now;
  minTime = this.now;


  constructor (private api: ProjectApiService, private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit(): void {
    this.keyCase.camelCase(this.report);

    this._plugInReportDetails(this.report.rootActivity, this.report.activityStatuses, this.report.variableProgresses);
    this.keyCase.camelCase(this.report); // intentional repetition

    this.api.calculateResourceProgress(this.report.rootActivityId, this.report.reportTime).subscribe(
      resourceProgresses => this.resourceProgresses = resourceProgresses, dialog.error);
    this.api.calculateOutcomeProgress(this.report.rootActivityId, this.report.reportTime).subscribe(
      outcomeProgresses => this.outcomeProgresses = outcomeProgresses, dialog.error);

    this.api.getPlanFromRootActivity(this.report.rootActivityId).subscribe(plan => {
      this.plan = plan;
      this.calcTimes();
      this.loading = false;
    });
  }


  get _scheduleStr(): string {
    // normalized milliseconds...
    const total = this.maxTime - this.minTime;
    const time = this.now - this.minTime;
    const done = this.plan.calculatedProgress * total;

    const days = Math.ceil((done - time) / (1000 * 60 * 60 * 24));

    return days === 0 ?
      `On Time` :
      (days > 0 ?
        `Ahead by ${days} days.` :
        `Lagging by ${Math.abs(days)} days.`);
  }

  calcTimes(activity: any = this.plan.rootActivity, reset = true): void {
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
      dialog.error('Missing parameters for ReportDetailComponent._plugInReportDetails');
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
