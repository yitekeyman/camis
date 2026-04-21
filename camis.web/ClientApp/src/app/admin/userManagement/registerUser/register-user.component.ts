import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormControl } from "@angular/forms";
import {AdminServices} from "../../../_services/admin.Services";
import {LookUpModel, RegisterUser, UserModel} from "../../../_model/user.model";
import dialog from "../../../_shared/dialog";



@Component({
  selector: 'app-register-user',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.scss']
})
export class RegisterUserComponent implements OnInit {
  @Input() isEditMode: boolean = false;
  @Input() userData: UserModel | null = null;
  @Output() closeEditForm = new EventEmitter<boolean>();
  @Output() reload = new EventEmitter<boolean>();

  public userModel: any;
  public registrationForm: FormGroup;
  public ROLES: any[] = [];
  public showPassword = false;
  public showConfirmPassword = false;

  constructor(
    private adminService: AdminServices,
    private fb: FormBuilder
  ) {
    this.registrationForm = this.createForm();
  }

  ngOnInit() {
    this.loadRoles();
    this.userModel = {
      username: '',
      password: '',
      fullname: '',
      roles: [],
      phoneNo: '',
      email: ''
    };
    if (this.isEditMode && this.userData) {
      this.userModel = {
        username: this.userData.userName,
        fullname: this.userData.fullName,
        roles: this.userData.roles,
        phoneNo: this.userData.phoneNo,
        email: this.userData.email,
      };
      this.populateFormForEdit();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      fullname: ['', [Validators.required, this.validateFullName]],
      phoneno: ['', Validators.required],
      email: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required, this.validateEmail]],
      role: [[], Validators.required],
      username: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required]],
      password: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required]],
      confirmPassword: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required, this.matchValidator]]
    });
  }

  private loadRoles() {
    this.adminService.getRoles().subscribe({
      next: (res) => {
        this.ROLES = res;
      },
      error: (err) => {
        console.error('Error loading roles:', err);
        //this.toastr.error('Failed to load roles', 'Error');
      }
    });
  }

  private populateFormForEdit() {
    if (!this.userData) return;
    this.registrationForm.patchValue({
      fullname: this.userData.fullName,
      phoneno: this.userData.phoneNo,
      email: this.userData.email,
      username: this.userData.userName,
      role: this.userData.roles?.map(role => role.id) || []
    });
    if (this.isEditMode) {

      this.registrationForm.removeControl('password');
      this.registrationForm.removeControl('confirmPassword');
    }
  }

  public register(): void {
    if (!this.registrationForm.valid) {
      this.markFormGroupTouched();
      return;
    }
    dialog.loading();
    const formValues = this.registrationForm.value;
    let userModel: any;

    if (this.isEditMode) {
      const selectedRoles = this.getSelectedRoles(formValues.role);

      const userData: UserModel = {
        userName: formValues.username,
        fullName: formValues.fullname,
        roles: selectedRoles,
        phoneNo: formValues.phoneno,
        email: formValues.email,
        status: this.userData.status
      };

      userModel = userData;
      this.updateUser(userModel);
    } else {
      const userData: RegisterUser = {
        username: formValues.username,
        password: formValues.password,
        fullname: formValues.fullname,
        roles: formValues.role,
        phoneNo: formValues.phoneno,
        email: formValues.email,
      };

      userModel = userData;
      this.createUser(userModel);
    }
  }

  private getSelectedRoles(selectedRoleIds: any[]): LookUpModel[] {
    return this.ROLES.filter(role => selectedRoleIds.includes(role.id))
      .map(role => ({
        id: role.id,
        name: role.name,
      }));
  }

  private createUser(userData: RegisterUser): void {

    this.adminService.register(userData).subscribe({
      next: (res) => {
        if (res.errorCode) {
          return dialog.error(res);
        } else {
          this.closeForm();
          this.reloadPage();
          return dialog.success('User registered successfully', 'Success');

        }
      },
      error: (err) => {
       return dialog.error(err);
      }
    });
  }

  private updateUser(userData: UserModel): void {
    this.adminService.editUser(userData).subscribe({
      next: (res) => {
        if (res.errorCode) {
          return dialog.error(res);
        } else {
          this.closeForm();
          this.reloadPage();
          return dialog.success('User Update successfully', 'Success');
        }
      },
      error: (err) => {
        return dialog.error(err);
      }
    });
  }

  private markFormGroupTouched() {
    Object.keys(this.registrationForm.controls).forEach(key => {
      this.registrationForm.get(key)?.markAsTouched();
    });
  }

  public closeForm() {
    this.closeEditForm.emit(true);
  }
public reloadPage(): void {
    this.reload.emit(true);
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


  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // Toggle confirm password visibility
  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
