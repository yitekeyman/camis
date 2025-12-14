import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from '../../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-fs-farm-modification',
  imports:[CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent],
  templateUrl: 'fs-farm-modification.component.html'
})
export class FsFarmModificationComponent implements OnInit {

  loading = true;

  workflowId: string;
  data: any;

  constructor (
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase:ObjectKeyCasingService
  ) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => this.workflowId = params['workflowId'], dialog.error)
      .add(this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.keyCase.camelCase(workItem);
        if (workItem) { this.data = workItem.data; }
        this.loading = false;
      }, dialog.error));
  }


  async onReject(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this modification request?')) {
      return;
    }
    const message = await dialog.prompt('Enter a message for the clerk (optional):');
    if (message === "") {
      await dialog.error('Please enter a message for the farm data registrar');
    }

    this.loading = true;
    dialog.loading();
    this.api.rejectFarmModification(this.workflowId, message).toPromise()
      .then(() => dialog.success('The modification request has been rejected successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

  async onApprove(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this modification request?')) {
      return;
    }

    this.loading = true;
    dialog.loading();
    this.api.approveFarmModification(this.workflowId, null).toPromise()
      .then(() => dialog.success('The modification request has been approved successfully.'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        this.loading = false;
        return dialog.error(err)
      });
  }

}
