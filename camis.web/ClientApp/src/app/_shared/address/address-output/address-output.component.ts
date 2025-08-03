import {Component, Input, OnInit} from '@angular/core';

import {AddressApiService} from '../../../_services/address-api.service';
import dialog from '../../dialog';
import {IAddressPairResponse} from '../interfaces';

@Component({
  selector: 'app-address-output',
  templateUrl: './address-output.component.html'
})
export class AddressOutputComponent implements OnInit {

  @Input('leafId')
  leafId: string;

  pairs: IAddressPairResponse[] = [];

  constructor(private api: AddressApiService) { }

  ngOnInit() {
    this.api.getAddressPairs(this.leafId).subscribe(pairs => this.pairs = pairs, dialog.error);
  }

}
