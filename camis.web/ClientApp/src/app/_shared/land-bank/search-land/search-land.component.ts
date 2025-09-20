import {FormGroup, AbstractControl, FormBuilder, FormsModule} from '@angular/forms';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import swal from 'sweetalert2';

import {DialogService} from '../../../_shared/dialog/dialog.service';
import {LandDataService} from '../../../_services/land-data.service';
import {PagerService} from '../../../_services/pager.service';

import {LandModel, SearchLandModel, ResultViewModel} from '../../../_shared/land-bank/land.model';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-search-land',
  templateUrl: './search-land.component.html',
  styleUrls: ['./search-land.component.css']
})
export class SearchLandComponent implements OnInit {

  searchForm: FormGroup;
  searchModel: SearchLandModel;
  keyword: AbstractControl;
  searchValue = [];
  landId = '';
  searchedResult: ResultViewModel[] = null;
  pager: any = {};
  defaultPager: any = {};
  pagedItems: any[];
  defaultPagedItems: any[];
  defaultLandType: any;
  defaultSearch: ResultViewModel[] = null;

  noSearchResult = false;
  noDefaultSearchResult = false;
  clerkRole = false;
  loginRole = '';
  selectedLandType: AbstractControl;

  landType: any[] = [];
  selectedLandId: number;
  landTypeList: any[] = [];

  constructor(private router: Router, public formBuilder: FormBuilder, public landService: LandDataService,
              private dialog: DialogService, private pagerService: PagerService, private keyCase: ObjectKeyCasingService) {

    this.searchForm = this.formBuilder.group({
      keyword: [''],
      selectedLandType: ['']
    });

    this.keyword = this.searchForm.controls.keyword;
    this.selectedLandType = this.searchForm.controls.selectedLandType;

    // if (localStorage.getItem('inputUpin') !== undefined ) {
    //   this.keyword.setValue(localStorage.getItem('inputUpin'));
    // }
    // if (localStorage.getItem('landType') !== undefined ) {
    //   this.selectedLandType.setValue(localStorage.getItem('landType'));
    // }

    if (localStorage.getItem('role') === '4') {
      this.clerkRole = true;
      this.loginRole = 'land-clerk';
    }
    if (localStorage.getItem('role') === '5') {
      this.loginRole = 'land-supervisor';
    }


    this.searchModel = {
      upin: '',
      landType: null,
      areaMin: null,
      areaMax: null
    };

  }

  ngOnInit() {

    this.landService.getLandType().subscribe(data => {
      this.landType = data.filter(d => d.id !== 1); // todo: SLI-temp
      console.log();

      this.defaultLandType = this.landType[0].id;

      this.searchModel.landType = this.defaultLandType;
      this.searchModel.areaMin = 0;
      this.searchModel.areaMax = 0;
      this.keyCase.PascalCase(this.searchModel);
      this.landService.SearchLand(this.searchModel).subscribe(resp => {
        this.keyCase.camelCase(resp);
        this.defaultSearch = resp.result;
        this.noDefaultSearchResult = this.defaultSearch.length == 0;

        console.log(this.defaultSearch);
        // this.setPage(1);
      });

    });

  }

  showLandDetail(landID: string) {
    localStorage.setItem('landType', this.selectedLandType.value);
    localStorage.setItem('inputUpin', this.keyword.value);

    this.router.navigate([`${this.loginRole}/land-dashboard/land-detail/${landID}`]);
  }

  selectChangeHandler(selectedLandType: number) {
    // console.log(this.selectedLandType.value);
    for (const type of this.landType) {
      if (selectedLandType == 0) {
        this.searchedResult = null;
        this.noSearchResult = false;

        this.searchModel.landType = null;

        return;
      }
      if (type.name === selectedLandType) {
        this.selectedLandId = type.id;
      }
    }

    this.searchModel.landType = this.selectedLandId;

    this.searchResult();

  }

  searchResult() {

    swal({allowOutsideClick: false});
    swal.disableButtons();
    swal.showLoading();
    this.searchedResult = [];
    this.searchModel.upin = this.keyword.value;
    this.keyCase.PascalCase(this.searchModel);
    this.landService.SearchLand(this.searchModel).subscribe(resp => {
      this.keyCase.camelCase(resp);
        this.searchedResult = resp.result;

        this.setPage(1);

        swal.close();

        this.noSearchResult = this.searchedResult.length == 0;

        this.prepareLandType();

      },
      (err) => {
        swal.close();
        // error alert
        this.dialog.error(err);
      }
    );

  }

  setPage(page: number) {
    console.log(page);

    this.pager = this.pagerService.getPager(this.searchedResult.length, page);
    console.log(this.defaultPager);
    this.pagedItems = this.searchedResult.slice(this.pager.startIndex, this.pager.endIndex + 1);

    // this.defaultPager = this.pagerService.getPager(this.defaultSearch.length, page);
    // this.defaultPagedItems = this.defaultSearch.slice(this.defaultPager.startIndex, this.defaultPager.endIndex + 1);


    console.log(this.defaultPagedItems);
  }

  prepareLandType() {
    for (const result of this.searchedResult) {
      for (const type of this.landType) {
        if (result.landType === type['id']) {
          result.landType = type['name'];
        }
      }
    }

  }

  editLand(wfid: string) {
    // routerLink="/land-clerk/pending-task/edit-land/:wfid"
    this.router.navigate([`${this.loginRole}/pending-task/edit-land/${wfid}`]);
  }

  goToRegistration() {
    this.router.navigate([`${this.loginRole}/new-land/new-land-form`]);
  }
}
