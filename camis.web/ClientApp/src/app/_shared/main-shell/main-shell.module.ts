import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import {MainShellComponent} from './main-shell/main-shell.component';

@NgModule({
    declarations: [
        MainShellComponent
    ],
    imports: [
        CommonModule,
        RouterModule
    ],
    exports: [
        MainShellComponent
    ],
    providers: []
})
export class MainShellModule { }
