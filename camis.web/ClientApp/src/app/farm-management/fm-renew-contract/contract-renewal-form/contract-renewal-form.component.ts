import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmApiService} from "../../../_services/farm-api.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";
import {LandDataService} from "../../../_services/land-data.service";
import {CamisMapComponent} from "../../../_shared/camismap/camismap.component";
import {
  LandbankDocumentSelectorComponent
} from "../../../_shared/land-bank/landbank-document-selector/landbank-document-selector.component";
import {
  ContractBudgetYearRenewalRequest,
  ContractWarningRequest
} from "../../../_shared/farm/interfaces";
import {WorkflowApiService} from "../../../_services/workflow-api.service";
import {AuthorityRegistrarComponent} from "../../../_shared/farm/authority-registrar/authority-registrar.component";

@Component({
  selector: 'app-contract-renewal-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent, AuthorityRegistrarComponent],
  templateUrl: "contract-renewal-form.component.html"
})
export class ContractRenewalFormComponent implements OnInit {
  @Input() workflowId: string | null = null;
  @Input() farmLand: any | null = null;
  @Input() farmId: string | null = null;
  @Input() data: ContractBudgetYearRenewalRequest | null = null;
  @Output() closeEditForm = new EventEmitter<boolean>();
  @Output() reload = new EventEmitter<boolean>();

  warningModel: ContractBudgetYearRenewalRequest;

  farmStatusTypes: any[] = [];

  rightType = [
    {id: 1, name: 'Lease From State'},
    {id: 2, name: 'Lease From Private'},
    {id: 3, name: 'Private'},
    {id: 4, name: 'Contract Farming'},
    {id: 5, name: 'Sub-lease'},
  ];
  registrationAuthorities: any[] = [];
  registrationTypes: any[] = [];
  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    public keyCase: ObjectKeyCasingService,
    private landService: LandDataService,
    private wfApi: WorkflowApiService
  ) {
    this.api.getFarmStatusTypeList().subscribe(statusType => {
      this.keyCase.camelCase(statusType);
      this.farmStatusTypes = statusType;
    }, dialog.error);
    this.api.getAllRegistrationAuthorities().subscribe(registrationAuthorities => {
      this.keyCase.camelCase(registrationAuthorities);
      this.registrationAuthorities = registrationAuthorities
    }, dialog.error);
    this.api.getAllRegistrationTypes().subscribe(registrationTypes => {
      this.keyCase.camelCase(registrationTypes);
      this.registrationTypes = registrationTypes
    }, dialog.error);
  }

  ngOnInit() {
    dialog.loading();
    this.initWarningModel();
    if (this.workflowId != null) {
      if (this.data == null) {
        this.wfApi.getLastWorkItem(this.workflowId).subscribe(workflow => {
          this.keyCase.camelCase(workflow);
          this.warningModel = workflow.data;
          this.warningModel.date=new Date(this.warningModel.date).toLocaleDateString('en-CA');
        }, dialog.error);
      } else {
        this.warningModel = this.data;
        this.warningModel.date=new Date(this.data.date).toLocaleDateString('en-CA');
      }
      dialog.close();
      this.setMap(this.warningModel.renewRight);

    } else if (this.farmLand != null) {
      dialog.close();
      this.loadWarningData();
      this.setMap(this.warningModel.renewRight);
    } else {
      dialog.close();
    }
  }

  private loadWarningData() {

    if (this.farmLand != null) {
      const status = {
        id: this.farmLand.rights.status,
        name: ''
      }
      this.warningModel.farmId = this.farmLand.farmId;
      this.warningModel.landId = this.farmLand.landId;
      this.warningModel.splitIndex = this.farmLand.splitIndex;
      this.warningModel.renewRight = {
        id: this.farmLand.upin,
        farmId: this.farmLand.farmId,
        landId: this.farmLand.landId,
        splitIndex: this.farmLand.splitIndex,
        rightType: this.farmLand.rights.rightType,
        rightFrom: this.farmLand.rights.rightFrom,
        rightTo: this.farmLand.rights.rightTo,
        yearlyRent: this.farmLand.rights.yearlyRent,
        landSectionArea: this.farmLand.area,
        commonTxtUid: "",
        geom: this.farmLand.rights.geom,
        status: status
      };
    }
  }

  private initWarningModel() {
    this.warningModel = {
      id: "",
      farmId: this.farmId,
      landId: "",
      splitIndex: 0,
      budgetYear: new Date().getFullYear(),
      date: new Date().toISOString().slice(0, 10),
      remark: '',
      wfid: '',
      supportiveDocument: [],
      renewRight: null,
    };
  }


  // UI Helpers
  getFarmStatus(id: number): string {
    const status = this.farmStatusTypes.find(s => s.id === id);
    return status ? status.name : null;
  }

  getRightType(id: number): string {
    const type = this.rightType.find(t => t.id === id);
    return type ? type.name : null;
  }

  getArea(a: number) {
    return Math.round(a / 10) / 1000 + ' ha';
  }


 public  setMap(fl: any) {
    const splitGeomData: any[] = [];
    if(fl!=null){
      const parts = fl.geom.split(";");
      splitGeomData.push({id: fl.id, wkt: parts[parts.length - 1]});
      this.map.setSplitGeomsByWKT(splitGeomData);
    }

  }

  async saveRenewal(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to request this renewal registration?')) {
      return;
    }
    const message = await dialog.prompt('Enter a message for the Farm Data Supervisor:');


    dialog.loading();

    const saveObservable = this.workflowId
      ? this.api.requestContractRenewal(this.workflowId, this.warningModel, message)
      : this.api.requestContractRenewal("", this.warningModel, message);

    saveObservable.subscribe({
      next: () => {
        dialog.success("Contract renewal saved successfully.");
        this.closeForm();
        this.reload.emit(true);
      },
      error: (err) => {
        dialog.close();
        dialog.error(err);
      }
    });
  }

  closeForm() {
    this.closeEditForm.emit(true);
  }

}
