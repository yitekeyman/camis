import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import {ProjectApiService} from '../../../_services/project-api.service';
import {ObjectKeyCasingService} from '../../../_services/object-key-casing.service';
import {IAuthorityRegistration} from '../../../_shared/farm/authority-registrar/interfaces';
import dialog from '../../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {DocumentModule} from "../../../_shared/document/document.module";
import {AddressModule} from "../../../_shared/address/address.module";
import {AuthorityRegistrarComponent} from "../../../_shared/farm/authority-registrar/authority-registrar.component";
import {IDocument} from "../../../_shared/document/interfaces";

@Component({
  selector: 'app-fc-farm-modification',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, DocumentModule, AddressModule, AuthorityRegistrarComponent],
  templateUrl: 'fc-farm-modification.component.html'
})
export class FcFarmModificationComponent implements OnInit {

  workflowId: string | null = null;

  step = 0;

  farmOperatorTypes: any[] = [];
  farmOperatorOrigins: any[] = [];
  farmTypes: any[] = [];
  registrationAuthorities: any[] = [];
  registrationTypes: any[] = [];

  opId?: string;
  opName = '';
  opNationality = '';
  opType: string | null = null;
  opAddress: string | null = null;
  opEmail = '';
  opPhone = '';
  opOriginId: string | null = null;
  opCapital = '';
  opRegistrations: IAuthorityRegistration[] = [];

  opGender: 'M' | 'F' = 'F';
  opMartialStatus: 1 | 2 | 3 | 4 = 1;
  opBirthdate = new Date(Date.now() - 1000 * 60 * 60 * 24 * 365.25 * 18).toISOString().slice(0, 10);
  opPhotoId: string = null;
  opPhoto: IDocument = {
    file: undefined,
    filename: '',
    mimetype: '',
    date: null,
    note: '',
    ref: '',
    id: ''
  };
  opPhotoFile = "";
  opVentures: string[] = [];
  opVentureResults: any[] = []; // only in UI
  opVentureResultSelections: string[] = []; // only in UI

  frId?: string;
  frType: string | null = null;
  frOtherTypeIds: number[] = [];
  frInvestedCapital = '';
  frDescription = '';
  frRegistrations: IAuthorityRegistration[] = [];

  constructor(
    private api: FarmApiService,
    private projectApi: ProjectApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
  ) {
  }

