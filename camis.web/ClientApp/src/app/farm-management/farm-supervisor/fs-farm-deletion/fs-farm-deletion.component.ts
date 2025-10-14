import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../../_services/farm-api.service';
import dialog from "../../../_shared/dialog";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {FarmDetailComponent} from "../../../_shared/farm/farm-detail/farm-detail.component";

@Component({
  selector: 'app-fs-farm-deletion',
  imports: [CommonModule, ReactiveFormsModule, FarmDetailComponent],
  templateUrl: 'fs-farm-deletion.component.html'
})
export class FsFarmDeletionComponent implements OnInit {

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
    this.ar.params.subscribe(params => this.workflowId = params['workflowId'])
      .add(this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.keyCase.camelCase(workItem);
        if (workItem) { this.data = workItem.data; }
        this.loading = false;
      }));
  }


  async onReject(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reject this deletion request?')) {
      return;
    }

    this.loading = true;
    dialog.loading();

    this.api.rejectFarmDeletion(this.workflowId, null).subscribe(res => {
      if (res.success) {
        this.router.navigate(['default/pending-task']).catch(dialog.error);
        return dialog.success('The deletion request has been rejected successfully.');
      } else {
        this.loading = false;
        return dialog.error(res);
      }
    }, err => {
      this.loading = false;
      return dialog.error(err);
    });
  }

  async onApprove(): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to approve this deletion request?')) {
      return;
    }

    this.loading = true;
    dialog.loading();

    this.api.approveFarmDeletion(this.workflowId, null).subscribe(res => {
      if (res.success) {
        this.router.navigate(['default/pending-task']).catch(dialog.error);
        return dialog.success('The deletion request has been approved successfully.');
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
