import {Component, EventEmitter, Input, OnInit, Output} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {FarmApiService} from "../../../_services/farm-api.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {WorkflowApiService} from "../../../_services/workflow-api.service";
import {LandDataService} from "../../../_services/land-data.service";
import dialog from "../../../_shared/dialog";
import {CamisMapComponent} from "../../../_shared/camismap/camismap.component";
import {
  LandbankDocumentSelectorComponent
} from "../../../_shared/land-bank/landbank-document-selector/landbank-document-selector.component";
import {
  SimpleFarmDetailsComponent
} from "../../../_shared/farm/farm-detail/simple-farm-details/simple-farm-details.component";
import {DocumentDetailComponent} from "../../../_shared/document/document-detail/document-detail.component";

@Component({
  selector: "app-farm-renewal-details",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, SimpleFarmDetailsComponent, DocumentDetailComponent],
  templateUrl: "./contract-renewal-details.component.html",
})

export class ContractRenewalDetailsComponent implements OnInit {

  @Input() farmId:string|null=null;
  @Input() landId:string|null=null;
  @Input() splitIndex:number;
  @Input() farm:any|null=null;
  @Output() closeEditForm = new EventEmitter<boolean>();

  warningModel:any = null;
  loadWarning: boolean = false;
  loadWf:boolean = false;
  error: any;
  farmLands:any[]=[];
  farmLand:any=null;

  rightType = [
    {id: 1, name: 'Lease From State'},
    {id: 2, name: 'Lease From Private'},
    {id: 3, name: 'Private'},
    {id: 4, name: 'Contract Farming'},
    {id: 5, name: 'Sub-lease'},
  ];
  constructor( private api: FarmApiService,
               private router: Router,
               private ar: ActivatedRoute,
               public keyCase: ObjectKeyCasingService,
               private wfApi: WorkflowApiService,
               private landService: LandDataService) {

  }

  ngOnInit() {
    this.getWarningDetails();
  }

  getWarningDetails(){
    dialog.loading();
    this.loadWarning = true;
    this.loadWf=true;
    this.api.GetRightWarning(this.farmId, this.landId, this.splitIndex).subscribe(res=>{
      this.keyCase.camelCase(res);
      this.warningModel = res;
      this.api.getFarmLands(this.farmId).subscribe(res2=>{
        this.keyCase.camelCase(res2);
        this.farmLands = res2;
        for(let l of this.farmLands){
          if(l.landId === this.landId && this.splitIndex == l.splitIndex){
            this.farmLand = l;
          }
        }
      })
      this.loadWarning = false;
      dialog.close();
    }, dialog.error)
  }

  closeForm() {
    this.closeEditForm.emit(true);
  }
  getRightType(id: number): string {
    const type = this.rightType.find(t => t.id === id);
    return type ? type.name : null;
  }

  getArea(a: number) {
    return Math.round(a / 10) / 1000 + ' ha';
  }
}
