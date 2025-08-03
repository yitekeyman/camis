import {NgModule} from "@angular/core";
import {Route, RouterModule} from "@angular/router";
import {DefaultheaderComponent} from "./defaultHeader/defaultheader.component";
import {DefaultHomeComponent} from "./home/defaultHome.component";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {AboutUsComponent} from "./aboutus/about-us.component";
import {BidsComponent} from "./bids/bids.component";
import {InvestorBidDetailComponent} from "../investor/investorBidDetail/investorBidDetail.component";
import {BidDetailComponent} from "./bidDetails/bidDetail.component";

const routes:Route[]=[{
    path:'',
    component:DefaultheaderComponent,
    children:[
        {path:'',pathMatch:'full', redirectTo:'bids'},
        {path:'bids', component:BidsComponent},
        {path:'home', component:DefaultHomeComponent},
        {path:'login', component:LoginComponent},
        {path:'register', component:RegisterComponent},
        {path:'about-us', component:AboutUsComponent},
        {path:'bid/:prom_id', component:BidDetailComponent}
    ]
}];
@NgModule({
    imports:[RouterModule.forChild(routes)],
    exports:[RouterModule]
})

export class DefaultRoutingModule {
    
}