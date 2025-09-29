import {Component, OnInit, AfterContentInit} from '@angular/core';

import {CommonModule} from "@angular/common";
import {MainShellComponent} from "../theme/layout/mainShell/mainShell.component";

declare var $: any;
@Component({
    selector: 'app-admin',
  imports: [CommonModule, MainShellComponent,],
  templateUrl: './admin.component.html'
})
export class AdminComponent {


}
