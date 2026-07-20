import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FarmApiService} from '../../_services/farm-api.service';
import {IWaitLandAssignmentRequest} from '../../_shared/farm/interfaces';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {CamisMapComponent} from "../../_shared/camismap/camismap.component";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {forEach} from "ol/geom/flat/segments";
import {WorkflowApiService} from "../../_services/workflow-api.service";
import {LandDataService} from "../../_services/land-data.service";

@Component({
  selector: 'app-land-selection',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, CamisMapComponent],
  templateUrl: 'land-selection.component.html'
})
export class LandSelectionComponent implements OnInit {

  loading = true;

  farmId: string;
  workflowId: string;
  data: any=null;

  yearlyLeaseRate?: number;
  landArea?: number;
  selectedLandPart?: number;
  landPart: any[] = [];
  workItem: any = null;
  workFlowItem: any = null;
  selectedUPID = "";
  userWorkItem: any = null;
  upin = '';
  allUPINs = [];
  public loginRole = 0;
  rightType = [
    {id: 1, name: 'Lease From State'},
    {id: 2, name: 'Lease From Private'},
    {id: 3, name: 'Private'},
    {id: 4, name: 'Contract Farming'},
    {id: 5, name: 'Sub-lease'},
  ];
  selectedUPINArea: any = null;
  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    public keyCase: ObjectKeyCasingService,
    private wfApi: WorkflowApiService,
    private landService: LandDataService
  ) {
    this.loginRole = parseInt(localStorage.getItem('role'), 10);
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => {
      this.farmId = params['farmId'];
      this.workflowId = params['workflowId'];
      let req = null;
      if (this.farmId) {
        req = this.api.getFarm(this.farmId);
      } else if (!this.farmId && this.workflowId) {
        req = this.api.getLastWorkItem(this.workflowId);
      }

      req.subscribe(resp => {
        dialog.loading();
        this.keyCase.camelCase(resp);
        if (this.workflowId) {
          this.workItem = resp;
          this.wfApi.getWorkflow(this.workflowId).subscribe(workflow => {
            this.keyCase.camelCase(workflow);
            this.workFlowItem = workflow;
            this.api.getUserWorkItem(this.workflowId).subscribe(uwi => {
              this.keyCase.camelCase(uwi);
              this.userWorkItem = uwi;
            })
          }, dialog.error);
        }
        this.data = !this.farmId && this.workflowId && resp.data || resp;
        if (this.data.landTransferRequest == null) {
          this.data.landTransferRequest = {
            farmer: this.data['farmer'],
            landID: null,
            landPart: 0,
            leaseFrom: null,
            leaseTo: null,
            right: null,
            yearlyLease: null,
            landSectionArea: null,
            farmId:this.data['id']
          };
        } else {
          dialog.loading();
          this.selectedLandPart=this.data.landTransferRequest.landPart;
          this.landService.GetLand(this.data.landTransferRequest.landID).subscribe(land => {

            this.selectedUPID = land.Upins[0];
            this.selectedUPINArea = land.Area;
            for (let p of land.LandSplit) {
              if (this.data.landTransferRequest.landPart == p.Id) {
                this.selectedUPID = this.selectedUPID + "-" + p.Indexes;
                this.selectedUPINArea = p.Area;
              }
            }
          })
        }
        dialog.loading();
        this.api.getFarm(this.data['id']).subscribe(res => {
          this.keyCase.camelCase(res);
          this.data['status'] = res.status;
        })
        if (this.data.landTransferRequest.farmer) {
          this.loading = false;
        } else {
          dialog.loading();
          this.api.getFarmOperator(this.data['operatorId']).subscribe(operator => {
            this.data.landTransferRequest.farmer = operator;


            this.loading = false;
          }, dialog.error);
        }
        dialog.close();
      }, dialog.error);
      dialog.loading();
      this.api.getUPINs().subscribe(resp => {
        this.keyCase.camelCase(resp);
        this.allUPINs = resp
      }, dialog.error);

      //dialog.close();
    }, dialog.error);

  }


  loadLand(): void {
    if (this.upin) {
      this.api.getLandByUpin(this.upin).subscribe(
        resp => {
          this.keyCase.camelCase(resp);
          if (resp && resp.area) {
            this.landArea = resp.area
          }

          if (resp && resp.landID) {
            this.data.landTransferRequest.landID = resp.landID;
          } else {
            this.data.landTransferRequest.landID = null;
          }
          if (resp && resp.landSplit?.length > 0) {
            this.landPart = [];
            for (let ls of resp.landSplit) {
              if (ls.status == 2)
                this.landPart.push(ls);
            }
          }
          this.map.setNrlaisParcel(this.upin);
        },
        error => {
          dialog.error(error);
          this.data.landTransferRequest.landID = null;
        });

    } else {
      this.data.landTransferRequest.landID = null;
    }
  }

  selectLandPart() {
    for (let ls of this.landPart) {
      if (this.selectedLandPart == ls.id) {
        this.landArea = ls.area;
        this.map.setWorkFlowGeomByWKT(ls.geom);
      }
    }
  }

  async selectLand(): Promise<void> {
    const message = await dialog.prompt('Enter a message to display while waiting for experts (optional):');
    if (message === null) {
      return
    }

    if (this.data && this.data.landTransferRequest) {

      this.data.landTransferRequest.yearlyLease = this.yearlyLeaseRate * this.landArea * 0.0001;

      if (this.data.landTransferRequest.right != null) {
        this.data.landTransferRequest.right = Number(this.data.landTransferRequest.right);
      }

      if (this.data.landTransferRequest.right == 4) {
        this.data.landTransferRequest.yearlyLease = null;
      }

      if (this.data.landTransferRequest.right != 5) {
        this.data.landTransferRequest.landSectionArea = null;
      }
      if (this.selectedLandPart > 0) {
        this.data.landTransferRequest.landPart = this.selectedLandPart;
      }
    }

    this.loading = true;
    dialog.loading();

    let req = this.api.newWaitLandAssignment(this.data, message);
    if (!this.farmId && this.workflowId) {
      req = this.api.waitLandAssignment(this.workflowId, this.data, message);
    }

    req.subscribe(res => {
      if (res.success) {
        this.router.navigateByUrl('default/pending-task').catch(dialog.error);
        return dialog.success("The request has been sent to NRLAIS successfully.");
      } else {
        this.loading = false;
        return dialog.error(res);
      }
    }, err => {
      this.loading = false;
      return dialog.error(err);
    });
  }

  protected readonly Number = Number;

  getRightType(id: number): string {
    for (let l of this.rightType) {
      if (id == l.id) {
        return l.name;
      }
    }
    return null;
  }

  getArea(id: number): string {
    return Math.round(id / 10) / 1000 + ' Hr';
  }

  async approveRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this task?')) {
      return;
    }

    const message = await dialog.prompt('Enter a note (optional):');
    dialog.loading();

    this.api.approveLandAssignment(this.workflowId, message).toPromise()
      .then(() => dialog.success('The parcel allocation has been approved successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        return dialog.error(err)
      });
  }

  async rejectRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reject this task?')) {
      return;
    }
    let msgQue = '';
    const message = await dialog.prompt("Enter a rejection note for land bank Administrator:");
    if (message === ""){
      await dialog.error("Putting note is required");
      return;
    }else{
      dialog.loading();

      this.api.rejectLandAssignment(this.workflowId, message).toPromise()
        .then(() => dialog.success('The parcel allocation has been rejected successfully.'))
        .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
        .catch(err => {
          return dialog.error(err)
        });
    }
  }

  async cancelRequest(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to cancel this request?')) {
      return;
    }
    const message = await dialog.prompt("Enter a task cancellation reason");
    if (message === '') {
      return;
    }
    dialog.loading();

  }


  public editParcelAllocation() {

  }
}
