import {Route, RouterModule} from "@angular/router";
import {InvestorHeaderComponent} from "./investor-header/investor-header.component";
import {DefaultHomeComponent} from "../default/home/defaultHome.component";
import {AboutUsComponent} from "../default/aboutus/about-us.component";
import {NgModule} from "@angular/core";
import {InvestorbidsComponent} from "./investorbids/investorbids.component";
import {InvestorBidDetailComponent} from "./investorBidDetail/investorBidDetail.component";
import {InvestorBidRegistrationComponent} from "./investorBidRegistration/investorBidRegistration.component";
import {InvestorAboutUsComponent} from "./aboutus/investorAbout-us.component";
import {InvestorHomeComponent} from "./home/investorHome.component";
import {InvestorProfileComponent} from "./investorProfile/investorProfile.component";
import {InvestorApplicationDetailComponent} from "./investorApplicationDetail/investorApplicationDetail.component";
import {ProjectsComponent} from "./projects/projects.component";
import {ProjectDetailsComponent} from "./projects/projectDetails/projectDetails.component";
import {ReportDetailsComponent} from "./projects/reportDetails/reportDetails.component";
import {SelfEvaluationComponent} from "./projects/selfEvaluation/self-evaluation.component";


const routes:Route[]=[{
    path:'',
    component:InvestorHeaderComponent,
    children:[
        {path:'',pathMatch:'full', redirectTo:'home'},
        {path:'home', component:InvestorHomeComponent},
        {path:'about-us', component:InvestorAboutUsComponent},
        {path:'inv-bids', component:InvestorbidsComponent},
        {path:'bid/:prom_id', component:InvestorBidDetailComponent},
        {path:'bid-reg/:prom_id/:promUnit_id', component:InvestorBidRegistrationComponent},
        {path:'inv-profile', component:InvestorProfileComponent},
        {path:'inv-app/:prom_id/:promUnit_id/:investor_id', component:InvestorApplicationDetailComponent},
        {path:'projects', component:ProjectsComponent},
        {path:'projectDetails/:promoUnitId/:investorId', component:ProjectDetailsComponent},
        {path:'selfEvaluation/:promoUnitId/:investorId', component:SelfEvaluationComponent}
    ]
}];

@NgModule({
    imports:[RouterModule.forChild(routes)],
    exports:[RouterModule]
})
export class InvestorRoutingModule {
    
}