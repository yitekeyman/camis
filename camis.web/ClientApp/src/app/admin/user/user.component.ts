import {Component, OnInit, AfterViewInit} from '@angular/core';
import {UserDetailViewModal, UserModel} from './user.model';
import {UserServices} from './user.services';
import {Router} from '@angular/router';
import {PagerService} from '../../_services/pager.service';
import {ToastrService} from 'ngx-toastr';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";

declare var init_sidebar: any;
declare var $: any;

@Component({
  selector: 'app_user',
  templateUrl: './user.component.html'

})
export class UserComponent implements OnInit, AfterViewInit {
  public isRegisterShowed = false;
  public users: UserDetailViewModal[] = [];
  public selectedUser: UserModel;
  public query: string;
  public toolTip: any;
  public pager: any = {};
  pagedItems: any[];
  status:number=1;

  public isEdit = false;

  constructor(public userService: UserServices, public pagerService: PagerService, public router: Router, public toastr: ToastrService, private keyCase: ObjectKeyCasingService) {
  }

  ngOnInit(): void {
    this.getAllUsers();
  }

  ngAfterViewInit(): void {
    this.toolTip = new init_sidebar();
  }

  public filterUsers(search: string) {
    this.userService.searchUsers(search, this.status).subscribe(res => {
      this.users = res;
      if (this.users.length > 0)
        this.keyCase.camelCase(this.users);
      this.setPage(1);
    });
  }

  public getAllUsers() {
    this.userService.getUsers(this.status).subscribe(res => {
      this.users = res;
      if (this.users.length > 0)
        this.keyCase.camelCase(this.users);
      this.setPage(1);

    });
  }

  public setPage(page: number) {
    if (page < 1 || page > this.pager.totalPages) {
      return;
    }

    this.pager = this.pagerService.getPager(this.users.length, page);

    //get the paged items
    this.pagedItems = this.users.slice(this.pager.startIndex, this.pager.endIndex + 1);

  }

  public deactivateUser(usr: UserModel): void {
    console.log(usr);
    this.userService.deactivateUser({username: usr.userName}).subscribe(res => {

      this.toastr.success('Successfully, Deactiveted user (' + usr.fullName + ')', 'User Deactivation');
      window.location.reload();
    });
  }

  public activateUser(usr: UserModel): void {
    console.log(usr);
    this.userService.activateUser({username: usr.userName}).subscribe(res => {

      this.toastr.success('Successfully, Activated User (' + usr.fullName + ')', 'User Activation');
      window.location.reload();

    });
  }

  public addBtnClick(): void {
    this.isRegisterShowed = true;
    this.isEdit = false;
    this.showRegistrationForm();
  }

  public showRegistrationForm(): void {
    $('#editUser').addClass('in');
    $('#editUser').show();
  }

  public showUpdateForm(): void {

    $('#editUser').addClass('in');
    $('#editUser').show();
  }

  public closeForm(evt: any) {
    this.isRegisterShowed = false;
    this.isEdit = false;
    $('#editUser').removeClass('in');
    $('#editUser').hide();
  }

  public showEditForm(usr: UserModel): void {
    this.selectedUser = usr;
    this.isRegisterShowed = false;
    this.isEdit = true;
    this.showUpdateForm();
  }

  public resetPass(usr: UserModel) {
    this.selectedUser = usr;
    $('#updatePass').addClass('in');
    $('#updatePass').show();
  }

  public closePassForm(): void {
    $('#updatePass').removeClass('in');
    $('#updatePass').hide();
    $('#updatePassForm').trigger('reset');
  }
}
