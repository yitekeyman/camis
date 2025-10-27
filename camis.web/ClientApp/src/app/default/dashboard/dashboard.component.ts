// Angular Import
import {Component, OnInit, ViewChild} from '@angular/core';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { BajajChartComponent } from 'src/app/theme/shared/components/apexchart/bajaj-chart/bajaj-chart.component';
import { BarChartComponent } from 'src/app/theme/shared/components/apexchart/bar-chart/bar-chart.component';
import { ChartDataMonthComponent } from 'src/app/theme/shared/components/apexchart/chart-data-month/chart-data-month.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";
import {AdminServices} from "../../_services/admin.Services";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import dialog from "../../_shared/dialog";

@Component({
  selector: 'app-dashboard',
  imports: [SharedModule, ReactiveFormsModule, CamisMapComponent, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit{
  dashboardData:any=null;
  constructor(private adminService: AdminServices, private keyCase: ObjectKeyCasingService) {
  }

  @ViewChild('camis_map') map: CamisMapComponent;
  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    dialog.loading();
    this.adminService.GetDashboard().subscribe(data => {
      this.keyCase.camelCase(data);
      this.dashboardData=data;
      dialog.close();
    },dialog.error)
  }
}
