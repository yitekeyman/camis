import {Component, Input, OnInit} from '@angular/core';

import {AddressApiService} from '../../../_services/address-api.service';
import dialog from '../../dialog';
import {IAddressPairResponse} from '../interfaces';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";

@Component({
  selector: 'app-address-output',
  imports:[CommonModule, ReactiveFormsModule],
  templateUrl: './address-output.component.html'
})
export class AddressOutputComponent implements OnInit {

  @Input('leafId')
  leafId: string;

  pairs: IAddressPairResponse[] = [];

  constructor(private api: AddressApiService, private keyCase:ObjectKeyCasingService) { }

  ngOnInit() {
    this.api.getAddressPairs(this.leafId).subscribe(pairs => {
      this.keyCase.camelCase(pairs);
        this.pairs = pairs;
    }, dialog.error);
  }

}
