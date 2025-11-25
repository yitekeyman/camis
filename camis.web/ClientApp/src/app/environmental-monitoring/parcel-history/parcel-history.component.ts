import {Component, OnInit, ViewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {EnvironmentalMonitoringService} from "../../_services/environmental-monitoring.service";
import {
  ChartConfiguration,
  ChartData,
  ChartType,
  ChartDataset,
  ChartOptions
} from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import {ParcelEnvironmentalMonitoring} from "../../_model/environmental-monitoring.model";

@Component({
  selector: "app-parcel-history",
  imports: [CommonModule, ReactiveFormsModule, FormsModule,BaseChartDirective],
  templateUrl: "./parcel-history.component.html",
  styleUrls: ["./parcel-history.component.scss"]
})

export class ParcelHistoryComponent implements OnInit{
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;
  parcelUpid = '';
  monthsBack = 12;
  historyData: ParcelEnvironmentalMonitoring[] = [];
  isLoading = false;
  error = '';

  chartType: ChartType = 'line';
  chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        title: {
          display: true,
          text: 'Date'
        },
        ticks: {
          maxTicksLimit: 8
        }
      },
      y: {
        title: {
          display: true,
          text: 'Index Value'
        },
        min: -1,
        max: 1
      }
    },
    plugins: {
      legend: {
        display: true
      },
      tooltip: {
        mode: 'index',
        intersect: false
      }
    },
    interaction: {
      mode: 'nearest',
      axis: 'x',
      intersect: false
    }
  };

  moistureChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        title: {
          display: true,
          text: 'Date'
        },
        ticks: {
          maxTicksLimit: 8
        }
      },
      y: {
        title: {
          display: true,
          text: 'Moisture Level'
        },
        min: 0,
        max: 1
      }
    },
    plugins: {
      legend: {
        display: true
      }
    }
  };

  ndviChartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };

  ndwiChartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };

  ndbiChartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };

  moistureChartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };
  constructor(private monitoringService:EnvironmentalMonitoringService){}
  ngOnInit() {}

  loadParcelHistory(): void {
    if (!this.parcelUpid) return;

    this.isLoading = true;
    this.error = '';
    this.historyData = [];

    this.monitoringService.GetParcelHistory(this.parcelUpid, this.monthsBack).subscribe({
      next: (data) => {
        this.historyData = data.sort((a, b) =>
          new Date(a.monitoringDate).getTime() - new Date(b.monitoringDate).getTime()
        );
        this.updateCharts();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Error loading parcel history. Please check the UPID and try again.';
        this.isLoading = false;
        console.error('Parcel history error:', err);
      }
    });
  }

  private updateCharts(): void {
    const labels = this.historyData.map(d =>
      new Date(d.monitoringDate).toLocaleDateString()
    );

    // NDVI Chart
    this.ndviChartData = {
      labels,
      datasets: [
        {
          label: 'NDVI',
          data: this.historyData.map(d => d.ndvi || 0),
          borderColor: '#28a745',
          backgroundColor: 'rgba(40, 167, 69, 0.1)',
          tension: 0.4,
          fill: true,
          pointBackgroundColor: '#28a745',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 4
        }
      ]
    };

    // NDWI Chart
    this.ndwiChartData = {
      labels,
      datasets: [
        {
          label: 'NDWI',
          data: this.historyData.map(d => d.ndwi || 0),
          borderColor: '#17a2b8',
          backgroundColor: 'rgba(23, 162, 184, 0.1)',
          tension: 0.4,
          fill: true,
          pointBackgroundColor: '#17a2b8',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 4
        }
      ]
    };

    // NDBI Chart
    this.ndbiChartData = {
      labels,
      datasets: [
        {
          label: 'NDBI',
          data: this.historyData.map(d => d.ndbi || 0),
          borderColor: '#dc3545',
          backgroundColor: 'rgba(220, 53, 69, 0.1)',
          tension: 0.4,
          fill: true,
          pointBackgroundColor: '#dc3545',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 4
        }
      ]
    };

    // Soil Moisture Chart
    this.moistureChartData = {
      labels,
      datasets: [
        {
          label: 'Soil Moisture',
          data: this.historyData.map(d => d.soilMoisture || 0),
          borderColor: '#8B4513',
          backgroundColor: 'rgba(139, 69, 19, 0.1)',
          tension: 0.4,
          fill: true,
          pointBackgroundColor: '#8B4513',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 4
        }
      ]
    };

    // Update charts if they exist
    if (this.chart) {
      this.chart.update();
    }
  }

  getIndexClass(value?: number): string {
    if (value === undefined || value === null) return '';
    if (value > 0.5) return 'index-high';
    if (value > 0.2) return 'index-medium';
    return 'index-low';
  }

  getMoistureClass(value?: number): string {
    if (value === undefined || value === null) return '';
    if (value > 0.6) return 'moisture-high';
    if (value > 0.3) return 'moisture-medium';
    return 'moisture-low';
  }
}
