import {Injectable} from "@angular/core";
import {LoginUser, SystemUserResponse, SystemUserRequest} from "../_models/user.model";
import {FormBuilder} from "@angular/forms";
import {Router} from "@angular/router";
import {ApiService} from "./api.services";

@Injectable()
export class AuthServices {
    
    public user:LoginUser;
    public formFb:FormBuilder;
    
    constructor(public router:Router, public apiService:ApiService){}
    
    login(body:LoginUser){
        return this.apiService.post(`User/Login`, body);
    }
    setUserRole(body: any){
        return this.apiService.post(`User/SetRole`, body);
    }
    
     checkUser(username:string){
        return this.apiService.get(`User/CheckUsername?username=${username}`);
    }
    
    checkEmail(email:string){
        return this.apiService.get(`User/CheckEmail?email=${email}`);
    }
    
    registerUser(body:any){
        return this.apiService.post(`User/RegisterUser`, body);
        
    }
    checkSession(body:any){
        return this.apiService.post(`User/CheckSession?userName=${body}`, null);
    }
    
    getAllSystemUsers(){
        return this.apiService.get(`User/GetUsers`);
    }
    getAllInvestors(){
        return this.apiService.get(`User/GetInvestors`);
    }
    activateUser(userName:any){
        return this.apiService.post(`User/ActivateUser?userName=${userName}`, null);
    }
    deactivateUser(userName:string){
        return this.apiService.post(`User/DeactivateUser?userName=${userName}`, null);
    }
    getUser(body:string){
        return this.apiService.get(`User/GetUser?userName=${body}`);
    }
    updateUser(body:any){
        return this.apiService.post(`User/UpdateUserRegistration`,body);
    }
    
    resetPassword(userName:string, newPassword:string){
        return this.apiService.post(`User/ResetPassword?userName=${userName}&newPassword=${newPassword}`, null);
    }
    changePassword(userName:string, oldPassword:string, newPassword:string){
        return this.apiService.post(`User/ChangePassword?userName=${userName}&oldPassword=${oldPassword}&newPassword=${newPassword}`, null)
    }
    logout(){
        return this.apiService.post(`User/Logout`, null).subscribe(res=>{
            localStorage.removeItem('username');
            localStorage.removeItem('role');
            localStorage.removeItem('routerLink');
            this.router.navigate(['/default/bids']);
        })
    }
    getAllRegions(){
        return this.apiService.get(`User/GetAllRegions`);
    }
    addRegion(body:any){
        return this.apiService.post(`User/AddRegion`, body);
    }
    updateRegion(body:any){
        return this.apiService.post(`User/UpdateRegion`, body);
    }
}