import {Component, EventEmitter, Input, Output, OnInit} from '@angular/core';
import {UserModel} from '../user.model';
import {UserServices} from '../user.services';
import {AdminServices} from '../../admin.Services';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';

declare var $: any;

@Component({
    selector: 'app-user-update',
    templateUrl: './update.component.html'
})

export class UpdateComponent implements OnInit {

    @Input() public userModel: UserModel;
    public editForm: FormGroup;
    public editModel: UserModel;
    public ROLES = [];
    @Output() closeEditForm = new EventEmitter();

    constructor(public userService: UserServices, public fb: FormBuilder, public toastr: ToastrService, public adminService: AdminServices, public router: Router) {
        this.editForm = fb.group({
            fullname: ['', Validators.required],
            phoneno: ['', Validators.required],
            role: ['', Validators.required],
            username: new FormControl({value: '', disabled: true}, Validators.required)
        });
    }
    ngOnInit(): void {

        this.adminService.getRoles().subscribe(res => {
            this.ROLES = res;
        });
        console.log(this.userModel);
        this.editModel = {
            userName: this.userModel.userName,
            fullName: this.userModel.fullName,
            phoneNo: this.userModel.phoneNo,
            status: this.userModel.status,
            roles: this.userModel.roles

        };
        $('#edit_user_role').val(this.editModel.roles);
    }
    public editUser() {
        this.userService.editUser(this.editModel).subscribe(resizeBy => {
            if (resizeBy.errorCode != null) {
                this.toastr.error(resizeBy.message, 'Updated User Info');
            } else {
                this.toastr.success('User Edited Successfully', 'Updated User Info');
                this.closeForm();
                window.location.reload();
            }

        });
    }

    public closeForm() {
        this.closeEditForm.next();
        $('#editForm').trigger('reset');
    }

}
