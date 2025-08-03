import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {ActivitiesComponent} from './activities/activities.component';
import {ActivityBarComponent} from './activities/activity-bar/activity-bar.component';
import {ActivityItemComponent} from './activities/activity-item/activity-item.component';
import {ActivityProgressComponent} from './activity-progress/activity-progress.component';

@NgModule({
  declarations: [
    ActivitiesComponent,
    ActivityBarComponent,
    ActivityItemComponent,
    ActivityProgressComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
  ],
  exports: [
    ActivitiesComponent,
    ActivityProgressComponent,
  ],
  providers: []
})
export class ProjectModule { }
