import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

@Component({
  selector: 'app-mne-farm-management',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: 'mne-farm-management.component.html'
})
export class MneFarmManagementComponent implements OnInit {

  loading = true;
  loginRole = '0';
  term = '';
  totalFarms = 0;
  farms: any[] = [];
  ownerTypes:any[]=[];
  farmStatusTypes:any[]=[];
  farmTypes:any[]=[];
  searchParm:any;
  constructor(private api: FarmApiService, public router: Router, private keyCase: ObjectKeyCasingService) {
    this.loginRole = localStorage.getItem("role");
    this.api.getAllFarmOperatorTypes().subscribe(res=>{
      this.keyCase.camelCase(res);
      this.ownerTypes = res;
    });
    this.api.getFarmStatusTypeList().subscribe(res=>{
      this.keyCase.camelCase(res);
      this.farmStatusTypes = res;
    });
    this.api.getAllFarmTypes().subscribe(res=>{
      this.keyCase.camelCase(res);
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
    dialog.loading();
    return this.api.searchFarms(this.term,this.searchParm.ownerType, this.searchParm.farmType, this.searchParm.farmStatus, skip, take).subscribe(farmsPaginator => {

      this.keyCase.camelCase(farmsPaginator);
      this.totalFarms = farmsPaginator.totalSize;
      this.farms = this.farms.slice(0, skip).concat(farmsPaginator.items);
      this.loading = false;
      dialog.close();
    }, dialog.error);
  }

  openDetail(farmId: string) {
   if (this.loginRole === '9') {
      return this.router.navigateByUrl(`/mne/task/pr/new/request?farmId=${farmId}`);
    } else {
      return this.router.navigateByUrl(`/mne/farm/${farmId}`);
    }
  }

}
