import {Component, OnInit, ViewChild} from '@angular/core';
import {
  Validators,
  FormBuilder,
  FormGroup,
  AbstractControl,
  FormArray,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  ValidatorFn
} from '@angular/forms';
import {map} from 'rxjs';

import {Router} from '@angular/router';
import {LandDataService} from '../../../_services/land-data.service';
import {DialogService} from '../../../_shared/dialog/dialog.service';
import {CommonModule} from "@angular/common";
import {CamisMapComponent} from "../../../_shared/camismap/camismap.component";
import {
  LandbankDocumentSelectorComponent
} from "../../../_shared/land-bank/landbank-document-selector/landbank-document-selector.component";
import dialog from "../../../_shared/dialog";


@Component({
  selector: 'app-new-land-form',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, LandbankDocumentSelectorComponent, CamisMapComponent],
  templateUrl: './new-land-form.component.html',
  styleUrls: ['./new-land-form.component.css']
})
export class NewLandFormComponent implements OnInit {

  landData = [];
  accessablity = [];
  soilTests = [];
  json = [];
  agroEchologicalZone = [];
  topographies = [];
  investment = [];
  moistureSrc = [];
  waterParams = [];
  waterType = [];
  existingLand = [];
  groundWater = [];
  surfaceWater = [];
  soilTypes = [];
  soilTextureClass=[];

  newLandFGroup: FormGroup | any;
  irrigationValues: FormGroup | any;
  agroEchologyZoneSubSec: FormGroup | any;
  upinName: AbstractControl;
  description: AbstractControl;
  formWizardStep = 1;
  add_file: '';
  file_data?: string;
  file_mime?: string;
  land = {
    uploadDocument: [] = []
  };

  selectedMoisture = null;
  selectedAgroZone = null;

  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(public router: Router, private dialog: DialogService, public landService: LandDataService,
              public formBuilder: FormBuilder) {
  }

  ngOnInit() {

    this.landService.getLandData()
      .subscribe(data => {
        this.landData = data;
      });

    this.landService.getAccessiblity()
      .subscribe(data => {
        this.accessablity = data;

        for (const x of this.accessablity) {
          this.addAccess();
        }
      });

    this.landService.getAgroEchologicalZone()
      .subscribe(data => {
        this.agroEchologicalZone = data;

        for (const x of this.agroEchologicalZone) {
          this.addAgroEchologyZone();
        }
      });

    this.landService.getMoistureSource()
      .subscribe(data => {
        this.moistureSrc = data;
        for (const x of this.moistureSrc) {
          this.addMoistureSource();
        }
      });

    this.landService.getWaterTestParameters()
      .subscribe(data => {
        this.waterParams = data;

        for (const x of this.waterParams) {
          this.addWaterParams();
        }
      });

    this.landService.getWaterSourceType()
      .subscribe(data => {
        this.waterType = data;
      });

    this.landService.getGroundWater()
      .subscribe(data => {
        this.groundWater = data;

        for (const x of this.groundWater) {
          this.addGroundWaterParams();
        }
      });

    this.landService.getSurfaceWater()
      .subscribe(data => {
        this.surfaceWater = data;

        for (const x of this.surfaceWater) {
          this.addSurfaceWaterParams();
        }
      });

    this.landService.getExistingLandUse()
      .subscribe(data => {
        this.existingLand = data;

        for (const x of this.existingLand) {
          this.addExistLand();
        }
      });

    this.landService.getTopography()
      .subscribe(data => {
        this.topographies = data;

        for (const x of this.topographies) {
          this.addTopography();
        }
      });

    this.landService.getInvestmentType()
      .subscribe(data => {
        this.investment = data;

        for (const x of this.investment) {
          this.addInvestmentType();
        }
      });

    this.landService.getsoilTestTypeUrl()
      .subscribe(data => {
        this.soilTests = data;

        for (const s of this.soilTests) {
          this.addSoilTest();
        }
      });

    this.landService.getjsonUrl()
      .subscribe(data => {
        this.json = data;

        for (const j of this.json) {
          this.addPrecipitation();
          this.addTempLow();
          this.addTempHigh();
          this.addTempAvg();

        }


      });
    this.landService.getSoilType().subscribe(data => {
      this.soilTypes = data;
    });
    this.landService.getSoilTextureClass().subscribe(data => {
      this.soilTextureClass = data;
    });

    this.newLandFGroup = this.formBuilder.group({
      uPINs: this.formBuilder.array([], [Validators.required, this.atLeastOneRequired()]),
      accessablity: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      agroEchologyZone: this.formBuilder.array([]),

      topography: this.formBuilder.array([], [this.atLeastOneRequired()]),
      investmentType: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      moistureSource: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),

      irrigationValues: this.formBuilder.group({
        waterSourceParameter: this.formBuilder.array([], [Validators.required, this.atLeastOneRequired()]),
        groundWater: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
        surfaceWater: this.formBuilder.array([])
      }),

      soilTests: this.formBuilder.array([], [this.atLeastOneRequired()]),
      // soilType: this.formBuilder.control(''),
      // textureClass: this.formBuilder.control(''),
      precipitation: this.formBuilder.array([], ),
      temp_low: this.formBuilder.array([]),
      temp_high: this.formBuilder.array([]),
      temp_avg: this.formBuilder.array([]),
      isAgriculturalZone: this.formBuilder.control(null, [Validators.required]),
      existLandUse: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      'description': ['', [Validators.required]],

    });
    this.irrigationValues = this.newLandFGroup.get('irrigationValues');
    // this.agroEchologyZoneSubSec = this.newLandFGroup.get('agroEchologyZoneSubSec');
    this.description = this.newLandFGroup.controls['description'];

