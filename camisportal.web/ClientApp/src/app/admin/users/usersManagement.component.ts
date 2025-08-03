import {Component, OnInit} from "@angular/core";
import {SystemUserResponse} from "../../_models/user.model";
import {AuthServices} from "../../_services/auth.services";
import swal from 'sweetalert2'
import {Router} from "@angular/router";
import dialog from "../../shared/loader/loader_dialog";
import {ListServices} from "../../_services/list.services";

declare var $:any;
@Component({
    selector:'app-users',
    templateUrl:'./usersManagement.component.html'
})
export class UsersManagementComponent implements OnInit{
    public users:SystemUserResponse[]=[];
    public roleList:any[]=[];
    public regionList:any[]=[];
    public usersList:any[]=[];
    public selectedUser = null;
    public loading:boolean=false;
    public selectedRegion:any;
    public selectedRole:any;
    constructor(public authServices:AuthServices, public router:Router, public listService:ListServices) {


    }
    ngOnInit():void{
        this.loading=true;
        this.selectedRegion=JSON.parse(localStorage.getItem('region'));
        this.selectedRole=parseInt(localStorage.getItem('role'));
        this.getAllUser();
    }

    public getAllUser():void{
        
        this.authServices.getAllSystemUsers().subscribe(res=>{
            if(this.selectedRegion!="99"){
                for(const ur of res){
                    if(this.selectedRegion == ur.region){
                        this.users.push(ur);
                    }
                }
            }
            else{
                this.users=res;
            }
            this.getList();
            
            
        });
    }
    public deactivate(username:string):void {

        swal({
            title: 'Are you sure?',
            text: "You want to deactivate " + username+"!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Deactivate!'
        }).then((result) => {
            if (result.value) {
                dialog.loading();
                this.authServices.deactivateUser(username).subscribe(res=>{
                    swal("Success!", "you have successfully deactivate user"+ username, "success").then(value => {
                        this.usersList=[];
                        this.getAllUser();
                        dialog.close();
                    })

                },e=>
                {
                    swal({
                        type: 'error',
                        title: 'Oops...',
                        text: e.message
                    });

                });
            }
        })


    }
    public activate(username:string):void {
        swal({
            title: 'Are you sure?',
            text: "You want to activate " + username+"!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Activate!'
        }).then((result) => {
            dialog.loading();
            if (result.value) {
                this.authServices.activateUser(username).subscribe(res => {
                    

                        swal("Success!", "you have successfully deactivate user" + username, "success").then(value => {
                            this.usersList=[];
                            this.getAllUser();
                            dialog.close();
                        })


                    }, e=> {
                        swal({
                            type: 'error', title: 'Oops...', text: e.message
                        });
                    
                });
            }
        });
    }

    public editUser(username:string):void{
        this.router.navigate([`/admin/editUser/${username}`]);
    }
    
    getList(){
        this.listService.getRole().subscribe(data=>{
            this.roleList=data;
            this.listService.getRegion().subscribe(data2=>{
                this.regionList=data2;
                this.prepareRoleRegion();
            });
            this.loading=false;
        })
    }
    prepareRoleRegion(){
        for(const ur of this.users){
            for (const role of this.roleList) {
                for (const reg of this.regionList) {
                if (ur.role === role['id'] && ur.region === reg['code']) {
                    ur['role'] = role['name'];
                    ur['region'] = reg['Name'];
                    this.usersList.push(ur);
                }}
            }
        }
    }

    public resetPass(usr:any){
        this.selectedUser=usr;
        $('#updatePass').modal('show');
    }
    
}