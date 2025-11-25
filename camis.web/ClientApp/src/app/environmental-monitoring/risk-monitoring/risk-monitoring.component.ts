import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {EnvironmentalMonitoringService} from "../../_services/environmental-monitoring.service";

@Component({
  selector: "app-risk-monitoring",
  imports:[CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: "./risk-monitoring.component.html",
  styleUrls: ["./risk-monitoring.component.scss"]
})

export class RiskMonitoringComponent implements OnInit{
  // Flood risk properties
  floodGeometryWkt = '';
  floodRiskResult: boolean | null = null;
  isLoadingFlood = false;

  // Drought risk properties
  droughtRegion = '';
  droughtResult: boolean | null = null;
  isLoadingDrought = false;

  // Significant changes properties
  significantChanges: any[] = [];
  changesStartDate: string;
  changesEndDate: string;
  changesRegion = '';
  changesLoaded = false;
  error = '';
  constructor(private monitoringService:EnvironmentalMonitoringService) {
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 7);

    this.changesStartDate = startDate.toISOString().split('T')[0];
    this.changesEndDate = endDate.toISOString().split('T')[0];
  }
  ngOnInit(): void {
    this.loadSignificantChanges();
  }

  checkFloodRisk(): void {
    if (!this.floodGeometryWkt) return;

    this.isLoadingFlood = true;
    this.error = '';

    this.monitoringService.CheckFloodRisk(this.floodGeometryWkt).subscribe({
      next: (result) => {
        this.floodRiskResult = result;
        this.isLoadingFlood = false;
      },
      error: (err) => {
        this.error = 'Error assessing flood risk. Please check the geometry format.';
        this.isLoadingFlood = false;
        console.error('Flood risk error:', err);
      }
    });
  }

  checkDroughtCondition(): void {
    if (!this.droughtRegion) return;

    this.isLoadingDrought = true;
    this.error = '';

    this.monitoringService.CheckDroughtCondition(this.droughtRegion).subscribe({
      next: (result) => {
        this.droughtResult = result;
        this.isLoadingDrought = false;
      },
      error: (err) => {
        this.error = 'Error assessing drought conditions. Please try again.';
        this.isLoadingDrought = false;
        console.error('Drought condition error:', err);
      }
    });
  }

  loadSignificantChanges(): void {
    const startDate = new Date(this.changesStartDate);
    const endDate = new Date(this.changesEndDate);
    const region = this.changesRegion || undefined;

    this.monitoringService.GetSignificantChanges(startDate, endDate, region).subscribe({
      next: (changes) => {
        this.significantChanges = changes;
        this.changesLoaded = true;
      },
      error: (err) => {
        this.error = 'Error loading significant changes.';
        console.error('Significant changes error:', err);
      }
    });
  }

}
