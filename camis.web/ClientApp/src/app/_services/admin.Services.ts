import {Router} from '@angular/router';
import {ApiService} from './api.service';
import {LoginUser, RegisterUser, UpdatePassword, ResetPassword, UserModel, SysConfigModel} from '../_model/user.model';
import {Injectable} from '@angular/core';
import {FormBuilder} from '@angular/forms';
import {ObjectKeyCasingService} from "./object-key-casing.service";
import {BehaviorSubject, Observable} from 'rxjs';
import { tap } from 'rxjs/operators';
@Injectable()
export class AdminServices {
  public user: LoginUser;
  public formFb: FormBuilder;
  public sessionExpired = new BehaviorSubject<boolean>(false);
  private sessionCheckInterval: any;

  constructor(public router: Router, public apiService: ApiService, private keyCase: ObjectKeyCasingService) {
    this.startSessionMonitoring();
  }

  // Add session check method
  public checkSession(): Observable<any> {
    return this.apiService.get('admin/checksession');
  }

  // Start session monitoring
  public startSessionMonitoring(): void {
    // Check session every 5 minutes
    this.sessionCheckInterval = setInterval(() => {
      this.checkSession().subscribe({
        next: (response) => {
          if (!response.isValid) {
            this.sessionExpired.next(true);
          }
        },
        error: () => {
          this.sessionExpired.next(true);
        }
      });
    }, 5 * 60 * 1000); // 5 minutes
  }

  // Stop session monitoring
  public stopSessionMonitoring(): void {
    if (this.sessionCheckInterval) {
      clearInterval(this.sessionCheckInterval);
    }
  }

  // Reset session monitoring after login
  public resetSessionMonitoring(): void {
    this.stopSessionMonitoring();
    this.startSessionMonitoring();
    this.sessionExpired.next(false);
  }

  public login(user: LoginUser) {
    return this.apiService.post('admin/login', user).pipe(
      tap((response: any) => {
        this.resetSessionMonitoring();
        return response;
      })
    );
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
    this.stopSessionMonitoring();
    this.sessionExpired.next(false);
    return this.apiService.post('admin/logout', null).subscribe(res => {
      localStorage.removeItem('username');
      localStorage.removeItem('role');
      localStorage.removeItem('fullname');
      this.router.navigate(['login']);
    });
  }

  public updatePassword(user: UpdatePassword) {
    return this.apiService.post('admin/changepassword', user);
  }

  public getUsers(status: number) {
    return this.apiService.get(`admin/getusers?status=${status}`);
  }

  public searchUsers(query: string, status: number) {
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

  public getSingleUserResult(username: string) {
    return this.apiService.get(`admin/GetSingleUserResult?username=${username}`);
  }

  public GetDashboard() {
    return this.apiService.get(`admin/GetDashboard`);
  }

  public GetAllSysConfig() {
    return this.apiService.get(`admin/GetAllSysConfig`);
  }

  public GetSysConfig(id: number) {
    return this.apiService.get(`admin/GetSysConfig?id=${id}`);
  }

  public GetSysConfigByName(name: string) {
    return this.apiService.get(`admin/GetSysConfigByName?name=${name}`);
  }

  public EditSysConfig(model: SysConfigModel) {
    return this.apiService.post(`admin/EditSysConfig`, model);
  }
  public GetSystemParameter(){
    return this.apiService.get(`admin/GetSystemParameter`);
  }


  private regionNameSubject = new BehaviorSubject<string>(localStorage.getItem('regionName') || 'Region');
  private regionCodeSubject = new BehaviorSubject<string>(localStorage.getItem('regionCode') || 'Code');
  private utmZoneSubject = new BehaviorSubject<string>(localStorage.getItem('UTM') || '37');

  regionName$: Observable<string> = this.regionNameSubject.asObservable();
  regionCode$: Observable<string> = this.regionCodeSubject.asObservable();
  utmZone$: Observable<string> = this.utmZoneSubject.asObservable();

  updateRegion(regionName: string, regionCode: string, utmZone: string) {
    localStorage.setItem('regionName', regionName);
    localStorage.setItem('regionCode', regionCode);
    localStorage.setItem('UTM', utmZone);

    this.regionNameSubject.next(regionName);
    this.regionCodeSubject.next(regionCode);
    this.utmZoneSubject.next(utmZone);
  }
}
