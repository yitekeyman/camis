import {Component, OnInit, ElementRef, ViewChild} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {LandDataService} from '../../../_services/land-data.service';
import {
  LandType, Month, SoilTestType, Accessablity, SearchResult, LandPreparationModel,
  Topography, AgroEchologicalZone, ExistingLandUse, InvestmentType, MoistureSource,
  GroundWater, SurfaceWater, WaterTestParameters
} from '../land.model';
import {DialogService} from '../../dialog/dialog.service';
import {
  FormBuilder,
  AbstractControl,
  FormGroup,
  Validators,
  ValidatorFn,
  ReactiveFormsModule,
  FormsModule
} from '@angular/forms';
import {CamisMapComponent} from '../../camismap/camismap.component';
import dialog from "../../dialog";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {CommonModule} from "@angular/common";
import {DocumentListComponent} from "../../document/document-list/document-list.component";
import {SimpleFarmDetailsComponent} from "../../farm/farm-detail/simple-farm-details/simple-farm-details.component";
import {FarmApiService} from "../../../_services/farm-api.service";
import {forEach} from "ol/geom/flat/segments";
import {AdminServices} from "../../../_services/admin.Services";

@Component({
  selector: 'app-search-result-detail',
  templateUrl: './search-result-detail.component.html',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent, DocumentListComponent, SimpleFarmDetailsComponent],
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
  groundWater: any[] = [];
  surfaceWater: any[] = [];
  waterSourceParamList: WaterTestParameters[] = [];
  waterSourceParams: any[] = [];
  moistureSources: any[] = [];

  prepareForm: FormGroup;

  prepareModel: LandPreparationModel;
  isIrrigated: boolean = false;
  farm: any = null;
  farmStatusTypes: any[] = [];
  rightType=[
    {id:1,name:'Lease From State'},
    {id:2,name:'Lease From Private'},
    {id:3,name:'Private'},
    {id:4,name:'Contract Farming'},
    {id:5,name:'Sub-lease'},
  ]
  @ViewChild('camis_map') map: CamisMapComponent;
 // @ViewChild('prepareBtnClose') prepareBtnClose: ElementRef;

  showModal = false;

  constructor(private router: Router, public landService: LandDataService, private activeRoute: ActivatedRoute,
              private formBuilder: FormBuilder, private dialog: DialogService, private keyCase: ObjectKeyCasingService, private farmService: FarmApiService) {

    if (localStorage.getItem('role') === '4') {
      this.clerkRole = true;
      this.loginRole = 'land-clerk';
    }
    if (localStorage.getItem('role') === '5') {
      this.loginRole = 'land-supervisor';
    }
    if (localStorage.getItem('role') === '6') {
      this.loginRole = 'land-admin';
    }

    this.farmService.getFarmStatusTypeList().subscribe(statusType => {
      this.keyCase.camelCase(statusType);
      this.farmStatusTypes = statusType;
    },dialog.error);
  }

  ngOnInit() {
    this.activeRoute.params.subscribe(params => {
      this.landID = params['landID'];
      this.getLandDetail();

    });


    this.prepareModel = {
      landId: '',
      noOfSplit: null,
      description: '',
      subLand: 0,
      geoms: []
    };

  }

  getLandDetail() {
    dialog.loading();
    this.landService.GetLand(this.landID).subscribe(data => {
      this.keyCase.camelCase(data);
      this.searchedLandDetail = data;

      const matchingKey = Object.keys(data.parcels).find(
        key => key.toLowerCase() === data.upins[0]?.toLowerCase()
      );
      if (this.searchedLandDetail['landType'] === 3) {
        this.farmService.getFarmByLandId(this.landID).subscribe(res => {
          this.keyCase.camelCase(res);
          this.farm = res;
        })
      }
      this.getDependecies();
      if (this.searchedLandDetail['landSplit']?.length > 0) {
        let splitGeomData: any[] = [];
        for (let p of this.searchedLandDetail['landSplit']) {
          let parts = p.geom.split(";");
          let parcel = {
            id: `${this.searchedLandDetail['upins'][0]}-${p.indexes}`,
            wkt: parts[parts.length - 1]
          }
          splitGeomData.push(parcel);
        }
        this.map.setSplitGeomsByWKT(splitGeomData);

      }
      else {
        let g = data.parcels[matchingKey];
        if (g) {
          let parts = g.geometry.split(";");
          this.map.setWorkFlowGeomByWKT(parts[parts.length - 1]);
        }
      }
      //console.log(this.searchedLandDetail);
      dialog.close();
    }, e => {
      return dialog.error(e);
    });
  }

  getArea() {
    return Math.round(this.searchedLandDetail.area / 10) / 1000 + ' ha';
  }
  getSplitArea(a) {
    return Math.round(a / 10) / 1000 + ' ha';
  }
 getSplitStatus(status) {
    let ret="Unknown";
   for (const landTypeList of this.landTypeList) {
     if (landTypeList.id === status) {
       ret = landTypeList.name;
     }
   }
   return ret;
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
          this.groundWater.push(groundWaterList.name);
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

  backButton() {
    this.router.navigate([`${this.loginRole}/land-dashboard`]);
  }

  positiveNumberValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const isNotOk = Number(control.value) <= 0;
      return isNotOk ? {nonPositive: {value: control.value}} : null;
    };
  }

  requestPreparation() {

    dialog.loading();

    const numberOfSplit = this.prepareForm.controls['noOfSplit'].value;

    this.prepareModel.landId = this.landID;
    this.prepareModel.noOfSplit = numberOfSplit;

    this.landService.RequestLandPreparation(this.prepareModel).subscribe(
      () => {
        dialog.success('Land successfully sent to be prepared!').then(() => {
          this.router.navigate([`/land-bank/search-parcel`]);
        })
      },
      (err) => {
        return dialog.error(err);
      }
    );
    //console.log(this.prepareModel);
  }

  manageModal() {
    if (this.showModal) {
      this.showModal = false;
    }
    else {
      this.showModal = true;

      if(this.searchedLandDetail['landSplit']?.length>0) {
        this.getAvaPart();
        this.prepareForm = this.formBuilder.group({
          parts:[0,[Validators.required,Validators.min(1)]],
          noOfSplit: ['', [Validators.required, Validators.min(2)]],
          description:['']
        });
      }else{
        this.prepareForm = this.formBuilder.group({
          noOfSplit: ['', [Validators.required, Validators.min(2)]],
          description:['']
        });
      }
    }
  }
avalabelParts=[];
  getAvaPart(){
    for(let p of this.searchedLandDetail['landSplit']){
      if(p.status==2)
        this.avalabelParts.push(p);
    }
  }
  splitLandRequest() {
    dialog.loading();

    this.prepareModel.landId = this.landID;
    this.prepareModel.noOfSplit = this.prepareForm.controls['noOfSplit'].value;
    this.prepareModel.description=this.prepareForm.controls['description'].value;

    this.landService.RequestParcelSplit(this.prepareModel, "").subscribe(
      () => {
        dialog.success('Your land split request successfully sent!').then(() => {
          this.router.navigate(['default/pending-task']).catch(dialog.error);
        })
      },
      (err) => {
        return dialog.error(err);
      }
    );
  }
  getFarmStatus(id: number): string {
    for(let l of this.farmStatusTypes) {
      if (id == l.id) {
        return l.name;
      }
    }
    return null;
  }
  getRightType(id: number): string {
    for(let l of this.rightType) {
      if (id == l.id) {
        return l.name;
      }
    }
    return null;
  }
}
