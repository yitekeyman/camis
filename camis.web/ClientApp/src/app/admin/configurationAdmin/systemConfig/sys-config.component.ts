import {Component, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AdminServices} from "../../../_services/admin.Services";
import {PagerService} from "../../../_services/pager.service";
import {AuditModel} from "../../../_model/AuditModel";
import {SysConfigModel} from "../../../_model/user.model";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import dialog from "../../../_shared/dialog";

@Component({
  selector: "app-sys-config",
  templateUrl: "./sys-config.component.html",
  imports:[CommonModule, ReactiveFormsModule, FormsModule],
  styleUrls: ["./sys-config.component.scss"]
})

export class SystemConfigComponent implements OnInit {
  public sysConfigModel: SysConfigModel[]=[];
  public pager: any = {};
  pagedItems: any[];
  showEditor=false;
  selectedConfig:SysConfigModel=null;
  value:string=null;
  constructor(private adminService:AdminServices, public pagerService:PagerService, private keyCase:ObjectKeyCasingService) {
  }
  ngOnInit() {
    this.loadConfiguration();
  }

  loadConfiguration(){
    dialog.loading();
    this.selectedConfig=null;
    this.showEditor=false;
    this.adminService.GetAllSysConfig().subscribe(res=>{
      this.keyCase.camelCase(res);
      this.sysConfigModel = res;
      this.setPage(1);
      dialog.close();
    },dialog.error)
  }
  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.sysConfigModel.length, page);

    //get the paged items
    this.pagedItems = this.sysConfigModel.slice(this.pager.startIndex, this.pager.endIndex + 1);
  }

  async selectEdit(config:SysConfigModel): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to the value of this sysConfig?')) {
      return;
    }

   this.selectedConfig = config;
    this.value=this.selectedConfig.value;
    this.showEditor=true;
  }
  async saveConfiguration():Promise<void>{
    if (!await dialog.confirm('Are you done editing configuration?')) {
      return;
    }
    if(this.selectedConfig.value===this.value){
      dialog.error(`You don't have made changes!`).then();
      return;
    }
    this.selectedConfig.value=this.value;
    dialog.loading();
    this.adminService.EditSysConfig(this.selectedConfig).subscribe(res=>{
      dialog.success('You have successfully update configuration value').then(dialog.close);
      this.loadConfiguration();
      this.updateChange();
    }, dialog.error)
  }
  updateChange(){
    this.adminService.GetSystemParameter().subscribe(res2 => {
      this.keyCase.camelCase(res2);
      this.adminService.updateRegion( res2.regionName,res2.regionCode, res2.utmZone);
    });
  }
  cancelEditing(){
    this.selectedConfig = null;
    this.showEditor=false;
  }
}
