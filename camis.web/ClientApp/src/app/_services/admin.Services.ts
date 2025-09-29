  import {Router} from '@angular/router';
import {ApiService} from './api.service';
import {LoginUser, RegisterUser, UpdatePassword,ResetPassword, UserModel} from '../_model/user.model';
import {Injectable} from '@angular/core';
import {FormBuilder} from '@angular/forms';
import {ObjectKeyCasingService} from "./object-key-casing.service";

@Injectable()
export class AdminServices {
    public user: LoginUser;
    public formFb: FormBuilder;

    constructor(public router: Router, public apiService: ApiService,private keyCase: ObjectKeyCasingService) {

    }

    public login(user: LoginUser) {
        return this.apiService.post('admin/login', user);
    }

    public getUserRoles() {
        return this.apiService.get('admin/getroles');
    }

    public setUserRole(role: any) {
        return this.apiService.post('admin/setrole', role);
    }
    public getRoles() {
        return this.apiService.get('lookup/role');
    }
    public register(user: RegisterUser) {
        return this.apiService.post('admin/register', user);
    }

    public logout() {
        return this.apiService.post('admin/logout', null).subscribe(res => {
            localStorage.removeItem('username');
            this.router.navigate(['login']);
        });
    }
    public updatePassword(user: UpdatePassword) {
        return this.apiService.post('admin/changepassword', user);
    }
  public getUsers(status:number) {
    return this.apiService.get(`admin/getusers?status=${status}`);
  }

  public searchUsers(query: string, status:number) {
    return this.apiService.get(`admin/search?query=${query}&status=${status}`);
  }

  public editUser(user: UserModel) {
    return this.apiService.post('admin/update', user);
  }

  public deactivateUser(username: any) {
    return this.apiService.post('admin/deactivate', username);
  }
  public activateUser(username: any) {
    return this.apiService.post('admin/activate', username);
  }
  public resetPass(user: ResetPassword) {
    return this.apiService.post('admin/resetpassword', user);
  }
}
