import {Component, OnInit} from "@angular/core";
import {BidServices} from "../../_services/bid.services";
import {ActivatedRoute, Router} from "@angular/router";
import {Form, FormBuilder, FormGroup, PatternValidator, RequiredValidator, Validator, Validators} from "@angular/forms";
import {InvestorServices} from "../../_services/investor.services";
import {c, v} from "@angular/core/src/render3";
import {
    ContactPerson,
    InvestorModel,
    Application,
    ProposalDocument,
    RegDocument,
    Registration
} from "../../_models/investor.model";
import swal from "sweetalert2";
import {ListServices} from "../../_services/list.services";
import {PATTERN_VALIDATOR} from "@angular/forms/src/directives/validators";

declare var $:any;
@Component({
    selector:'app-bid-reg',
    templateUrl:'./investorBidRegistration.component.html'
})
export class InvestorBidRegistrationComponent implements OnInit{

    public userRole:number|null;
    public username:string|"";
    public investorProfileForm:FormGroup;
    public registrationForm:FormGroup;
    public proposalDocForm:FormGroup;
    public contactForm:FormGroup;
    public proposalAbstractForm:FormGroup;

    public promotionID:string;
    public  query:any;
    public searchResult:any[];
    public selectedPromotion:any;

    public step:number|0;
    public step1Class:string;
    public step2Class:string|"disabled";
    public step3Class:string|"disabled";


    public investorModel:InvestorModel;
    public registrationModel:Registration;
    public regDocument:RegDocument;
    public proposalDocumentModel:ProposalDocument;
    public participatePro:Application;
    public contactModel:ContactPerson;
    public registrations:any[];
    public proDocuments:any[];
    opAddress: string | null = null;

    requestedInvestor:any;
    profile:any;
    header:string;
    requestedProfile:any;
    isEdit:boolean|false;
    isNew:boolean|false;
    investorId:string|'';

    docTypeList:any[]=[];
    docAuthList:any[]=[];

    regDoc:any[]=[];
    regModel:any;
    
    loading:boolean=false;
    public promotionUnitId:any;

