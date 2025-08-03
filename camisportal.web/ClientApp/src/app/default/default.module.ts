import {NgModule} from "@angular/core";
import {DefaultheaderComponent} from "./defaultHeader/defaultheader.component";
import {DefaultHomeComponent} from "./home/defaultHome.component";
import {LoginComponent} from "./login/login.component";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {DefaultRoutingModule} from "./default.routing.module";
import {MainBodyModule} from "../shared/main-body/main-body.module";
import {RegisterComponent} from "./register/register.component";
import {AboutUsComponent} from "./aboutus/about-us.component";
import {BidsComponent} from "./bids/bids.component";
import {InvestorBidDetailComponent} from "../investor/investorBidDetail/investorBidDetail.component";
import {BidDetailComponent} from "./bidDetails/bidDetail.component";
import {PromotionDetailsModule} from "../shared/promotionDetail/promotion_details.module";

@NgModule({
    declarations:[
        DefaultheaderComponent,
        DefaultHomeComponent,
        AboutUsComponent,
        LoginComponent,
        RegisterComponent,
        BidsComponent,
        BidDetailComponent
    ],
    imports:[
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        DefaultRoutingModule,
        MainBodyModule,
        PromotionDetailsModule
    ],
    providers:[]
})
export class DefaultModule {
    
}