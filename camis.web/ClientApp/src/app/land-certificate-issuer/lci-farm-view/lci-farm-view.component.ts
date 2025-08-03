import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ProjectApiService} from '../../_services/project-api.service';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-lci-farm-view',
  templateUrl: 'lci-farm-view.component.html'
})
export class LciFarmViewComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;
  plan: any;

  constructor (
    private router: Router,
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private ar: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.farmId = params.farmId;

      this.api.getFarm(this.farmId).subscribe(farm => {
        this.farm = farm;

        this.projectApi.getPlanFromRootActivity(this.farm.activityId).subscribe(plan => {
          this.plan = plan;
          this.loading = false;
        }, dialog.error);
      }, dialog.error);
    }, dialog.error);
  }

}
