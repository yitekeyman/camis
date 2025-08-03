import { element } from 'protractor';
import { LandModel } from './../../../_shared/land-bank/land.model';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators, FormBuilder, FormGroup, AbstractControl, FormArray, FormControl } from '@angular/forms';
import { map } from 'rxjs/operators';
import swal from 'sweetalert2';

import { Router } from '@angular/router';

import { CamisMapModule } from '../../../_shared/camismap/camismap.module';

import { CamisMapComponent } from '../../../_shared/camismap/camismap.component';

import { LandDataService } from '../../../_services/land-data.service';
import { DialogService } from '../../../_shared/dialog/dialog.service';


@Component({
  selector: 'app-new-land-form',
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
  existingLand  = [];
  groundWater = [];
  surfaceWater = [];

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

  selectedMoisture = {};
  selectedAgroZone = {};

  @ViewChild('camis_map') map: CamisMapComponent;
  constructor(public router: Router, private dialog: DialogService, public landService: LandDataService,
    public formBuilder: FormBuilder) { }

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

    this.newLandFGroup = this.formBuilder.group({
      uPINs: this.formBuilder.array([]),
      accessablity: this.formBuilder.array([]),
      agroEchologyZone: this.formBuilder.array([]),

      topography: this.formBuilder.array([]),
      investmentType: this.formBuilder.array([]),
      moistureSource: this.formBuilder.control(null),

      irrigationValues: this.formBuilder.group({
        waterSourceParameter: this.formBuilder.array([]),
        groundWater: this.formBuilder.array([]),
        surfaceWater: this.formBuilder.array([])
      }),

      soilTests: this.formBuilder.array([]),
      // soilType: this.formBuilder.control(''),
      // textureClass: this.formBuilder.control(''),
      precipitation: this.formBuilder.array([]),
      temp_low: this.formBuilder.array([]),
      temp_high: this.formBuilder.array([]),
      temp_avg: this.formBuilder.array([]),
      isAgriculturalZone: this.formBuilder.control(null),
      existLandUse: this.formBuilder.array([]),
      'description' : [''],

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

  get topography() {
    return this.newLandFGroup.get('topography') as FormArray;
  }
  addTopography() {
    this.topography.push(this.formBuilder.control(''));
  }

  get waterSourceParams() {
    return this.irrigationValues.get('waterSourceParameter') as FormArray;
  }
  addWaterParams() {
    this.waterSourceParams.push(this.formBuilder.control(''));
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
    this.SoilTests.push(this.formBuilder.control(''));
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
    this.selectedMoisture = { value: event.target.value, index: i };
  }

  selectedAgroEchoZone(event, i) {
    this.selectedAgroZone = { value: event.target.value, index: i };
  }

  openSection(id: number) {
    this.formWizardStep = id;
  }

  previousStep() {
    this.formWizardStep -= 1;
    const scrollElement = document.getElementById('panel-content');
    scrollElement.scrollIntoView();
    // window.location.hash = `step-${this.formWizardStep}`;
  }
  nextStep() {
    this.formWizardStep += 1;
    const scrollElement = document.getElementById('panel-content');
    scrollElement.scrollIntoView();
    // window.location.hash = `step-${this.formWizardStep}`;
  }

  saveNewLand() {

    swal({allowOutsideClick: false});
    swal.disableButtons();
    swal.showLoading();

// getting what the form group return values from allthe fields into one array
    let land;
    land = this.newLandFGroup.value;

// for upins,accessablity and soilTests: mapping as the api needs and filtering all null values
    land.uPINs = land.uPINs.map((upins) => upins + '');
    land.accessablity = land.accessablity.map((access, i) => access === true ? i + 1 : null).filter(a => a !== null);
    land.agroEchologyZone = land.agroEchologyZone.map((agro, i) => ({ agroType: i + 1, result: agro })).filter(res => res.result != '' );

    land.investmentType = land.investmentType.map((investType, i) => investType === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.waterSourceParameter = land.irrigationValues.waterSourceParameter.map((waterSource, i) => ({ waterSourceType: i + 1, result: waterSource})).filter(res => res.result != '' );
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.surfaceWater = land.irrigationValues.surfaceWater.map((surfaceWater, i) => ({ surfaceWaterType: i + 1, result: surfaceWater })).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    land.irrigationValues.groundWater = land.irrigationValues.groundWater.map((gWater, i) => gWater === true ? i + 1 : null).filter(a => a !== null);
    // delete land.irrigationValues.groundWater;
    // land.irrigationValues.groundWater = 1;


    // land.soilTests.push(land.soilType);
    // land.soilTests.push(land.textureClass);
    land.soilTests = land.soilTests.map((soilValue, i) => ({testType : i + 1, result : '' + soilValue})).filter(res => res.result != '' );

    // delete land.soilType;
    // delete land.textureClass;
    // tslint:disable-next-line:max-line-length

    land.existLandUse = land.existLandUse.map((existLand, i) => existLand === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    land.topography = land.topography.map((topographyValue, i) => ({ topographyType : i + 1, result : topographyValue})).filter(res => res.result !== '' );

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
        swal.close();

        swal({
          position: 'center',
          type: 'success',
          title: 'Your work has been saved',
          showConfirmButton: false,
          timer: 1500
        }).then(
          () => {
            this.router.navigate(['/land-clerk/land-dashboard']);
          });
      },
        (err) => {
          swal.close();
          this.dialog.error(err);
        }
      );

      console.log(land);
  }

  refreshButton() {
    this.ngOnInit();
  }


}
