import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {LandDataService} from "../../_services/land-data.service";
import {Router} from "@angular/router";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import dialog from "../../_shared/dialog";
import {PagerService} from "../../_services/pager.service";

@Component({
  selector: "app-lb-pending-task",
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: "./lb-pending-task.component.html",
})

export class LbPendingTaskComponent implements OnInit{
  clerkRole = false;
  loginRole = '';
  user = '';
  noItem = false;
  public pager: any = {};
  pagedItems: any[];
  public userWorkItems: any= [];

  constructor(private landService: LandDataService, private router: Router, private keyCase: ObjectKeyCasingService, private pagerService: PagerService) {
    this.loginRole = localStorage.getItem("role");
    if (this.loginRole === '4') {
      this.user = 'Land Bank Registrar';
    }
    if (this.loginRole === '5') {
      this.user = 'Land Bank Supervisor';
    }
    if (this.loginRole === '6') {
      this.user = 'Land Bank Administrator';
    }
    if (this.loginRole === '7') {
      this.user = 'Land Bank Certificate Issuer';
    }
  }

  ngOnInit() {
    this.getTasks();
  }

  public getTasks() {
    dialog.loading();
    this.landService.GetUserWorkItems().subscribe(data => {
      this.keyCase.camelCase(data);
      for (const workItems of data) {

        if (workItems.workFlowType === 4 || workItems.workFlowType === 7) {
          this.userWorkItems.push(workItems);
        }

      }
      this.setPage(1);
      dialog.close();
    }, dialog.error);
  }
  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.userWorkItems.length, page);

    //get the paged items
    this.pagedItems = this.userWorkItems.slice(this.pager.startIndex, this.pager.endIndex + 1);

  }
  editLand(wfid: string) {
    this.router.navigate([`land-bank/task/edit-parcel-info/${wfid}`]);
  }

  taskDetail(wfid: string) {
    this.router.navigate([`land-bank/task/parcel-details/${wfid}`]);
  }
}

