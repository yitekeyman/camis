import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FarmApiService} from '../../_services/farm-api.service';
import {ObjectKeyCasingService} from '../../_services/object-key-casing.service';
import {IDocument} from '../../_shared/document/interfaces';
import {ISingleDocumentSelectorChangeEvent} from '../../_shared/document/single-document-selector/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-lci-certification',
  templateUrl: 'lci-certification.component.html'
})
export class LciCertificationComponent implements OnInit {

  loading = true;

  workflowId: string;
  data: any;

  certification: { doc: IDocument | null } = { doc: null };
  leaseContract: { doc: IDocument | null } = { doc: null };

  constructor (
    private api: FarmApiService,
    private router: Router,
    private ar: ActivatedRoute,
    private keyCase: ObjectKeyCasingService,
  ) {
  }

  ngOnInit(): void {
    this.ar.params.subscribe(params => this.workflowId = params.workflowId, dialog.error)
      .add(this.api.getLastWorkItem(this.workflowId).subscribe(workItem => {
        if (workItem) {
          this.keyCase.camelCase(workItem.data);
          this.data = workItem.data;
        }
        this.loading = false;
      }, dialog.error));
  }


  chooseCertificationDocument($event: ISingleDocumentSelectorChangeEvent): void {
    if ($event.document || $event.document === null) {
      this.certification.doc = $event.document;
    }
  }

  chooseLeaseContractDocument($event: ISingleDocumentSelectorChangeEvent): void {
    if ($event.document || $event.document === null) {
      this.leaseContract.doc = $event.document;
    }
  }


  clear() {
    this.certification = { doc: null };
    this.leaseContract = { doc: null };
  }


  onCertify() {
    const body = this.data;

    // note: this logic works only for single land per investment
    body.farmLands = [{
      farmId: body.farmLands[0].farmId,
      landId: body.farmLands[0].landId,

      certificateDoc: this.certification.doc,
      leaseContractDoc: this.leaseContract.doc,
    }];

    this.loading = true;
    dialog.loading();
    this.api.certifyLandAssignment(this.workflowId, body, null).subscribe(res => {
      if (res.success) {
        this.router.navigate(['land-certificate-issuer/dashboard']).catch(dialog.error);
        return dialog.success('The land has been certified successfully.')
      } else {
        this.loading = false;
        return dialog.error(res)
      }
    }, err => {
      this.loading = false;
      return dialog.error(err)
    });
  }

}
