import {Component, Input, OnInit} from "@angular/core";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthServices} from "../../_services/auth.services";
import {SystemUserRequest} from "../../_models/user.model";
import swal from "sweetalert2";
import dialog from "../../shared/loader/loader_dialog";
declare var $:any;
@Component({
    selector:'app-reset-pass',
    templateUrl:'./resetPassword.component.html'
})
export class ResetPasswordComponent implements OnInit{
    
    public resetPassForm:FormGroup;
    public userName:string;
    public fullName:string;
    public newPassword:string;
    
    @Input() public userModel:any;
    constructor(public authService:AuthServices, public fb:FormBuilder){
        this.resetPassForm=this.fb.group({
            newPassword:['', Validators.required],
            confirmPassword:['', [Validators.required, ResetPasswordComponent.matchValidator]]
        });
    }
    
    ngOnInit(){
        this.fullName=this.userModel.fullName;
        this.userName=this.userModel.userName;
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
        this.newPassword=this.resetPassForm.controls['newPassword'].value;
        this.authService.resetPassword(this.userName, this.newPassword).subscribe(res=>{
            dialog.close();
            this.resetPassForm.reset();
            $('#updatePass').modal('hide');
            swal("Success!", "you have successfully Reset " + this.userName +" password", "success").then(value => {
               
            })
        }, e=>{
            dialog.close();
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }
}