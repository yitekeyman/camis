import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ReportModule } from '../report/report.module';
import { CamisMapModule } from '../camismap/camismap.module';

import { SearchLandComponent } from './search-land/search-land.component';
import { SearchResultDetailComponent } from './search-result-detail/search-result-detail.component';
import { EditLandComponent } from './edit-land/edit-land.component';
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
    SearchLandComponent,
    SearchResultDetailComponent,
    EditLandComponent,
    LandbankDocumentSelectorComponent
  ],
  declarations: [
  ],
  exports: [
    SearchLandComponent,
    SearchResultDetailComponent,
    EditLandComponent,
    LandbankDocumentSelectorComponent

  ]
})
export class LandBankModule { }
