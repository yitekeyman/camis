import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import { CamisMapComponent} from './camismap.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

@NgModule({
    declarations: [

    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
      CamisMapComponent
    ],
    exports: [
      CamisMapComponent
    ],
    providers: []
})
export class CamisMapModule { }
