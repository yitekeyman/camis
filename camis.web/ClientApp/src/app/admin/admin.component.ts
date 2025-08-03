import {Component, OnInit, AfterContentInit} from '@angular/core';
import {Router} from '@angular/router';
import {AdminServices} from './admin.Services';

declare var init_sidebar: any;
declare var $: any;
@Component({
    selector: 'app-admin',
    templateUrl: './admin.component.html'
})
export class AdminComponent implements OnInit, AfterContentInit {
    public menuShowed = false;
    public storage: WindowLocalStorage;
    public username: string|null;

    constructor(public adminService: AdminServices, public router: Router) {

    }
    ngOnInit(): void {

        this.username = localStorage.getItem('username');


    }
    ngAfterContentInit(): void {
        new init_sidebar();
    }
    public showdropDown() {
        this.menuShowed = !this.menuShowed;
        if (this.menuShowed) {
            $('.dropdown-menu').show();
        } else {
            $('.dropdown-menu').hide();
        }
    }

    public logout(): void {
        this.adminService.logout();
        this.router.navigate(['login']);
    }
}
