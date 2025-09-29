import {AfterViewInit, Component,} from "@angular/core";
import {MainShellComponent} from "../theme/layout/mainShell/mainShell.component";
import {DefaultRoutingModule} from "./default-routing.module";
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-default',
  imports: [CommonModule,MainShellComponent],
  templateUrl:'./default.component.html'
})

export class DefaultComponent implements AfterViewInit {
  ngAfterViewInit() {}
}
