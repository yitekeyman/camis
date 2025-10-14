import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {MainShellComponent} from "../theme/layout/mainShell/mainShell.component";


@Component({
  selector:'app-farm-management',
  imports:[CommonModule, MainShellComponent],
  templateUrl:'./farm-management.component.html'
})

export class FarmManagementComponent{

}
