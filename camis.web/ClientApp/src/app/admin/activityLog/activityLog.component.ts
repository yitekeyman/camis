import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {AdminDashboardService} from "../../_services/adminDashboard.service";
import {PagerService} from "../../_services/pager.service";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {AuditModel} from "../../_model/AuditModel";
import dialog from "../../_shared/dialog";
import {DetailAuditLogComponent} from "./detialAuditLog/detail-audit-log.component";

@Component({
  selector: "app-activity-log",
  imports: [CommonModule, ReactiveFormsModule, DetailAuditLogComponent],
  templateUrl: "./activityLog.component.html",
  styleUrls: ["./activityLog.component.scss"]
})
export class ActivityLogComponent implements OnInit{
  public auditModels: AuditModel[]=[];
  public pager: any = {};
  pagedItems: any[];
  showAuditLogModal=false;
  auditDetails:any=null;
  constructor(public auditService: AdminDashboardService, public pagerServer: PagerService,  private keyCase: ObjectKeyCasingService) {}

  ngOnInit() {
    this.getAllActions();
  }
  public getAllActions() {
    dialog.loading();
    this.auditService.getAudits().subscribe(res => {
      this.auditModels = res;
      if(this.auditModels.length > 0){
        this.keyCase.camelCase(this.auditModels);
      }
      this.setPage(1);
      dialog.close();
    },error => {
      return dialog.error(error);
    });
  }
  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerServer.getPager(this.auditModels.length, page);

    //get the paged items
    this.pagedItems = this.auditModels.slice(this.pager.startIndex, this.pager.endIndex + 1);

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
