import {Injectable} from '@angular/core';
import {QueryEncoder} from '@angular/http';
import {Observable} from 'rxjs/Observable';

import {ApiService} from './api.service';
import {
  IAddressPairResponse,
  IAddressResponse,
  IAddressUnitResponse,
  ICustomAddressRequest,
  ISchemeResponse
} from '../_shared/address/interfaces';

@Injectable()
export class AddressApiService {

  private qs: QueryEncoder;

  constructor(private api: ApiService) {
    this.qs = new QueryEncoder();
  }


  getAllSchemes(): Observable<ISchemeResponse[]> {
    return this.api.get(`Addresses/AllSchemes`);
  }

  getAddressUnits(schemeId: number): Observable<IAddressUnitResponse[]> {
    return this.api.get(`Addresses/AddressUnits?schemeId=${this.qs.encodeValue(schemeId.toString())}`);
  }

  getAddresses(schemeId: number, parentId: string): Observable<IAddressResponse[]> {
    return this.api.get(`Addresses/Addresses?schemeId=${this.qs.encodeValue(schemeId.toString())}&parentId=${this.qs.encodeValue(parentId)}`);
  }


  getAddressPairs(leafId: string): Observable<IAddressPairResponse[]> {
    return this.api.get(`Addresses/AddressPairs?leafId=${this.qs.encodeKey(leafId)}`);
  }


  saveAddress(body: ICustomAddressRequest): Observable<IAddressResponse> {
    return this.api.post(`Addresses/SaveAddress`, body);
  }
}
