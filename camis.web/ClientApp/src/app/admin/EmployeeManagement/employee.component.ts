import {Component, OnInit} from '@angular/core';

declare var $: any;
@Component ({
    selector: 'app_employee',
    templateUrl: './employee.component.html'
})
export class EmployeeComponent  implements OnInit {
    public isRegisterShowed = false;
    public isEdit = false;
    ngOnInit(): void {

    }

    public closeForm(evt: any) {
        this.isRegisterShowed = false;
        this.isEdit = false;
        $('#editEmployee').removeClass('in');
        $('#editEmployee').hide();
    }

    public addBtnClick(): void {
        this.isRegisterShowed = true;
        this.isEdit = false;
        this.showRegistrationForm();
    }

    public showRegistrationForm(): void {
        $('#editEmployee').addClass('in');
        $('#editEmployee').show();
    }
}
