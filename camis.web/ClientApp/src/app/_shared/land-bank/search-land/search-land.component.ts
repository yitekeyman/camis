import {FormGroup, AbstractControl, FormBuilder, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {LandDataService} from '../../../_services/land-data.service';
import {PagerService} from '../../../_services/pager.service';

import {SearchLandModel, ResultViewModel} from '../../../_shared/land-bank/land.model';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {CommonModule} from "@angular/common";
import dialog from "../../dialog";
import {CamisMapComponent} from "../../camismap/camismap.component";

@Component({
  selector: 'app-search-land',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent],
  templateUrl: './search-land.component.html',
  styleUrls: ['./search-land.component.css']
})
export class SearchLandComponent implements OnInit {

  searchForm: FormGroup;
  searchModel: SearchLandModel;
  keyword: AbstractControl;
  landId = '';
  searchedResult: ResultViewModel[] = null;
  pager: any = {};
  pagedItems: any[];
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

  constructor(private router: Router, public formBuilder: FormBuilder, public landService: LandDataService, private pagerService: PagerService, private keyCase: ObjectKeyCasingService) {

    this.searchForm = this.formBuilder.group({
      keyword: [''],
      selectedLandType: ['']
    });

    this.keyword = this.searchForm.controls['keyword'];
    this.selectedLandType = this.searchForm.controls['selectedLandType'];

    const role = localStorage.getItem('role');

    if (role === '4') {
      this.clerkRole = true;
      this.loginRole = role;
    }
    if (role === '5') {
      this.loginRole = role;
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


      this.defaultLandType = this.landType[0].id;

      this.searchModel.landType = this.defaultLandType ;
      this.searchModel.areaMin = 0;
      this.searchModel.areaMax = 0;
      this.selectedLandType.setValue(this.defaultLandType)
      this.searchResult();
    });

  }

  showLandDetail(landID: string) {
    localStorage.setItem('landType', this.selectedLandType.value);
    localStorage.setItem('inputUpin', this.keyword.value);

    this.router.navigate([`land-bank/parcel-details/${landID}`]);
  }

  selectChangeHandler() {
    if (this.selectedLandId == 0)
      return;
    this.searchResult();

  }

  searchResult() {

    dialog.loading();
    this.searchedResult = [];
    this.searchModel.landType = this.selectedLandType.value;
    this.searchModel.upin = this.keyword.value;
   this.keyCase.PascalCase(this.searchModel);
    this.landService.SearchLand(this.searchModel).subscribe(resp => {
        this.keyCase.camelCase(resp);
        this.searchedResult = resp.result;
        this.setPage(1);
        dialog.close();
      },
      (err) => {
        return dialog.error(err);
      }
    );

  }

  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.searchedResult.length, page);
    this.pagedItems = this.searchedResult.slice(this.pager.startIndex, this.pager.endIndex + 1);

  }

  prepareLandType(id) {
    for (const type of this.landType) {
      if (id === type['id']) {
        return type['name'];
      }
    }
  }

  editLand(wfid: string) {
    // routerLink="/land-clerk/pending-task/edit-land/:wfid"
    this.router.navigate([`land-bank/task/edit-parcel-info/${wfid}`]);
  }

  goToRegistration() {
    this.router.navigate([`land-bank/register-parcel`]);
  }
}
