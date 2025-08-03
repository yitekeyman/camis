import {Component, OnInit} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {Application} from "../../_models/investor.model";
import {Evaluation, EvaluationTeamModel, PromotionUnit} from "../../_models/bid.model";
import swal from "sweetalert2";
import {Form, FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthServices} from "../../_services/auth.services";
import {ListServices} from "../../_services/list.services";
import dialog from "../../shared/loader/loader_dialog";

@Component({
    selector:'app-evaluation',
    templateUrl:'./adminEvaluation.component.html'
})
export class AdminEvaluationComponent implements OnInit{
    promotionID:string;
    investorID:string;
    isApply:boolean=false;
    
    public application:Application;
    public invProfile:any;
    public promotionUnit:PromotionUnit;
    public evaluationTeam:EvaluationTeamModel[]=[];
    public evaluationForm:FormGroup;
    public initResult:FormGroup;
    public initSubResult:FormGroup;
    public initSubResultOne:FormGroup;
    public initSubResultTwo:FormGroup;
    public initSubResultThree:FormGroup;
    
    
    public currentUsername:string;
    public result:any;
    public subResult:any;
    public subResultOne:any;
    public subResultTwo:any;
    public subResultThree:any;
    public user:any;
    public selectedEvaluationTeam:EvaluationTeamModel;
    public id:string='';
    public evaluationResult:Evaluation;
    public promotionUnitId:string;
    
    
    regionList:any[]=[];
    region:any;
    originList:any[]=[];
    origin:any;
    investorTypeList:any[]=[];
    investorType:any;
    investmentType:any[]=[];
    invType:any[]=[];
    investmentTypeList:any[]=[];
    docTypeList:any[]=[];
    docAuthList:any[]=[];
    registrations:any[]=[];
    evaluationData:any;

    constructor(public activatedRoute:ActivatedRoute, public bidServices:BidServices, public fb:FormBuilder, public authService:AuthServices, public router:Router, public listServices:ListServices){
        this.evaluationForm=this.fb.group({
            evaluationTeamID:'',
            result:this.fb.array([])
        });
    }
    
    ngOnInit(){
        this.promotionID=this.activatedRoute.snapshot.params['prom_id'];
        this.investorID=this.activatedRoute.snapshot.params['investor_id'];
        this.promotionUnitId=this.activatedRoute.snapshot.params['promUnit_id'];
        this.currentUsername=JSON.parse(<string>localStorage.getItem('username'));
        this.getApplication();
        this.getPromotion();
        
        this.evaluationResult={
            evaluationTeamID:'',
            evaluatorUserName:this.currentUsername,
            investorID:this.investorID,
            promoID:this.promotionID,
            promotionUnitID:this.promotionUnitId,
            result:[]
        }
    }

    getApplication(){
        this.bidServices.getApplication(this.promotionID, this.investorID).subscribe(res=>{
            this.application=res;
            this.invProfile=JSON.parse(this.application.invProfile.defaultProfile);
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }

    getPromotion(){
        this.authService.getUser(this.currentUsername).subscribe(res=>{
            this.user=res;
        });
        this.bidServices.getPromotionUnit(this.promotionUnitId, this.promotionID).subscribe(res=>{
            this.promotionUnit=res;
            for(const team of this.promotionUnit.evalTeams){
                for(const member of team.members){
                    if(this.currentUsername==member.userName){
                        this.evaluationTeam.push(team);
                    }
                }
            }

        }, e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }

    selectTeam(teamId:any) {
        this.selectedEvaluationTeam=null;
        this.bidServices.getEvaluationData(this.promotionUnitId, teamId, this.currentUsername,this.investorID).subscribe(res=>{
            this.evaluationData=res;
            if(this.evaluationData==null){
               
            }
        });
        for (const team of this.evaluationTeam) {

            if (teamId == team.id) {
                this.selectedEvaluationTeam = team;

                this.evaluationForm.controls['evaluationTeamID'].setValue(this.selectedEvaluationTeam.id);

                for (const criteria of team.criterion) {
                    this.result = this.evaluationForm.get('result')as FormArray;
                    this.initResult = this.fb.group({
                        id: [criteria.id], val: [null, [Validators.max(criteria.maxVal),Validators.min(0)]], subResult: this.fb.array([])
                    });

                    if (criteria.cubCriterion != undefined) {

                        for (const subCri of criteria.cubCriterion) {
                            this.subResult = this.initResult.get('subResult')as FormArray;
                            this.initSubResult = this.fb.group({
                                id:[subCri.id], val: [null, [Validators.max(subCri.maxVal),Validators.min(0)]], subResult: this.fb.array([])
                            });
                            if(subCri.cubCriterion != undefined){
                                for(const subCriOne of subCri.cubCriterion){
                                    this.subResultOne= this.initSubResult.get('subResult') as FormArray;
                                    this.initSubResultOne=this.fb.group({
                                        id:[subCriOne.id], val:[null, [Validators.max(subCriOne.maxVal),Validators.min(0)]], subResult:this.fb.array([])
                                    });
                                    if(subCriOne.cubCriterion != undefined){
                                        for(const subCriTwo of subCriOne.cubCriterion){
                                            this.subResultTwo= this.initSubResultOne.get('subResult') as FormArray;
                                            this.initSubResultTwo=this.fb.group({
                                                id:[subCriTwo.id], val:[null, [Validators.max(subCriTwo.maxVal),Validators.min(0)]], subResult:this.fb.array([])
                                            });
                                            if(subCriTwo.cubCriterion !=null){
                                                for(const subCriThree of subCriTwo.cubCriterion){
                                                    this.subResultThree=this.initSubResultTwo.get('subResult') as FormArray;
                                                    this.initSubResultThree=this.fb.group({
                                                        id:[subCriThree.id], val:[null, [Validators.max(subCriThree.maxVal),Validators.min(0)]], subResult:this.fb.array([])
                                                    });
                                                    this.subResultThree.push(this.initSubResultThree);
                                                }
                                            }

                                            this.subResultTwo.push(this.initSubResultTwo);
                                        }
                                    }
                                    this.subResultOne.push(this.initSubResultOne);
                                }
                            }
                            this.subResult.push(this.initSubResult);
                        }
                    }
                    this.result.push(this.initResult);
                }
            }
        }
       
    }

    saveEvaluationResult(){
        dialog.loading();
        this.evaluationResult.result=this.evaluationForm.controls['result'].value;
        this.evaluationResult.evaluationTeamID=this.evaluationForm.controls['evaluationTeamID'].value;
        
        this.bidServices.saveEvaluation(this.evaluationResult).subscribe(res=>{
            dialog.close();
            swal({
                type: 'success', title: 'Success', text: 'You Have Successfully Submit Your Evaluation'
            }).then(value => {
                this.router.navigate(['/admin/manageBid']);
            })
        }, e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }
    
}