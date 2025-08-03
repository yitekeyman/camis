import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpModule} from '@angular/http';
import {UserComponent} from './user.component';
import {RegisterComponent} from './register/register.component';
import {UpdateComponent} from './update/update.component';
import {ResetpasswordComponent} from './resetPassword/resetpassword.component';
import {UserServices} from './user.services';
import {EmployeeComponent} from '../EmployeeManagement/employee.component';
import {RegisteremployeeComponent} from '../EmployeeManagement/register/registeremployee.component';


@NgModule({
    imports: [CommonModule, HttpModule, FormsModule, ReactiveFormsModule],
    declarations: [UserComponent, RegisterComponent, UpdateComponent, ResetpasswordComponent, EmployeeComponent, RegisteremployeeComponent],
    providers: [UserServices],
    exports: [UserComponent]
})
export class UserModule {

}
