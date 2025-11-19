// Angular import
import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router, RouterModule} from '@angular/router';
import { Subscription } from 'rxjs';
// third party import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import {AdminServices} from "../../../../../_services/admin.Services";
import dialog from "../../../../../_shared/dialog";
import {ChangePassComponent} from "../../../../../admin/userManagement/changePassword/change-pass.component";
import {ProfileDetailsComponent} from "../../../../../admin/userManagement/profileDetails/profile-details.component";
import {SimpleLoginComponent} from "../../../../../login/simpleLogin/simple-login.component";

@Component({
  selector: 'app-nav-right',
  imports: [RouterModule, SharedModule, ChangePassComponent, ProfileDetailsComponent, SimpleLoginComponent],
  templateUrl: './nav-right.component.html',
  styleUrls: ['./nav-right.component.scss']
})
export class NavRightComponent implements OnInit, OnDestroy{
  public username:string;
  public fullname:string;
  public role:number;
  public roleName:string;
  public isChangePassword=false;
  public isProfileShow=false;
  public showLogin=false;
  private sessionSubscription: Subscription;
  constructor(private  adminService:AdminServices, private router:Router) {
    this.username=localStorage.getItem('username');
    this.roleName=localStorage.getItem('roleName');
    this.fullname=localStorage.getItem('fullname');
  }
  ngOnInit() {
    this.sessionSubscription = this.adminService.sessionExpired.subscribe(expired => {
      if (expired && !this.showLogin) {
        this.showLogin = true;
      }
    });

    // Check initial session status when component loads
    this.checkInitialSession();
  }
  ngOnDestroy() {
    if (this.sessionSubscription) {
      this.sessionSubscription.unsubscribe();
    }
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
      this.showLogin=false;
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
  public showLoginModal(){
    this.showLogin=true;
  }
  public closeLoginModal(){

    this.adminService.checkSession().subscribe({
      next: (response) => {
        if (response.isValid) {
          this.showLogin = false;
          window.location.reload();
        }
      },
      error: () => {
       this.showLogin=true;
      }
    });
  }
  private checkInitialSession(): void {
    this.adminService.checkSession().subscribe({
      next: (response) => {
        if (!response.isValid) {
          this.showLogin = true;
        }
      },
      error: () => {
        this.showLogin = true;
      }
    });
  }
}
