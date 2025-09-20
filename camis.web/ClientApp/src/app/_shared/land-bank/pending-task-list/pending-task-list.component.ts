import { Component, OnInit } from '@angular/core';
import { LandDataService } from '../../../_services/land-data.service';
import { Router } from '@angular/router';
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

@Component({
  selector: 'app-pending-task-list',
  templateUrl: './pending-task-list.component.html',
  styleUrls: ['./pending-task-list.component.css']
})
export class PendingTaskListComponent implements OnInit {

  clerkRole = false;
  loginRole = '';
  user = '';
  noItem = false;

  public userWorkItems: any[] = [];

  constructor(private landService: LandDataService, private router: Router, private keyCase:ObjectKeyCasingService) {

    if (localStorage.getItem('role') === '4' ) {
      this.clerkRole = true;
      this.loginRole = 'land-clerk';
      this.user = 'Clerk';
      console.log(this.loginRole);
    }
    if (localStorage.getItem('role') === '5') {
      this.loginRole = 'land-supervisor';
      this.user = 'Supervisor';
    }
   }

  ngOnInit() {
    this.landService.GetUserWorkItems().subscribe(data => {
      this.keyCase.camelCase(data);
      this.userWorkItems = data;
      console.log(this.userWorkItems);
    });

    for (const workItems of this.userWorkItems) {
      this.noItem = false;
      if (workItems.workFlowType !== 4) {
        this.noItem = true;
      }
      this.noItem = false;
      if (workItems.workFlowType !== 7) {
        this.noItem = true;
      }
    }
  }

  editLand(wfid: string) {
    this.router.navigate([`land-clerk/pending-task/edit-land/${wfid}`]);
  }

  taskDetail(wfid: string) {
    this.router.navigate([`land-supervisor/pending-task/task-detail/${wfid}`]);
  }
}
