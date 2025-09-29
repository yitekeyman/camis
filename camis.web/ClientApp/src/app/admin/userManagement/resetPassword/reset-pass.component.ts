import {Component, EventEmitter, Input, Output, OnInit} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserModel} from "../../../_model/user.model";
import {ResetPassword} from "../../../_model/user.model";
import dialog from "../../../_shared/dialog";
import {AdminServices} from "../../../_services/admin.Services";

@Component({
  selector: "app-reset-pass",
  imports:[CommonModule, ReactiveFormsModule],
  templateUrl: "./reset-pass.component.html",
  styleUrls: ["./reset-pass.component.scss"]
})

export class ResetPassComponent implements OnInit {
  @Input() public userModel: UserModel;

  @Output() closeResetPassForm = new EventEmitter();
  @Output() reload = new EventEmitter<boolean>();
  public newPassConfirm: string;
  public updatePassForm: FormGroup;
  public passwordModel: ResetPassword;
  public username: string|null
  public showPassword = false;
  public showConfirmPassword = false;
  constructor(private fb: FormBuilder, private adminService:AdminServices) {
    this.updatePassForm = fb.group({
      password: ['', [Validators.required]],
      confirmPassword: ['',[Validators.required, this.matchValidator]]
    });
  }
  ngOnInit(): void {
    this.username = this.userModel.userName;
    this.passwordModel = {
      UserName: this.userModel.userName,
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
    this.adminService.resetPass(this.passwordModel).subscribe(res => {
      if (res.errorCode != null) {
        return dialog.error(res);
      } else {
        this.closeForm();
        this.reload.emit(true);
        return dialog.success('Successfully Reset the password', 'Password Update');
      }
    });

  }
  private markFormGroupTouched() {
    Object.keys(this.updatePassForm.controls).forEach(key => {
      this.updatePassForm.get(key)?.markAsTouched();
    });
  }
  public closeForm() {
    this.closeResetPassForm.emit(true);
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

}
