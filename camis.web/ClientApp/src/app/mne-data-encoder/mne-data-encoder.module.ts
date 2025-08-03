import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {MneDataEncoderRoutingModule} from './mne-data-encoder-routing.module';
import {DocumentModule} from '../_shared/document/document.module';
import {FarmModule} from '../_shared/farm/farm.module';
import {DashboardModule} from '../_shared/dashboard/dashboard.module';
import {MainShellModule} from '../_shared/main-shell/main-shell.module';
import {ProjectModule} from '../_shared/project/project.module';

import {MneDataEncoderComponent} from './mne-data-encoder/mne-data-encoder.component';
import {MnedeDashboardComponent} from './mnede-dashboard/mnede-dashboard.component';
import {MnedeFarmsComponent} from './mnede-farms/mnede-farms.component';
import {MnedeFarmViewComponent} from './mnede-farm-view/mnede-farm-view.component';
import {MnedePrReadyComponent} from './mnede-pr-ready/mnede-pr-ready.component';
import {MnedeReportViewComponent} from './mnede-report-view/mnede-report-view.component';
import {MnedeReportsComponent} from './mnede-reports/mnede-reports.component';

@NgModule({
    declarations: [
        MneDataEncoderComponent,
        MnedeDashboardComponent,
        MnedeFarmsComponent,
        MnedeFarmViewComponent,
        MnedePrReadyComponent,
        MnedeReportViewComponent,
        MnedeReportsComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MneDataEncoderRoutingModule,

        DashboardModule,
        DocumentModule,
        FarmModule,
        MainShellModule,
        ProjectModule,
    ],
    providers: []
})
export class MneDataEncoderModule { }
