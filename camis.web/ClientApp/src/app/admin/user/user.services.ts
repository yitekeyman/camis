import {ApiService} from '../../_services/api.service';
import {UserModel} from './user.model';
import {Injectable} from '@angular/core';
import {ResetPassword} from './user.model';
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";

@Injectable()
export class UserServices {
    public static ROLES = ['admin', 'PMUser', 'PMManager'];

    constructor(public apiService: ApiService) {

    }

    public getUsers() {
        return this.apiService.get('admin/getusers');
    }

    public searchUsers(query: string) {
        return this.apiService.get(`admin/search?query=${query}`);
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
