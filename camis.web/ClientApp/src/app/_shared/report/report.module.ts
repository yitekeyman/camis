import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import { ReportComponent} from './report.component';
import { GenerateReportComponent } from './generate-report/generate-report.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    declarations: [
      ReportComponent,
      GenerateReportComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        ReactiveFormsModule
    ],
    exports: [
      ReportComponent,
      GenerateReportComponent
    ],
    providers: []
})
export class ReportModule { }
