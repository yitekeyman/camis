import {Component, EventEmitter, Input, Output, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {UpdatePassword} from "../../../_model/user.model";
import dialog from "../../../_shared/dialog";
import {AdminServices} from "../../../_services/admin.Services";

@Component({
  selector: "app-change-pass",
  imports:[CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: "./change-pass.component.html",
  styleUrls: ["./change-pass.component.scss"]
})

export class ChangePassComponent implements OnInit {

  @Output() closeChangePassForm = new EventEmitter();
  @Output() reload = new EventEmitter<boolean>();
  public newPassConfirm: string;
  public updatePassForm: FormGroup;
  public passwordModel: UpdatePassword;
  public showOldPassword=false;
  public showPassword = false;
  public showConfirmPassword = false;
  constructor(private fb: FormBuilder, private adminService:AdminServices) {
    this.updatePassForm = fb.group({
      oldPassword:['', [Validators.required]],
      password: ['', [Validators.required, this.misMatchValidator]],
      confirmPassword: ['',[Validators.required, this.matchValidator]]
    });
  }
  ngOnInit(): void {
    this.passwordModel = {
      OldPassword: '',
      NewPassword: ''
    };
  }
  public updatePass(): void {
    if (!this.updatePassForm.valid) {
      this.markFormGroupTouched();
      return;
    }
    dialog.loading();
    const formValues = this.updatePassForm.value;
    this.passwordModel.NewPassword = formValues.password;
    this.passwordModel.OldPassword=formValues.oldPassword;
    this.adminService.updatePassword(this.passwordModel).subscribe(res => {
      if (res.errorCode != null) {
        return dialog.error(res);
      } else {
        this.closeForm();
        this.reload.emit(true);
        return dialog.success('Successfully Change the password! login again please...', 'Password Update');
      }
    });

  }
  private markFormGroupTouched() {
    Object.keys(this.updatePassForm.controls).forEach(key => {
      this.updatePassForm.get(key)?.markAsTouched();
    });
  }
  public closeForm() {
    this.closeChangePassForm.emit(true);
  }
  toggleOldPasswordVisibility(): void {
    this.showOldPassword = !this.showOldPassword;
  }
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  // Toggle confirm password visibility
  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
  matchValidator(control: FormControl) {
    const password = control.root.get('password');
    const confirmPassword = control.value;

    if (password && confirmPassword !== password.value) {
      return {mismatch: true};
    }

    return null;
  }

  misMatchValidator(control: FormControl) {

    const password = control.value;
    const oldPassword = control.root.get('oldPassword');

    if (oldPassword && password && oldPassword.value === password) {
      return {mismatch: true};
    }

    return null;
  }

}
