import {Component, Input, OnInit} from '@angular/core';

import {AddressServices} from "../../../_services/address.services";

interface IAddressResponse {
  id: string;
  unit: string;
  name: string;
}

interface ISchemeResponse {
  id: number;
  name: string;
  addresses: IAddressResponse[];
}

@Component({
  selector: 'app-address-output',
  templateUrl: './address-output.component.html'
})
export class AddressOutputComponent implements OnInit {

  @Input('leafId')
  leafId: string;

  pairs: any[] = [];

  constructor(private api: AddressServices) { }

  ngOnInit() {
    this.api.getAddressPairs(this.leafId).subscribe(pairs => this.pairs = pairs);
  }

}
