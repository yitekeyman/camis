import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";

@Component({
  selector: 'app-fs-farm-list',
  templateUrl: 'fs-farm-list.component.html'
})
export class FsFarmListComponent implements OnInit {

  loading = true;

  term = '';
  totalFarms = 0;
  farms: any[] = [];

  constructor (private api: FarmApiService, public router: Router, private keyCaseService:ObjectKeyCasingService) {
  }

  ngOnInit(): void {
    this.loading = true;
    this.load(0);
  }

  load(skip = this.farms.length, take = 10) {
    return this.api.searchFarms(this.term, skip, take).subscribe(farmsPaginator => {
      this.keyCaseService.camelCase(farmsPaginator);
      this.totalFarms = farmsPaginator.totalSize;
      this.farms = this.farms.slice(0, skip).concat(farmsPaginator.items);
      this.loading = false;
    }, dialog.error);
  }


  openDetail(farmId: string) {
    return this.router.navigate([`supervisor/farm/${farmId}`]);
  }

}
