import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {FarmApiService} from '../../_services/farm-api.service';
import dialog from '../../_shared/dialog';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";

@Component({
  selector: 'app-fs-farm-registration',
  templateUrl: 'fs-farm-registration.component.html'
})
export class FsFarmRegistrationComponent implements OnInit {

  loading = true;

  workflowId: string;
  data: any;

  constructor (
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService
  ) {

  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => this.workflowId = params.workflowId, dialog.error)
      .add(this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        this.keyCase.camelCase(workItem);
        if (workItem) { this.data = workItem.data; }
        this.loading = false;
      }, dialog.error));
  }


  async onReject(): Promise<void> {
    const message = await dialog.prompt('Enter a message for the clerk (optional):');
    if (message === null) {
      return
    }

    this.loading = true;
    dialog.loading();

    this.api.rejectFarmRegistration(this.workflowId, null).subscribe(res => {
      if (res.success) {
        this.router.navigate(['supervisor/dashboard']).catch(dialog.error);
        return dialog.success('The registration request has been rejected successfully.');
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
    if (!await dialog.confirm('Are you sure you want to approve this registration request?')) {
      return;
    }

    const message = await dialog.prompt('Enter a message for the land admin (optional):');

    this.loading = true;
    dialog.loading();

    this.api.approveFarmRegistration(this.workflowId, message).subscribe(res => {
      if (res.success) {
        this.router.navigate(['supervisor/dashboard']).catch(dialog.error);
        return dialog.success('The registration request has been approved successfully.');
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
