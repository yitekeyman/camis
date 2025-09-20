import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LandDataService} from '../../../_services/land-data.service';
import { LandType, Month, SoilTestType, Accessablity, SearchResult, LandPreparationModel,
  Topography, AgroEchologicalZone, ExistingLandUse, InvestmentType, MoistureSource,
  GroundWater, SurfaceWater, WaterTestParameters} from '../../../_shared/land-bank/land.model';
import { DialogService } from '../../dialog/dialog.service';
import { FormBuilder, AbstractControl, FormGroup, Validators, ValidatorFn } from '@angular/forms';
import swal from 'sweetalert2';
import { CamisMapComponent } from '../../../_shared/camismap/camismap.component';
import dialog from "../../dialog";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-search-result-detail',
  templateUrl: './search-result-detail.component.html',
  styleUrls: ['./search-result-detail.component.css']
})
export class SearchResultDetailComponent implements OnInit {

  landID: string;
  searchedLandDetail: SearchResult[] | any = [];
  clerkRole = false;
  loginRole = '';

  // data from api
  monthesList: Month[] = [];
  soilTestTypesList: SoilTestType[] = [];
  accessiblitiesList: Accessablity[] = [];
  landTypeList: LandType[] = [];
  // data for view
  climates: any[] = [];
  soilTestTypes: any[] = [];
  accessiblities: any[] = [];
  topographyList: Topography[] = [];
  topography: any[] = [];
  agroEchologyList: AgroEchologicalZone[] = [];
  agroEchologyZones: any[] = [];
  existingLandList: ExistingLandUse[] = [];
  existingLandes: any[] = [];
  investmentTypeList: InvestmentType[] = [];
  investmentTypes: any[] = [];
  landTypes: String;
  isAgricultural: string;
  moistureSourceList: MoistureSource[] = [];
  grounWaterList: GroundWater[] = [];
  surfaceWaterList: SurfaceWater[] = [];
  groundWater: any;
  surfaceWater: any[] = [];
  waterSourceParamList: WaterTestParameters[] = [];
  waterSourceParams: any[] = [];
  moistureSources: any;

  prepareForm: FormGroup;
  splitNumberControl: AbstractControl;

  prepareModel: LandPreparationModel;
  @ViewChild('camis_map') map: CamisMapComponent;
  @ViewChild('prepareBtnClose') prepareBtnClose: ElementRef;

