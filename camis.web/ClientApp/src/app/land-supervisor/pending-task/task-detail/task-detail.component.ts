import {Component, OnInit, ElementRef, ViewChild} from '@angular/core';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import {Validators, FormBuilder, FormGroup, AbstractControl, FormArray, FormControl} from '@angular/forms';
import swal from 'sweetalert2';

import {DialogService} from '../../../_shared/dialog/dialog.service';
import {LandDataService} from '../../../_services/land-data.service';
import {LandBankWorkFlowLand, LandBankWorkItem} from '../../../_shared/land-bank/land.model';
import {CamisMapComponent} from '../../../_shared/camismap/camismap.component';
import {WorkflowApiService} from '../../../_services/workflow-api.service';
import dialog from "../../../_shared/dialog";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit {

  wfid: string;
  workflowLand: LandBankWorkFlowLand[] = [];
  userWorkItems: LandBankWorkItem[] = [];

  messageForm: FormGroup;
  message: AbstractControl;
  rejectMessageForm: FormGroup;
  rejectMessage: AbstractControl;
  cancelMessageForm: FormGroup;
  cancelMessage: AbstractControl;

  rejectNote = '';
  cancelNote = '';

  @ViewChild('btnClose') btnClose: ElementRef;
  @ViewChild('rejectBtnClose') rejectBtnClose: ElementRef;
  @ViewChild('cancelBtnClose') cancelBtnClose: ElementRef;
  @ViewChild('camis_map') map: CamisMapComponent;

  constructor(private router: Router, private landService: LandDataService, private activeRoute: ActivatedRoute,
              public formBuilder: FormBuilder, private dialog: DialogService, private workflowservice: WorkflowApiService, private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit() {

    this.activeRoute.params.subscribe(params => {
      this.wfid = params.wfid;
      this.getLandDetail();
    });

// form group to retrieve data from the approve modal
    this.messageForm = this.formBuilder.group({
      message: ['', Validators.compose([Validators.required])],
    });

    this.message = this.messageForm.controls['message'];

// form group to retrieve data from the reject modal
    this.rejectMessageForm = this.formBuilder.group({
      rejectMessage: ['', Validators.compose([Validators.required])]
    });

    this.rejectMessage = this.rejectMessageForm.controls['rejectMessage'];

// form group to retrieve data from the cancel modal
    this.cancelMessageForm = this.formBuilder.group({
      cancelMessage: ['', Validators.compose([Validators.required])]
    });

    this.cancelMessage = this.cancelMessageForm.controls['cancelMessage'];

  }

  getLandDetail() {
    dialog.loading();
    this.landService.GetUserWorkItems().subscribe(data => {
      this.keyCase.camelCase(data);
      for (const workItem of data) {
        if (workItem.wfid === this.wfid) {
          this.userWorkItems = workItem;
          if (workItem.workFlowType == 7) {
            this.landService.GetSplitWorkItem(this.wfid)
              .subscribe(wi => {
                const wkts: String[] = [];
                wi.data.geoms.forEach(g => {
                  if (g) {
                    const parts = g.split(';');
                    wkts.push(parts[parts.length - 1]);
                  }
                });
                this.map.setSplitGeomsByWKT(wkts);

              });
          }
        }
      }
      dialog.close();
    }, e => {
      dialog.close();
    });

    this.landService.GetWorkFlowLand(this.wfid).subscribe(data => {
      this.keyCase.camelCase(data);
      this.workflowLand = data;
      if (data.upins && data.upins.length > 0) {
        if (data.parcels[data.upins[0]]) {
          const wkt: String = data.parcels[data.upins[0]].geometry;
          if (wkt) {
            const parts = wkt.split(';');
            this.map.setWorkFlowGeomByWKT(parts[parts.length - 1]);
          }
        }
      }
      dialog.close();
    }, e => {
      dialog.close();
    });
  }

  backButton() {
    this.router.navigate([`land-supervisor/pending-task`]);
  }


  sendMessage() {

    this.btnClose.nativeElement.click();

    if (this.userWorkItems['workFlowType'] === 4) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();
      const registerNote = this.message.value;

      this.landService.ApproveRegistration(this.wfid, registerNote).subscribe(
        () => {

          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'The land identified successfully!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    } else if (this.userWorkItems['workFlowType'] === 7) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();
      const prepareNote = this.message.value;

      this.landService.ApprovePreparation(this.wfid, prepareNote).subscribe(
        () => {
          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'The land prepared successfully!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    }
  }

  rejectApproval() {

    this.rejectBtnClose.nativeElement.click();

    if (this.userWorkItems['workFlowType'] === 4) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();

      this.rejectNote = this.rejectMessage.value;

      this.landService.RejectRegistrationRequest(this.wfid, this.rejectNote).subscribe(
        () => {

          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'You successfully rejected clerk\'s land approval request!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    } else if (this.userWorkItems['workFlowType'] === 7) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();

      this.rejectNote = this.rejectMessage.value;

      this.landService.RejectPreparationRequest(this.wfid, this.rejectNote).subscribe(
        () => {

          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'You successfully rejected clerk\'s land preparation request!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    }

  }

  cancelRequest() {

    this.cancelBtnClose.nativeElement.click();

    if (this.userWorkItems['workFlowType'] === 4) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();

      this.cancelNote = this.cancelMessage.value;

      this.landService.CancelRegistrationRequest(this.wfid, this.cancelNote).subscribe(
        () => {

          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'Land approval request canceled successfully!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    } else if (this.userWorkItems['workFlowType'] === 7) {
      swal({allowOutsideClick: false});
      swal.disableButtons();
      swal.showLoading();

      this.cancelNote = this.cancelMessage.value;

      this.landService.CancelPreparationRequest(this.wfid, this.cancelNote).subscribe(
        () => {

          // success
          swal.close();

          swal({
            position: 'center',
            type: 'success',
            title: 'Land preparation request canceled successfully!',
            showConfirmButton: false,
            timer: 1500
          }).then(
            () => {
              this.router.navigate(['/land-supervisor/land-dashboard']);
            });
        },
        (err) => {
          swal.close();
          // error alert
          this.dialog.error(err);
        }
      );
    }

  }

}
