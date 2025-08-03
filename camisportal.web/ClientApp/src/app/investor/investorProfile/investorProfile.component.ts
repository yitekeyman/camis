import {Component, OnInit} from "@angular/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {InvestorModel, RegDocument, Registration} from "../../_models/investor.model";
import {InvestorServices} from "../../_services/investor.services";
import {Router} from "@angular/router";
import swal from "sweetalert2";
import {ListServices} from "../../_services/list.services";
import dialog from "../../shared/loader/loader_dialog";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";

declare var $:any;
@Component({
    selector:'.app-inv-profile',
    templateUrl:'./investorProfile.component.html'
})
export class InvestorProfileComponent implements OnInit{
    public investorProfileForm:FormGroup;
    public registrationForm:FormGroup;
    public investorModel:InvestorModel;
    public registrationModel:Registration;
    public regDocument:RegDocument;
    public registrations:any[];
    public regi:any[];
    public reqRegistrations:any[]=[];
    opAddress: string | null = null;
    investorProfile:any;
    requestedInvestor:any;
    username:string;
    profile:any;
    header:string;
    requestedProfile:any;
    isEdit:boolean|false;
    isRegister:boolean|false;
    investorId:string=null;
    
    docTypeList:any[]=[];
    docAuthList:any[]=[];
    invTypeList:any[]=[];
    invType:any;
    invOriginList:any[]=[];
    invOrigin:any;
    regDoc:any[]=[];
    regModel:any;
    
    
    

    constructor(public investorService:InvestorServices, public fb:FormBuilder, public router:Router, public listService:ListServices){
        this.investorProfileForm=this.fb.group({
            investorName:['', [Validators.required, Validators.pattern('^[a-zA-Z ]+$')]],
            nationality:['',[Validators.required, Validators.pattern('^[a-zA-Z ]+$')]],
            operatorType:[0, [Validators.required,Validators.min(1)]],
            operatorOrigin:[0,[Validators.required,Validators.min(1)]],
            capital:['',[Validators.required, Validators.min(1)]],
            phoneNo:['',[Validators.required, Validators.pattern('^[0-9+]+$'), Validators.minLength(9), Validators.maxLength(13)]],
            email:['',[Validators.required, Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')]]
        });

        this.registrationForm=this.fb.group({
            type:[0, Validators.required],
            authority:[0, Validators.required],
            regNumber:['',Validators.required],
            refNumber:['']
        })
    }
    ngOnInit():void{
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        
        this.investorService.getInvestor(this.username).subscribe(res=>{
            this.requestedInvestor=res;
            if(res==null){
                this.header="Register Your Profile";
                this.isRegister=true;
            }
            else {
                this.header="Your Profile Details";
                
                this.requestedProfile=JSON.parse(<string>this.requestedInvestor.defaultProfile);
                this.registrations=this.requestedProfile.Registrations;
                this.getRegDocList();
            }
        });
        this.regDocument={

            File:'',
            Mimetype:'',
            FileName:'',
            Date:new Date().getTime(),
            Ref:'',
            Note:'',
            Type:0
        };
        this.registrationModel={

            TypeId:0,
            TypeName:'',
            AuthorityId:0,
            AuthorityName:'',
            RegistrationNumber:'',
            Document:this.regDocument

        };
        this.regModel={

            TypeId:0,
            TypeName:'',
            AuthorityId:0,
            AuthorityName:'',
            RegistrationNumber:'',
            Document:this.regDocument

        };
        this.investorModel={

            Name:'',
            Nationality:'',
            TypeId:0,
            OriginId:0,
            Capital:'',
            Phone:'',
            Email:'',
            AddressId:'',
            Registrations:[]
        };

    }

    public regDocumentUploadTrigger():void{
      
            $("#uploadRegistrationDocumentPro").click();
       
    }

    public uploadRegistrationDocument(evt:any):void{
        let files = evt.target.files;
        let fileName=$('#uploadRegistrationDocumentPro').val();
        let file = files[0];


        if (files && file) {

            let reader = new FileReader();

            this.regDocument.Mimetype = file.type;
            this.regDocument.FileName="Registration";
            reader.onload = this.onLoadRegistrationDocument.bind(this);

            reader.readAsBinaryString(file);
            $('#registrationDocumentLabel').val(fileName);
        }
    }

    private onLoadRegistrationDocument(readerEvt: any): void {
        let binaryString = readerEvt.target.result;
        this.regDocument.File = btoa(binaryString);
    }

    public saveDocument(){
        this.investorModel.Registrations.push(this.registrationModel);
        this.registrations=this.investorModel.Registrations;
        this.getRegDocList();
        $("#add-document-dialog").modal('hide');
        console.log(this.regDoc);
    }
    showAddDocumentModal(){
        $("#add-document-dialog").modal('show');
    }
    removeReg(index: number): void {
        this.investorModel.Registrations.splice(index, 1);
        this.registrations=this.investorModel.Registrations;
        this.getRegDocList();
    }

    public saveProfile(){
        dialog.loading;
        this.investorModel.AddressId=this.opAddress;
        this.profile=JSON.stringify(this.investorModel);
        this.investorProfile={Id:this.investorId, DefaultProfile:this.profile};
        
        if(this.investorProfile.Id==null){
            console.log(this.investorProfile);
       this.investorService.registerInvestor(this.investorProfile).subscribe(res=> {
dialog.close();
           swal({
               type: 'success',
               title: 'You have successfully register your profile',
               showConfirmButton: false,
               timer: 1500
           });
           this.router.navigate(['investor/inv-bids']);
       },e=>{
           swal({
               type: 'error', title: 'Oops...', text: e.message
           });
       });
        }
        else{
            this.investorService.updateInvestor(this.investorProfile).subscribe(res=> {
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You have successfully update your profile',
                    showConfirmButton: false,
                    timer: 1500
                });
                window.location.reload();
            },e=>{
                    swal({
                        type: 'error', title: 'Oops...', text: e.message
                    });
            });
        }
    }
    
