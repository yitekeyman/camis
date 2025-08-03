export interface IRegistrationType {
  id: number;
  name: string;
}

export interface IRegistrationAuthority {
  id: number;
  name: string;
}

export interface IWaitLandAssignmentRequest {
  landTransferRequest: {
    farmer: any;
    landID: string;
    leaseFrom: Date; // date
    leaseTo: Date; // date
    right: 1 | 2 | 3 | 4 | 5 | number;
    yearlyLease: number | null; // not null if right != 4
    landSectionArea: number | null; // not null if right == 5
  };
}
