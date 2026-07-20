import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FarmApiService } from "../../../_services/farm-api.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ObjectKeyCasingService } from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";
import { LandDataService } from "../../../_services/land-data.service";
import { CamisMapComponent } from "../../../_shared/camismap/camismap.component";
import { DocumentDetailComponent } from "../../../_shared/document/document-detail/document-detail.component";
import { LandbankDocumentSelectorComponent } from "../../../_shared/land-bank/landbank-document-selector/landbank-document-selector.component";
import {
  ContractCancellationRequest,
  CancelledRightRequest,
  ContractRenewalRequest
} from "../../../_shared/farm/interfaces";
import {WorkflowApiService} from "../../../_services/workflow-api.service";

@Component({
  selector: 'app-contract-renewal-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent, LandbankDocumentSelectorComponent],
  templateUrl: "contract-renewal-form.component.html"
})
export class ContractRenewalFormComponent implements OnInit {
  @Input() workflowId: string | null = null;
  @Input() farmId: string | null = null;
  @Input() landId: string | null = null;
  @Input() splitIndex: number = 0;
  @Input() data: ContractRenewalRequest | null = null;
  @Output() closeEditForm = new EventEmitter<boolean>();
  @Output() reload = new EventEmitter<boolean>();

  renewalModel: ContractRenewalRequest;
  farmLandRights: any[] = [];
  renewFarmLandRights: any[] = [];
  selectedFarmLandRights: any=null;
  farmStatusTypes: any[] = [];
  submitted = false;
  showEditRightForm: boolean = false;

  rightType = [
    { id: 1, name: 'Lease From State' },
    { id: 2, name: 'Lease From Private' },
    { id: 3, name: 'Private' },
    { id: 4, name: 'Contract Farming' },
    { id: 5, name: 'Sub-lease' },
  ];

