import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {AddressSelectorComponent} from "./address-selector/address-selector.component";
import {AddressOutputComponent} from "./addressOutput/address-output.component";
import {FormsModule} from "@angular/forms";

@NgModule({
    declarations: [
        AddressSelectorComponent,
        AddressOutputComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule
    ],
    exports: [
        AddressSelectorComponent,
        AddressOutputComponent
    ],
    providers: []
})
export class AddressModule {
    
}