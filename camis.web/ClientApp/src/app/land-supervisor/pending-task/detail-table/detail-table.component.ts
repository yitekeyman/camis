import { Component, OnInit, Input } from '@angular/core';
import { LandBankWorkFlowLand, LandBankWorkItem, Month, SoilTestType, Accessablity, LandType,
      AgroEchologicalZone, ExistingLandUse, InvestmentType, MoistureSource, WaterTestParameters,
      GroundWater, SurfaceWater, Topography} from '../../../_shared/land-bank/land.model';
import { Router, ActivatedRoute } from '@angular/router';
import { LandDataService } from '../../../_services/land-data.service';
import { timingSafeEqual } from 'crypto';

@Component({
  selector: 'app-detail-table',
  templateUrl: './detail-table.component.html',
  styleUrls: ['./detail-table.component.css']
})
export class DetailTableComponent implements OnInit {

  wfid: string;
  workflowLand: LandBankWorkFlowLand[] | any = [];
  userWorkItems: LandBankWorkItem[] | any = [];

  // data from api
  monthesList: Month[] = [];
  soilTestTypesList: SoilTestType[] = [];
  accessiblitiesList: Accessablity[] = [];
  existingLandList: ExistingLandUse[] = [];
  agroEchologyList: AgroEchologicalZone[] = [];
  investmentTypeList: InvestmentType[] = [];
  moistureSourceList: MoistureSource[] = [];
  grounWaterList: GroundWater[] = [];
  surfaceWaterList: SurfaceWater[] = [];
  waterSourceParamList: WaterTestParameters[] = [];
  topographyList: Topography[] = [];
  landTypeList: LandType[] = [];
  // data for view
  climates: any[] = [];
  soilTestTypes: any[] = [];
  accessiblities: any[] = [];
  agroEchologyZones: any[] = [];
  existingLandes: any[] = [];
  investmentTypes: any[] = [];
  moistureSources: any;
  groundWater: any[] = [];
  isAgricultural: string;
  topography: any[] = [];
  surfaceWater: any[] = [];
  waterSourceParams: any[] = [];
  landType: String;


