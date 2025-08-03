import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import {AppRoutingModule} from "./app.routing.module";
import {ApiService} from "./_services/api.services";
import {BidServices} from "./_services/bid.services";
import {AuthServices} from "./_services/auth.services";
import {ResolverServices} from "./_services/resolver.services";
import {InvestorBidDetailComponent} from "./investor/investorBidDetail/investorBidDetail.component";
import {AboutUsComponent} from "./default/aboutus/about-us.component";
import {DefaultHomeComponent} from "./default/home/defaultHome.component";
import {InvestorServices} from "./_services/investor.services";
import {AddressServices} from "./_services/address.services";
import {ListServices} from "./_services/list.services";
import {ProjectServices} from "./_services/project.services";
import {ObjectKeyCasingService} from "./_services/object-key-casing.service";


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule, 
      AppRoutingModule,
  ],
  providers: [
      ApiService,
      BidServices,
      AuthServices,
      ResolverServices,
      InvestorServices,
      AddressServices,
      ListServices,
      ProjectServices,
      ObjectKeyCasingService
      
  ],
    
  bootstrap: [AppComponent]
})
export class AppModule { }
