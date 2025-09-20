import {Component, EventEmitter, Input, Output, OnInit} from '@angular/core';
import {RegisterUser, UserModel} from '../user.model';
import {UserServices} from '../user.services';
import {AdminServices} from '../../admin.Services';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {forEach} from "@angular/router/src/utils/collection";

declare var $: any;

@Component({
  selector: 'app-user-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})

export class UpdateComponent implements OnInit {

  @Input() public userModel: UserModel;
  public editForm: FormGroup;
  public editModel: UserModel;
  public ROLES = [];
  @Output() closeEditForm = new EventEmitter();
  public selectedRoleIds:number[] = [];

  constructor(public userService: UserServices, public fb: FormBuilder, public toastr: ToastrService, public adminService: AdminServices, public router: Router) {
    this.editForm = fb.group({
      fullname: ['', Validators.required],
      phoneno: ['', Validators.required],
      email: ['', Validators.required],
      role: [[], Validators.required],
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
      email: this.userModel.email,
      status: this.userModel.status,
      roles: this.userModel.roles

    };
    const userRoleIds = this.userModel.roles.map(role => role.id);

    // Set the value of the 'role' form control to userRoleIds
    this.editForm.get('role').setValue(userRoleIds);
    $('#edit_user_role').val(this.editModel.roles);
  }

  public editUser() {
    if (this.editForm.valid) {
      // Create user data from form values
      const formValues = this.editForm.value;
      const userToUpdate: UserModel = {
        userName: formValues.username,
        fullName: formValues.fullname,
        phoneNo: formValues.phoneno,
        email: formValues.email,
        status: this.editModel.status,
        roles: formValues.role
      };

      this.userService.editUser(userToUpdate).subscribe(resizeBy => {
        if (resizeBy.errorCode != null) {
          this.toastr.error(resizeBy.message, 'Updated User Info');
        } else {
          this.toastr.success('User Edited Successfully', 'Updated User Info');
          this.closeForm();
          window.location.reload();
        }

      });
    } else {
      // Mark all fields as touched to show validation messages
      Object.keys(this.editForm.controls).forEach(key => {
        this.editForm.get(key).markAsTouched();
      });
    }
  }

  public closeForm() {
    this.closeEditForm.next();
    $('#editForm').trigger('reset');
  }

  isRoleSelected(roleId: number): boolean {
    const roleControl = this.editForm.get('role');
    const selectedRoles = roleControl ? roleControl.value : [];
    return selectedRoles.includes(roleId);
  }

  // Handle checkbox change events
  onRoleChange(event: any, roleId: number): void {
    const roleControl = this.editForm.get('role');
    const selectedRoles = roleControl ? roleControl.value : [];

    if (event.target.checked) {
      // Add role if checked
      if (!selectedRoles.includes(roleId)) {
        this.editForm.get('role').setValue([...selectedRoles, roleId]);
      }
    } else {
      // Remove role if unchecked
      this.editForm.get('role').setValue(selectedRoles.filter((id: number) => id !== roleId));
    }

    // Mark the control as touched for validation
    this.editForm.get('role').markAsTouched();
  }

  // Get the number of selected roles
  getSelectedCount(): number {
    const roleControl = this.editForm.get('role');
    return roleControl && roleControl.value ? roleControl.value.length : 0;
  }

  // Get the selected role objects
  getSelectedRoles(): any[] {
    const roleControl = this.editForm.get('role');
    const selectedIds = roleControl ? roleControl.value : [];
    return this.ROLES.filter(role => selectedIds.includes(role.id));
  }
}
