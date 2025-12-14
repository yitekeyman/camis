import {Component, OnInit, ViewChild} from '@angular/core';
import {
  Validators,
  FormBuilder,
  FormGroup,
  AbstractControl,
  FormArray,
  FormControl,
  ReactiveFormsModule, FormsModule, ValidatorFn
} from '@angular/forms';
import {Router, ActivatedRoute} from '@angular/router';
import {CamisMapComponent} from '../../../_shared/camismap/camismap.component';

import {LandDataService} from '../../../_services/land-data.service';
import {DialogService} from '../../../_shared/dialog/dialog.service';
import {LandModel, Accessablity} from '../../../_shared/land-bank/land.model';
import {CommonModule} from "@angular/common";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";
import {
  LandbankDocumentSelectorComponent
} from "../../../_shared/land-bank/landbank-document-selector/landbank-document-selector.component";

@Component({
  selector: 'app-edit-land',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent, LandbankDocumentSelectorComponent],
  templateUrl: './edit-land.component.html',
  styleUrls: ['./edit-land.component.css']
})
export class EditLandComponent implements OnInit {

  wfid: string;
  landData: LandModel;

  accessablity: Accessablity[];
  soilTests = [];
  agroEchologies = [];
  json = [];
  investment = [];
  topographies = [];
  landData_soilTests = [];
  existingLands = [];
  landData_agroEchologies = [];
  landData_investment = [];
  landData_topographies = [];
  landData_existingLands = [];
  landData_moistureSrc = [];
  moistureSrc = [];
  groundWater = [];
  landData_groundWater = [];
  waterTestPrams = [];
  surfaceWater = [];
  landData_surfaceWater = [];
  landData_waterTestParams = [];
  climate = [];
  existLands = [];
  landData_existLands = [];
  editedLand = {
    uploadDocument: []
  };
  soilTypes = [];
  soilTextureClass=[];

  selectedMoisture = {
    'value': false,
    index: null
  };
  isAgriculture: string;
  formWizardStep = 1;

