import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {DefaultRoutingModule} from "../default/default.routing.module";
import {MainBodyModule} from "../shared/main-body/main-body.module";
import {InvestorHeaderComponent} from "./investor-header/investor-header.component";
import {InvestorRoutingModule} from "./investor.routing.module";
import {DefaultHomeComponent} from "../default/home/defaultHome.component";
import {AboutUsComponent} from "../default/aboutus/about-us.component";
import {InvestorbidsComponent} from "./investorbids/investorbids.component";
import {InvestorBidDetailComponent} from "./investorBidDetail/investorBidDetail.component";
import {InvestorBidRegistrationComponent} from "./investorBidRegistration/investorBidRegistration.component";
import {InvestorHomeComponent} from "./home/investorHome.component";
import {InvestorAboutUsComponent} from "./aboutus/investorAbout-us.component";
import {InvestorProfileComponent} from "./investorProfile/investorProfile.component";
import {AddressModule} from "../shared/address/address.module";
import {InvestorApplicationDetailComponent} from "./investorApplicationDetail/investorApplicationDetail.component";
import {PromotionDetailsModule} from "../shared/promotionDetail/promotion_details.module";
import {InvestorProfileDetailModule} from "../shared/investorProfileDetail/investorProfileDetail.module";
import {ProjectsComponent} from "./projects/projects.component";
import {ProjectDetailsComponent} from "./projects/projectDetails/projectDetails.component";
import {ProjectModule} from "../shared/project/project.module";
import {ReportDetailsComponent} from "./projects/reportDetails/reportDetails.component";
import {SelfEvaluationComponent} from "./projects/selfEvaluation/self-evaluation.component";
import {InvestorApplicationModule} from "../shared/investorApplicationDetail/investorApplication.module";

@NgModule({
    declarations:[
        InvestorHeaderComponent,
        InvestorHomeComponent,
        InvestorAboutUsComponent,
        InvestorbidsComponent,
        InvestorBidRegistrationComponent,
        InvestorBidDetailComponent,
        InvestorProfileComponent,
        InvestorApplicationDetailComponent,
        ProjectsComponent,
        ProjectDetailsComponent,
        ReportDetailsComponent,
        SelfEvaluationComponent
    ],
    imports:[
        CommonModule,
        FormsModule,
        ReactiveFormsModule, 
        InvestorRoutingModule,
        MainBodyModule,
        AddressModule,
        PromotionDetailsModule,
        InvestorProfileDetailModule,
        ProjectModule,
        InvestorApplicationModule
        
    ],
    providers:[]
})
export class InvestorModule {
    
}