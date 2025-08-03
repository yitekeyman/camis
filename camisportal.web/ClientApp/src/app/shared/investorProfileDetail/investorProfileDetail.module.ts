import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {RouterModule} from "@angular/router";
import {InvestorProfileDetailComponent} from "./investorProfileDetail.component";
import {AddressModule} from "../address/address.module";

@NgModule({
    declarations: [
        InvestorProfileDetailComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        AddressModule
    ],
    exports: [
       InvestorProfileDetailComponent
    ],
    providers: []
})
export class InvestorProfileDetailModule {
    
}