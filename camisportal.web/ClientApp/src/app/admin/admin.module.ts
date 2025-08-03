import {NgModule} from "@angular/core";

import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MainBodyModule} from "../shared/main-body/main-body.module";
import {AdminPostBidComponent} from "./postBid/adminPostBid.component";
import {AdminRoutingModule} from "./admin.routing.module";
import {AdminHeaderComponent} from "./adminHeader/adminHeader.component";
import {AdminManageBidsComponent} from "./ManageBid/adminManageBids.component";
import {AdminDashboardComponent} from "./adminDashboard/adminDashboard.component";
import {UsersManagementComponent} from "./users/usersManagement.component";
import {AdminEditUserComponent} from "./adminEditUser/adminEditUser.component";
import {AdminViewPromotionComponent} from "./adminViewPromotion/adminViewPromotion.component";
import {AdminEvaluationComponent} from "./evaluate/adminEvaluation.component";
import {AddressModule} from "../shared/address/address.module";
import {PdfViewerModule} from "ng2-pdf-viewer";
import {EvaluationDetailsComponent} from "./evaluationDetails/evaluationDetails.component";
import {ResetPasswordComponent} from "./resetPassword/resetPassword.component";
import {PromotionDetailsModule} from "../shared/promotionDetail/promotion_details.module";
import {InvestorProfileDetailModule} from "../shared/investorProfileDetail/investorProfileDetail.module";
import {InvestorApplicationModule} from "../shared/investorApplicationDetail/investorApplication.module";
import {RegionsSettingComponent} from "./regionsSetting/regionsSetting.component";


@NgModule({
    declarations:[
       AdminPostBidComponent,
        AdminHeaderComponent,
        AdminManageBidsComponent,
        AdminDashboardComponent,
        UsersManagementComponent,
        AdminEditUserComponent,
        AdminViewPromotionComponent,
        AdminEvaluationComponent,
        EvaluationDetailsComponent,
        ResetPasswordComponent,
        RegionsSettingComponent
    ],
    imports:[
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        AdminRoutingModule,
        MainBodyModule,
        AddressModule,
        PdfViewerModule,
        PromotionDetailsModule,
        InvestorProfileDetailModule,
        InvestorApplicationModule,
    ],
    providers:[]
})

export class AdminModule {
    
}