    updateProfile(){
        this.isEdit=true;
        this.investorId=this.requestedInvestor.id;
        this.header="Update Your Profile";
        this.investorModel=this.requestedProfile;
        this.opAddress=this.investorModel.AddressId;
        this.regi=this.investorModel.Registrations;
        this.getRegDocList();
    }
    
   
    getRegDocList(){
        this.listService.getAuthority().subscribe(data=>{
            this.docAuthList=data;
            this.listService.getRegistrationType().subscribe(data2=>{
                this.docTypeList=data2;
                this.prepareRegDoc();
            })
        });
        
        this.listService.getInvestorOrigin().subscribe(data=>{
            this.invOriginList=data;
            this.prepareInvestorOrigin();
        });
        
        this.listService.getOperationType().subscribe(data=>{
            this.invTypeList=data;
            this.prepareInvestorType();
        })
    }
    prepareRegDoc(){
        this.regDoc=[];
        for(const reg of this.registrations){
            for(const auth of this.docAuthList){
                if(auth['id']==reg['AuthorityId']){
                for(const docTy of this.docTypeList){
                    if(docTy["id"]==reg['TypeId']){
                        this.regModel.AuthorityName=auth["name"];
                        this.regModel.AuthorityId=reg['AuthorityId'];
                        this.regModel.TypeName=docTy["name"];
                        this.regModel.TypeId=reg['TypeId'];
                        this.regModel.RegistrationNumber=reg.RegistrationNumber;
                        this.regModel.Document=reg.Document;
                        
                       
                    }
                }
                    this.regDoc.push(this.regModel);
                    this.regModel={

                        TypeId:0,
                        TypeName:'',
                        AuthorityId:0,
                        AuthorityName:'',
                        RegistrationNumber:'',
                        Document:this.regDocument

                    };
                }
            }
        }
    }
    
    prepareInvestorOrigin(){
        for(const org of this.invOriginList){
            if(org["id"]==this.requestedProfile.OriginId){
                this.invOrigin=org["name"];
            }
        }
    }

    prepareInvestorType(){
        for(const type of this.invTypeList){
            if(type["id"]==this.requestedProfile.TypeId){
                this.invType=type["name"];
            }
        }
    }
    
    changeAddress(){
        this.investorModel.AddressId='';
    }
    openDoc(investorID:string, index:number){
        const overrideFilePath=`${configs.url}Investor/GetRegistrationDoc?investorId=${investorID}&index=${index}`;
        $("#view-document-dialog").modal("show");
        $("#docIframe").attr("src", overrideFilePath);
    }
}