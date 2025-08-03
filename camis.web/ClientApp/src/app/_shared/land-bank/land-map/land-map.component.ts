import { Component, OnInit } from '@angular/core';
import { ReportModule } from '../../../_shared/report/report.module';
import { ReportComponent } from '../../../_shared/report/report.component';

@Component({
  selector: 'app-land-map',
  templateUrl: './land-map.component.html',
  styleUrls: ['./land-map.component.css']
})
export class LandMapComponent implements OnInit {

  rep: String = 'LandBankSummary';

  constructor() { }

  ngOnInit() {
  }

}
