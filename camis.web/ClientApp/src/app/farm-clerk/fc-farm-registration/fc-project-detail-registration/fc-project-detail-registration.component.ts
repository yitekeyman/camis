import {Component, Input, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ProjectApiService} from '../../../_services/project-api.service';
import {FormBuilder} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {IActivityItemChange} from '../../../_shared/project/activities/activity-item/interfaces';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-fc-project-detail-registration',
  templateUrl: 'fc-project-detail-registration.component.html'
})
export class FcProjectDetailRegistrationComponent implements OnInit {

  @Input('plan') activityPlan: any;

  statusTypes: any[] = [];


  constructor (private router: Router, private api: ProjectApiService, private fb: FormBuilder, private toastor: ToastrService,  private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit(): void {
    this.api.getAllActivityStatusTypes().subscribe(statusTypes => {
      this.keyCase.camelCase(statusTypes);
      this.statusTypes = statusTypes;
      if (this.statusTypes.length) { this.activityPlan.statusId = this.statusTypes[0].id; }
    });
  }

  onRootActivityItemChange($event: IActivityItemChange): void {
    if (!$event.activity) { return; }

    this.activityPlan.rootActivity = $event.activity;
  }

}
