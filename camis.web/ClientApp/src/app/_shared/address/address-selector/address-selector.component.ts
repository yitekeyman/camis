import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {AddressApiService} from '../../../_services/address-api.service';
import dialog from '../../dialog';
import {IAddressResponse, IAddressUnitResponse, ISchemeResponse} from '../interfaces';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-address-selector',
  templateUrl: './address-selector.component.html'
})
export class AddressSelectorComponent implements OnInit {

  outputMode = false;

  selectedSchemeId: number | null = null;
  @Input('finalAddressId') finalAddressId: string | null = null;

  schemes: ISchemeResponse[] = [];
  addressUnits: IAddressUnitResponse[] = [];
  addresses2d: IAddressResponse[][] = [];

  @Input('label') label = 'Select an Address:';
  @Input('noFooter') noFooter = false;

  @Output('onChange') changeNotification: EventEmitter<string | null> = new EventEmitter<string | null>();
  @Output('onSave') saveNotification: EventEmitter<string | null> = new EventEmitter<string | null>();

  constructor(private api: AddressApiService, private keyCase: ObjectKeyCasingService) { }

  ngOnInit() {
    this.schemes = [];
    this.addresses2d = [];

    if (this.finalAddressId) {
      this.outputMode = true;
    }

    this.api.getAllSchemes().subscribe(schemes => {
      this.keyCase.camelCase(schemes);
      this.schemes = schemes;

      if (!this.finalAddressId) {
        this.onFormatChanged(this.schemes.length ? this.schemes[0].id : null);
      }
    }, dialog.error);
  }


  onFormatChanged(selectedSchemeId: number | null): void {
    dialog.loading({ allowOutsideClick: true, allowEscapeKey: true });

    if (selectedSchemeId === null) {
      this.selectedSchemeId = null;
      this.finalAddressId = null;

      this.addressUnits = [];
      this.addresses2d = [];

      dialog.close();
    }
    else {
      this.api.getAddressUnits(selectedSchemeId).subscribe(units => {
        this.keyCase.camelCase(units);
        this.selectedSchemeId = selectedSchemeId;
        this.finalAddressId = null;

        this.addressUnits = units.map(u => {
          u._selectedAddressId = null;
          u._selectedAddressName = null;
          return u
        });

        this.addresses2d = [];
        for (const scheme of this.schemes) {
          if (scheme.id == this.selectedSchemeId) {
            this.addresses2d = [scheme.addresses];
            break;
          }
        }

        this.onChange();

        dialog.close();
      }, dialog.error);
    }
  }

  async onAddressSelected(parentId: string, unit: IAddressUnitResponse, i: number, customAddressName?: string): Promise<void> {
    dialog.loading({ allowOutsideClick: true, allowEscapeKey: true });

    if (!parentId ||parentId == 'null' || customAddressName === '') {
      parentId = null;

      this.addresses2d = this.addresses2d.slice(0, i + 1);
      this.finalAddressId = null;
      this.onChange();

      dialog.close();
      return;
    }

    if (customAddressName) {
      this.api
        .saveAddress({ parentId, unitId: unit.id, customAddressName })
        .subscribe(address => {
          this.keyCase.camelCase(address);
          this.onAddressSelected(address.id, unit, i, undefined).catch(dialog.error);
        }, dialog.error);
      return;
    }

    if (i === this.addressUnits.length - 1) { // success
      this.finalAddressId = parentId; // leaf
      this.onChange();

      return dialog.close();
    }

    this.finalAddressId = null;
    this.onChange();

    this.api
      .getAddresses(this.selectedSchemeId, parentId)
      .subscribe(addresses => {
        this.keyCase.camelCase(addresses);
        this.addresses2d = this.addresses2d.slice(0, i + 1);
        this.addresses2d.push(addresses);
        this.onChange();

        dialog.close();
      }, dialog.error);
  }

  onChange(): void {
    this.changeNotification.emit(this.finalAddressId);
  }

  onSave(): void {
    this.outputMode = true;
    this.saveNotification.emit(this.finalAddressId);
  }

  _determineParentId(i: number): string | null {
    if (this.addressUnits.length > 1) {
      const id = this.addressUnits[i - 1]._selectedAddressId;
      const name = this.addressUnits[i - 1]._selectedAddressName;

      const addressesFromId = id && this.addresses2d[i - 1].filter(a => a.id === id) || [];
      const addressesFromName = name && this.addresses2d[i - 1].filter(a => a.name === name) || [];

      return addressesFromId.concat(addressesFromName)[0].id;
    }

    return null;
  }
}
