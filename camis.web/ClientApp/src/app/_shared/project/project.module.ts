import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {DocumentModule} from '../document/document.module';

import {ActivitiesComponent} from './activities/activities.component';
import {ActivityBarComponent} from './activities/activity-bar/activity-bar.component';
import {ActivityItemComponent} from './activities/activity-item/activity-item.component';
import {ActivityProgressComponent} from './activity-progress/activity-progress.component';
import {ReportDetailComponent} from './report-detail/report-detail.component';

@NgModule({
  declarations: [
    ActivitiesComponent,
    ActivityBarComponent,
    ActivityItemComponent,
    ActivityProgressComponent,
    ReportDetailComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    DocumentModule,
  ],
  exports: [
    ActivitiesComponent,
    ActivityProgressComponent,
    ReportDetailComponent,
  ],
  providers: []
})
export class ProjectModule { }
