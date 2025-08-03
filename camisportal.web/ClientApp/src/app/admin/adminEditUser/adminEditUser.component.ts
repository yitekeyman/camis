import {Component, OnInit} from "@angular/core";
import {FormBuilder, FormGroup, Validators, AbstractControl} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {AuthServices} from "../../_services/auth.services";
import {SystemUserRequest} from "../../_models/user.model";
import {forEach} from "@angular/router/src/utils/collection";
import {ListServices} from "../../_services/list.services";
import {ValueTransformer} from "@angular/compiler/src/util";
import swal from 'sweetalert2';
import {el} from "@angular/platform-browser/testing/src/browser_util";
import dialog from "../../shared/loader/loader_dialog";
declare var $:any;
@Component({
    selector:'app-edit-user',
    templateUrl:'./adminEditUser.component.html'
})
export class AdminEditUserComponent implements OnInit{
    
    public isRegistration:boolean=true;
    public isEdit:boolean=false;
    public userModel:SystemUserRequest;
    public editedUser:string;
    public editUserForm:FormGroup;
    public currentUserRole:number=null;
    public currentUsername:string='';
    public users:any[];
    public availableUsername:boolean=false;
    public newPassConfirm: string='';
    public valid: boolean=true;
    public REGIONS:any[];
    public title:string='Register';
    
    
    constructor(public activatedRoute:ActivatedRoute, public fb:FormBuilder, public router:Router, public authService:AuthServices, public listService:ListServices){
      
        
       
    }
    ngOnInit():void{
       
        this.getRegions();
        this.userModel={
            FullName:'',
            Role:0,
            PhoneNo:'',
            EMail:'',
            Region:'0',
            UserName:'',
            Password:'',
            CamisPassword:'',
            CamisUserName:''
        };
        this.editedUser=this.activatedRoute.snapshot.params['username'];
        
        if(this.editedUser!=null || this.editedUser != undefined){
            this.isRegistration=false;
            this.isEdit=true;
            this.title='Update';
            this.authService.getUser(this.editedUser).subscribe(res=>{
                this.userModel.FullName=res.fullName;
                this.userModel.Role=res.role;
                this.userModel.Region=res.region;
                this.userModel.UserName=res.userName;
                this.userModel.EMail=res.eMail;
                this.userModel.PhoneNo=res.phoneNo;
                this.userModel.CamisUserName=res.camisUserName;
                this.userModel.CamisPassword=res.camisPassword;
            });
        }
        if(this.isRegistration==true) {
            this.editUserForm = this.fb.group({
                fullname: ['', [Validators.required, Validators.pattern('^[a-zA-Z ]+$')]],
                role: ['0', [Validators.required, Validators.min(1)]],
                phoneNo: ['', [Validators.required,  Validators.pattern('^[0-9+]+$'), Validators.minLength(10), Validators.maxLength(13)]],
                email: ['', [Validators.required,  Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]],
                region: ['0', [Validators.required, Validators.min(1)]],
                CAMISUsername: ['', Validators.required],
                CAMISPassword: ['', Validators.required],
                username: ['', Validators.required],
                password: ['', Validators.required],
                confirmPassword: ['', [Validators.required, AdminEditUserComponent.matchValidator]]
            });
        }
        else{
            this.editUserForm = this.fb.group({
                fullname: ['',  [Validators.required, Validators.pattern('^[a-zA-Z ]+$')]],
                role: ['0', [Validators.required, Validators.min(1)]],
                phoneNo: ['', [Validators.required,  Validators.pattern('^[0-9+]+$'), Validators.minLength(10), Validators.maxLength(13)]],
                email: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]],
                region: ['0', [Validators.required,  Validators.min(1)]],
                CAMISUsername: ['', Validators.required],
                CAMISPassword: ['', Validators.required],
                username: ['', Validators.required],
            });
        }
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
    public registerUser(){
        dialog.loading();
        if(this.isEdit==true){
            this.authService.updateUser(this.userModel).subscribe(res=> {
                dialog.close();
                swal({
                    type: 'success',
                    title: '"You have Update user',
                    showConfirmButton: false,
                    timer: 1500
                }).then(value => {
                    this.router.navigate(['/admin/manageUsers']);
                });

            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });
        }
        else{
            this.authService.registerUser(this.userModel).subscribe(res=> {
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You have registered new user ('+this.userModel.FullName+')',
                    showConfirmButton: false,
                    timer: 1500
                }).then(value => {
                    this.router.navigate(['/admin/manageUsers']);
                });

            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });
        }
       
    }
    
    public getRegions(){
        this.listService.getRegion().subscribe(res=>{
            this.REGIONS=res;
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }
}