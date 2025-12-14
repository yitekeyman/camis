import {Component, Input, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {
  Accessablity,
  AgroEchologicalZone,
  ExistingLandUse, GroundWater, InvestmentType,
  LandType, MoistureSource,
  Month, SearchResult,
  SoilTestType, SurfaceWater,
  Topography, WaterTestParameters
} from "../../land.model";
import {ActivatedRoute, Router} from "@angular/router";
import {LandDataService} from "../../../../_services/land-data.service";
import {DialogService} from "../../../dialog/dialog.service";
import {ObjectKeyCasingService} from "../../../../_services/object-key-casing.service";
import dialog from "../../../dialog";

@Component({
  selector: "app-simple-land-bank-details",
  imports:[CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: "./simple-land-bank-details.component.html",
  styleUrls:["./simple-land-bank-details.component.scss"]
})

export class SimpleLandBankDetails implements OnInit{
  @Input() landID: string;
  public loading = true;
  searchedLandDetail: SearchResult[] | any = [];
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
  groundWater: any[]=[];
  surfaceWater: any[] = [];
  waterSourceParamList: WaterTestParameters[] = [];
  waterSourceParams: any[] = [];
  moistureSources: any[]=[];
  isIrrigated = false;
  constructor(private router: Router, public landService: LandDataService, private activeRoute: ActivatedRoute,
              private formBuilder: FormBuilder, private dialog: DialogService, private keyCase: ObjectKeyCasingService) {}

  ngOnInit() {
   if(this.landID){
     this.getLandDetail();
   }
  }
  getLandDetail() {
    this.landService.GetLand(this.landID).subscribe(data => {
      this.keyCase.camelCase(data);
      this.searchedLandDetail = data;
      this.getDependecies();

    }, e => {
      return dialog.error(e);
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
      for (const moi of this.moistureSources) {
        if (moi === 'Irrigated') {
          this.isIrrigated = true;
        }
      }
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
    this.loading=false;
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
      for (const moi of this.searchedLandDetail['moistureSource']) {
        if (moistureSourceList.id === moi) {
          this.moistureSources.push(moistureSourceList.name);
        }
      }
    }
  }

  prepareGroundWater() {
    for (const gw of this.searchedLandDetail.irrigationValues['groundWater']) {
      for (const groundWaterList of this.grounWaterList) {
        if (groundWaterList.id === gw) {
          this.groundWater.push(groundWaterList.name) ;
        }
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
}
