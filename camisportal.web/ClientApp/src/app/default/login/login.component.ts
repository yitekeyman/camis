import {Component, OnInit} from "@angular/core";
import {LoginUser} from "../../_models/user.model";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {AuthServices} from "../../_services/auth.services";
import swal from "sweetalert2";
@Component({
    selector:'app-login',
    templateUrl:'./login.component.html'
})

export class LoginComponent implements OnInit{
    public loggedIn:boolean =false;
    public user:LoginUser;
    public loginForm:FormGroup;
    public selectedRole:number;
    public selectedRegion:any;
    public routerLink:string|null;
    public loading:boolean=false;
    
    constructor(public router:Router, public authServices:AuthServices, public fb:FormBuilder){
        this.loginForm=fb.group({
            username:['', Validators.required],
            password:['',Validators.required]
        });
    }
    
    ngOnInit():void{
        this.routerLink=null;
        this.user={
            username:'',
            password:'',
        }
    }
    
    public login():void{
        this.loading=true;
        console.log(this.user);
        this.authServices.login(this.user).subscribe(res=> {
            this.selectedRole = res.role;
            this.selectedRegion=res.region;
            if (this.selectedRole == 2 || this.selectedRole == 3 || this.selectedRole==4) {
                localStorage.setItem('role', JSON.stringify(this.selectedRole));
                localStorage.setItem('username', JSON.stringify(this.user.username));
                localStorage.setItem('region', JSON.stringify(this.selectedRegion));
                this.router.navigate(['/admin/dashboard']);
                
            } else if (this.selectedRole == 1) {
                localStorage.setItem('role', JSON.stringify(this.selectedRole));
                localStorage.setItem('username', JSON.stringify(this.user.username));
                localStorage.setItem('region', JSON.stringify(this.selectedRegion));
                if(this.routerLink != null){
                    
                    this.router.navigate([this.routerLink]);
                }
                else{
                   
                    this.router.navigate(['/investor/inv-bids']);
                    
                }
               
            }

        },e=>{
            swal({
                type: 'error',
                title: 'Oops...',
                text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
            });
           this.loading=false;
        });
    }
    
    
}