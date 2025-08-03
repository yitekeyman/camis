import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {UserComponent} from '../user.component';
import {RegisterUser} from '../user.model';
import {ToastrService} from 'ngx-toastr';
import {AdminServices} from '../../admin.Services';
import {AbstractControl, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';

declare var $: any;

@Component({
    selector: 'app-user-register',
    templateUrl: './register.component.html'
})

export class RegisterComponent implements OnInit {
    public userModel: RegisterUser;
    public registrationForm: FormGroup;
    public newPassConfirm: string;
    public valid: boolean |true;

    @Output() closeEditForm = new EventEmitter();

    public ROLES = [];

    constructor(public adminService: AdminServices, public fb: FormBuilder, public toastr: ToastrService, public userComponent: UserComponent, public router: Router) {
        this.registrationForm = fb.group({
            fullname: ['', Validators.required],
            phoneno: ['', Validators.required],
            role: ['', Validators.required],
            username: ['', [Validators.required, Validators.min(6)]],
            password: ['', Validators.required],
            confirmPassword: ['', [ Validators.required, RegisterComponent.matchValidator]]
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
            phoneNo: ''
        };
    }


    public register() {
        console.log(this.userModel);
        this.adminService.register(this.userModel).subscribe(res => {
            if (res.errorCode != null) {
                this.toastr.error(res.message, 'User Registration');
            } else {

                this.toastr.success('User Registered Successfully', 'User Registration');
                this.closeForm();
                window.location.reload();
            }
        });
    }
    public checkDiff() {

        if ($('#conf_pass').val() != $('#user_pass').val()) {
            this.newPassConfirm = 'New and Confirm Password Missmatch';
            this.valid = false;
        } else {
            this.newPassConfirm = '';
            this.valid = true;
        }
        return this.valid;
    }
    public closeForm() {
        this.closeEditForm.next();
        $('#registrationForm').trigger('reset');
    }

    static matchValidator(abs: AbstractControl) {

        const control = abs.parent;
        if (control) {
            const passwordCtrl = control.get('password');
            const confirmPassCtrl = control.get('confirmPassword');

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
