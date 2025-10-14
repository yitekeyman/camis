import {Component, OnInit, ViewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {
  Accessablity, AgroEchologicalZone, ExistingLandUse, GroundWater, InvestmentType,
  LandBankWorkFlowLand,
  LandBankWorkItem, LandType, MoistureSource,
  Month,
  SoilTestType, SurfaceWater, Topography, WaterTestParameters
} from "../../../_shared/land-bank/land.model";
import {ActivatedRoute, Router} from "@angular/router";
import {LandDataService} from "../../../_services/land-data.service";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";
import {DocumentListComponent} from "../../../_shared/document/document-list/document-list.component";
import {CamisMapComponent} from "../../../_shared/camismap/camismap.component";

@Component({
  selector: 'app-land-details-table',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, DocumentListComponent, CamisMapComponent],
  templateUrl: './land-details-table.component.html',
  styleUrls: ['land-details-table.component.scss']
})

export class LandDetailsTableComponent implements OnInit {
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
  @ViewChild('camis_map') map: CamisMapComponent;

  public loginRole=0;
  constructor(
    private router: Router,
    private landService: LandDataService,
    private activeRoute: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
    this.loginRole=parseInt(localStorage.getItem('role'), 10);
  }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.wfid = params['wfid'];
      this.getLandDetail();
    });
  }

  getArea() {
    return Math.round(this.workflowLand.area / 10) / 1000 + ' ha';
  }

  getLandDetail() {
    dialog.loading();
    this.landService.GetUserWorkItems().subscribe(data => {
      this.keyCase.camelCase(data);
      for (const workItem of data) {
        if (workItem.wfid === this.wfid) {
          this.userWorkItems = workItem;

        }
      }
    }, dialog.error);

    this.landService.GetWorkFlowLand(this.wfid).subscribe(data => {
      this.keyCase.camelCase(data);
      this.workflowLand = data;
      let g = data.parcels[data.upins[0]];
      if (g) {
        let parts = g.geometry.split(";");
        this.map.setWorkFlowGeomByWKT(parts[parts.length-1]);
      }
      console.log(this.workflowLand);
      this.getDependecies();
      dialog.close();
    }, dialog.error);
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

  async approveRequest(): Promise<void>{
    if (!await dialog.confirm('Are you sure you want to approve this registration request?')) {
      return;
    }

    const message = await dialog.prompt('Enter a note (optional):');
    dialog.loading();
    if(this.userWorkItems['workFlowType'] === 4) {
      this.landService.ApproveRegistration(this.wfid, message).subscribe(res=>{
        dialog.success('The parcel identification has been approved successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }else if(this.userWorkItems['workFlowType'] === 7) {
      this.landService.ApprovePreparation(this.wfid, message).subscribe(res=>{
        dialog.success('The parcel preparation has been approved successfully.')
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }
  }
  async rejectRequest(): Promise<void>{
    if (!await dialog.confirm('Are you sure you want to reject this request?')) {
      return;
    }

    const message = await dialog.prompt('Enter a rejection note for land bank registrar:');
    if (message===null) {
      return;
    }
    dialog.loading();
    if(this.userWorkItems['workFlowType'] === 4) {
      this.landService.RejectRegistrationRequest(this.wfid, message).subscribe(res=>{
        dialog.success('The parcel identification has been rejected successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }
    else if(this.userWorkItems['workFlowType'] === 7) {
      this.landService.RejectPreparationRequest(this.wfid, message).subscribe(res=>{
        dialog.success('The parcel preparation has been rejected successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }
  }
  async cancelRequest(): Promise<void>{
    if (!await dialog.confirm('Are you sure you want to cancel this request?')) {
      return;
    }

    const message = await dialog.prompt('Enter a cancellation note for land bank registrar:');
    if (message===null) {
      return;
    }
    dialog.loading();
    if(this.userWorkItems['workFlowType'] === 4) {
      this.landService.CancelRegistrationRequest(this.wfid, message).subscribe(res=>{

        dialog.success('The parcel identification has been cancelled successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);

      }, dialog.error);
    }
    else if(this.userWorkItems['workFlowType'] === 7) {
      this.landService.CancelPreparationRequest(this.wfid, message).subscribe(res=>{
        dialog.success('The parcel preparation has been cancelled successfully.');
        this.router.navigate(['default/pending-task']).catch(dialog.error);
      }, dialog.error);
    }
  }

  public editParcel(){
    this.router.navigate([`land-bank/task/edit-parcel-info/${this.wfid}`]).catch(dialog.error)
  }
}
