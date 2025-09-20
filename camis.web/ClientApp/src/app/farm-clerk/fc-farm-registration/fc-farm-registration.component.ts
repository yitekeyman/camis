import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IAuthorityRegistration} from '../../_shared/farm/authority-registrar/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-fc-farm-registration',
  templateUrl: 'fc-farm-registration.component.html'
})
export class FcFarmRegistrationComponent implements OnInit {

  workflowId: string | null = null;

  step = 0;
  howOperator: 'NEW' | 'EXISTING' | null = 'NEW';

  farmOperatorTypes: any[] = [];
  farmOperatorOrigins: any[] = [];
  farmTypes: any[] = [];
  registrationAuthorities: any[] = [];
  registrationTypes: any[] = [];

  opId = null;
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

  opVentures: string[] = [];
  opVentureResults: any[] = []; // only in UI
  opVentureResultSelections: string[] = []; // only in UI

  searching = true;
  operatorTerm = '';
  totalOperators = 0;
  operators: any[] = [];
  selectedOperatorId = null;

  frId = null;
  frType: string | null = null;
  frOtherTypeIds: number[] = [];
  frInvestedCapital = '';
  frDescription = '';
  frRegistrations: IAuthorityRegistration[] = [];

  plan = {
    rootActivity: {
      name: 'Business Plan',
      description: 'Commercial Agriculture Business Plan',
      weight: 1,
      schedules: [],
      activityPlanDetails: [],
      children: []
    },
    documents: []
  };