  editLandFGroup: FormGroup;
  irrigationValues: FormGroup | any;
  upinName: AbstractControl;
  description: AbstractControl;

  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(public router: Router, private activeRoute: ActivatedRoute, private dialog: DialogService,
              public landDataService: LandDataService, public formBuilder: FormBuilder, private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.wfid = params['wfid'];
    });

    this.landDataService.GetWorkFlowLand(this.wfid).subscribe(data => {
      this.keyCase.camelCase(data);
      this.landData = data;
      console.log(this.landData);
      this.editLandFGroup.controls['uPINs'].setValue(this.landData.upins);

      for (const upload of this.landData['uploadDocument']) {
        this.editedLand.uploadDocument.push(upload);
      }

      this.landDataService.getAccessiblity().subscribe(res => {
        this.accessablity = res;

        const selectedCheckbox = [];
        for (const x of this.accessablity) {
          for (const acc of this.landData.accessablity) {
            if (x.id === acc) {
              x.checked = true;

              break;
            } else {
              x.checked = false;
            }
          }
          this.addAccess();
          if (x.checked !== undefined) {
            selectedCheckbox.push(x.checked);

          }
        }

        if (selectedCheckbox.length !== 0) {
          this.editLandFGroup.controls['accessablity'].setValue(selectedCheckbox);

        }

      });
      this.landDataService.getMoistureSource().subscribe(res => {
        this.moistureSrc = res;

        const selectedCheckbox = [];
        for (const x of this.moistureSrc) {
          for (const acc of this.landData['moistureSource']) {
            if (x.id === acc) {
              x.checked = true;
              if (x.id === 2) {
                this.selectedMoisture = {value: true, index: 1};
              }
              break;
            } else {
              x.checked = false;
            }
          }
          this.addMoistureSource();
          if (x.checked !== undefined) {
            selectedCheckbox.push(x.checked);
          }
        }

        if (selectedCheckbox.length !== 0) {
          this.editLandFGroup.controls['moistureSource'].setValue(selectedCheckbox);

        }

      });

      this.landDataService.getWaterTestParameters().subscribe(res => {
        this.waterTestPrams = res;

        for (const x of this.waterTestPrams) {
          let temp = '';
          for (const water of this.landData['irrigationValues']['waterSourceParameter']) {
            if (x.id === water['waterSourceType']) {
              temp = water['result'];
              break;
            }
          }

          this.landData_waterTestParams.push(temp);
          this.addWaterTests();
        }
        this.irrigationValues.controls['waterSourceParameter'].setValue(this.landData_waterTestParams);

      });

      this.landDataService.getSurfaceWater().subscribe(res => {
        this.surfaceWater = res;
        for (const x of this.surfaceWater) {
          // console.log(x);
          let temp = '';
          for (const sWater of this.landData['irrigationValues']['surfaceWater']) {
            // console.log(sWater);
            if (x.id === sWater['surfaceWaterType']) {
              temp = sWater['result'];
              break;
            }
          }

          this.landData_surfaceWater.push(temp);
          this.addSurfaceWater();
        }

        this.irrigationValues.controls['surfaceWater'].setValue(this.landData_surfaceWater);
      });

      this.landDataService.getGroundWater().subscribe(res => {
        this.groundWater = res;

        // let temp = null;
        for (const x of this.groundWater) {

          for (const grdWater of this.landData['irrigationValues']['groundWater']) {
            if (x.id === grdWater) {
              x.checked = true;
              break;
            } else {
              x.checked = false;
            }
          }

          if (x.checked !== undefined) {
            this.landData_groundWater.push(x.checked);

          }

          this.addGroundWater();

        }
        if (this.landData_groundWater.length !== 0) {
          this.irrigationValues.controls['groundWater'].setValue(this.landData_groundWater);

        }

      });

      if (this.landData['isAgriculturalZone'] === 'Yes') {
        this.isAgriculture = 'Yes';
      } else if (this.landData['isAgriculturalZone'] === 'No') {
        this.isAgriculture = 'No';
      }
      this.editLandFGroup.controls['isAgriculturalZone'].setValue(this.isAgriculture);

      this.landDataService.getAgroEchologicalZone().subscribe(res => {
        this.agroEchologies = res;

        for (const agroEchology of this.agroEchologies) {
          let temp = '';
          for (const agroData of this.landData['agroEchologyZone']) {
            if (agroData['agroType'] == agroEchology['id']) {
              temp = agroData['result'];
              break;
            }
          }

          this.landData_agroEchologies.push(temp);
          this.addAgroEchologies();
        }
        this.editLandFGroup.controls['agroEchologyZone'].setValue(this.landData_agroEchologies);
      });

      this.landDataService.getInvestmentType().subscribe(res => {
        this.investment = res;

        const selectedCheckbox = [];
        for (const x of this.investment) {
          for (const inv of this.landData['investmentType']) {
            if (x.id === inv) {
              x.checked = true;

              break;
            } else {
              x.checked = false;
            }
          }
          this.addInvestmentType();
          if (x.checked !== undefined) {
            selectedCheckbox.push(x.checked);

          }

        }
        if (selectedCheckbox.length !== 0) {
          this.editLandFGroup.controls['investmentType'].setValue(selectedCheckbox);

        }

      });

      // this.landDataService.getExistingLandUse().subscribe(res => {
      //   this.existingLands = res;

      // });

      this.landDataService.getTopography().subscribe(res => {
        this.topographies = res;

        for (const topog of this.topographies) {
          let temp = '';
          for (const topogData of this.landData['topography']) {
            if (topogData['topographyType'] === topog['id']) {
              temp = topogData['result'];
              break;
            }
          }

          this.landData_topographies.push(temp);
          this.addTopographies();
        }
        this.editLandFGroup.controls['topography'].setValue(this.landData_topographies);
      });

      this.landDataService.getsoilTestTypeUrl().subscribe(res => {
        this.soilTests = res;

        for (const s of this.soilTests) {
          let temp = '';
          for (const st of this.landData['soilTests']) {
            if (s['id'] === st['testType']) {
              temp = st['result'];
            }
          }

          this.landData_soilTests.push(temp);
          this.addSoilTest();
        }

        this.editLandFGroup.controls['soilTests'].setValue(this.landData_soilTests);

      });
      this.landDataService.getExistingLandUse().subscribe(res => {
        this.existLands = res;

        for (const x of this.existLands) {

          for (const exist of this.landData['existLandUse']) {
            if (x.id == exist) {
              x.checked = true;
              break;
            } else {
              x.checked = false;
            }
          }
          if (x.checked !== undefined) {
            this.landData_existLands.push(x.checked);
          }
          this.addExistingLand();

        }
        if (this.landData_existLands.length !== 0) {
          this.editLandFGroup.controls['existLandUse'].setValue(this.landData_existLands);
        }
      });

      this.landDataService.getjsonUrl().subscribe(res => {
        this.json = res;

        for (const j of this.json) {
          let temp = {};
          for (const ld of this.landData['climate']) {
            if (j['id'] == ld['month']) {
              temp = ld;
              break;
            }
          }
          this.climate.push(temp);


          // this.addClimate();

          this.addPrecipitation();
          this.addTempLow();
          this.addTempHigh();
          this.addTempAvg();

        }
        for (let c = 0; c < this.json.length; c++) {

          if (this.climate[c] !== undefined) {
            this.editLandFGroup.controls['precipitation']['controls'][c].setValue(this.climate[c]['precipitation']);
            this.editLandFGroup.controls['temp_low']['controls'][c].setValue(this.climate[c]['temp_low']);
            this.editLandFGroup.controls['temp_high']['controls'][c].setValue(this.climate[c]['temp_high']);
            this.editLandFGroup.controls['temp_avg']['controls'][c].setValue(this.climate[c]['temp_avg']);
          } else if (this.climate[c] === undefined) {
            this.editLandFGroup.controls['precipitation']['controls'][c].setValue('');
            this.editLandFGroup.controls['temp_low']['controls'][c].setValue('');
            this.editLandFGroup.controls['temp_high']['controls'][c].setValue('');
            this.editLandFGroup.controls['temp_avg']['controls'][c].setValue('');
          }
        }


      });

      this.description.setValue(this.landData.description);
    });
    this.landDataService.getSoilType().subscribe(data => {
      this.soilTypes = data;
    });
    this.landDataService.getSoilTextureClass().subscribe(data => {
      this.soilTextureClass = data;
    });
    this.editLandFGroup = this.formBuilder.group({
      uPINs: this.formBuilder.array([], [Validators.required, this.atLeastOneRequired()]),
      accessablity: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      agroEchologyZone: this.formBuilder.array([]),
      investmentType: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),

      soilTests: this.formBuilder.array([], [this.atLeastOneRequired()]),

      precipitation: this.formBuilder.array([]),
      temp_low: this.formBuilder.array([]),
      temp_high: this.formBuilder.array([]),
      temp_avg: this.formBuilder.array([]),
      isAgriculturalZone: this.formBuilder.control([], [Validators.required]),
      topography: this.formBuilder.array([], [this.atLeastOneRequired()]),
      existLandUse: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      moistureSource: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
      irrigationValues: this.formBuilder.group({
        waterSourceParameter: this.formBuilder.array([], [Validators.required, this.atLeastOneRequired()]),
        groundWater: this.formBuilder.array([], [this.atLeastOneCheckboxSelected()]),
        surfaceWater: this.formBuilder.array([])
      }),
      'description': ['', Validators.compose([Validators.required])],

    });
    this.irrigationValues = this.editLandFGroup.get('irrigationValues');

    this.description = this.editLandFGroup.controls['description'];

    // triggers the upin default field
    this.addNewRow();

  }

  selectedMoistureSource(event, i) {
    if (i === 1)
      this.selectedMoisture = {value: event.target.checked, index: i};
  }

