import {Component, OnInit} from '@angular/core';
import {AuditModel} from './AuditModel';
import {AdminDashboardService} from './adminDashboard.service';
import {PagerService} from '../../_services/pager.service';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";


declare var $: any;
@Component({
    selector: 'app_dashboard',
    templateUrl: './adminDashboard.component.html'
})



export class AdminDashboardComponent implements OnInit {
     public auditModels: AuditModel[]=[];
    public pager: any = {};
    pagedItems: any[];
     constructor(public auditService: AdminDashboardService, public pagerServer: PagerService,  private keyCase: ObjectKeyCasingService) {

     }
    ngOnInit(): void {
         this.getAllActions();
    }

    public getAllActions() {
        this.auditService.getAudits().subscribe(res => {
            this.auditModels = res;
            if(this.auditModels.length > 0){
              this.keyCase.camelCase(this.auditModels);
            }
            this.setPage(1);
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
}
