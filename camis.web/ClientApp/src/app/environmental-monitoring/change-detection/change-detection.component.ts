import {Component, OnInit, ViewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {ChangeDetectionRequest, EnvironmentalAnalysisResult, EnvironmentalChangeEventDto} from "../../_model/environmental-monitoring.model";
import {EnvironmentalMonitoringService} from "../../_services/environmental-monitoring.service";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";

@Component({
  selector:'app-change-detection',
  templateUrl:'./change-detection.component.html',
  imports:[CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent],
  styleUrls:['change-detection.component.scss']
})
export class ChangeDetectionComponent implements OnInit{
  public changeDetectionForm: FormGroup;
  public result?: EnvironmentalAnalysisResult;
  public isLoading = false;
  public error = '';
  public selectedChange?: EnvironmentalChangeEventDto;
  public mapInitialized = false;

  @ViewChild('camis_map', { static: false }) mapComponent!: CamisMapComponent;

  constructor(
    private fb: FormBuilder,
    private monitoringService: EnvironmentalMonitoringService,
    public keyCase: ObjectKeyCasingService
  ) {
    this.changeDetectionForm = this.fb.group({
      parcelUpid: [''],
      region: [''],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      threshold: [0.15, [Validators.min(0), Validators.max(1)]]
    });
  }

  ngOnInit(): void {
    // Set default dates (last 30 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);

    this.changeDetectionForm.patchValue({
      startDate: startDate.toISOString().split('T')[0],
      endDate: endDate.toISOString().split('T')[0]
    });

    // Mark map as initialized after a short delay to ensure it's rendered
    setTimeout(() => {
      this.mapInitialized = true;
      console.log('Map component should be initialized now');
    }, 1000);
  }

  onDetectChanges(): void {
    if (this.changeDetectionForm.valid) {
      this.isLoading = true;
      this.error = '';
      this.result = undefined;
      this.selectedChange = undefined;

      // Clear any previous changes from the map
      if (this.mapComponent) {
        this.mapComponent.clearEnvironmentalChanges();
      }

      const formValue = this.changeDetectionForm.value;
      const request: ChangeDetectionRequest = {
        parcelUpid: formValue.parcelUpid || undefined,
        region: formValue.region || undefined,
        startDate: new Date(formValue.startDate),
        endDate: new Date(formValue.endDate),
        changeThreshold: formValue.threshold,
        changeTypes:[]
      };

      //console.log('Starting change detection with request:', request);
      this.keyCase.PascalCase(request);

      this.monitoringService.DetectChanges(request).subscribe({
        next: (result) => {
          this.keyCase.camelCase(result);
          this.result = result;
          this.isLoading = false;

          // Display changes on the map if any were found
          if (this.mapComponent && result.changes && result.changes.length > 0) {
            console.log(`Displaying ${result.changes.length} changes on map`);
            this.mapComponent.displayEnvironmentalChanges(result.changes);
          } else {
            console.log('No changes found to display on map');
          }
        },
        error: (err) => {
          this.keyCase.camelCase(err);
          this.error = 'Error detecting changes. Please try again.';
          this.isLoading = false;
          console.error('Change detection error:', err);
        }
      });
    }
  }

  onMapChangeClick(change: EnvironmentalChangeEventDto): void {
    this.selectedChange = change;
    console.log('Change selected from map:', change);
  }

  clearChanges(): void {
    if (this.mapComponent) {
      this.mapComponent.clearEnvironmentalChanges();
    }
    this.result = undefined;
    this.selectedChange = undefined;
    this.error = '';
  }

  toggleChangeLayer(): void {
    if (this.mapComponent) {
      this.mapComponent.toggleChangeLayerVisibility();
    }
  }

  getChangeTypeColor(eventType: string): string {
    switch (eventType) {
      case 'VEGETATION_LOSS': return '#FF0000';
      case 'VEGETATION_GROWTH': return '#00FF00';
      case 'WATER_INCREASE': return '#0000FF';
      case 'WATER_DECREASE': return '#FFA500';
      case 'URBANIZATION': return '#808080';
      default: return '#666666';
    }
  }

  getSeverityClass(severity: string): string {
    return severity.toLowerCase();
  }

  // Helper method for template
  objectKeys(obj: any): string[] {
    return Object.keys(obj || {});
  }
}
