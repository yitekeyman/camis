import {Component, OnInit} from "@angular/core";
import {CardComponent} from "../../theme/shared/components/card/card.component";
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-configuration-admin',
  imports:[CommonModule, CardComponent,],
  templateUrl:'./configurationAdmin.component.html',
  styleUrls:['./configurationAdmin.component.scss']
})

export class ConfigurationAdminComponent implements OnInit {
  constructor() {
  }

  ngOnInit() {}
}