    constructor(public activatedRoute:ActivatedRoute, public router:Router, public bidService:BidServices, public fb:FormBuilder, public investorServices:InvestorServices, public listService:ListServices){
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
            type:['', Validators.required],
            authority:['', Validators.required],
            regNumber:['',Validators.required],
            refNumber:['']
        });

        this.proposalDocForm=this.fb.group({
            referenceNo:['', Validators.required],
            document:['', Validators.required]
        });

        this.contactForm=this.fb.group({
            contactName:[''],
            contactEmail:['', Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$')],
            contactPhone:['', [Validators.pattern('^[0-9+]+$'), Validators.minLength(10), Validators.maxLength(13)]]
        });
        
        this.proposalAbstractForm=this.fb.group({
            proposalAbstract:[''],
            capital:['', [Validators.required, Validators.min(1)]],
            investmentType:['', Validators.required]
        })
    }

    ngOnInit(){
        this.loading=true;
        this.userRole=JSON.parse(<string>localStorage.getItem("role"));
        this.username=JSON.parse(<string>localStorage.getItem("username"));
        this.promotionID=this.activatedRoute.snapshot.params['prom_id'];
        this.promotionUnitId=this.activatedRoute.snapshot.params['promUnit_id'];

        this.query={
            region:'',
            states:[]
        };
        this.getPromotionUnit();

        if (!this.step) {
            this.step = 1;
            this.step1Class="selected";
            this.step2Class="disabled";
            this.step3Class="disabled";
        }

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

        this.proposalDocumentModel={
            data:'',
            mime:'',
            docRef:'',
            documentType:0
        };

        this.contactModel={
            id:'',
            name:'',
            phone:'',
            email:''
        }
        this.investorServices.getInvestor(this.username).subscribe(res=>{
            this.requestedInvestor=res;
            if(res==null){
                this.header="Register Your Profile";
                this.isNew=true;
                this.loading=false;
            }
            else {
                this.requestedProfile=JSON.parse(<string>this.requestedInvestor.defaultProfile);
                this.isEdit=true;
                this.investorId=this.requestedInvestor.id;
                this.header="Update Your Profile";
                this.investorModel=this.requestedProfile;
                this.opAddress=this.investorModel.AddressId;
                this.registrations=this.investorModel.Registrations;
                this.getRegList();
                this.loading=false;
            }
        });

        this.participatePro={
            invProfile:null,
            proposalDocument:[],
            contactAddress:'',
            investmentTypes:[],
            proposalAbstract:'',
            proposedCapital:null,
            promoID:this.promotionID,
            applicationTime:new Date(),
            promotionUnitId:this.promotionUnitId
        }
        
        
    }

    getPromotionUnit(){
        this.bidService.getPromotionUnit(this.promotionUnitId, this.promotionID).subscribe(res=>{
            this.selectedPromotion=res;
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
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
        this.getRegList();
        $("#add-document-dialog").modal('hide');
        this.registrationModel={

            TypeId:0,
            AuthorityName:'',
            AuthorityId:0,
            TypeName:'',
            RegistrationNumber:'',
            Document:this.regDocument

        };
    }
    showAddDocumentModal(){
        $("#add-document-dialog").modal('show');
    }
    public removeReg(index: number): void {

        this.investorModel.Registrations.splice(index, 1);
        this.registrations=this.investorModel.Registrations;
        this.getRegList();
    }

    saveProfile(){
        this.investorModel.AddressId=this.opAddress;
        this.profile=JSON.stringify(this.investorModel);
        this.participatePro.invProfile={Id:this.investorId,UserName:this.username, DefaultProfile:this.profile};
        this.step2Class="selected";
        this.step1Class="disabled";
        this.step=2;
    }

    //upload proposal Document

    public docUploadTrigger():void{

        $("#uploadDocumentPro").click();

    }

    public uploadDocument(evt:any):void{
        let files = evt.target.files;
        let fileName=$('#uploadDocumentPro').val();
        let file = files[0];


        if (files && file) {

            let reader = new FileReader();

            this.proposalDocumentModel.mime = file.type;
            reader.onload = this.onLoadDocument.bind(this);

            reader.readAsBinaryString(file);
            $('#DocumentLabel').val(fileName);
        }
    }

    private onLoadDocument(readerEvt: any): void {
        let binaryString = readerEvt.target.result;
        this.proposalDocumentModel.data = btoa(binaryString);
    }
    showDialog(){
        $("#add-pro-document-dialog").modal('show');
    }

    addProposalDocument(){
        this.participatePro.proposalDocument.push(this.proposalDocumentModel);
        this.proDocuments=this.participatePro.proposalDocument;
        $("#add-pro-document-dialog").modal('hide');
        this.proposalDocumentModel={
            data: '',
            mime: '',
            docRef: '',
            documentType: 0
        };
        $('#DocumentLabel').val('');
    }

    public removeDoc(index:number):void{

        this.participatePro.proposalDocument.splice(index, 1);
        this.proDocuments=this.participatePro.proposalDocument;
    }
    public backTo1(){
        this.step2Class="disabled";
        this.step1Class="selected";
        this.step=1;
    }
    
    
    public saveProposalAbs(){
        this.participatePro.proposedCapital=this.proposalAbstractForm.controls['capital'].value;
        this.participatePro.proposalAbstract=this.proposalAbstractForm.controls['proposalAbstract'].value;
        this.participatePro.investmentTypes=this.proposalAbstractForm.controls['investmentType'].value;
        this.step2Class="disabled";
        this.step3Class="selected";
        this.step=3;
    }

    //register contact person

    public savePromotionSubmission(){
        this.participatePro.contactAddress=this.contactModel;
        this.bidService.applyForPromotion(this.participatePro).subscribe(res=>{
            swal({
                type: 'success', title: 'Success', text: 'You Have Successfully Apply For Promotion'
            }).then(value => {
                this.router.navigate(['/investor/inv-bids']);
                localStorage.setItem('routerLink','');
            })
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }

    public backTo2(){
        this.step2Class="selected";
        this.step3Class="disabled";
        this.step=2;
      
    }
    getRegList(){
        this.listService.getAuthority().subscribe(data=>{
            this.docAuthList=data;
            this.listService.getRegistrationType().subscribe(data2=>{
                this.docTypeList=data2;
                this.prepareAuth();
            })
        })
    }
    prepareAuth(){
        this.regDoc=[];
        for(const reg of this.registrations){
            for(const auth of this.docAuthList){
                if(auth['id']==reg['AuthorityId']){
                    for(const docTy of this.docTypeList){
                        if(docTy["id"]==reg['TypeId']){
                            this.regModel.AuthorityName=auth["name"]
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

    changeAddress(){
        this.investorModel.AddressId='';
    }
    
    change(){
        if(this.investorProfileForm.controls['nationality'].errors!=null && this.investorProfileForm.controls['nationality'].errors.pattern!=null)
        {
            console.log('error messeage');
        }
    }
}