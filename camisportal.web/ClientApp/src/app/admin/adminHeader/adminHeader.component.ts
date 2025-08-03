import {Component, OnInit} from "@angular/core";
import {IChildShellRoute, IMainShellRoute} from "../../shared/interface";
import {AuthServices} from "../../_services/auth.services";
import swal from "sweetalert2";
import {Router} from "@angular/router";
declare var $:any;
@Component({
    selector:'app-admin',
    templateUrl:'./adminHeader.component.html'
})
export class AdminHeaderComponent implements OnInit{
    username:string;
    child:IChildShellRoute[];
    routes:IMainShellRoute[];
    constructor(public authService:AuthServices, public router:Router){}
    ngOnInit() {
        this.username = JSON.parse(<string>localStorage.getItem("username"));
        this.checkSession();
        this.child = [{
            route: 'change_password', title: 'Change Password', icon: '', click: 'changePass()'
        }];

        this.routes = [{
            route: 'dashboard', title: 'Dashboard', icon: '', click: '', child: null
        },{
            route: 'manageUsers', title: 'Users', icon: '', click: '', child: null
        },
            {
                route: 'manageBid', title: 'Promotion', icon: '', click: '', child: null
            },
            {
            route: '', title: this.username, icon: 'fa-angle-down', click: '', child: this.child
        }];
    }
    
    checkSession(){
        this.authService.checkSession(this.username).subscribe(res=>{
            
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            }).then(value => {
                this.router.navigate(['/default/login']);
                localStorage.setItem('username','');
            });
        })
    }
    
    
    
}