  modificationReasonList: any[] = [];
  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    public keyCase: ObjectKeyCasingService,
    private landService: LandDataService,
    private wfApi:WorkflowApiService
  ) {
    this.api.getFarmStatusTypeList().subscribe(statusType => {
      this.keyCase.camelCase(statusType);
      this.farmStatusTypes = statusType;
    }, dialog.error);
    this.api.getAllModificationReasonList().subscribe(statusType => {
      this.keyCase.camelCase(statusType);
      this.modificationReasonList = statusType;
    }, dialog.error);
  }

  ngOnInit() {
    dialog.loading();

    if (this.workflowId != null) {
      if(this.data==null){
        this.wfApi.getLastWorkItem(this.workflowId).subscribe(workflow => {
          this.keyCase.camelCase(workflow);
          this.renewalModel=workflow.data;
          this.loadFarmLandsAndMap(this.renewalModel.farmId);
        }, dialog.error);
      }
      else{
        this.renewalModel=this.data;
        this.loadFarmLandsAndMap(this.renewalModel.farmId);
      }

      dialog.close();
    } else if (this.landId != null) {
      this.loadCancellationDataByLandId();
    } else if (this.farmId != null) {
      this.loadCancellationDataByFarmId();
    } else {
      dialog.close();
    }
  }
  private syncSelectedRights() {
    if (!this.renewalModel?.modifiedRight?.length){
      if(this.farmLandRights.length ==1){
        this.renewFarmLandRights=this.farmLandRights;
      }
      return;
    }
    for (const modified of this.renewalModel.modifiedRight) {
      const match = this.farmLandRights.find(fl =>
        fl.farmId?.toString() === modified.farmId?.toString() &&
        fl.landId?.toString() === modified.landId?.toString() &&
        fl.splitIndex === modified.splitIndex
      );
      if (match) {
        match.rights.rightFrom=modified.rightFrom;
        match.rights.rightTo=modified.rightTo;
        match.rights.yearlyRent=modified.yearlyRent;
        this.renewFarmLandRights.push(match);
      }
    }
  }
  private loadCancellationDataByLandId() {
    this.api.getFarmByLandId(this.landId).subscribe(res => {
      this.keyCase.camelCase(res);
      this.initRenewalModel(res);
      this.loadFarmLandsAndMap(res.farmLands?.length ? res.id : null);
    }, error => {
      dialog.close();
      dialog.error(error);
    });
  }

  private loadCancellationDataByFarmId() {
    this.api.getFarm(this.farmId).subscribe(res => {
      this.keyCase.camelCase(res);
      this.initRenewalModel(res);
      this.loadFarmLandsAndMap(this.farmId);
    }, error => {
      dialog.close();
      dialog.error(error);
    });
  }

  private initRenewalModel(farm: any) {
    this.renewalModel = {
      farmId: farm.id?.toString() || null,
      description: farm.description,
      modificationReason: {id:0,name:""},
      status: farm.status,
      locked: farm.locked,
      modifiedRight: [],
      supportiveDocument: [],
      farmLands: []
    };

    if (this.data) {
      this.renewalModel.modificationReason = this.data.modificationReason || null;
      this.renewalModel.modifiedRight = this.data.modifiedRight || [];
      this.renewalModel.supportiveDocument = this.data.supportiveDocument || [];
    }
  }

  private loadFarmLandsAndMap(farmIdToLoad: string | null) {
    if (!farmIdToLoad) {
      dialog.close();
      return;
    }

    this.api.getFarmLands(farmIdToLoad).subscribe(data => {
      this.keyCase.camelCase(data);
      this.farmLandRights = data.map((fl: any) => ({ ...fl, selected: false }));

      // After mapping, sync selections from existing cancelledRight
      this.syncSelectedRights();

      const splitGeomData: any[] = [];
      // Clear farmLands to avoid duplicates when reloading (important for workflow case)
      this.renewalModel.farmLands = [];

      for (let fl of this.farmLandRights) {
        const parts = fl.rights.geom.split(";");
        splitGeomData.push({ id: fl.upin, wkt: parts[parts.length - 1] });
        this.renewalModel.farmLands.push({
          farmId: fl.farmId.toString(),
          landId: fl.landId.toString(),
          splitIndex: fl.splitIndex,
          certificateDoc: fl.rights.certificateDocument,
          leaseContractDoc: fl.rights.contractDocument
        });
      }

      this.map.setSplitGeomsByWKT(splitGeomData);
      dialog.close();
    }, error => {
      dialog.close();
      dialog.error(error);
    });
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

  async saveRenewal():Promise<void>   {
    this.submitted = true;
    if (!await dialog.confirm('Are you sure you want to request this renewal request?')) {
      return;
    }
    const message = await dialog.prompt('Enter a message for the suppervisour:');
    if (message === "") {
      return
    }
    // Validation
    if (!this.renewalModel.description?.trim()) {
       await dialog.error("Please provide a modification reason details.");
return ;
    }


    if (this.renewFarmLandRights.length === 0) {
      await dialog.error("Please select at least one farm right to renewal.");
      return ;
    }

    // Build cancelledRight array
    this.renewalModel.modifiedRight = this.renewFarmLandRights.map(fl => ({
      farmId: fl.farmId.toString(),
      landId: fl.landId.toString(),
      splitIndex: fl.splitIndex,
      id:fl.farmId.toString(),
      rightFrom: fl.rights.rightFrom,
      rightTo: fl.rights.rightTo,
      rightType: fl.rights.rightType,
      yearlyRent: fl.rights.yearlyRent,
      landSectionArea: 0,
      commonTxtUid: null,
      geom: fl.geom,
      status: {id:3, name:""}
    } as CancelledRightRequest));

    dialog.loading();

    const saveObservable = this.workflowId
      ? this.api.requestContractModification(this.workflowId, this.renewalModel, message)
      : this.api.requestContractModification("", this.renewalModel, message);

    saveObservable.subscribe({
      next: () => {
        dialog.close();
        dialog.success("Contract Renewal saved successfully.");
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

  renewRight(right:any){
    this.selectedFarmLandRights=right;
    this.showEditRightForm=true;
  }

  doneEditRightForm(){
    if (this.renewFarmLandRights.length > 0) {
      let r=[];
      for(let f of this.renewFarmLandRights){
        if(f.farmId===this.selectedFarmLandRights.farmId && f.landId===this.selectedFarmLandRights.landId && f.splitIndex===this.selectedFarmLandRights.splitIndex){
          r.push(this.selectedFarmLandRights);
        }else{
          r.push(f);
        }
      }
      this.renewFarmLandRights=r;
    }else{
      this.renewFarmLandRights.push(this.selectedFarmLandRights);
    }
    this.selectedFarmLandRights=null;
    this.showEditRightForm=false;
  }
}
