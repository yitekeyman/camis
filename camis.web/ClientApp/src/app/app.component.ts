// Angular import
import {Component, NgModule} from '@angular/core';
import {RouterOutlet} from '@angular/router';

// project import
import {SpinnerComponent} from './theme/shared/components/spinner/spinner.component';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ApiService} from "./_services/api.service";
import {CommonModule} from "@angular/common";
import {AddressApiService} from "./_services/address-api.service";
import {AdminServices} from "./_services/admin.Services";
import {FarmApiService} from "./_services/farm-api.service";
import {ObjectKeyCasingService} from "./_services/object-key-casing.service";
import {PagerService} from "./_services/pager.service";
import {ProjectApiService} from "./_services/project-api.service";
import {LandDataService} from "./_services/land-data.service";
import {WorkflowApiService} from "./_services/workflow-api.service";
import {ReportAPIService} from "./_services/report-api.service";
import {DialogModule} from "./_shared/dialog/dialog.module";
import {DialogService} from "./_shared/dialog/dialog.service";
import {BrowserModule} from "@angular/platform-browser";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    RouterOutlet,
    SpinnerComponent,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    DialogModule
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
  ]
})
export class AppComponent {
  title = 'CAMIS-v2';
}
