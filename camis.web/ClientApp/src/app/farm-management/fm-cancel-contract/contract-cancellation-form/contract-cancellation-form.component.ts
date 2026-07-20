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
import { ContractCancellationRequest, CancelledRightRequest } from "../../../_shared/farm/interfaces";
import {WorkflowApiService} from "../../../_services/workflow-api.service";

@Component({
  selector: 'app-contract-cancellation-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, CamisMapComponent, DocumentDetailComponent, LandbankDocumentSelectorComponent],
  templateUrl: "contract-cancellation-form.component.html"
})
export class ContractCancellationFormComponent implements OnInit {
  @Input() workflowId: string | null = null;
  @Input() farmId: string | null = null;
  @Input() landId: string | null = null;
  @Input() splitIndex: number = 0;
  @Input() data: ContractCancellationRequest | null = null;
  @Output() closeEditForm = new EventEmitter<boolean>();
  @Output() reload = new EventEmitter<boolean>();

  cancellationModel: ContractCancellationRequest;
  farmLandRights: any[] = [];
  farmStatusTypes: any[] = [];
  submitted = false;

  rightType = [
    { id: 1, name: 'Lease From State' },
    { id: 2, name: 'Lease From Private' },
    { id: 3, name: 'Private' },
    { id: 4, name: 'Contract Farming' },
    { id: 5, name: 'Sub-lease' },
  ];

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
  }

  ngOnInit() {
    dialog.loading();

    if (this.workflowId != null) {
      if(this.data==null){
        this.wfApi.getLastWorkItem(this.workflowId).subscribe(workflow => {
          this.keyCase.camelCase(workflow);
          this.cancellationModel=workflow.data;
          this.loadFarmLandsAndMap(this.cancellationModel.id);
        }, dialog.error);
      }
      else{
        this.cancellationModel=this.data;
        this.loadFarmLandsAndMap(this.cancellationModel.id);
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
    if (!this.cancellationModel?.cancelledRight?.length) return;
    for (const cancelled of this.cancellationModel.cancelledRight) {
      const match = this.farmLandRights.find(fl =>
        fl.farmId?.toString() === cancelled.farmId?.toString() &&
        fl.landId?.toString() === cancelled.landId?.toString() &&
        fl.splitIndex === cancelled.splitIndex
      );
      if (match) match.selected = true;
    }
  }
  private loadCancellationDataByLandId() {
    this.api.getFarmByLandId(this.landId).subscribe(res => {
      this.keyCase.camelCase(res);
      this.initCancellationModel(res);
      this.loadFarmLandsAndMap(res.farmLands?.length ? res.id : null);
    }, error => {
      dialog.close();
      dialog.error(error);
    });
  }

  private loadCancellationDataByFarmId() {
    this.api.getFarm(this.farmId).subscribe(res => {
      this.keyCase.camelCase(res);
      this.initCancellationModel(res);
      this.loadFarmLandsAndMap(this.farmId);
    }, error => {
      dialog.close();
      dialog.error(error);
    });
  }

  private initCancellationModel(farm: any) {
    this.cancellationModel = {
      id: farm.id?.toString() || null,
      operatorId: farm.operatorId?.toString() || null,
      typeId: farm.typeId,
      activityId: farm.activityId?.toString(),
      investedCapital: farm.investedCapital,
      description: farm.description,
      otherTypeIds: farm.otherTypeIds,
      cancellationReason: "",
      status: farm.status,
      locked: farm.locked,
      cancelledRight: [],
      cancellationSupDoc: [],
      farmLands: [],
      wfid:'',
      date:new Date().toISOString().slice(0, 10),
      reason:''
    };

    if (this.data) {
      this.cancellationModel.reason = this.data.reason||"";
      this.cancellationModel.wfid = this.data.wfid||"";
      this.cancellationModel.date=new Date(this.data.date).toLocaleDateString('en-CA');
      this.cancellationModel.cancellationReason = this.data.cancellationReason || "";
      this.cancellationModel.cancelledRight = this.data.cancelledRight || [];
      this.cancellationModel.cancellationSupDoc = this.data.cancellationSupDoc || [];
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
      this.cancellationModel.farmLands = [];

      for (let fl of this.farmLandRights) {
        const parts = fl.rights.geom.split(";");
        splitGeomData.push({ id: fl.upin, wkt: parts[parts.length - 1] });
        this.cancellationModel.farmLands.push({
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

  toggleSelectAll(event: any) {
    const checked = event.target.checked;
    this.farmLandRights.forEach(fl => fl.selected = checked);
  }

  isAllSelected(): boolean {
    return this.farmLandRights.length > 0 && this.farmLandRights.every(fl => fl.selected);
  }

  async saveCancellation():Promise<void>   {
    this.submitted = true;
    if (!await dialog.confirm('Are you sure you want to reject this registration request?')) {
      return;
    }
    const message = await dialog.prompt('Enter a message for the clerk:');
    if (message === "") {
      return
    }
    // Validation
    if (!this.cancellationModel.cancellationReason?.trim()) {
       await dialog.error("Please provide a cancellation reason.");
    }

    const selectedRights = this.farmLandRights.filter(fl => fl.selected);
    if (selectedRights.length === 0) {
      await dialog.error("Please select at least one farm right to cancel.");
    }

    // Build cancelledRight array
    this.cancellationModel.cancelledRight = selectedRights.map(fl => ({
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
      ? this.api.requestContractCancellation(this.workflowId, this.cancellationModel, message)
      : this.api.requestContractCancellation("", this.cancellationModel, message);

    saveObservable.subscribe({
      next: () => {
        dialog.close();
        dialog.success("Contract cancellation saved successfully.");
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
