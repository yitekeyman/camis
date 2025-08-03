import {Component, Input, OnInit} from "@angular/core";
import {IMainShellRoute} from "../interface";
import {AuthServices} from "../../_services/auth.services";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import dialog from "../loader/loader_dialog";
import swal from "sweetalert2";
declare var mobile:any;
declare var mobile2:any;
declare var $:any;
@Component({
    selector:'app-menu-shell',
    templateUrl:'./main-body.component.html'
})
export class MainBodyComponent implements OnInit{
    
    @Input('routes')
    public routes:IMainShellRoute[]=[];
    public changePassForm:FormGroup;
    public userName:string= null;
    public fullName:string;
    public newPassword:string;
    public oldPassword:string;
    constructor(public authService:AuthServices, public fb:FormBuilder, public router:Router){
        this.changePassForm=this.fb.group({
            oldPassword:['', Validators.required],
            newPassword:['', Validators.required],
            confirmPassword:['', [Validators.required, MainBodyComponent.matchValidator]]
        });
    }
    
    ngOnInit():void{
        new mobile();
        this.userName=JSON.parse(<string>localStorage.getItem("username"));

        if(this.userName == undefined){
            this.userName=null;
        }
    }
    onclicklink(evt){
        new mobile2();
    }
    logout(){
        this.authService.logout();
    }

    changePass(){
        $("#changePass").modal('show');
    }

    static matchValidator(abs: AbstractControl){

        const control = abs.parent;
        if (control){
            const passwordCtrl = control.get('newPassword');
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

    updatePass(){
        dialog.loading();
        this.oldPassword=this.changePassForm.controls['oldPassword'].value;
        this.newPassword=this.changePassForm.controls['newPassword'].value;
        this.authService.changePassword(this.userName, this.oldPassword, this.newPassword).subscribe(res=>{
            dialog.close();
            swal("Success!", "you have successfully change your password", "success").then(value => {
                $("#changePass").modal('hide');
                this.logout();
                this.router.navigate(['/default/login']);
            })
        }, e=>{
            dialog.close();
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }
}