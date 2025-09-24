import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';
import {
  IAddressPairResponse,
  IAddressResponse,
  IAddressUnitResponse,
  ICustomAddressRequest,
  ISchemeResponse
} from '../_shared/address/interfaces';

@Injectable()
export class AddressApiService {

  constructor(private api: ApiService) { }

  getAllSchemes(): Observable<ISchemeResponse[]> {
    return this.api.get(`Addresses/AllSchemes`);
  }

  getAddressUnits(schemeId: number): Observable<IAddressUnitResponse[]> {
    // Using HttpParams through the ApiService options
    return this.api.get(`Addresses/AddressUnits`, {
      params: { schemeId: schemeId.toString() }
    });
  }

  getAddresses(schemeId: number, parentId: string): Observable<IAddressResponse[]> {
    // Using HttpParams for multiple parameters
    return this.api.get(`Addresses/Addresses`, {
      params: {
        schemeId: schemeId.toString(),
        parentId: parentId
      }
    });
  }

  getAddressPairs(leafId: string): Observable<IAddressPairResponse[]> {
    // Using HttpParams through the ApiService options
    return this.api.get(`Addresses/AddressPairs`, {
      params: { leafId: leafId }
    });
  }

  saveAddress(body: ICustomAddressRequest): Observable<IAddressResponse> {
    return this.api.post(`Addresses/SaveAddress`, body);
  }
}
