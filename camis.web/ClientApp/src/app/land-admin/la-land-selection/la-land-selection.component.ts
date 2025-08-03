import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FarmApiService} from '../../_services/farm-api.service';
import {IWaitLandAssignmentRequest} from '../../_shared/farm/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-la-land-selection',
  templateUrl: 'la-land-selection.component.html'
})
export class LaLandSelectionComponent implements OnInit {

  loading = true;

  farmId: string;
  workflowId: string;
  data: { [key: string]: any } & IWaitLandAssignmentRequest;

  yearlyLeaseRate?: number;
  landArea?: number;

  upin = '';
  allUPINs: string[] = [];

  constructor (
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute
  ) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => {
      this.farmId = params.farmId;
      this.workflowId = params.workflowId;

      let req = this.api.getFarm(this.farmId);
      if (!this.farmId && this.workflowId) {
        req = this.api.getLastWorkItem(this.workflowId);
      }

      req.subscribe(resp => {
        this.data = !this.farmId && this.workflowId && resp.data || resp;
        this.data.landTransferRequest = {
          farmer: this.data.operator,
          landID: null,
          leaseFrom: null,
          leaseTo: null,
          right: null,
          yearlyLease: null,
          landSectionArea: null,
        };

        if (this.data.landTransferRequest.farmer) {
          this.loading = false;
        } else {
          this.api.getFarmOperator(this.data.operatorId).subscribe(operator => {
            this.data.landTransferRequest.farmer = operator;
            this.loading = false;
          }, dialog.error);
        }
      }, dialog.error);

      this.api.getUPINs().subscribe(resp => this.allUPINs = resp, dialog.error);
    }, dialog.error);
  }


  loadLand(): void {
    if (this.upin) {
      this.api.getLandByUpin(this.upin).subscribe(
        resp => {
          if (resp && resp.area) {
            this.landArea = resp.area
          }

          if (resp && resp.landID) {
            this.data.landTransferRequest.landID = resp.landID;
          } else {
            this.data.landTransferRequest.landID = null;
          }
        },
        error => {
          dialog.error(error);
          this.data.landTransferRequest.landID = null;
        });

    } else {
      this.data.landTransferRequest.landID = null;
    }
  }

  async selectLand(): Promise<void> {
    const message = await dialog.prompt('Enter a message to display while waiting for NRLAIS (optional):');
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
    }

    this.loading = true;
    dialog.loading();

    let req = this.api.newWaitLandAssignment(this.data, message);
    if (!this.farmId && this.workflowId) {
      req = this.api.waitLandAssignment(this.workflowId, this.data, message);
    }

    req.subscribe(res => {
      if (res.success) {
        this.router.navigateByUrl('land-admin/dashboard').catch(dialog.error);
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

}
