// request...

export interface ICustomAddressRequest {
  parentId: string;
  unitId: number;
  customAddressName: string;
}

// responses...

export interface ISchemeResponse {
  id: number;
  name: string;
  addresses: IAddressResponse[];
}

export interface IAddressUnitResponse {
  id: number;
  name: string;
  custom: boolean;

  _selectedAddressId: string | null;   // for use this angular code only
  _selectedAddressName: string | null; // for use this angular code only (for custom addresses)
}

export interface IAddressResponse {
  id: string;       // address id
  unit: string;     // unit name
  name: string;     // address name
  unitId: number;   // unit id
  custom: boolean;  // unit custom
}

export interface IAddressPairResponse {
  unit: string;
  name: string;
}