  constructor(private router: Router, private landService: LandDataService, private activeRoute: ActivatedRoute,
              ) { }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.wfid = params.wfid;
      this.getLandDetail();
    });

  }
  getArea() {
    return Math.round(this.workflowLand.area / 10) / 1000 + ' ha';
  }
  getLandDetail() {
    this.landService.GetUserWorkItems().subscribe(data => {

      for (const workItem of data) {
        if (workItem.wfid === this.wfid) {
          this.userWorkItems = workItem;
        }
      }
    });

    this.landService.GetWorkFlowLand(this.wfid).subscribe(data => {
      this.workflowLand = data;

      console.log(this.workflowLand);
      this.getDependecies();
    });
  }

  getDependecies() {
    this.landService.getAccessiblity().subscribe(data => {
      this.accessiblitiesList = data;
      this.prepareAccessiblityData();
    });
    this.landService.getExistingLandUse().subscribe(data => {
      this.existingLandList = data;
      this.prepareExistingLand();
    });
    this.landService.getInvestmentType().subscribe(data => {
      this.investmentTypeList = data;
      this.prepareInvestmentType();
    });
    this.landService.getsoilTestTypeUrl().subscribe(data => {
      this.soilTestTypesList = data;
      this.prepareSoilTestTypesList();

    });
    this.landService.getMoistureSource().subscribe(data => {
      this.moistureSourceList = data;
      this.prepareMoistureSource();
    });
    this.landService.getWaterTestParameters().subscribe(data => {
      this.waterSourceParamList = data;
      this.prepareWaterSourceParams();
    });
    this.landService.getGroundWater().subscribe(data => {
      this.grounWaterList = data;
      this.prepareGroundWater();
    });
    this.landService.getSurfaceWater().subscribe(data => {
      this.surfaceWaterList = data;
      this.prepareSurfaceWater();
    });
    this.landService.getAgroEchologicalZone().subscribe(data => {
      this.agroEchologyList = data;
      this.prepareAgroEchologyZone();
    });
    this.landService.getjsonUrl().subscribe(data => {
      this.monthesList = data;
      this.prepareMonth();

    });
    this.landService.getTopography().subscribe(data => {
      this.topographyList = data;
      this.prepareTopography();
    });
    this.landService.getLandType().subscribe(data => {
      this.landTypeList = data;
      this.prepareLandType();
    });

    this.isAgricultural = this.workflowLand['isAgriculturalZone'];
  }

  prepareAccessiblityData() {
    for (const accessiblity of this.workflowLand['accessablity']) {
      for (const accessablityList of this.accessiblitiesList) {
        if (accessiblity === accessablityList.id) {

          this.accessiblities.push(accessablityList.name);
        }
      }
    }

  }
  prepareExistingLand() {
    for (const existLand of this.workflowLand['existLandUse']) {
      for (const existLandList of this.existingLandList) {
        if (existLand === existLandList.id) {
          this.existingLandes.push(existLandList.name);
        }
      }
    }
  }
  prepareInvestmentType() {
    for (const investmentType of this.workflowLand['investmentType']) {
      for (const investmentList of this.investmentTypeList) {
        if (investmentType === investmentList.id) {
          this.investmentTypes.push(investmentList.name);
        }
      }
    }
  }
  prepareSoilTestTypesList() {
    for (const soilTests of this.workflowLand['soilTests']) {
      for (const soilTestList of this.soilTestTypesList) {
        if (soilTests.testType === soilTestList.id) {
          soilTests.name = soilTestList.name;
          this.soilTestTypes.push(soilTests);
        }
      }
    }

  }
  prepareMoistureSource() {
    for (const moistureSourceList of this.moistureSourceList) {
      if (moistureSourceList.id === this.workflowLand['moistureSource']) {
        this.moistureSources = moistureSourceList.name;
      }
    }
  }
  prepareWaterSourceParams() {
    for (const waterSource of this.workflowLand.irrigationValues['waterSourceParameter']) {
      for (const waterSourceList of this.waterSourceParamList) {
        if (waterSource.waterSourceType === waterSourceList.id) {
          waterSource.name = waterSourceList.name;
          this.waterSourceParams.push(waterSource);
        }
      }
    }
  }
  prepareGroundWater() {
    for (const gWater of this.workflowLand.irrigationValues['groundWater']) {
      console.log(gWater);
      for (const groundWaterList of this.grounWaterList) {
        if (groundWaterList.id === gWater) {
          this.groundWater.push(groundWaterList.name);
          console.log(this.groundWater);
        }
      }
    }

  }
  prepareSurfaceWater() {
    for (const sWater of this.workflowLand.irrigationValues['surfaceWater']) {
      for (const sWaterList of this.surfaceWaterList) {
        if (sWater.surfaceWaterType === sWaterList.id) {
          sWater.name = sWaterList.name;
          this.surfaceWater.push(sWater);
        }
      }
    }
  }
  prepareAgroEchologyZone() {
    for (const agroEchologies of this.workflowLand['agroEchologyZone']) {
      for (const agroEchologyList of this.agroEchologyList) {
        if (agroEchologies.agroType === agroEchologyList.id) {
          agroEchologies.name = agroEchologyList.name;
          this.agroEchologyZones.push(agroEchologies);
        }
      }
    }
  }
  prepareTopography() {
    for (const topography of this.workflowLand['topography']) {
      for (const topographyList of this.topographyList) {
        if (topography.topographyType === topographyList.id) {
          topography.name = topographyList.name;
          this.topography.push(topography);
        }
      }
    }
  }
  prepareMonth() {
    for (const climate of this.workflowLand['climate']) {
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
      if (landTypeList.id === this.workflowLand['landType']) {
        this.landType = landTypeList.name;
      }
    }
  }

}
