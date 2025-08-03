import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ReportModule } from '../report/report.module';
import { CamisMapModule } from '../camismap/camismap.module';

import { LandMapComponent } from './land-map/land-map.component';
import { SearchLandComponent } from './search-land/search-land.component';
import { SearchResultDetailComponent } from './search-result-detail/search-result-detail.component';
import { DetailLandMapComponent } from './detail-land-map/detail-land-map.component';
import { EditLandComponent } from './edit-land/edit-land.component';
import { PendingTaskListComponent } from './pending-task-list/pending-task-list.component';
import { LandbankDocumentSelectorComponent } from './landbank-document-selector/landbank-document-selector.component'
import {DocumentModule} from '../document/document.module';
@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ReportModule,
    CamisMapModule,
    DocumentModule,
  ],
  declarations: [
    LandMapComponent,
    SearchLandComponent,
    SearchResultDetailComponent,
    DetailLandMapComponent,
    EditLandComponent,
    PendingTaskListComponent,
    LandbankDocumentSelectorComponent
  ],
  exports: [
    LandMapComponent,
    SearchLandComponent,
    SearchResultDetailComponent,
    DetailLandMapComponent,
    EditLandComponent,
    PendingTaskListComponent,
    LandbankDocumentSelectorComponent

  ]
})
export class LandBankModule { }
