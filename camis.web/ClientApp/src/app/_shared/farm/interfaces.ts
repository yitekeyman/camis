import {IDocument} from "../document/interfaces";

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
    landPart: number;
    leaseFrom: Date; // date
    leaseTo: Date; // date
    right: 1 | 2 | 3 | 4 | 5 | number;
    yearlyLease: number | null; // not null if right != 4
    landSectionArea: number | null; // not null if right == 5
    farmId: string;
  };
}

export interface ContractCancellationRequest {
  id: string;
  operatorId: string;
  typeId: number;
  activityId: string;
  investedCapital: number;
  description: string;
  otherTypeIds: number[];
  cancellationReason: string;
  status: IRegistrationType;
  locked: boolean;
  cancelledRight: CancelledRightRequest[];
  cancellationSupDoc: IDocument[];
  farmLands: FarmLandRequest[];
}

export interface CancelledRightRequest {
  id: string;
  landId: string;
  farmId: string;
  rightFrom: any;
  rightTo: any;
  rightType: number;
  yearlyRent: number;
  landSectionArea: number;
  splitIndex: number;
  commonTxtUid: string;
  geom: string;
  status: IRegistrationType;
}

export interface FarmLandRequest {
  landId: string;
  farmId: string;
  certificateDoc: IDocument;
  leaseContractDoc: IDocument;
  splitIndex: number;
}
