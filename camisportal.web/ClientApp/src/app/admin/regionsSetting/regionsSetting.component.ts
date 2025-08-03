import {Component, OnInit} from "@angular/core";
import {AuthServices} from "../../_services/auth.services";
import {Router} from "@angular/router";
import {ListServices} from "../../_services/list.services";
import dialog from "../../shared/loader/loader_dialog";
import swal from "sweetalert2";

@Component({
    selector:'app_regions_setting',
    templateUrl:'./regionsSetting.component.html'
})

export class RegionsSettingComponent implements OnInit{
    public regions:any;
    public loading:boolean=false;
    public selectedRegion:string;
    public selectedRole:any;
    public regionModel:any;
    public isNew:boolean=true;
    public isUpdate:boolean=false;
    constructor(public authServices:AuthServices, public router:Router, public listService:ListServices ){}
    ngOnInit(){
        this.loading=true;
        this.selectedRegion=JSON.parse(localStorage.getItem('region'));
        this.selectedRole=parseInt(localStorage.getItem('role'));
        
        this.getAllRegions();
        
        this.regionModel={
            Code:'',
            Name:'',
            CamisUrl:''
        }
    }
    
    public getAllRegions(){
        this.authServices.getAllRegions().subscribe(reg=>{
            this.regions=reg;
            this.loading=false;
        });
    }
    
    public updateRegion():void{
        dialog.loading();
        if(this.isNew==true){
            this.authServices.addRegion(this.regionModel).subscribe(reg=>{
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You have successfully add new region',
                    showConfirmButton: false,
                    timer: 1500
                }).then(value => {
                    this.getAllRegions();
                    this.regionModel={
                        Code:'',
                        Name:'',
                        CamisUrl:''
                    };
                    this.isNew=true;
                    this.isUpdate=false;
                });
            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            })
        }
        else if(this.isUpdate==true){
            this.authServices.updateRegion(this.regionModel).subscribe(reg=>{
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You have successfully update new region',
                    showConfirmButton: false,
                    timer: 1500
                }).then(value => {
                    this.getAllRegions();
                    this.regionModel={
                        Code:'',
                        Name:'',
                        CamisUrl:''
                    };
                    this.isNew=true;
                    this.isUpdate=false;
                });
            },e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            })
        }
    }
    
    public editRegion(reg:any):void{
        this.isNew=false;
        this.isUpdate=true;
        this.regionModel.CamisUrl=reg.camisUrl;
        this.regionModel.Code=reg.code;
        this.regionModel.Name=reg.name;
    }
}