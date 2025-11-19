import {Component, EventEmitter, OnInit, Output} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AdminServices} from "../../../_services/admin.Services";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";
import {DetailAuditLogComponent} from "../../activityLog/detialAuditLog/detail-audit-log.component";

@Component({
  selector: "app-profile-details",
  templateUrl: "./profile-details.component.html",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, DetailAuditLogComponent]
})

export class ProfileDetailsComponent implements OnInit{
  @Output() closeProfileDetails = new EventEmitter();
  username:string = "";
  profileDetails:any;
  showAuditLogModal=false;
  auditDetails:any=null;
  constructor(private adminService:AdminServices, public keyCase:ObjectKeyCasingService) {
    this.username = localStorage.getItem("username");
  }
  ngOnInit() {
    this.loadProfileDetails();
  }
  loadProfileDetails() {
    dialog.loading();
    this.adminService.getSingleUserResult(this.username).subscribe(res=>{
      this.keyCase.camelCase(res);
      this.profileDetails = res;
      dialog.close();
    },dialog.error)
  }
  public closeModal() {
    this.closeProfileDetails.emit(true);
  }
  showAuditLog(action){
    if(!action.auditLog){
      return;
    }
    this.showAuditLogModal=true;
    this.auditDetails = action;
  }
  public closeAuditLog(){
    this.showAuditLogModal=false;
    this.auditDetails = null;
  }
}
