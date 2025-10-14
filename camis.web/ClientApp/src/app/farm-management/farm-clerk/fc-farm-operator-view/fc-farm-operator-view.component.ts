import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from '../../../_shared/dialog';
import {ProjectApiService} from "../../../_services/project-api.service";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-fc-farm-operator-view',
  imports:[CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent],
  templateUrl: 'fc-farm-operator-view.component.html'
})
export class FcFarmOperatorViewComponent implements OnInit {

  loading = true;

  farmOperatorId: string;
  farmOperator: any;

  constructor (private api: FarmApiService, private projectApi: ProjectApiService, private router: Router, private ar: ActivatedRoute, private keyCase:ObjectKeyCasingService) {}

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => this.farmOperatorId = params['farmOperatorId'], dialog.error)
      .add(this.api.getFarmOperator(this.farmOperatorId).subscribe(farmOperator => {
        this.keyCase.camelCase(farmOperator);
        this.farmOperator = farmOperator;
        this.loading = false;
        dialog.close();
      }, dialog.error));
  }
}
