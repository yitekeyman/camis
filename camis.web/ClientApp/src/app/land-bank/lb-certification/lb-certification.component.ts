import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocument} from '../../_shared/document/interfaces';
import {ISingleDocumentSelectorChangeEvent} from '../../_shared/document/single-document-selector/interfaces';
import dialog from '../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmDetailComponent} from "../../_shared/farm/farm-detail/farm-detail.component";
import {
  SingleDocumentSelectorComponent
} from "../../_shared/document/single-document-selector/single-document-selector.component";
import {LandDataService} from "../../_services/land-data.service";

@Component({
  selector: 'app-lb-certification',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FarmDetailComponent, SingleDocumentSelectorComponent],
  templateUrl: 'lb-certification.component.html'
})
export class LbCertificationComponent implements OnInit {

  loading = true;

  workflowId: string;
  data: any;
  userWorkItems: any = null;
  workItem: any = null;
  certification: { doc: IDocument | null } = {doc: null};
  leaseContract: { doc: IDocument | null } = {doc: null};

  constructor(
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
    private landService:LandDataService
  ) {
  }

  ngOnInit(): void {
    dialog.loading();
    this.ar.params.subscribe(params => this.workflowId = params['workflowId'], dialog.error)
      .add(this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        if (workItem) {
          this.keyCase.camelCase(workItem);
          this.getWorkflow(workItem.workflowId);
          this.data = workItem.data;
        }

        this.loading = false;
        dialog.close();
      }, dialog.error));
  }

getWorkflow(workflowId: string) {
  this.landService.GetUserWorkItems().subscribe(data => {
    this.keyCase.camelCase(data);
    for (const workItem of data) {
      if (workItem.wfid === workflowId) {
        this.userWorkItems = workItem;

      }
    }
  });
}
  chooseCertificationDocument(event: any): void {
    const document = event.document || (event as ISingleDocumentSelectorChangeEvent)?.document;
    if (document) {
      this.certification.doc = document;
    }
  }

  chooseLeaseContractDocument(event: any): void {
    const document = event.document || (event as ISingleDocumentSelectorChangeEvent)?.document;
    if (document) {
      this.leaseContract.doc = document;
    }
  }


  clear() {
    this.certification = {doc: null};
    this.leaseContract = {doc: null};
  }


  async onCertify():Promise<void> {
    if (!await dialog.confirm('Are you sure you want to certify this parcel?')) {
      return;
    }
    const body = this.data;

    // note: this logic works only for single land per investment
    body.farmLands = [{
      farmId: body.farmLands[0].farmId,
      landId: body.farmLands[0].landId,
      splitIndex:body.farmLands[0].splitIndex,
      certificateDoc: this.certification.doc,
      leaseContractDoc: this.leaseContract.doc,
    }];

    dialog.loading();
    this.api.certifyLandAssignment(this.workflowId, body, null).toPromise()
      .then(() => dialog.success('The land has been certified successfully..'))
      .then(() => this.router.navigate(['default/pending-task']).catch(dialog.error))
      .catch(err => {
        return dialog.error(err)
      });
  }

}
