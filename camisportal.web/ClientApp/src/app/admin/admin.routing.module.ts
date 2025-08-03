import {Route, RouterModule} from "@angular/router";
import {DefaultheaderComponent} from "../default/defaultHeader/defaultheader.component";
import {DefaultHomeComponent} from "../default/home/defaultHome.component";
import {BidsComponent} from "../default/bids/bids.component";
import {LoginComponent} from "../default/login/login.component";
import {RegisterComponent} from "../default/register/register.component";
import {AboutUsComponent} from "../default/aboutus/about-us.component";
import {NgModule} from "@angular/core";
import {AdminPostBidComponent} from "./postBid/adminPostBid.component";
import {AdminHeaderComponent} from "./adminHeader/adminHeader.component";
import {AdminManageBidsComponent} from "./ManageBid/adminManageBids.component";
import {AdminDashboardComponent} from "./adminDashboard/adminDashboard.component";
import {UsersManagementComponent} from "./users/usersManagement.component";
import {AdminEditUserComponent} from "./adminEditUser/adminEditUser.component";
import {AdminEvaluationComponent} from "./evaluate/adminEvaluation.component";
import {EvaluationDetailsComponent} from "./evaluationDetails/evaluationDetails.component";
import {RegionsSettingComponent} from "./regionsSetting/regionsSetting.component";


const routes:Route[]=[{
    path:'',
    component:AdminHeaderComponent,
    children:[
        {path:'',pathMatch:'full', redirectTo:'dashboard'},
        {path:'postBid', component:AdminPostBidComponent},
        {path:'manageBid', component:AdminManageBidsComponent},
        {path:'dashboard', component:AdminDashboardComponent},
        {path:'manageUsers', component:UsersManagementComponent},
        {path:'editUser', component:AdminEditUserComponent},
        {path:'editUser/:username', component:AdminEditUserComponent},
        {path:'editPro/:promUnit_id/:prom_id', component:AdminPostBidComponent},
        {path:'addPro/:prom_id/:status', component:AdminPostBidComponent},
        {path:'evaluate/:prom_id/:promUnit_id/:investor_id', component:AdminEvaluationComponent},
        {path:'eva-details/:prom_id/:promUnit_id/:investor_id', component:EvaluationDetailsComponent},
        {path:'invEva-details/:prom_id/:promUnit_id', component:EvaluationDetailsComponent},
        {path:'regionsSetting', component:RegionsSettingComponent}
    ]
}];
@NgModule({
    imports:[RouterModule.forChild(routes)],
    exports:[RouterModule]
})
export class AdminRoutingModule {
    
}