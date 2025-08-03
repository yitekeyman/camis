import {Component, OnInit, Input, Output, EventEmitter} from "@angular/core";
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
interface IAddressUnitResponse {
  
    id: number;
    name: string;
    custom: boolean;

    _selectedAddressId: string | null;   // for use this angular code only
    _selectedAddressName: string | null; // for use this angular code only (for custom addresses)
}

@Component({
    selector:'app-address-selector',
    templateUrl:'./address-selector.component.html'
})

export class AddressSelectorComponent implements OnInit{
    outputMode = false;
    selectedSchemeId: number | null = null;
    @Input('finalAddressId') finalAddressId: string | null = null;

    schemes: ISchemeResponse[]=[];
    addresses: IAddressResponse[][];
    addressUnits: IAddressUnitResponse[] = [];
    addresses2d: IAddressResponse[][] = [];

    @Input('label')
    label = 'Select an Address:';
    @Input('noFooter')
    noFooter = false;

    @Output('onChange')
    changeNotification: EventEmitter<string | null> = new EventEmitter<string | null>();
    @Output('onSave')
    saveNotification: EventEmitter<string | null> = new EventEmitter<string | null>();

    constructor(private api: AddressServices) { }

    ngOnInit() {
        this.schemes = [];
        this.addresses2d = [];

        if (this.finalAddressId) {
            this.outputMode = true;
        }

        this.api.getAllSchemes().subscribe(schemes => {
            this.schemes = schemes;

            if (!this.finalAddressId) {
                this.onFormatChanged(this.schemes.length ? this.schemes[0].id : null);
            }
        });
    }


    onFormatChanged(selectedSchemeId: number | null): void {

        if (selectedSchemeId === null) {
            this.selectedSchemeId = null;
            this.finalAddressId = null;

            this.addressUnits = [];
            this.addresses2d = [];
        }
        else {
            this.api.getAddressUnits(selectedSchemeId).subscribe(units => {
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
            });
        }
    }


    async onAddressSelected(parentId: string, unit: IAddressUnitResponse, i: number, customAddressName?: string): Promise<void> {

        if (!parentId ||parentId == 'null' || customAddressName === '') {
            parentId = null;

            this.addresses2d = this.addresses2d.slice(0, i + 1);
            this.finalAddressId = null;
            this.onChange();

            return;
        }

        if (customAddressName) {
            this.api
                .saveAddress({ parentId, unitId: unit.id, customAddressName })
                .subscribe(address => {
                    this.onAddressSelected(address.id, unit, i, undefined).catch();
                });
            return;
        }

        if (i === this.addressUnits.length - 1) { // success
            this.finalAddressId = parentId; // leaf
            this.onChange();

            return;
        }

        this.finalAddressId = null;
        this.onChange();

        this.api
            .getAddresses(this.selectedSchemeId, parentId)
            .subscribe(addresses => {
                this.addresses2d = this.addresses2d.slice(0, i + 1);
                this.addresses2d.push(addresses);
                this.onChange();

            });
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