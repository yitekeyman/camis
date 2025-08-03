import {Router} from '@angular/router';
import {ApiService} from '../_services/api.service';
import {LoginUser, RegisterUser, UpdatePassword} from './user/user.model';
import {Injectable} from '@angular/core';
import {FormBuilder} from '@angular/forms';
import {ObjectKeyCasingService} from "../_services/object-key-casing.service";

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
}
