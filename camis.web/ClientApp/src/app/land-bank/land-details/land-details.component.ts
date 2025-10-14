import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {LandDataService} from "../../_services/land-data.service";
import {SearchResultDetailComponent} from "../../_shared/land-bank/search-result-detail/search-result-detail.component";
import {CamisMapModule} from "../../_shared/camismap/camismap.module";

@Component({
  selector: 'app-land-details',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, SearchResultDetailComponent, CamisMapModule],
  templateUrl:'./land-details.component.html',
  styleUrls:['./land-details.component.scss']
})

export class LandDetailsComponent{

  constructor(private landService:LandDataService) {
  }
}
