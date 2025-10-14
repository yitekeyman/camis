import {CommonModule} from "@angular/common";
import {Component, OnInit} from "@angular/core";
import {SearchLandComponent} from "../../_shared/land-bank/search-land/search-land.component";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";
import {ReactiveFormsModule} from "@angular/forms";
import {SearchResultDetailComponent} from "../../_shared/land-bank/search-result-detail/search-result-detail.component";

@Component({
  selector: 'app-search-land-bank',
  imports: [CommonModule, SearchLandComponent, ReactiveFormsModule],
  templateUrl:'./search-land-bank.component.html',
  styleUrls:['./search-land-bank.component.scss']
})

export class SearchLandBankComponent implements OnInit {
  constructor() {
  }

  ngOnInit() {
  }

}
