import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from '../../../_shared/dialog';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";

@Component({
    selector: 'app-fs-farm-view',
    imports: [
        FarmDetailComponent,
      CommonModule,
      ReactiveFormsModule
    ],
    templateUrl: 'fs-farm-view.component.html'
})
export class FsFarmViewComponent implements OnInit {

  loading = true;

  farmId: string;
  farm: any;

  constructor (private router: Router, private api: FarmApiService, private ar: ActivatedRoute, private keyCaseService: ObjectKeyCasingService) {
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params =>{ this.keyCaseService.camelCase(params);this.farmId = params['farmId']}, dialog.error)
      .add(this.api.getFarm(this.farmId).subscribe(farm => {
        this.keyCaseService.camelCase(farm);
        this.farm = farm;
        this.loading = false;
        dialog.close()
      },dialog.error));
  }

}
