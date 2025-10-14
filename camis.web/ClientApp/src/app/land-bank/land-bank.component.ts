import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {MainShellComponent} from "../theme/layout/mainShell/mainShell.component";

@Component({
  selector:"app-land-bank",
  imports:[CommonModule, MainShellComponent],
  templateUrl:'./land-bank.component.html',
})

export class LandBankComponent{
  constructor() {
  }
}
