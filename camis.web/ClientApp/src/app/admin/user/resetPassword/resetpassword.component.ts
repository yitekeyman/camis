import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ResetPassword} from '../user.model';
import {ToastrService} from 'ngx-toastr';
import {UserModel} from '../user.model';
import {UserServices} from '../user.services';

declare var $: any;

@Component({
    selector: 'app-reset-password',
    templateUrl: './resetpassword.component.html'
})
export class ResetpasswordComponent implements OnInit {
    @Input() public userModel: UserModel;

    @Output() closeResetPassForm = new EventEmitter();
    public newPassConfirm: string;
    public updatePassForm: FormGroup;
    public passwordModel: ResetPassword;
    public username: string|null;

    public valid: boolean |true;

    constructor(public fb: FormBuilder, public userService: UserServices, public toastr: ToastrService, ) {
        this.updatePassForm = fb.group({
            newPass: ['', Validators.required],
            confirmPass: ['', [Validators.required, ResetpasswordComponent.matchValidator]]
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
        console.log(this.passwordModel);
        this.userService.resetPass(this.passwordModel).subscribe(res => {
            if (res.errorCode != null) {
                this.toastr.error(res.message, 'Password Update Error');
            } else {
                this.closePassForm();
                this.toastr.success('Successfully Reset the password', 'Password Update');
            }
        });

    }
    public closePassForm(): void {
        this.closeResetPassForm.next();
    }
    public checkDiff() {

        if ($('#confirmPass').val() != $('#newPass').val()) {
            this.newPassConfirm = 'New and Confirm Password Missmatch';
            this.valid = false;
        } else {
            this.newPassConfirm = '';
            this.valid = true;
        }
        return this.valid;
    }
    static matchValidator(abs: AbstractControl) {

        const control = abs.parent;
        if (control) {
            const passwordCtrl = control.get('newPass');
            const confirmPassCtrl = control.get('confirmPass');

            if (passwordCtrl && confirmPassCtrl) {
                const pass = passwordCtrl.value;
                const confirmPass = confirmPassCtrl.value;

                if (pass != confirmPass) {
                    return {matchPassword : true};
                } else {
                    return null;
                }
            }
        } else {
            return null;
        }
    }
}
