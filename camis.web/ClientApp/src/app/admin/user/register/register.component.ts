import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {UserComponent} from '../user.component';
import {RegisterUser} from '../user.model';
import {ToastrService} from 'ngx-toastr';
import {AdminServices} from '../../admin.Services';
import {AbstractControl, FormBuilder, FormGroup, Validators, FormControl} from '@angular/forms';
import {Router} from '@angular/router';

declare var $: any;

@Component({
  selector: 'app-user-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  public userModel: RegisterUser;
  public registrationForm: FormGroup;
  public newPassConfirm: string;
  public valid: boolean | true;
  showPassword = false;
  showConfirmPassword = false;

  @Output() closeEditForm = new EventEmitter();

  public ROLES = [];

  constructor(public adminService: AdminServices, public fb: FormBuilder, public toastr: ToastrService, public userComponent: UserComponent, public router: Router) {
    this.registrationForm = fb.group({
      fullname: ['', [Validators.required, this.validateFullName]],
      phoneno: ['', Validators.required],
      email: ['', [Validators.required, this.validateEmail]],
      role: [[], Validators.required],
      username: ['', [Validators.required, Validators.minLength(6)]],
      password: ['', Validators.required],
      confirmPassword: ['', [Validators.required, this.matchValidator]]
    });
  }

  public ngOnInit(): void {
    this.adminService.getRoles().subscribe(res => {
      this.ROLES = res;
    });
    this.userModel = {
      username: '',
      password: '',
      fullname: '',
      roles: [],
      phoneNo: '',
      email: ''
    };
  }

  public register() {
    if (this.registrationForm.valid) {
      // Create user data from form values
      const formValues = this.registrationForm.value;
      const userToRegister: RegisterUser = {
        username: formValues.username,
        password: formValues.password,
        fullname: formValues.fullname,
        roles: formValues.role,
        phoneNo: formValues.phoneno,
        email: formValues.email,
      };

      this.adminService.register(userToRegister).subscribe(res => {
        if (res.errorCode != null) {
          this.toastr.error(res.message, 'User Registration');
        } else {
          this.toastr.success('User Registered Successfully', 'User Registration');
          this.closeForm();
          window.location.reload();
        }
      });
    } else {
      // Mark all fields as touched to show validation messages
      Object.keys(this.registrationForm.controls).forEach(key => {
        this.registrationForm.get(key).markAsTouched();
      });
    }
  }

  public closeForm() {
    this.closeEditForm.next();
    $('#registrationForm').trigger('reset');
  }

  matchValidator(control: FormControl) {
    const password = control.root.get('password');
    const confirmPassword = control.value;

    if (password && confirmPassword !== password.value) {
      return {mismatch: true};
    }

    return null;
  }

  validateFullName(control: FormControl) {
    const value = control.value;
    if (!value) {
      return null;
    }
    const regex = /^[a-zA-ZÀ-ÿ]+([ '-][a-zA-ZÀ-ÿ]+)*$/;
    const valid = regex.test(value) && value.trim().split(/\s+/).length >= 2;

    return valid ? null : {invalidFullName: true};
  }

  validateEmail(control: FormControl) {
    const value = control.value;
    if (!value) {
      return null;
    }
    const regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const valid = regex.test(value);

    return valid ? null : {invalidEmail: true};
  }

  // Check if a role is selected
  isRoleSelected(roleId: number): boolean {
    const roleControl = this.registrationForm.get('role');
    const selectedRoles = roleControl ? roleControl.value : [];
    return selectedRoles.includes(roleId);
  }

  // Handle checkbox change events
  onRoleChange(event: any, roleId: number): void {
    const roleControl = this.registrationForm.get('role');
    const selectedRoles = roleControl ? roleControl.value : [];

    if (event.target.checked) {
      // Add role if checked
      if (!selectedRoles.includes(roleId)) {
        this.registrationForm.get('role').setValue([...selectedRoles, roleId]);
      }
    } else {
      // Remove role if unchecked
      this.registrationForm.get('role').setValue(selectedRoles.filter((id: number) => id !== roleId));
    }

    // Mark the control as touched for validation
    this.registrationForm.get('role').markAsTouched();
  }

  // Get the number of selected roles
  getSelectedCount(): number {
    const roleControl = this.registrationForm.get('role');
    return roleControl && roleControl.value ? roleControl.value.length : 0;
  }

  // Get the selected role objects
  getSelectedRoles(): any[] {
    const roleControl = this.registrationForm.get('role');
    const selectedIds = roleControl ? roleControl.value : [];
    return this.ROLES.filter(role => selectedIds.includes(role.id));
  }

  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // Toggle confirm password visibility
  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