    // triggers the upin default field
    this.addNewRow();
  }

  get initItemRows() {
    return this.newLandFGroup.get('uPINs') as FormArray;
  }

  addNewRow() {
    this.initItemRows.push(this.formBuilder.control('', Validators.required));
  }

  get accessablities() {
    return this.newLandFGroup.get('accessablity') as FormArray;
  }

  addAccess() {
    this.accessablities.push(this.formBuilder.control(''));
  }

  get agroEchology() {
    return this.newLandFGroup.get('agroEchologyZone') as FormArray;
  }

  addAgroEchologyZone() {
    this.agroEchology.push(this.formBuilder.control(''));
  }

  get moistureSource() {
    return this.newLandFGroup.get('moistureSource') as FormArray;
  }

  addMoistureSource() {
    this.moistureSource.push(this.formBuilder.control(''));
  }

  get topography() {
    return this.newLandFGroup.get('topography') as FormArray;
  }

  addTopography() {
    this.topography.push(this.formBuilder.control('', [Validators.required]));
  }

  get waterSourceParams() {
    return this.irrigationValues.get('waterSourceParameter') as FormArray;
  }

  addWaterParams() {
    this.waterSourceParams.push(this.formBuilder.control('', Validators.required));
  }

  get surfaceWaterParams() {
    return this.irrigationValues.get('surfaceWater') as FormArray;
  }

  addSurfaceWaterParams() {
    this.surfaceWaterParams.push(this.formBuilder.control(''));
  }

  get groundWaterParams() {
    return this.irrigationValues.get('groundWater') as FormArray;
  }

  addGroundWaterParams() {
    this.groundWaterParams.push(this.formBuilder.control(''));
  }

  get investmentType() {
    return this.newLandFGroup.get('investmentType') as FormArray;
  }

  addInvestmentType() {
    this.investmentType.push(this.formBuilder.control(''));
  }

  get SoilTests() {
    return this.newLandFGroup.get('soilTests') as FormArray;
  }

  addSoilTest() {
    this.SoilTests.push(this.formBuilder.control('', Validators.required));
  }

  get existLand() {
    return this.newLandFGroup.get('existLandUse') as FormArray;
  }

  addExistLand() {
    this.existLand.push(this.formBuilder.control(''));
  }

  get precipitations() {
    return this.newLandFGroup.get('precipitation') as FormArray;
  }

  addPrecipitation() {
    this.precipitations.push(this.formBuilder.control(''));
  }

  get tempLow() {
    return this.newLandFGroup.get('temp_low') as FormArray;
  }

  addTempLow() {
    this.tempLow.push(this.formBuilder.control(''));
  }

  get tempHigh() {
    return this.newLandFGroup.get('temp_high') as FormArray;
  }

  addTempHigh() {
    this.tempHigh.push(this.formBuilder.control(''));
  }

  get tempAvg() {
    return this.newLandFGroup.get('temp_avg') as FormArray;
  }

  addTempAvg() {
    this.tempAvg.push(this.formBuilder.control(''));
  }

  onUpinChanged(event: any) {
    this.map.setNrlaisParcel(event.target.value);
  }

  selectedMoistureSource(event, i) {
    if (i == 1)
      this.selectedMoisture = {value: event.target.checked, index: i};
  }

  selectedAgroEchoZone(event, i) {
    this.selectedAgroZone = {value: event.target.value, index: i};
  }

  openSection(id: number) {
    this.formWizardStep = id;
  }

  previousStep() {
    this.formWizardStep -= 1;
    const scrollElement = document.getElementById('panel-content');
    scrollElement.scrollIntoView();

  }

  async nextStep(): Promise<void> {
    if (this.validateCurrentStep()) {
      this.formWizardStep += 1;
      const scrollElement = document.getElementById('panel-content');
      scrollElement.scrollIntoView();
    } else {
      // Show error message or handle validation failure
      await dialog.error('Please fill all required fields correctly before proceeding.');
    }
  }

  async saveNewLand(): Promise<void> {
    if (!this.validateCurrentStep() || !this.newLandFGroup.valid) {
      await dialog.error('Please fill all required fields correctly before submitting.');
      return ;
    }
    dialog.loading();

// getting what the form group return values from allthe fields into one array
    let land;
    land = this.newLandFGroup.value;

// for upins,accessablity and soilTests: mapping as the api needs and filtering all null values
    land.uPINs = land.uPINs.map((upins) => upins + '');
    land.accessablity = land.accessablity.map((access, i) => access === true ? i + 1 : null).filter(a => a !== null);
    land.agroEchologyZone = land.agroEchologyZone.map((agro, i) => ({
      agroType: i + 1,
      result: agro
    })).filter(res => res.result != '');

    land.investmentType = land.investmentType.map((investType, i) => investType === true ? i + 1 : null).filter(a => a !== null);
    land.moistureSource = land.moistureSource.map((moisture, i) => moisture === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.waterSourceParameter = land.irrigationValues.waterSourceParameter.map((waterSource, i) => ({
      waterSourceType: i + 1,
      result: waterSource
    })).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.surfaceWater = land.irrigationValues.surfaceWater.map((surfaceWater, i) => ({
      surfaceWaterType: i + 1,
      result: surfaceWater
    })).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.groundWater = land.irrigationValues.groundWater.map((gWater, i) => gWater === true ? i + 1 : null).filter(a => a !== null);
    // delete land.irrigationValues.groundWater;
    // land.irrigationValues.groundWater = 1;


    // land.soilTests.push(land.soilType);
    // land.soilTests.push(land.textureClass);
    land.soilTests = land.soilTests.map((soilValue, i) => ({
      testType: i + 1,
      result: '' + soilValue
    })).filter(res => res.result != '');

    // delete land.soilType;
    // delete land.textureClass;
    // tslint:disable-next-line:max-line-length

    land.existLandUse = land.existLandUse.map((existLand, i) => existLand === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    land.topography = land.topography.map((topographyValue, i) => ({
      topographyType: i + 1,
      result: topographyValue
    })).filter(res => res.result !== '');

    land.uploadDocument = this.land.uploadDocument;

// making values from the preciptation,temp-low,temp-high and temp-avg into one climate array removing empty rows
    land.climate = [];
    for (let i = 0; i < 12; i++) {

      if (land.precipitation[i] !== '' || land.temp_low[i] !== '' || land.temp_high[i] !== '' ||
        land.temp_avg[i] !== '') {

        land.climate.push({
          month: i + 1,
          precipitation: land.precipitation[i],
          temp_low: land.temp_low[i],
          temp_high: land.temp_high[i],
          temp_avg: land.temp_avg[i]
        });
      }
    }

// filtering unfilled attributes from the final returned array
    for (let x = 0; x < land.climate.length; x++) {
      if (land.climate[x].precipitation === '') {
        delete land.climate[x].precipitation;
      }
      if (land.climate[x].temp_low === '') {
        delete land.climate[x].temp_low;
      }
      if (land.climate[x].temp_high === '') {
        delete land.climate[x].temp_high;
      }
      if (land.climate[x].temp_avg === '') {
        delete land.climate[x].temp_avg;
      }
    }

// removing values outside of climate array that are included in the array to avoid redundency
    delete land.precipitation;
    delete land.temp_high;
    delete land.temp_low;
    delete land.temp_avg;

// calling the api to register the above returned land array
    this.landService.RequestLandRegistration(land).subscribe
    (() => {
        dialog.success('Your work has been saved').then(() => {
          this.router.navigate(['/land-bank/search-parcel']);
        })
      },
      (err) => {
        return dialog.error(err);
      }
    );
  }

  refreshButton() {
    this.ngOnInit();
  }

  atLeastOneCheckboxSelected(): ValidatorFn {
    return (formArray: FormArray): { [key: string]: boolean } | null => {
      const atLeastOneSelected = formArray.controls.some(control => control.value === true);
      return atLeastOneSelected ? null : {'atLeastOneRequired': true};
    };
  }

  atLeastOneRequired(): ValidatorFn {
    return (formArray: FormArray): { [key: string]: boolean } | null => {
      const hasValue = formArray.controls.some(control => control.value && control.value.toString().trim() !== '');
      return hasValue ? null : {'atLeastOneRequired': true};
    };
  }

  requiredTextValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } | null => {
      return control.value && control.value.toString().trim() !== '' ? null : {'required': true};
    };
  }

  hasFormArrayError(formArrayName: string, errorType: string): boolean {
    const formArray = this.newLandFGroup.get(formArrayName) as FormArray;
    return formArray.errors && formArray.errors[errorType] && formArray.touched;
  }

  // Helper method to check if control has error
  hasControlError(controlName: string, errorType: string): boolean {
    const control = this.newLandFGroup.get(controlName);
    return control.errors && control.errors[errorType] && control.touched;
  }

  validateCurrentStep(): boolean {
    // Mark all controls as touched to trigger validation display
    this.markFormGroupTouched(this.newLandFGroup);

    switch (this.formWizardStep) {
      case 1:
        return this.validateStep1();
      case 2:
        return this.validateStep2();
      case 3:
        return this.validateStep3();
      case 4:
        return this.validateStep4();
      default:
        return false;
    }
  }

  // Step-specific validation methods
  private validateStep1(): boolean {
    const upinsValid = this.newLandFGroup.get('uPINs').valid;
    const investmentValid = this.newLandFGroup.get('investmentType').valid;
    const accessValid = this.newLandFGroup.get('accessablity').valid;

    return upinsValid && investmentValid && accessValid;
  }

  private validateStep2(): boolean {
    const moistureValid = this.newLandFGroup.get('moistureSource').valid;
    const topographyValid = this.newLandFGroup.get('topography').valid;
    const soilTestsValid = this.newLandFGroup.get('soilTests').valid;

    // Only validate irrigation if moisture source is irrigation
    if (this.selectedMoisture?.value === 'on' && this.selectedMoisture?.index === 1) {
      const groundWaterValid = this.irrigationValues.get('groundWater').valid;
      const waterSourceParameterValid = this.irrigationValues.get('waterSourceParameter').valid;
      return moistureValid && groundWaterValid && topographyValid && soilTestsValid && waterSourceParameterValid;
    }

    return moistureValid && topographyValid && soilTestsValid;
  }

  private validateStep3(): boolean {
    //const precipitationValid = this.newLandFGroup.get('precipitation').valid;
    return true;//precipitationValid;
  }

  private validateStep4(): boolean {
    const existLandValid = this.newLandFGroup.get('existLandUse').valid;
    const agriculturalZoneValid = this.newLandFGroup.get('isAgriculturalZone').valid;
    const descriptionValid = this.newLandFGroup.get('description').valid;

    return existLandValid && agriculturalZoneValid && descriptionValid;
  }

  // Helper to mark all controls as touched
  private markFormGroupTouched(formGroup: FormGroup | FormArray) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);

      if (control instanceof FormGroup || control instanceof FormArray) {
        this.markFormGroupTouched(control);
      } else {
        control.markAsTouched();
      }
    });
  }

  validateClimateData(): ValidatorFn {
    return (formArray: FormArray): { [key: string]: boolean } | null => {
      // Use a safer approach - get the parent form group
      const formGroup = formArray.parent;
      if (!formGroup) {
        return null;
      }

      const precipitationArray = formArray;
      const tempLowArray = formGroup.get('temp_low') as FormArray;
      const tempHighArray = formGroup.get('temp_high') as FormArray;
      const tempAvgArray = formGroup.get('temp_avg') as FormArray;

      // Check if all arrays exist
      if (!tempLowArray || !tempHighArray || !tempAvgArray) {
        return null;
      }

      let hasAnyData = false;
      let hasIncompleteMonth = false;

      // Check each month
      for (let i = 0; i < precipitationArray.length; i++) {
        const precip = precipitationArray.at(i).value;
        const tempLow = tempLowArray.at(i)?.value;
        const tempHigh = tempHighArray.at(i)?.value;
        const tempAvg = tempAvgArray.at(i)?.value;

        // Check if this month has any data
        const monthHasData =
          (precip !== null && precip !== undefined && precip !== '') ||
          (tempLow !== null && tempLow !== undefined && tempLow !== '') ||
          (tempHigh !== null && tempHigh !== undefined && tempHigh !== '') ||
          (tempAvg !== null && tempAvg !== undefined && tempAvg !== '');

        if (monthHasData) {
          hasAnyData = true;

          // Check if all fields for this month are filled
          const allFieldsFilled =
            precip !== null && precip !== undefined && precip !== '' &&
            tempLow !== null && tempLow !== undefined && tempLow !== '' &&
            tempHigh !== null && tempHigh !== undefined && tempHigh !== '' &&
            tempAvg !== null && tempAvg !== undefined && tempAvg !== '';

          if (!allFieldsFilled) {
            hasIncompleteMonth = true;
            break; // No need to check further
          }
        }
      }

      // If any data is entered but some months are incomplete, return error
      if (hasAnyData && hasIncompleteMonth) {
        return {'incompleteClimateData': true};
      }

      return null;
    };
  }

  // Validator for numeric fields with range validation
  numberRangeValidator(min: number, max: number): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value && control.value !== 0) {
        return null; // Allow empty fields (use required validator separately)
      }

      const value = parseFloat(control.value);
      if (isNaN(value)) {
        return {'notANumber': true};
      }

      if (value < min || value > max) {
        return {'outOfRange': {min, max, actual: value}};
      }

      return null;
    };
  }

  getFormArrayErrorMessage(formArrayName: string): string {
    const formArray = this.newLandFGroup.get(formArrayName) as FormArray;

    if (!formArray.errors) return '';

    if (formArray.errors['atLeastOneRequired']) {
      return `At least one ${this.getFieldDisplayName(formArrayName)} is required`;
    }

    if (formArray.errors['incompleteClimateData']) {
      return 'If you enter climate data for any month, you must fill all fields for that month';
    }

    if (formArray.errors['outOfRange']) {
      const error = formArray.errors['outOfRange'];
      return `Value must be between ${error.min} and ${error.max}`;
    }

    return 'This field is required';
  }

  // Helper to get display names for error messages
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      'topography': 'topography field',
      'soilTests': 'soil test',
      'precipitation': 'precipitation field'
    };

    return displayNames[fieldName] || fieldName;
  }

  // Method to validate individual numeric inputs
  validateNumberInput(control: AbstractControl, min: number, max: number): boolean {
    if (!control.value && control.value !== 0) return true; // Allow empty

    const value = parseFloat(control.value);
    if (isNaN(value)) return false;

    return value >= min && value <= max;
  }

  checkMonthHasError(monthIndex: number): boolean {
    const precipControl = this.precipitations.at(monthIndex);
    const tempLowControl = this.tempLow.at(monthIndex);
    const tempHighControl = this.tempHigh.at(monthIndex);
    const tempAvgControl = this.tempAvg.at(monthIndex);

    // Check if any field has data but not all are filled
    const hasPartialData =
      (precipControl.value || tempLowControl.value || tempHighControl.value || tempAvgControl.value) &&
      (!precipControl.value || !tempLowControl.value || !tempHighControl.value || !tempAvgControl.value);

    return hasPartialData;
  }

  getFormArray(formArrayName: string): FormArray {
    return this.newLandFGroup.get(formArrayName) as FormArray;
  }