// to get the map from nrlais
  onUpinChanged(event: any) {
    this.map.setNrlaisParcel(event.target.value);
  }

  get initItemRows() {
    return this.editLandFGroup.get('uPINs') as FormArray;
  }

  addNewRow() {
    this.initItemRows.push(this.formBuilder.control('', Validators.required));
  }

  // deleteRow(index: number) {
  //   // control refers to the formarray
  //   const control = <FormArray>this.editLandFGroup.controls['uPINs'];
  //   // remove the chosen row
  //   this.initItemRows.removeAt(index);
  // }
  get moistureSource() {
    return this.editLandFGroup.get('moistureSource') as FormArray;
  }

  addMoistureSource() {
    this.moistureSource.push(this.formBuilder.control(''));
  }

  get accessablities() {
    return this.editLandFGroup.get('accessablity') as FormArray;
  }

  addAccess() {
    this.accessablities.push(this.formBuilder.control(''));
  }

  get topo() {
    return this.editLandFGroup.get('topography') as FormArray;
  }

  addTopographies() {
    this.topo.push(this.formBuilder.control(''));
  }

  get SoilTests() {
    return this.editLandFGroup.get('soilTests') as FormArray;
  }

  addSoilTest() {
    this.SoilTests.push(this.formBuilder.control(''));
  }

  get agroEchology() {
    return this.editLandFGroup.get('agroEchologyZone') as FormArray;
  }

  addAgroEchologies() {
    this.agroEchology.push(this.formBuilder.control(''));
  }

  get waterTests() {
    return this.irrigationValues.get('waterSourceParameter') as FormArray;
  }

  addWaterTests() {
    this.waterTests.push(this.formBuilder.control(''));
  }

  get surfWater() {
    return this.irrigationValues.get('surfaceWater') as FormArray;
  }

  addSurfaceWater() {
    this.surfWater.push(this.formBuilder.control(''));
  }

  get investmentT() {
    return this.editLandFGroup.get('investmentType') as FormArray;
  }

  addInvestmentType() {
    this.investmentT.push(this.formBuilder.control(''));
  }

  get existLand() {
    return this.editLandFGroup.get('existLandUse') as FormArray;
  }

  addExistingLand() {
    this.existLand.push(this.formBuilder.control(null));
  }

  get precipitations() {
    return this.editLandFGroup.get('precipitation') as FormArray;
  }

  addPrecipitation() {
    this.precipitations.push(this.formBuilder.control(''));
  }

  get groundWaters() {
    return this.irrigationValues.get('groundWater') as FormArray;
  }

  addGroundWater() {
    this.groundWaters.push(this.formBuilder.control(''));
  }

  get tempLow() {
    return this.editLandFGroup.get('temp_low') as FormArray;
  }

  addTempLow() {
    this.tempLow.push(this.formBuilder.control(''));
  }

  get tempHigh() {
    return this.editLandFGroup.get('temp_high') as FormArray;
  }

  addTempHigh() {
    this.tempHigh.push(this.formBuilder.control(''));
  }

  get tempAvg() {
    return this.editLandFGroup.get('temp_avg') as FormArray;
  }

  addTempAvg() {
    this.tempAvg.push(this.formBuilder.control(''));
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

      await dialog.error('Please fill all required fields correctly before proceeding.');
    }
  }

  async editLand(): Promise<void> {
    if (!this.validateCurrentStep() || !this.editLandFGroup.valid) {
      await dialog.error('Please fill all required fields correctly before submitting.');
    }
    dialog.loading();

    let editedLand;
    editedLand = this.editLandFGroup.value;

// for upins,accessablity and soilTests: mapping as the api needs and filtering all null values
    editedLand.uPINs = editedLand.uPINs.map((upins) => upins + '');
    editedLand.accessablity = editedLand.accessablity.map((access, i) => access === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.agroEchologyZone = editedLand.agroEchologyZone.map((agro, i) => ({
      agroType: i + 1,
      result: agro
    })).filter(res => res.result != '').filter(res => res.result != null);

    // tslint:disable-next-line:max-line-length
    editedLand.investmentType = editedLand.investmentType.map((investType, i) => investType === true ? i + 1 : null).filter(a => a !== null);
    editedLand.moistureSource = editedLand.moistureSource.map((moi, i) => moi === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.topography = editedLand.topography.map((topographyValue, i) => ({
      topographyType: i + 1,
      result: topographyValue
    })).filter(res => res.result !== '').filter(res => res.result !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.waterSourceParameter = editedLand.irrigationValues.waterSourceParameter.map((waterSource, i) => ({
      waterSourceType: i + 1,
      result: waterSource
    })).filter(res => res.result != null).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.surfaceWater = editedLand.irrigationValues.surfaceWater.map((surfaceWater, i) => ({
      surfaceWaterType: i + 1,
      result: surfaceWater
    })).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.groundWater = editedLand.irrigationValues.groundWater.map((gWater, i) => gWater === true ? i + 1 : null).filter(a => a !== null);

    editedLand.uploadDocument = this.editedLand.uploadDocument;
    // tslint:disable-next-line:max-line-length

    // tslint:disable-next-line:max-line-length
    editedLand.soilTests = editedLand.soilTests.map((soilValue, i) => ({
      testType: i + 1,
      result: '' + soilValue
    })).filter(res => res.result != '').filter(res => res.result != 'null');

    editedLand.existLandUse = editedLand.existLandUse.map((existLand, i) => existLand === true ? i + 1 : null).filter(a => a !== null);

    if (editedLand.moistureSource === 1) {
      delete editedLand.irrigationValues.waterSourceParameter;
      delete editedLand.irrigationValues.surfaceWater;
      delete editedLand.irrigationValues.groundWater;
      editedLand.irrigationValues.waterSourceParameter = [];
      editedLand.irrigationValues.surfWater = [];
      editedLand.irrigationValues.groundWater = null;
    }
// making values from the preciptation,temp-low,temp-high and temp-avg into one climate array removing empty rows
    editedLand.climate = [];
    for (let i = 0; i < 12; i++) {

      if (editedLand.precipitation[i] !== undefined || editedLand.temp_low[i] !== undefined || editedLand.temp_high[i] !== undefined ||
        editedLand.temp_avg[i] !== undefined) {

        editedLand.climate.push({
          month: i + 1,
          precipitation: editedLand.precipitation[i],
          temp_low: editedLand.temp_low[i],
          temp_high: editedLand.temp_high[i],
          temp_avg: editedLand.temp_avg[i]
        });
      }
    }

// filtering unfilled attributes from the final returned array
    for (let x = 0; x < editedLand.climate.length; x++) {
      if (editedLand.climate[x].precipitation === '' || editedLand.climate[x].temp_low === undefined) {
        delete editedLand.climate[x].precipitation;
      }
      if (editedLand.climate[x].temp_low === '' || editedLand.climate[x].temp_low === undefined) {
        delete editedLand.climate[x].temp_low;
      }
      if (editedLand.climate[x].temp_high === '' || editedLand.climate[x].temp_high === undefined) {
        delete editedLand.climate[x].temp_high;
      }
      if (editedLand.climate[x].temp_avg === '' || editedLand.climate[x].temp_avg === undefined) {
        delete editedLand.climate[x].temp_avg;
      }

    }

// removing values outside of climate array that are included in the array to avoid redundency
    delete editedLand.precipitation;
    delete editedLand.temp_high;
    delete editedLand.temp_low;
    delete editedLand.temp_avg;

    this.landDataService.RequestLandEdit(editedLand, this.wfid).subscribe
    (() => {
        dialog.success('Your work has been edited successfully!').then(
          () => {
            this.router.navigate(['/default/pending-task']);
          });
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
    const formArray = this.editLandFGroup.get(formArrayName) as FormArray;
    return formArray.errors && formArray.errors[errorType] && formArray.touched;
  }

  // Helper method to check if control has error
  hasControlError(controlName: string, errorType: string): boolean {
    const control = this.editLandFGroup.get(controlName);
    return control.errors && control.errors[errorType] && control.touched;
  }

  validateCurrentStep(): boolean {
    // Mark all controls as touched to trigger validation display
    this.markFormGroupTouched(this.editLandFGroup);

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
    const upinsValid = this.editLandFGroup.get('uPINs').valid;
    const investmentValid = this.editLandFGroup.get('investmentType').valid;
    const accessValid = this.editLandFGroup.get('accessablity').valid;

    return upinsValid && investmentValid && accessValid;
  }

  private validateStep2(): boolean {
    const moistureValid = this.editLandFGroup.get('moistureSource').valid;
    const topographyValid = this.editLandFGroup.get('topography').valid;
    const soilTestsValid = this.editLandFGroup.get('soilTests').valid;

    // Only validate irrigation if moisture source is irrigation
    if (this.selectedMoisture?.value  && this.selectedMoisture?.index === 1) {
      const groundWaterValid = this.irrigationValues.get('groundWater').valid;
      const waterSourceParameterValid = this.irrigationValues.get('waterSourceParameter').valid;
      return moistureValid && groundWaterValid && topographyValid && soilTestsValid && waterSourceParameterValid;
    }

    return moistureValid && topographyValid && soilTestsValid;
  }

  private validateStep3(): boolean {
    //const precipitationValid = this.editLandFGroup.get('precipitation').valid;
    return true;// precipitationValid;
  }

  private validateStep4(): boolean {
    const existLandValid = this.editLandFGroup.get('existLandUse').valid;
    const agriculturalZoneValid = this.editLandFGroup.get('isAgriculturalZone').valid;
    const descriptionValid = this.editLandFGroup.get('description').valid;

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
    const formArray = this.editLandFGroup.get(formArrayName) as FormArray;

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
    return this.editLandFGroup.get(formArrayName) as FormArray;
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
