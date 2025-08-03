import {Observable} from "rxjs";
import {Injectable} from "@angular/core";
import {ApiService} from "./api.services";
import {QueryEncoder} from "@angular/http";
import {
    IAddressResponse, IAddressUnitResponse, ICustomAddressRequest
} from "../../../../../camis.web/ClientApp/src/app/_shared/address/interfaces";

@Injectable()
export class AddressServices {
    private qs: QueryEncoder;

    constructor(private api: ApiService) {
        this.qs = new QueryEncoder();
    }


    getAllSchemes(): Observable<any> {
        return this.api.get(`Addresses/AllSchemes`);
    }

    getAddresses(schemeId: number, parentId: string): Observable<any> {
        console.log("parentid:"+ parentId);
        return this.api.get(`Addresses/Addresses?schemeId=${schemeId}&parentId=${parentId}`);
    }


    getAddressPairs(leafId: string): Observable<any> {
        return this.api.get(`Addresses/AddressPairs?leafId=${this.qs.encodeKey(leafId)}`);
    }

    getAddressUnits(schemeId: number): Observable<any> {
        return this.api.get(`Addresses/AddressUnits?schemeId=${this.qs.encodeValue(schemeId.toString())}`);
    }

    saveAddress(body: any): Observable<any> {
        return this.api.post(`Addresses/SaveAddress`, body);
    }

}