  constructor(private router: Router, public landService: LandDataService, private activeRoute: ActivatedRoute,
    private formBuilder: FormBuilder, private dialog: DialogService, private keyCase:ObjectKeyCasingService) {

    if (localStorage.getItem('role') === '4' ) {
      this.clerkRole = true;
      this.loginRole = 'land-clerk';
    }
    if (localStorage.getItem('role') === '5') {
      this.loginRole = 'land-supervisor';
    }

  }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.landID = params.landID;
      this.getLandDetail();
    });
    this.prepareForm = this.formBuilder.group({
      splitNumberControl : ['', Validators.compose([Validators.required, this.positiveNumberValidator()])],
    });

    this.splitNumberControl = this.prepareForm.controls['splitNumberControl'];

    this.prepareModel = {
      landID: '',
      n: null,
    };

  }

  getLandDetail() {
    dialog.loading();
    this.landService.GetLand(this.landID).subscribe(data => {
      this.keyCase.camelCase(data);
      this.searchedLandDetail = data;
      this.getDependecies();
      dialog.close();
      var g = data.parcels[data.upins[0]];
      if (g) {
        var parts = g.geometry.split(";");
        this.map.setWorkFlowGeomByWKT(parts[parts.length-1]);
      }
      console.log(this.searchedLandDetail);

    },e=>{
      dialog.close();
    });
  }

  getArea() {
    return Math.round(this.searchedLandDetail.area / 10) / 1000 + ' ha';
  }

  getDependecies() {
    this.landService.getAccessiblity().subscribe(data => {
      this.keyCase.camelCase(data);
      this.accessiblitiesList = data;
      this.prepareAccessiblityData();
    });
    this.landService.getsoilTestTypeUrl().subscribe(data => {
      this.keyCase.camelCase(data);
      this.soilTestTypesList = data;
      this.prepareSoilTestTypesList();

    });
    this.landService.getjsonUrl().subscribe(data => {
      this.monthesList = data;
      this.prepareMonth();

    });
    this.landService.getLandType().subscribe(data => {
      this.landTypeList = data;
      this.prepareLandType();

    });
    this.landService.getTopography().subscribe(data => {
      this.topographyList = data;
      this.prepareTopography();
    });
    this.landService.getAgroEchologicalZone().subscribe(data => {
      this.agroEchologyList = data;
      this.prepareAgroEchologyZone();
    });
    this.landService.getExistingLandUse().subscribe(data => {
      this.existingLandList = data;
      this.prepareExistingLand();
    });
    this.landService.getInvestmentType().subscribe(data => {
      this.investmentTypeList = data;
      this.prepareInvestmentType();
    });
    this.landService.getMoistureSource().subscribe(data => {
      this.moistureSourceList = data;
      this.prepareMoistureSource();
    });
    this.landService.getGroundWater().subscribe(data => {
      this.grounWaterList = data;
      this.prepareGroundWater();
    });
    this.landService.getSurfaceWater().subscribe(data => {
      this.surfaceWaterList = data;
      this.prepareSurfaceWater();
    });
    this.landService.getWaterTestParameters().subscribe(data => {
      this.waterSourceParamList = data;
      this.prepareWaterSourceParams();
    });
    this.isAgricultural = this.searchedLandDetail['isAgriculturalZone'];
  }

  prepareAccessiblityData() {
    for (const accessiblity of this.searchedLandDetail['accessablity']) {
      for (const accessablityList of this.accessiblitiesList) {
        if (accessiblity === accessablityList.id) {

          this.accessiblities.push(accessablityList.name);
        }
      }
    }

  }
  prepareSoilTestTypesList() {
    for (const soilTests of this.searchedLandDetail['soilTests']) {
      for (const soilTestList of this.soilTestTypesList) {
        if (soilTests.testType === soilTestList.id) {
          soilTests.name = soilTestList.name;
          this.soilTestTypes.push(soilTests);
        }
      }
    }

  }
  prepareMonth() {
    for (const climate of this.searchedLandDetail['climate']) {
      for (const climateList of this.monthesList) {
        if (climate.month === climateList.id) {
          climate.month = climateList.name;
          this.climates.push(climate);
        }
      }
    }

  }
  prepareLandType() {
    for (const landTypeList of this.landTypeList) {
      if (landTypeList.id === this.searchedLandDetail['landType']) {
        this.landTypes = landTypeList.name;
        console.log(this.landTypes);
      }
    }
  }
  prepareTopography() {
    for (const topography of this.searchedLandDetail['topography']) {
      for (const topographyList of this.topographyList) {
        if (topography.topographyType === topographyList.id) {
          topography.name = topographyList.name;
          this.topography.push(topography);
        }
      }
    }
  }
  prepareAgroEchologyZone() {
    for (const agroEchologies of this.searchedLandDetail['agroEchologyZone']) {
      for (const agroEchologyList of this.agroEchologyList) {
        if (agroEchologies.agroType === agroEchologyList.id) {
          agroEchologies.name = agroEchologyList.name;
          this.agroEchologyZones.push(agroEchologies);
        }
      }
    }
  }
  prepareExistingLand() {
    for (const existLand of this.searchedLandDetail['existLandUse']) {
      for (const existLandList of this.existingLandList) {
        if (existLand === existLandList.id) {
          this.existingLandes.push(existLandList.name);
        }
      }
    }
  }
  prepareInvestmentType() {
    for (const investmentType of this.searchedLandDetail['investmentType']) {
      for (const investmentList of this.investmentTypeList) {
        if (investmentType === investmentList.id) {
          this.investmentTypes.push(investmentList.name);
        }
      }
    }
  }
  prepareMoistureSource() {
    for (const moistureSourceList of this.moistureSourceList) {
      if (moistureSourceList.id === this.searchedLandDetail['moistureSource']) {
        this.moistureSources = moistureSourceList.name;
      }
    }
  }
  prepareGroundWater() {
    for (const groundWaterList of this.grounWaterList) {
      if (groundWaterList.id === this.searchedLandDetail.irrigationValues['groundWater']) {
        this.groundWater = groundWaterList.name;
      }
    }
  }
  prepareSurfaceWater() {
    for (const sWater of this.searchedLandDetail.irrigationValues['surfaceWater']) {
      for (const sWaterList of this.surfaceWaterList) {
        if (sWater.surfaceWaterType === sWaterList.id) {
          sWater.name = sWaterList.name;
          this.surfaceWater.push(sWater);
        }
      }
    }
  }
  prepareWaterSourceParams() {
    for (const waterSource of this.searchedLandDetail.irrigationValues['waterSourceParameter']) {
      for (const waterSourceList of this.waterSourceParamList) {
        if (waterSource.waterSourceType === waterSourceList.id) {
          waterSource.name = waterSourceList.name;
          this.waterSourceParams.push(waterSource);
          console.log(this.waterSourceParams);
        }
      }
    }
  }

  backButton() {
    this.router.navigate([`${this.loginRole}/land-dashboard`]);
  }

  positiveNumberValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const isNotOk = Number(control.value) <= 0;
      return isNotOk ? { nonPositive: { value: control.value } } : null;
    };
  }

  requestPreparation() {
    this.prepareBtnClose.nativeElement.click();
    swal({allowOutsideClick: false});
    swal.showLoading();

    const numberOfSplit = this.prepareForm.controls['splitNumberControl'].value;

    this.prepareModel.landID = this.landID;
    this.prepareModel.n = numberOfSplit;

    this.landService.RequestLandPreparation(this.prepareModel).subscribe(
      () => {

      // success
      swal.close();

      swal({
        position: 'center',
        type: 'success',
        title: 'Land successfully sent to be prepared!',
        showConfirmButton: false,
        timer: 1500
      }).then(
        () => {
          this.router.navigate([`/${this.loginRole}/land-dashboard`]);
        });
    },
      (err) => {
        swal.close();
        // error alert
        this.dialog.error(err);
      }
    );
    console.log(this.prepareModel);
  }


}
