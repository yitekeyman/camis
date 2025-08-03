import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {RouterModule} from "@angular/router";
import {AddressModule} from "../address/address.module";
import {InvestorApplicationComponent} from "./investorApplication.component";

@NgModule({
    declarations: [
        InvestorApplicationComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        AddressModule
    ],
    exports: [
        InvestorApplicationComponent
    ],
    providers: []
})
export class InvestorApplicationModule {
    
}