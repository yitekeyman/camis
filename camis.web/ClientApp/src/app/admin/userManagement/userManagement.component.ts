import {CommonModule} from "@angular/common";
import {Component, OnInit, ViewChild, TemplateRef} from "@angular/core";
import {AdminServices} from "../../_services/admin.Services";
import dialog from "../../_shared/dialog";
import {UserDetailViewModal} from "../../_model/user.model";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import {UserModel} from "../../_model/user.model";
import {ReactiveFormsModule} from "@angular/forms";
import {RegisterUserComponent} from "./registerUser/register-user.component";
import {PagerService} from "../../_services/pager.service";
import {ResetPassComponent} from "./resetPassword/reset-pass.component";



@Component({
  selector: "app-user-management",
  imports: [CommonModule, RegisterUserComponent, ReactiveFormsModule, ResetPassComponent],
  templateUrl: "./userManagement.component.html",
  styleUrls: ['./userManagement.component.scss']
})
export class UserManagementComponent implements OnInit {
  public isRegisterShowed:boolean = false;
  public isResetPass:boolean=false;
  public users: UserDetailViewModal[] = [];
  public selectedUser: UserModel;
  public query: string;
  status: number = 1;
  public isEdit = false;
  public pager: any = {};
  pagedItems: any[];

  constructor(private userService: AdminServices, private keyCase: ObjectKeyCasingService, public pagerService: PagerService,) {
  }

  ngOnInit() {
    this.getAllUsers();
  }

  public getAllUsers() {
    dialog.loading();
    this.userService.getUsers(this.status).subscribe(res => {
      this.keyCase.camelCase(res);
      this.users = res;
      this.setPage(1);
      dialog.close();
    }, err => {
      return dialog.error(err.message);
    });
  }

  public filterUsers(search: string) {
    this.userService.searchUsers(search, this.status).subscribe(res => {
      this.keyCase.camelCase(res);
      this.users = res;
      this.setPage(1);
    });
  }

  async deactivateUser(usr: UserModel):Promise<void> {
    if (!await dialog.confirm('Are you sure you want to deactivate user account?')) {
      return;
    }
    dialog.loading();
    this.userService.deactivateUser({username: usr.userName}).subscribe(res => {

      dialog.success('Deactivated user (' + usr.fullName + ')', 'User Deactivation').then(dialog.close);
      this.getAllUsers();
    }, error => {
      return dialog.error(error);
    });
  }

  async activateUser(usr: UserModel): Promise<void> {
    if (!await dialog.confirm('Are you sure you want to activate user account?')) {
      return;
    }
    dialog.loading();
    this.userService.activateUser({username: usr.userName}).subscribe(res => {

      dialog.success('Successfully, Activated User (' + usr.fullName + ')', 'User Activation').then(dialog.close);
      this.getAllUsers();

    }, error => {
      return dialog.error(error);
    });
  }

  public addBtnClick(): void {
    this.isRegisterShowed = true;
    this.isEdit = false;
    this.selectedUser = null;
    console.log('Add button clicked, modal should show'); // Debug log
  }

  public closeForm(close: boolean): void {
    if (close) {
      this.isRegisterShowed = false;
      this.isEdit = false;
      this.selectedUser = null;
      this.isResetPass = false;
    }
  }
public reloadPage(rel:boolean){
    if(rel){
      this.getAllUsers();
    }
}
  public showEditForm(usr: UserModel): void {
    this.selectedUser = usr;
    this.isRegisterShowed = true;
    this.isEdit = true;
    console.log('Edit button clicked, user:', usr); // Debug log
  }


  async resetPass(usr: UserModel):Promise<void> {
    if (!await dialog.confirm('Are you sure you want to reset user password?')) {
      return;
    }
      this.selectedUser = usr;
      this.isResetPass=true;

  }

  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.users.length, page);
    this.pagedItems = this.users.slice(this.pager.startIndex, this.pager.endIndex + 1);

  }
}
