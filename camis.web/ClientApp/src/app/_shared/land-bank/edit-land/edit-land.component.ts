import { Component, OnInit, ViewChild } from '@angular/core';
import { Validators, FormBuilder, FormGroup, AbstractControl, FormArray, FormControl } from '@angular/forms';
import { map } from 'rxjs/operators';
import swal from 'sweetalert2';
import { Router, ActivatedRoute } from '@angular/router';

import { CamisMapModule } from '../../../_shared/camismap/camismap.module';
import { CamisMapComponent } from '../../../_shared/camismap/camismap.component';

import { LandDataService } from '../../../_services/land-data.service';
import { DialogService } from '../../../_shared/dialog/dialog.service';
import { LandModel, Accessablity } from '../../../_shared/land-bank/land.model';

@Component({
  selector: 'app-edit-land',
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
    uploadDocument: [] = []
  };

  selectedMoisture = {
    'value': 'off',
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
    public landDataService: LandDataService, public formBuilder: FormBuilder) { }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.wfid = params.wfid;
    });

    this.landDataService.GetWorkFlowLand(this.wfid).subscribe(data => {
      this.landData = data;
      console.log(this.landData);
      this.editLandFGroup.controls.uPINs.setValue(this.landData.upins);

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
            this.editLandFGroup.controls.accessablity.setValue(selectedCheckbox);

          }

      });
      this.landDataService.getMoistureSource().subscribe(res => {
        this.moistureSrc = res;

        let temp = null;
        for (const x of this.moistureSrc) {

          if (x.id === this.landData['moistureSource']) {
              temp = x['id'];
          }

        }
        this.editLandFGroup.controls.moistureSource.setValue(temp);

        if (temp === 2) {
          this.selectedMoisture = { value: 'on', index: 1};

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
        this.irrigationValues.controls.waterSourceParameter.setValue(this.landData_waterTestParams);

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

        this.irrigationValues.controls.surfaceWater.setValue(this.landData_surfaceWater);
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
          this.irrigationValues.controls.groundWater.setValue(this.landData_groundWater);

        }

      });

      if (this.landData['isAgriculturalZone'] === 'Yes') {
        this.isAgriculture = 'Yes';
      } else if (this.landData['isAgriculturalZone'] === 'No') {
        this.isAgriculture = 'No';
      }
      this.editLandFGroup.controls.isAgriculturalZone.setValue(this.isAgriculture);

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
        this.editLandFGroup.controls.agroEchologyZone.setValue(this.landData_agroEchologies);
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
          this.editLandFGroup.controls.investmentType.setValue(selectedCheckbox);

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
        this.editLandFGroup.controls.topography.setValue(this.landData_topographies);
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

          this.editLandFGroup.controls.soilTests.setValue(this.landData_soilTests);

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
          this.editLandFGroup.controls.existLandUse.setValue(this.landData_existLands);
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
        for (let c = 0; c < this.json.length; c++ ) {

          if (this.climate[c] !== undefined) {
            this.editLandFGroup.controls.precipitation['controls'][c].setValue(this.climate[c]['precipitation']);
            this.editLandFGroup.controls.temp_low['controls'][c].setValue(this.climate[c]['temp_low']);
            this.editLandFGroup.controls.temp_high['controls'][c].setValue(this.climate[c]['temp_high']);
            this.editLandFGroup.controls.temp_avg['controls'][c].setValue(this.climate[c]['temp_avg']);
          } else if (this.climate[c] === undefined) {
            this.editLandFGroup.controls.precipitation['controls'][c].setValue('');
            this.editLandFGroup.controls.temp_low['controls'][c].setValue('');
            this.editLandFGroup.controls.temp_high['controls'][c].setValue('');
            this.editLandFGroup.controls.temp_avg['controls'][c].setValue('');
          }
        }


      });

      this.description.setValue(this.landData.description);
    });

    this.editLandFGroup = this.formBuilder.group({
      uPINs: this.formBuilder.array([]),
      accessablity: this.formBuilder.array([]),
      agroEchologyZone: this.formBuilder.array([]),
      investmentType: this.formBuilder.array([]),

      soilTests: this.formBuilder.array([]),

      precipitation: this.formBuilder.array([]),
      temp_low: this.formBuilder.array([]),
      temp_high: this.formBuilder.array([]),
      temp_avg: this.formBuilder.array([]),
      isAgriculturalZone: this.formBuilder.control([]),
      topography: this.formBuilder.array([]),
      existLandUse: this.formBuilder.array([]),
      moistureSource: this.formBuilder.control([]),
      irrigationValues: this.formBuilder.group({
        waterSourceParameter: this.formBuilder.array([]),
        groundWater: this.formBuilder.array([]),
        surfaceWater: this.formBuilder.array([])
      }),
      'description' : ['', Validators.compose([Validators.required])],

    });
    this.irrigationValues = this.editLandFGroup.get('irrigationValues');

    this.description = this.editLandFGroup.controls['description'];

    // triggers the upin default field
    this.addNewRow();

  }

  selectedMoistureSource(event, i) {

    this.selectedMoisture = { value: event.target.value, index: i };

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

  get agroEchology () {
    return this.editLandFGroup.get('agroEchologyZone') as FormArray;
  }
  addAgroEchologies() {
    this.agroEchology.push(this.formBuilder.control(''));
  }

  get waterTests () {
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
    // window.location.hash = `step-${this.formWizardStep}`;
  }
  nextStep() {
    this.formWizardStep += 1;
    const scrollElement = document.getElementById('panel-content');
    scrollElement.scrollIntoView();
    // window.location.hash = `step-${this.formWizardStep}`;
  }

  editLand() {
    swal({allowOutsideClick: false});
    swal.disableButtons();
    swal.showLoading();

// getting what the form group return values from allthe fields into one array
    let editedLand;
    editedLand = this.editLandFGroup.value;

// for upins,accessablity and soilTests: mapping as the api needs and filtering all null values
    editedLand.uPINs = editedLand.uPINs.map((upins) => upins + '');
    editedLand.accessablity = editedLand.accessablity.map((access, i) => access === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.agroEchologyZone = editedLand.agroEchologyZone.map((agro, i) => ({ agroType: i + 1, result: agro })).filter(res => res.result != '').filter(res => res.result != null);

    // tslint:disable-next-line:max-line-length
    editedLand.investmentType = editedLand.investmentType.map((investType, i) => investType === true ? i + 1 : null).filter(a => a !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.topography = editedLand.topography.map((topographyValue, i) => ({ topographyType: i + 1, result: topographyValue })).filter(res => res.result !== '').filter(res => res.result !== null);
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.waterSourceParameter = editedLand.irrigationValues.waterSourceParameter.map((waterSource, i) => ({ waterSourceType: i + 1, result: waterSource })).filter(res => res.result != null).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.surfaceWater = editedLand.irrigationValues.surfaceWater.map((surfaceWater, i) => ({ surfaceWaterType: i + 1, result: surfaceWater })).filter(res => res.result != '');
    // tslint:disable-next-line:max-line-length
    editedLand.irrigationValues.groundWater = editedLand.irrigationValues.groundWater.map((gWater, i) => gWater === true ? i + 1 : null).filter(a => a !== null);

    editedLand.uploadDocument = this.editedLand.uploadDocument;
    // tslint:disable-next-line:max-line-length

    // tslint:disable-next-line:max-line-length
    editedLand.soilTests = editedLand.soilTests.map((soilValue, i) => ({testType : i + 1, result : '' + soilValue})).filter(res => res.result != '').filter(res => res.result != 'null' );

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


    console.log('edited land:');
    console.log(editedLand);

    this.landDataService.RequestLandEdit(editedLand, this.wfid).subscribe
    (() => {
      // success
      swal.close();

      swal({
        position: 'center',
        type: 'success',
        title: 'Your work has been edited successfully!',
        showConfirmButton: false,
        timer: 1500
      }).then(
        () => {
          this.router.navigate(['/land-clerk/pending-task']);
        });
    },
      (err) => {
        swal.close();
        // error alert
        this.dialog.error(err);
      }
    );
  }

  refreshButton() {
    this.ngOnInit();
  }
}
