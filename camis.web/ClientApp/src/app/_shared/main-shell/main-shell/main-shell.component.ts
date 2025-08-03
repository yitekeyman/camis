import {Component, Input, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {AdminServices} from '../../../admin/admin.Services';
import {IMainShellRoute} from '../interfaces';

declare var init_sidebar: any;
declare var $: any;

@Component({
    selector: 'app-main-shell',
    templateUrl: 'main-shell.component.html'
})
export class MainShellComponent implements OnInit {

    @Input('routes')
    public routes: IMainShellRoute[] = [];

    public menuShowed = false;
    public storage: WindowLocalStorage;
    public username: string | null;

    constructor (private adminService: AdminServices, public router: Router) {
    }

    ngOnInit(): void {
        new init_sidebar();
        this.username = localStorage.getItem('username');

    }

    showdropDown() {
        this.menuShowed = !this.menuShowed;

        if (this.menuShowed) { $('.dropdown-menu').show(); } else { $('.dropdown-menu').hide(); }
    }

    logout() {
        // window.location.reload();
        this.adminService.logout();
        this.router.navigate(['login']);
    }

}
