import {IDocument} from '../../document/interfaces';

export interface IAuthorityRegistration {
    typeId: number;
    authorityId: number;
    registrationNumber: string;
    document?: IDocument;
}

export interface IAuthorityRegistrarChangeEvent {
    registrations?: IAuthorityRegistration[];
}
