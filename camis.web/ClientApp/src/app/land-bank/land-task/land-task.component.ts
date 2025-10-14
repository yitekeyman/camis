import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {LandDetailsTableComponent} from "./land-details-table/land-details-table.component";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";

@Component({
  selector:'app-land-task',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, LandDetailsTableComponent],
  templateUrl:'./land-task.component.html',
  styleUrls:['./land-task.component.scss']
})

export class LandTaskComponent{
  constructor() { }
}
