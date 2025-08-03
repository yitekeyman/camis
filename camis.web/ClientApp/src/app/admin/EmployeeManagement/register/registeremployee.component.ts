import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormGroup} from '@angular/forms';
declare var $: any;
@Component({
    selector: 'app_employee_register',
    templateUrl: './registeremployee.component.html'
})

export class RegisteremployeeComponent implements OnInit {
    @Output() closeEditForm = new EventEmitter();
    ngOnInit(): void {}

    registrationForm = new FormGroup({});

    register() {}

    public closeForm() {
        this.closeEditForm.next();
        $('#registrationForm').trigger('reset');
    }
}
