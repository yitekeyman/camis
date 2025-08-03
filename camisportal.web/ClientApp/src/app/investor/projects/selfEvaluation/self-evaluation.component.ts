import {Component, OnInit} from "@angular/core";
import {ProjectServices} from "../../../_services/project.services";
import {ActivatedRoute, Router} from "@angular/router";
import {configs} from "../../../app-config";
import {RegDocument} from "../../../_models/investor.model";
import {document} from "../../../_models/bid.model";
import {InvestorServices} from "../../../_services/investor.services";
import swal from "sweetalert2";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";

declare var $:any;

@Component({
    selector:'.app-self-evaluation',
    templateUrl:'./self-evaluation.component.html'
})
export class SelfEvaluationComponent implements OnInit{
    
    public project:any;
    public investorID:string;
    public promotionUnitId:string;
    public activityPlan: any = {
        reportDocuments: []
    };
    error:boolean=false;
    loading:boolean=false;

    readonly readonlyReporting = false;
    readonly urlPrefix = configs.url;

    statusTypes: any[] = [];
    
    document:document;
    
    constructor(public projectService:ProjectServices, public router:Router, public activeRoute:ActivatedRoute, public investorService:InvestorServices, public keyCase:ObjectKeyCasingService){}
    ngOnInit(){
        this.loading=true;
        this.promotionUnitId=this.activeRoute.snapshot.params['promoUnitId'];
        this.investorID=this.activeRoute.snapshot.params['investorId'];
        
        this.getWorkflow();
        
        this.document={
            file:'',
            filename:'',
            date:'',
            type:0,
            mimetype:'',
            ref:'',
            note:''
        }
    }
    
    getWorkflow(){
        this.investorService.getLatestApplication(this.promotionUnitId, this.investorID).subscribe(pro=>{
            this.project=pro.investment;
            this.activityPlan=pro.activityPlan;
            this.activityPlan.reportStatusId = 1;
            this.activityPlan.reportDocuments = [];
            this.projectService.getStatus().subscribe(data3=>{
                this.statusTypes=data3;
            });
            this.loading=false;
        })
        /*this.projectService.getProject().subscribe(data=>{
            for(const pro of data){
                if(pro.id==this.projectID){
                    this.project=pro;
                    this.projectService.getWorkflow().subscribe(data2=>{
                        for(const wf of data2){
                            if(wf.data.rootActivityId==pro.activityId){
                                this.activityPlan=wf.data;
                                
                                if(!this.activityPlan.reportDocuments){
                                    this.activityPlan.reportDocuments = [];
                                }
                                
                                this.projectService.getStatus().subscribe(data3=>{
                                    this.statusTypes=data3;
                                    if (this.statusTypes.length && !this.activityPlan.reportStatusId) {
                                        this.activityPlan.reportStatusId = this.statusTypes[0].id;
                                    }
                                });
                            }
                            
                        }
                    })
                }
            }
            this.loading=false;
        })*/
    }

    public DocUploadTrigger(): void {
        $("#uploadDoc").click();
    }

    public handleDocumentUpload(evt: any): void {
        let files = evt.target.files;
        let fileName = $('#uploadDoc').val().replace(/\\/gi, '/').split('/').pop();
        let file = files[0];


        if (files && file) {

            let reader = new FileReader();

            this.document.mimetype = file.type;
            this.document.filename=fileName;
            reader.onload = this.handleDocumentOnLoad.bind(this);

            reader.readAsBinaryString(file);
            $('#uploadDocLabel').val(fileName);
        }
    }

    private handleDocumentOnLoad(readerEvt: any): void {
        let binaryString = readerEvt.target.result;
        this.document.file = btoa(binaryString);
    }
    
    saveDocument(){
        this.document.date=new Date(this.document.date).getTime();
        this.activityPlan.reportDocuments.push(this.document);
        $('#uploadDocLabel').val("");
        this.document={
            file:'',
            filename:'',
            date:'',
            type:0,
            mimetype:'',
            ref:'',
            note:''
        }
    }
    deleteDoc(index:number){
        this.activityPlan.reportDocuments.splice(index, 1);
    }
    async encodeProgressReport(): Promise<void> {
        const body = this.activityPlan;
        body.isAdditional = body.isAdditional == 'true';
        body.reportDate = new Date(body.reportDate).getTime();
        this.keyCase.PascalCase(body);
        this.investorService.submitSelfEvaluation(body, this.promotionUnitId).subscribe(res=>{
            swal({
                type: 'success', title: 'Success', text: 'You Have Successfully Submit Your Evaluation'
            }).then(value => {
                this.router.navigate([`investor/projectDetails/${this.promotionUnitId}/${this.investorID}`]);
            })
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
      
    }
}