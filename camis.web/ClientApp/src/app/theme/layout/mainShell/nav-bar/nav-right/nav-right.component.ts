// Angular import
import {Component, OnInit} from '@angular/core';
import {Router, RouterModule} from '@angular/router';

// third party import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import {AdminServices} from "../../../../../_services/admin.Services";
import dialog from "../../../../../_shared/dialog";
import {ChangePassComponent} from "../../../../../admin/userManagement/changePassword/change-pass.component";
import {ProfileDetailsComponent} from "../../../../../admin/userManagement/profileDetails/profile-details.component";

@Component({
  selector: 'app-nav-right',
  imports: [RouterModule, SharedModule, ChangePassComponent, ProfileDetailsComponent],
  templateUrl: './nav-right.component.html',
  styleUrls: ['./nav-right.component.scss']
})
export class NavRightComponent implements OnInit{
  public username:string;
  public fullname:string;
  public role:number;
  public roleName:string;
  public isChangePassword=false;
  public isProfileShow=false;
  constructor(private  adminService:AdminServices, private router:Router) {
    this.username=localStorage.getItem('username');
    this.roleName=localStorage.getItem('roleName');
    this.fullname=localStorage.getItem('fullname');
  }
  ngOnInit() {

  }
  logout() {
    this.adminService.logout();
    this.router.navigate(['login']);
  }

  async changePassword():Promise<void> {
    if (!await dialog.confirm('Are you sure you want to change password?')) {
      return;
    }
    this.isChangePassword=true;

  }
  public closeForm(close: boolean): void {
    if (close) {
      this.isChangePassword = false;
      this.isProfileShow=false;
    }
  }
  public reloadPage(rel:boolean){
    if(rel){
      this.logout();
    }
  }

  public seeProfile(){
    this.isProfileShow=true;
  }
}
