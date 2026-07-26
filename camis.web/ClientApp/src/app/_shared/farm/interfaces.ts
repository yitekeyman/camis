import {IDocument} from "../document/interfaces";
import {IAuthorityRegistration} from "./authority-registrar/interfaces";

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
  reason: string;
  wfid: string;
  date: any;
}

export interface ContractWarningRequest {
  id: string;
  farmId: string;
  landId: string;
  splitIndex: number;
  date: any;
  description: string;
  stage: number;
  reason: string;
  wfid: string;
  warnedRight: CancelledRightRequest;
  supportiveDocument: IDocument[];
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

export interface ContractRenewalRequest {
  farmId: string;
  modificationReason: IRegistrationType;
  description: string;
  status: IRegistrationType;
  locked: boolean;
  modifiedRight: CancelledRightRequest[];
  supportiveDocument: IDocument[];
  farmLands: FarmLandRequest[];
}

export interface ContractBudgetYearRenewalRequest {
  id: string;
  farmId: string;
  landId: string;
  splitIndex: number;
  date: any;
  remark: string;
  budgetYear: number;
  wfid: string;
  renewRight: CancelledRightRequest;
  supportiveDocument: IAuthorityRegistration[];
}

export interface ContractRenewalDocumentRequest {
  registrationNumber: string;
  authorityId: number;
  typeId: number;
  documentId: string;
  document: IDocument;
}
