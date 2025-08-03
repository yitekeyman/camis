import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouteReuseStrategy } from '@angular/router';

import { AppComponent } from './_app/app.component';

import {AddressApiService} from './_services/address-api.service';
import {AdminServices} from './admin/admin.Services';
import {ApiService} from './_services/api.service';
import {FarmApiService} from './_services/farm-api.service';
import {ObjectKeyCasingService} from './_services/object-key-casing.service';
import {PagerService} from './_services/pager.service';
import {ProjectApiService} from './_services/project-api.service';
import {WorkflowApiService} from './_services/workflow-api.service';
import { LandDataService } from './_services/land-data.service';

import {AppRoutingModule} from './app-routing.module';
import {DialogModule} from './_shared/dialog/dialog.module';
import {ToastrModule} from 'ngx-toastr';
import { CustomRouteReuseStrategyinterface } from './_shared/land-bank/CustomRouteReuseStrategy.interface';
import { ReportAPIService } from './_services/report-api.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    DialogModule,
    // LandBankModule,
    ToastrModule.forRoot(),
  ],
  providers: [
    AddressApiService,
    AdminServices,
    ApiService,
    FarmApiService,
    ObjectKeyCasingService,
    PagerService,
    ProjectApiService,
    WorkflowApiService,
    LandDataService,
    ReportAPIService,

    { provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategyinterface }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
