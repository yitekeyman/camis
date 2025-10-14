import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {NewLandFormComponent} from "./new-land-form/new-land-form.component";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";

@Component({
  selector:'app-register-land-bank',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, NewLandFormComponent],
  templateUrl:'./register-land-bank.component.html',
  styleUrls:['./register-land-bank.component.scss']
})

export class RegisterLandBankComponent{}
