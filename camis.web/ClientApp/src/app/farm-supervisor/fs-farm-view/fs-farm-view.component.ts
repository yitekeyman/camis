import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-fs-farm-view',
  templateUrl: 'fs-farm-view.component.html'
})
export class FsFarmViewComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;

  constructor (private router: Router, private api: FarmApiService, private ar: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => this.farmId = params.farmId, dialog.error)
      .add(this.api.getFarm(this.farmId).subscribe(farm => {
        this.farm = farm;
        this.loading = false;
      }, dialog.error));
  }

}