  ngOnInit(): void {
    dialog.loading();
    this.api.getAllFarmOperatorTypes().subscribe(farmOperatorTypes => {
      this.keyCase.camelCase(farmOperatorTypes);
      this.farmOperatorTypes = farmOperatorTypes
    }, dialog.error);
    this.api.getAllFarmOperatorOrigins().subscribe(farmOperatorOrigins => {
      this.keyCase.camelCase(farmOperatorOrigins);
      this.farmOperatorOrigins = farmOperatorOrigins
    }, dialog.error);
    this.api.getAllFarmTypes().subscribe(farmTypes => {
      this.keyCase.camelCase(farmTypes);
      this.farmTypes = farmTypes
    }, dialog.error);
    this.api.getAllRegistrationAuthorities().subscribe(registrationAuthorities => {
      this.keyCase.camelCase(registrationAuthorities);
      this.registrationAuthorities = registrationAuthorities
    }, dialog.error);
    this.api.getAllRegistrationTypes().subscribe(registrationTypes => {
      this.keyCase.camelCase(registrationTypes);
      this.registrationTypes = registrationTypes
    }, dialog.error);

    this.ar.params.subscribe(params => {
      this.workflowId = params['workflowId'] ? params['workflowId'] : null;
      this.frId = params['farmId'];

      if (this.workflowId) { // a started workflow
        this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
          this.keyCase.camelCase(workItem);
          this.setFields(workItem.data);
          this.step = 1;
          dialog.close();
        }, dialog.error);
      } else if (this.frId) { // new workflow to be started using the farm ID (this.frId)
        this.api.getFarm(this.frId).subscribe(farm => {
          this.keyCase.camelCase(farm);
          this.setFields(farm);
          this.step = 1;
          dialog.close();
        }, dialog.error);
      }
    }, dialog.error);
  }

  private setFields(f: any): void {
    if (!f) {
      return;
    }
    this.keyCase.camelCase(f);

    const o = f.operator;
    if (o) {
      this.opId = o.id;
      this.opName = o.name;
      this.opNationality = o.nationality;
      this.opType = o.typeId;
      this.opAddress = o.addressId;
      this.opEmail = o.email;
      this.opPhone = o.phone;
      this.opOriginId = o.originId;
      this.opCapital = o.capital;
      this.opRegistrations = o.registrations;

      this.opGender = o.gender || this.opGender;
      this.opMartialStatus = o.martialStatus || this.opMartialStatus;
      this.opBirthdate = o.birthdate && new Date(o.birthdate).toISOString().slice(0, 10) || this.opBirthdate;

      this.opVentures = o.ventures || [];
      this.opPhoto=o.photo;
      this.opPhotoId=o.photoId;
    }

    this.frId = f.id;
    this.frType = f.typeId;
    this.frOtherTypeIds = f.otherTypeIds || [];
    this.frInvestedCapital = f.investedCapital;
    this.frDescription = f.description;
    this.frRegistrations = f.registrations;
  }


  searchVentures(term: string): void {
    this.api.searchFarmOperators(term, 0, 25)
      .subscribe(result => this.opVentureResults = result.items, dialog.error)
  }

  addVentures(): void {
    console.log(this.opVentureResultSelections);
    for (const farmOperatorId of this.opVentureResultSelections)
      if (!this.opVentures.includes(farmOperatorId))
        this.opVentures.push(farmOperatorId)
  }

  removeFromVentures(farmOperatorId: string): void {
    this.opVentures = this.opVentures.filter(v => v !== farmOperatorId)
  }


  async cancel(): Promise<void> {
    if (this.workflowId) {
      if (!await dialog.confirm('Are you sure you want to cancel this workflow? This is irreversible.')) {
        return;
      }

      dialog.loading();
      this.api.cancelFarmModification(this.workflowId, null).subscribe(res => {
        if (res.success) {
          this.router.navigateByUrl(`default/pending-task`).catch(dialog.error);
          return dialog.success('The workflow has been cancelled successfully.');
        } else {
          return dialog.error(res);
        }
      }, err => {
        return dialog.error(err)
      });
    } else {
      this.router.navigateByUrl(`default/pending-task`).catch(dialog.error);
    }
  }

  dumpSubmit(e: any): boolean {
    e.preventDefault();
    return false;
  }

  async onSubmit(e: any): Promise<void> {
    this.dumpSubmit(e);

    const message = await dialog.prompt('Enter a message for the supervisor (optional):');
    if (message === "null") {
      return
    }

    dialog.loading();

    const body: any = {
      id: this.frId,
      typeId: Number(this.frType),
      otherTypeIds: this.frOtherTypeIds.map(str => Number(str)),
      investedCapital: Number(this.frInvestedCapital),
      description: this.frDescription,
      registrations: this.frRegistrations,
      operator: {
        id: this.opId,
        name: this.opName,
        nationality: this.opNationality,
        typeId: Number(this.opType),
        addressId: this.opAddress,
        phone: this.opPhone,
        email: this.opEmail,
        originId: Number(this.opOriginId),
        capital: Number(this.opCapital),
        registrations: this.opRegistrations,

        gender: this.opGender,
        martialStatus: Number(this.opMartialStatus),
        birthdate: new Date(this.opBirthdate).getTime(),

        ventures: this.opType == '6' ? this.opVentures : [],
        photo:this.opPhoto.file!=null?this.opPhoto:null,
        photoId:this.opPhotoId,
      }
    };

    this.keyCase.PascalCase(body);

    const req = this.workflowId ?
      this.api.requestFarmModification(this.workflowId, body, message) :
      this.api.requestNewFarmModification(body, message);

    req.toPromise()
      .then(() => dialog.success('Your modification request has been sent to the supervisor successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        return dialog.error(err)
      });
  }

  validEmail = true;

  validateEmail() {
    const value = this.opEmail
    if (!value) {
      this.validEmail = true;
      return;
    }
    const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const valid = regex.test(value);

    if (!valid) {
      this.validEmail = false;
    } else {
      this.validEmail = true;
    }
  }

  getPhotoSource(): string {
    if (this.opPhoto.file) {
      // Convert base64 string to data URL for display
      return `data:${this.opPhoto.mimetype};base64,${this.opPhoto.file}`;
    }
    return 'assets/images/user/avatar.jpg';
  }

  readPhotoFile(e: any): void {
    const file = e.target.files[0] as File;

    if (!file) {
      this.opPhoto.file = undefined;
      return;
    }

    // Check if the file is an image
    if (!file.type.startsWith('image/')) {
      alert('Please select an image file (JPEG, PNG, GIF, etc.)');
      this.resetPhoto();
      return;
    }

    const reader = new FileReader();

    reader.onload = (ev) => {
      const result = (ev.target as any).result;
      // Convert to base64 and remove data URL prefix if present
      const base64String = result.includes('base64,')
        ? result.split('base64,')[1]
        : btoa(result);

      this.opPhoto.file = base64String;
    };

    reader.onerror = (error) => {
      console.error('Error reading file:', error);
      this.resetPhoto();
    };

    // Use readAsDataURL for better image handling
    reader.readAsDataURL(file);

    this.opPhoto.filename = file.name;
    this.opPhoto.mimetype = file.type;
    this.opPhoto.date = new Date().getTime();
    this.opPhoto.note = "Profile Photo";
    this.opPhoto.ref = "Photo";
    this.opPhoto.id = '';
  }

  changePhoto(): void {
    // Trigger the file input click
    const fileInput = document.getElementById('opPhoto') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  resetPhoto(): void {
    this.opPhoto.file = undefined;
    this.opPhoto.filename = '';
    this.opPhoto.mimetype = '';
    this.opPhoto.date = null;

    // Reset the file input
    const fileInput = document.getElementById('opPhoto') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }
}
