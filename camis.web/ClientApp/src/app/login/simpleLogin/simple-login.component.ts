import {Component, EventEmitter, OnInit, Output} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginUser} from "../../_model/user.model";
import {Router} from "@angular/router";
import {AdminServices} from "../../_services/admin.Services";
import {ObjectKeyCasingService} from "../../_services/object-key-casing.service";
import dialog from "../../_shared/dialog";

declare var $: any;

@Component({
  selector: "app-simple-login",
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: "./simple-login.component.html",
  styleUrls: ["./simple-login.component.scss"]
})

export class SimpleLoginComponent implements OnInit {
  @Output() closeLoginForm = new EventEmitter();
  public loginForm: FormGroup;
  public loggedIn: boolean | false;
  public user: LoginUser;
  public selectedRole: number;
  showPassword = false;
  isLoggedIn = false;
  oldRole = '0';
  oldUsername = "";

  public ROLES: any[];

  constructor(public fb: FormBuilder, public router: Router, public adminService: AdminServices, private keyCase: ObjectKeyCasingService) {
    this.loginForm = fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    this.oldRole = localStorage.getItem('role');
    this.oldUsername = localStorage.getItem('username');
  }

  ngOnInit(): void {
    this.user = {
      username: '',
      password: ''
    };
    this.selectedRole = 0;
  }

  public login(): void {
    dialog.loading();

    this.adminService.login(this.user).subscribe({
      next:(res)=>{
      this.keyCase.camelCase(res);
      localStorage.setItem('fullname', JSON.stringify(res.fullName));
      localStorage.setItem('username', this.user.username);

      this.adminService.getUserRoles().subscribe(res2 => {
        this.isLoggedIn = true;
        this.ROLES = res2;

        dialog.close();

        if (this.ROLES.length == 1) { // only one role, log the user with it
          this.selectedRole = this.ROLES[0].id;
          this.proceed();
        } else {
          this.loginForm.addControl('role', new FormControl('', [Validators.required, Validators.min(1)]));
        }
      }, dialog.error);

    },error:(err)=> {return dialog.error(err)}});
  }

  public proceed(): void {
    dialog.loading();

    this.adminService.setUserRole({role: this.selectedRole}).subscribe(res => {
      localStorage.setItem('role', '' + this.selectedRole);
      for (let role of this.ROLES) {
        if (role.id == this.selectedRole) {
          localStorage.setItem('roleName', role.name);
        }
      }
      let path = 'default/dashboard';
      if (this.user.username == this.oldUsername && this.selectedRole.toString() === this.oldRole) {
        window.location.reload();
      } else {
        this.router.navigateByUrl(path)
          .then(() => {
            this.closeLoginForm.emit(true);
            dialog.close();
          })
          .catch(dialog.error);
      }


    }, dialog.error);
  }

  public closeForm(): void {
    // $('#roleModal').hide();
    this.selectedRole = this.loginForm.value.role;
    this.proceed();
  }

  public cancelLogin(): void {
    $('#roleModal').hide().removeClass('in');
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
