import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IAuthorityRegistrarChangeEvent, IAuthorityRegistration} from './interfaces';
import {IRegistrationAuthority, IRegistrationType} from '../interfaces';
import {IDocument} from '../../document/interfaces';
import {ISingleDocumentSelectorChangeEvent} from '../../document/single-document-selector/interfaces';
import dialog from '../../dialog';

@Component({
  selector: 'app-authority-registrar',
  templateUrl: 'authority-registrar.component.html',
})
export class AuthorityRegistrarComponent implements OnInit {

  /* Inputs */
  @Input('title') title = 'Registrations';
  @Input('registrations') registrations: IAuthorityRegistration[] = [];
  @Input('registrationTypes') registrationTypes: IRegistrationType[] = [];
  @Input('registrationAuthorities') registrationAuthorities: IRegistrationAuthority[] = [];

  /* Outputs */
  @Output('change') change = new EventEmitter<IAuthorityRegistrarChangeEvent>();

  add_type?: number;
  add_authority?: number;
  add_number?: string;
  add_document: { doc: IDocument | null } = { doc: null };

  editIndex?: number;
  editingEditDocument = false;
  edit_type?: number;
  edit_authority?: number;
  edit_number?: string;
  edit_document: { doc: IDocument | null } = { doc: null };

  constructor () {
  }

  ngOnInit(): void {
  }


  get _noDuplicate(): boolean {
    for (const reg of this.registrations) {
      if (reg.typeId == this.add_type && reg.authorityId == this.add_authority) {
        return false;
      }
    }

    return true;
  }


  chooseDocument($event: ISingleDocumentSelectorChangeEvent): void {
    if ($event.document || $event.document === null) {
      this.add_document.doc = $event.document;
    }
  }

  chooseEditDocument($event: ISingleDocumentSelectorChangeEvent): void {
    if ($event.document || $event.document === null) {
      this.edit_document.doc = $event.document;
    }
  }


  editReg(index: number): void {
    const reg = this.registrations[index];

    this.editIndex = index;
    this.edit_type = reg.typeId;
    this.edit_authority = reg.authorityId;
    this.edit_number = reg.registrationNumber;
    this.edit_document = { doc: reg.document };
  }

  cancelEdit() {
    this.editIndex = undefined;
    this.edit_type = undefined;
    this.edit_authority = undefined;
    this.edit_number = undefined;
    this.edit_document = { doc: null };
  }

  doEdit() {
    const reg = this.registrations[this.editIndex];

    reg.typeId = this.edit_type;
    reg.authorityId = this.edit_authority;
    reg.registrationNumber = this.edit_number;
    reg.document = this.edit_document.doc;

    this.editIndex = undefined;
  }

  removeReg(index: number): void {
    this.registrations.splice(index, 1);

    this.change.emit({ registrations: this.registrations });
  }

  addRegistration(docSelector?: any): void {
    if (this.add_type == undefined || this.add_authority == undefined || this.add_number == undefined) {
      dialog.error('Missing required fields.');
      return;
    }

    this.registrations.push({
      typeId: this.add_type,
      authorityId: this.add_authority,
      registrationNumber: this.add_number,
      document: this.add_document.doc
    });

    this.change.emit({ registrations: this.registrations });

    this.clear(docSelector);
  }

  clear(docSelector?: any): void {
    this.add_type = undefined;
    this.add_authority = undefined;
    this.add_number = undefined;
    this.add_document.doc = undefined;

    if (docSelector && typeof docSelector.clear === 'function') {
      docSelector.clear();
    }
  }


  findRegType(id: number): IRegistrationType | null {
    for (const type of this.registrationTypes) {
      if (type.id == id) {
        return type;
      }
    }
    return null;
  }

  findRegAuth(id: number): IRegistrationAuthority | null {
    for (const auth of this.registrationAuthorities) {
      if (auth.id == id) {
        return auth;
      }
    }
    return null;
  }
}
