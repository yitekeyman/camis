import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-mnee-farms',
  templateUrl: 'mnee-farms.component.html'
})
export class MneeFarmsComponent implements OnInit {

  loading = true;

  term = '';
  totalFarms = 0;
  farms: any[] = [];

  constructor (private api: FarmApiService, public router: Router) {
  }

  ngOnInit(): void {
    this.loading = true;
    this.load(0);
  }

  load(skip = this.farms.length, take = 10) {
    return this.api.searchFarms(this.term, skip, take).subscribe(farmsPaginator => {
      this.totalFarms = farmsPaginator.totalSize;
      this.farms = this.farms.slice(0, skip).concat(farmsPaginator.items);
      this.loading = false;
    }, dialog.error);
  }


  openDetail(farmId: string) {
    return this.router.navigateByUrl(`/mne-expert/farm/${farmId}`);
  }

}
