import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {LoginUser} from '../admin/user/user.model';
import {AdminServices} from '../admin/admin.Services';
import dialog from '../_shared/dialog';

declare var $: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {

  public loginForm: FormGroup;
  public loggedIn: boolean | false;
  public user: LoginUser;
  public selectedRole: number;
  showPassword = false;

  public ROLES: any[];


  constructor(public fb: FormBuilder, public router: Router, public adminService: AdminServices) {
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
        } else {
          $('#roleModal').show().addClass('in');
        }
      }, dialog.error);

    }, dialog.error);
  }

  public proceed(): void {
    dialog.loading();

    this.adminService.setUserRole({role: this.selectedRole}).subscribe(res => {
      localStorage.setItem('role', '' + this.selectedRole);

      let path = null;
      switch (Number(this.selectedRole)) {
        case 1: path = 'admin'; break;
        case 2: path = 'clerk'; break;
        case 3: path = 'supervisor'; break;
        case 4: path = 'land-clerk/land-dashboard'; break;
        case 5: path = 'land-supervisor/land-dashboard'; break;
        case 6: path = 'land-admin'; break;
        case 7: path = 'land-certificate-issuer'; break;
        case 8: path = 'mne-expert'; break;
        case 9: path = 'mne-supervisor'; break;
        case 10: path = 'mne-data-encoder'; break;
        case 11: path = 'configuration-admin'; break;
        default:
          dialog.error({ message: 'Selected user role has no UI.' });
          return;
      }

      this.router.navigateByUrl(path)
        .then(() => dialog.close())
        .catch(dialog.error);
    }, dialog.error);
  }

  public closeForm(): void {
    $('#roleModal').hide();
    this.proceed();
  }
  public cancelLogin(): void {
    $('#roleModal').hide().removeClass('in');
  }
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