  constructor (
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {
  }

  ngOnInit(): void {
    this.api.getAllFarmOperatorTypes().subscribe(farmOperatorTypes => {this.keyCase.camelCase(farmOperatorTypes);this.farmOperatorTypes = farmOperatorTypes}, dialog.error);
    this.api.getAllFarmOperatorOrigins().subscribe(farmOperatorOrigins =>{this.keyCase.camelCase(farmOperatorOrigins); this.farmOperatorOrigins = farmOperatorOrigins}, dialog.error);
    this.api.getAllFarmTypes().subscribe(farmTypes => {this.keyCase.camelCase(farmTypes);this.farmTypes = farmTypes}, dialog.error);
    this.api.getAllRegistrationAuthorities().subscribe(registrationAuthorities => {this.keyCase.camelCase(registrationAuthorities);this.registrationAuthorities = registrationAuthorities}, dialog.error);
    this.api.getAllRegistrationTypes().subscribe(registrationTypes => {this.keyCase.camelCase(registrationTypes);this.registrationTypes = registrationTypes}, dialog.error);

    this.ar.params.subscribe(params => {
      this.workflowId = params.workflowId ? params.workflowId : null;

      this.loadOperators(0)
        .add(() => {
          if (this.workflowId) { // a started workflow

            // fill data...
            this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
              this.keyCase.camelCase(workItem);
              const f = workItem.data;
              if (!f) { return; }
              this.keyCase.camelCase(f);

              // step 1 [& step 2]
              if (f.operatorId) {
                this.howOperator = 'EXISTING';
                this.selectedOperatorId = f.operatorId;
              } else {
                this.howOperator = 'NEW';
              }

              // [step 2]
              const o = f.operator;
              if (o) {
                this.opId = o.id || null;
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

                this.opVentures = o.ventures || []
              }

              // step 3
              this.frId = f.id || null;
              this.frType = f.typeId;
              this.frOtherTypeIds = f.otherTypeIds || [];
              this.frInvestedCapital = f.investedCapital;
              this.frDescription = f.description;
              this.frRegistrations = f.registrations;

              // step 4
              if (f.activityPlan) {
                if (!f.activityPlan.rootActivity) {
                  f.activityPlan.rootActivity = {
                    name: 'Business Plan',
                    description: 'Commercial Agriculture Business Plan',
                    weight: 1,
                    schedules: [],
                    activityPlanDetails: [],
                    children: []
                  }
                }
                this.plan = f.activityPlan;
              }
            }, dialog.error);
          } else { // a new workflow to be started

            // ensure step...
            this.ar.queryParams.subscribe(query => {
              if (query.step == '3' && query.operatorId) {
                this.api.getFarmOperator(query.operatorId).subscribe(operator => {
                  this.howOperator = 'EXISTING';
                  this.selectedOperatorId = operator.id;
                  this.step = 3;
                }, dialog.error);
              }
            }, dialog.error);
          }

          if (!this.step) { this.step = 1; }
        });
    }, dialog.error);
  }


  loadOperators(skip = this.operators.length, take = 10) {
    return this.api.searchFarmOperators(this.operatorTerm, skip, take).subscribe(operatorsPaginator => {
this.keyCase.camelCase(operatorsPaginator);
      this.totalOperators = operatorsPaginator.totalSize
      this.operators = this.operators.slice(0, skip).concat(operatorsPaginator.items);
      this.searching = false;
    }, dialog.error);
  }


  searchVentures(term: string): void {
    this.api.searchFarmOperators(term, 0, 25)
      .subscribe(result => {this.keyCase.camelCase(result);this.opVentureResults = result.items}, dialog.error)
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


  async onSave(): Promise<void> {
    const message = await dialog.prompt('Enter a saving message for later reference (optional):');
    if (message === null) {
      return
    }

    const body = this._parseRequestBody();

    const req = this.workflowId ?
      this.api.saveFarmRegistration(this.workflowId, body, message) :
      this.api.saveNewFarmRegistration(body, message);

    req.subscribe(res => {
      if (res.success) {
        this.router.navigateByUrl('clerk/dashboard').catch(dialog.error);
        return dialog.success('You progress has been saved. You can come back later to finish and submit it.');
      } else {
        return dialog.error(res.message);
      }
    }, err => {
      return dialog.error(err)
    });
  }

  async cancel(): Promise<void> {
    if (this.workflowId) {
      if (!await dialog.confirm('Are you sure you want to cancel this workflow? This is irreversible.')) {
        return;
      }

      dialog.loading();
      this.api.cancelFarmRegistration(this.workflowId, null).subscribe(res => {
        this.keyCase.camelCase(res);
        if (res.success) {
          this.router.navigateByUrl(`clerk/dashboard`).catch(dialog.error);
          return dialog.success('The workflow has been cancelled successfully.');
        } else {
          return dialog.error(res);
        }
      }, err => {
        return dialog.error(err)
      });
    } else {
      this.router.navigateByUrl(`clerk/dashboard`).catch(dialog.error);
    }
  }

  dumpSubmit(e: any): boolean {
    e.preventDefault();
    return false;
  }

  async onSubmit(e: any): Promise<void> {
    this.dumpSubmit(e);

    const message = await dialog.prompt('Enter a message for the supervisor (optional):');
    if (message === null) {
      return
    }

    dialog.loading();

    const body = this._parseRequestBody();

    const req = this.workflowId ?
      this.api.requestFarmRegistration(this.workflowId, body, message) :
      this.api.requestNewFarmRegistration(body, message);

    req.subscribe(res => {
      this.keyCase.camelCase(res);
      if (res.success) {
        this.router.navigateByUrl('clerk/dashboard').catch(dialog.error);
        return dialog.success('Your registration request has been sent to the supervisor successfully.');
      } else {
        this.keyCase.camelCase(body);
        return dialog.error(res.message);
      }
    }, err => {
      this.keyCase.camelCase(body);
      return dialog.error(err);
    });
  }

  private _parseRequestBody(): any {
    const body: any = {
      id: this.frId,
      typeId: Number(this.frType),
      otherTypeIds: this.frOtherTypeIds.map(str => Number(str)),
      investedCapital: Number(this.frInvestedCapital),
      description: this.frDescription,
      registrations: this.frRegistrations,
      activityPlan: this.plan,
    };

    switch (this.howOperator) {
      case 'NEW':
        body.operator = {
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

          ventures: this.opType == '6' ? this.opVentures : []
        };
        break;
      case 'EXISTING':
        body.operatorId = this.selectedOperatorId;
        break;
    }

    this.keyCase.PascalCase(body);
    return body;
  }

}
