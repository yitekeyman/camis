import {Component, Input, OnInit, ViewChild} from '@angular/core';

import {FarmApiService} from '../../../_services/farm-api.service';
import {ProjectApiService} from '../../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import {IActivityItemChange} from '../../project/activities/activity-item/interfaces';
import dialog from '../../dialog';
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {AddressModule} from "../../address/address.module";
import {DocumentModule} from "../../document/document.module";
import {ProjectModule} from "../../project/project.module";
import {
  SimpleLandBankDetails
} from "../../land-bank/search-result-detail/simple-land-bank-details/simple-land-bank-details.component";
import {CamisMapComponent} from "../../camismap/camismap.component";
import {LandDataService} from "../../../_services/land-data.service";

@Component({
  selector: 'app-farm-detail',
  imports: [CommonModule, ReactiveFormsModule, AddressModule, DocumentModule, ProjectModule, SimpleLandBankDetails, CamisMapComponent],
  templateUrl: 'farm-detail.component.html',
  styleUrls: ['farm-detail.component.scss']

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
  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private keyCase: ObjectKeyCasingService,
    private landService: LandDataService
  ) {
  }

  ngOnInit(): void {
    this.api.getAllFarmTypes().subscribe(frTypes => {
      this.keyCase.camelCase(frTypes);
      this.frTypes = frTypes
    }, dialog.error);
    this.api.getAllFarmOperatorTypes().subscribe(opTypes => {
      this.keyCase.camelCase(opTypes);
      this.opTypes = opTypes
    }, dialog.error);
    this.api.getAllFarmOperatorOrigins().subscribe(opOrigins => {
      this.keyCase.camelCase(opOrigins);
      this.opOrigins = opOrigins
    }, dialog.error);
    this.api.getAllRegistrationAuthorities().subscribe(regAuths => {
      this.keyCase.camelCase(regAuths);
      this.regAuths = regAuths
    }, dialog.error);
    this.api.getAllRegistrationTypes().subscribe(regTypes => {
      this.keyCase.camelCase(regTypes);
      this.regTypes = regTypes
    }, dialog.error);

    this.keyCase.camelCase(this.farm);
    if (this.farm.farmLands?.length > 0) {
      this.landService.GetLand(this.farm.farmLands[0].landId).subscribe(data => {
        this.keyCase.camelCase(data);
        let g = data.parcels[data.upins[0]];
        if (g) {
          let parts = g.geometry.split(";");
          this.map.setWorkFlowGeomByWKT(parts[parts.length - 1]);
        }
      }, e => {
        return dialog.error(e);
      });
    }

    if (this.farm.activityPlan && this.farm.activityPlan.rootActivity) {
      this.plan = this.farm.activityPlan;
    } else if (!this.farm.activityPlan && this.farm.activityId) {
      this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
        this.keyCase.camelCase(plan);
        this.plan = plan;
      }, dialog.error);
    }

    if (this.farm.operator) {
      this.loading = false;
    } else if (!this.farm.operator && this.farm.operatorId) {
      this.api.getFarmOperator(this.farm.operatorId)
        .subscribe(operator => {
          this.keyCase.camelCase(operator);
          this.farm.operator = operator;
          this.farm.operator.ventures = this.farm.operator.ventures || []
          this.loading = false;
        }, dialog.error);
    } else if (this.isOperatorOptional) {
      this.loading = false;
    }
  }


  onRootActivityItemChange($event: IActivityItemChange): void {
    if (!$event.activity) {
      return;
    }

    if (!this.plan) {
      this.plan = {};
    }
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
      case 'F':
        return 'Female';
      case 'M':
        return 'Male';
      default:
        return 'Unknown.';
    }
  }

  parseMartialStatus(martialStatus: number): string {
    switch (martialStatus) {
      case 1:
        return 'Not Married';
      case 2:
        return 'Married';
      case 3:
        return 'Divorced';
      case 4:
        return 'Widowed';
      default:
        return 'Unknown.';
    }
  }
  getPhotoSource(): string {
    if (this.farm?.operator?.photo?.file) {
      // Convert base64 string to data URL for display
      return `data:${this.farm.operator.photo.mimetype};base64,${(this.farm.operator.photo.file)}`;
    }
    return 'assets/images/user/user_profile.PNG';
  }
}