// Helper to check if form array control is invalid
  isFormControlInvalid(formArrayName: string, index: number): boolean {
    const formArray = this.getFormArray(formArrayName);
    if (!formArray || !formArray.controls[index]) {
      return false;
    }
    return formArray.controls[index].invalid && formArray.controls[index].touched;
  }

// Helper to check if a specific form array control has specific error
  hasFormControlError(formArrayName: string, index: number, errorType: string): boolean {
    const formArray = this.getFormArray(formArrayName);
    if (!formArray || !formArray.controls[index]) {
      return false;
    }
    const control = formArray.controls[index];
    return control.errors && control.errors[errorType] && control.touched;
  }

  // Method to validate a specific month's climate data
  validateClimateMonth(monthIndex: number) {
    // Trigger validation for the precipitation array (which has the climate validator)
    this.precipitations.updateValueAndValidity();

    // Mark all controls as touched to show errors
    this.precipitations.controls[monthIndex].markAsTouched();
    this.tempLow.controls[monthIndex].markAsTouched();
    this.tempHigh.controls[monthIndex].markAsTouched();
    this.tempAvg.controls[monthIndex].markAsTouched();
  }

  // Method to check if a specific month has incomplete climate data
  hasIncompleteClimateData(monthIndex: number): boolean {
    const precip = this.precipitations.at(monthIndex).value;
    const tempLow = this.tempLow.at(monthIndex).value;
    const tempHigh = this.tempHigh.at(monthIndex).value;
    const tempAvg = this.tempAvg.at(monthIndex).value;

    // Check if any field has data but not all are filled
    const hasPartialData =
      (precip !== null && precip !== undefined && precip !== '') ||
      (tempLow !== null && tempLow !== undefined && tempLow !== '') ||
      (tempHigh !== null && tempHigh !== undefined && tempHigh !== '') ||
      (tempAvg !== null && tempAvg !== undefined && tempAvg !== '');

    const allFilled =
      precip !== null && precip !== undefined && precip !== '' &&
      tempLow !== null && tempLow !== undefined && tempLow !== '' &&
      tempHigh !== null && tempHigh !== undefined && tempHigh !== '' &&
      tempAvg !== null && tempAvg !== undefined && tempAvg !== '';

    return hasPartialData && !allFilled;
  }
}
