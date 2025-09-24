import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators, FormControl} from '@angular/forms';
import dialog from '../_shared/dialog';
import {AdminServices} from "../_services/admin.Services";
import {LoginUser} from "../_model/user.model";
import {CommonModule} from "@angular/common";
import {DialogModule} from "../_shared/dialog/dialog.module";


declare var $: any;

@Component({

  selector: 'app-login',
  imports:[CommonModule, ReactiveFormsModule, DialogModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {

  public loginForm: FormGroup;
  public loggedIn: boolean | false;
  public user:LoginUser;
  public selectedRole: number;
  showPassword = false;

  public ROLES: any[];


  constructor(public fb: FormBuilder, public router: Router, public adminService:AdminServices) {
    this.loginForm = fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.user = {
      username: '',
      password: ''
    };
    this.selectedRole=0;
  }

  public login(): void {
    dialog.loading();

    this.adminService.login(this.user).subscribe(res => {

      localStorage.setItem('username', this.user.username);
      this.adminService.getUserRoles().subscribe(res2 => {
        this.ROLES = res2;

        dialog.close();

        if (this.ROLES.length == 1) { // only one role, log the user with it
          this.selectedRole = this.ROLES[0].id;
          this.proceed();
        }else{
          this.loginForm.addControl('role',new FormControl('', [Validators.required,Validators.min(1)]));
        }
      }, dialog.error);

    }, dialog.error);
  }

  public proceed(): void {
    dialog.loading();

    this.adminService.setUserRole({role: this.selectedRole}).subscribe(res => {
      localStorage.setItem('role', '' + this.selectedRole);
      for(let role of this.ROLES) {
        if(role.id == this.selectedRole){
          localStorage.setItem('roleName',role.name);
        }
      }


      let path = 'dashboard';

      this.router.navigateByUrl(path)
        .then(() => dialog.close())
        .catch(dialog.error);
    }, dialog.error);
  }

  public closeForm(): void {
    // $('#roleModal').hide();
    this.selectedRole=this.loginForm.value.role;
    this.proceed();
  }
  public cancelLogin(): void {
    $('#roleModal').hide().removeClass('in');
  }
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
