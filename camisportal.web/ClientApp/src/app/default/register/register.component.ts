import {Component, OnInit} from "@angular/core";
import {LoginUser, SystemUserRequest} from "../../_models/user.model";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {AuthServices} from "../../_services/auth.services";
import swal from "sweetalert2";
import dialog from "../../shared/loader/loader_dialog";

declare var $:any;

@Component({
    selector:'app-register',
    templateUrl:'./register.component.html'
})

export class RegisterComponent implements OnInit{
    public userModel:SystemUserRequest;
    public userForm:FormGroup;
    public newPassConfirm: string|'';
    public user:LoginUser;
    public valid:boolean;
    public selectedRole:number|null;
    public usernameLoad:boolean=false;
    public emailLoad:boolean=false;
    public validUsername:any;
    public validEmail:any;
    
    public ROLES:any[];
    constructor(public fb:FormBuilder, public router:Router, public authService:AuthServices){
        this.userForm=fb.group({
            fullname:['', [Validators.required, Validators.pattern('^[a-zA-Z ]+$')]],
            phoneNo:['', [Validators.required,  Validators.pattern('^[0-9+]+$'), Validators.minLength(10), Validators.maxLength(13)]],
            email:['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]],
            username:['', [Validators.required]],
            password:['', Validators.required],
            confirmPassword:['', [Validators.required, RegisterComponent.matchValidator]]

        });
    }
    ngOnInit():void{
        
        this.userModel={
            FullName:'',
            Role:1,
            PhoneNo:'',
            EMail:'',
            Region:'99',
            UserName:'',
            Password:'',
            CamisUserName:'',
            CamisPassword:''
        };

    }
  
    public checkUser(ur:string){
        if(this.userForm.controls['username'].errors==null) {
            this.usernameLoad = true;
            this.authService.checkUser(ur).subscribe(res => {
                if (res != '') {
                    this.validUsername = false;
                } else {
                    this.validUsername = true;
                }
                this.usernameLoad = false;
            })
        }
        else{this.validUsername=null}
          
        
    }
    
    public checkEmail(em:string){
               
                if(this.userForm.controls['email'].errors==null) {
                    this.emailLoad=true;
                    this.authService.checkEmail(em).subscribe(res => {
                        if (res != '') {
                            this.validEmail = false;
                        } else {
                            this.validEmail = true;
                        }
                        this.emailLoad = false;
                    })
                }
                else {this.validEmail=null}
               
    }

    static matchValidator(abs: AbstractControl){

        const control = abs.parent;
        if (control){
            const passwordCtrl = control.get('password');
            const confirmPassCtrl = control.get('confirmPassword');

            if(passwordCtrl && confirmPassCtrl){
                const pass = passwordCtrl.value;
                const confirmPass = confirmPassCtrl.value;

                if(pass != confirmPass){
                    return {matchPassword : true};
                }
                else {
                    return null;
                }
            }


        }

        else{
            return null;
        }
    }

    public validatePassword() {

        if ($('#conf_pass').val() != $('#user_pass').val()) {
            this.newPassConfirm = 'New Password and Confirm Password Mismatch';
            this.valid = false;
        } else {
            this.newPassConfirm = '';
            this.valid = true;
        }

    }
  
    public register(){
        dialog.loading();
        console.log(this.userModel);
        this.authService.registerUser(this.userModel).subscribe(res=>{
           
                swal({
                    type: 'success',
                    title: 'You have successfully create account',
                    showConfirmButton: false,
                    timer: 1500
                });
                    this.user={
                        username:this.userModel.UserName,
                        password:this.userModel.Password
                    };

                    this.authService.login(this.user).subscribe(res=>{
                       
                            this.selectedRole=res.role;
                            if (this.selectedRole == 2 || this.selectedRole==3) {
                                localStorage.setItem('role', JSON.stringify(this.selectedRole));
                                localStorage.setItem('username',JSON.stringify(this.user.username));
                                this.router.navigate(['/admin/dashboard'])
                                    .then(() => dialog.close());
                            } else if (this.selectedRole == 1) {
                                localStorage.setItem('role', JSON.stringify(this.selectedRole));
                                localStorage.setItem('username',JSON.stringify(this.user.username));
                                this.router.navigate(['/investor/inv-profile'])
                                    .then(() => dialog.close());
                            }
                        
                    },e=>{
                        swal({
                            type: 'error', title: 'Oops...', text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
                        });
                    });
            
                 
            
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e && (e.message || JSON.stringify(e)) || 'Unknown error.'
            });
        });
    }
}