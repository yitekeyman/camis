import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from '../../../_shared/dialog';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

@Component({
  selector: 'app-fs-farm-list',
  imports:[CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: 'fs-farm-list.component.html'
})
export class FsFarmListComponent implements OnInit {

  loading = true;

  term = '';
  totalFarms = 0;
  farms: any[] = [];
  ownerTypes:any[]=[];
  farmStatusTypes:any[]=[];
  farmTypes:any[]=[];
  searchParm:any;
  constructor (private api: FarmApiService, public router: Router, private keyCaseService:ObjectKeyCasingService) {
    this.api.getAllFarmOperatorTypes().subscribe(res=>{
      this.keyCaseService.camelCase(res);
      this.ownerTypes = res;
    });
    this.api.getFarmStatusTypeList().subscribe(res=>{
      this.keyCaseService.camelCase(res);
      this.farmStatusTypes = res;
    });
    this.api.getAllFarmTypes().subscribe(res=>{
      this.keyCaseService.camelCase(res);
      this.farmTypes = res;
    })
  }

  ngOnInit(): void {
    this.loading = true;
    this.searchParm = {
      term:"",
      ownerType:0,
      farmType:0,
      farmStatus:0,
    };
    this.load(0);
  }

  load(skip = this.farms.length, take = 10) {
    return this.api.searchFarms(this.term,this.searchParm.ownerType, this.searchParm.farmType, this.searchParm.farmStatus, skip, take).subscribe(farmsPaginator => {
      this.keyCaseService.camelCase(farmsPaginator);
      this.totalFarms = farmsPaginator.totalSize;
      this.farms = this.farms.slice(0, skip).concat(farmsPaginator.items);
      this.loading = false;
    }, dialog.error);
  }
  searchFarm(skip: number, take=10) {
    return this.api.searchFarms(this.term, this.searchParm.ownerType, this.searchParm.farmType, this.searchParm.farmStatus, skip, take).subscribe(farmsPaginator => {

      this.keyCaseService.camelCase(farmsPaginator);
      this.totalFarms = farmsPaginator.totalSize;
      this.farms = this.farms.slice(0, skip).concat(farmsPaginator.items);
      this.loading = false;
    }, dialog.error);
  }

  openDetail(farmId: string) {
    return this.router.navigate([`farm-management/fs/farm/${farmId}`]);
  }

}
