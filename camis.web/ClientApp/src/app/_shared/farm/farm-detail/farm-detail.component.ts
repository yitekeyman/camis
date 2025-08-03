import {Component, Input, OnInit} from '@angular/core';

import {FarmApiService} from '../../../_services/farm-api.service';
import {ProjectApiService} from '../../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import {IActivityItemChange} from '../../project/activities/activity-item/interfaces';
import dialog from '../../dialog';

@Component({
  selector: 'app-farm-detail',
  templateUrl: 'farm-detail.component.html',
})
export class FarmDetailComponent implements OnInit {

  @Input('farm')
  farm: any = {};

  @Input('isOperatorOptional')
  isOperatorOptional = false;

  loading = true;

  frTypes: any[] = [];
  opTypes: any[] = [];
  opOrigins: any[] = [];
  regAuths: any[] = [];
  regTypes: any[] = [];

  plan: any = {};

  progressPercent: number;

  constructor (
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private keyCase: ObjectKeyCasingService
  ) {
  }

  ngOnInit(): void {
    this.api.getAllFarmTypes().subscribe(frTypes => this.frTypes = frTypes, dialog.error);
    this.api.getAllFarmOperatorTypes().subscribe(opTypes => this.opTypes = opTypes, dialog.error);
    this.api.getAllFarmOperatorOrigins().subscribe(opOrigins => this.opOrigins = opOrigins, dialog.error);
    this.api.getAllRegistrationAuthorities().subscribe(regAuths => this.regAuths = regAuths, dialog.error);
    this.api.getAllRegistrationTypes().subscribe(regTypes => this.regTypes = regTypes, dialog.error);

    this.keyCase.camelCase(this.farm);

    if (this.farm.activityPlan && this.farm.activityPlan.rootActivity) {
      this.plan = this.farm.activityPlan;
    } else if (!this.farm.activityPlan && this.farm.activityId) {
      this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
        this.plan = plan;
      }, dialog.error);
    }

    if (this.farm.operator) {
      this.loading = false;
    } else if (!this.farm.operator && this.farm.operatorId) {
      this.api.getFarmOperator(this.farm.operatorId)
        .subscribe(operator => {
          this.farm.operator = operator;
          this.farm.operator.ventures = this.farm.operator.ventures || []
          this.loading = false;
        }, dialog.error);
    } else if (this.isOperatorOptional) {
      this.loading = false;
    }
  }


  onRootActivityItemChange($event: IActivityItemChange): void {
    if (!$event.activity) { return; }

    if (!this.plan) { this.plan = {}; }
    this.plan.rootActivity = $event.activity;
  }


  frType(frTypeId: number): any {
    for (const type of this.frTypes) {
      if (type.id == frTypeId) {
        return type;
      }
    }
    return null;
  }

  opType(opTypeId: number): any {
    for (const type of this.opTypes) {
      if (type.id == opTypeId) {
        return type;
      }
    }
    return null;
  }

  opOrigin(opOriginId: number): any {
    for (const origin of this.opOrigins) {
      if (origin.id == opOriginId) {
        return origin;
      }
    }
    return null;
  }

  regAuth(regAuthId: number): any {
    for (const auth of this.regAuths) {
      if (auth.id == regAuthId) {
        return auth;
      }
    }
    return null;
  }

  regType(regTypeId: number): any {
    for (const type of this.regTypes) {
      if (type.id == regTypeId) {
        return type;
      }
    }
    return null;
  }


  parseGender(gender: string): string {
    switch (gender) {
      case 'F': return 'Female';
      case 'M': return 'Male';
      default: return 'Unknown.';
    }
  }

  parseMartialStatus(martialStatus: number): string {
    switch (martialStatus) {
      case 1: return 'Not Married';
      case 2: return 'Married';
      case 3: return 'Divorced';
      case 4: return 'Widowed';
      default: return 'Unknown.';
    }
  }

}
