import {Component, OnInit} from "@angular/core";
import {ActivatedRoute, Router} from "@angular/router";
import {BidServices} from "../../_services/bid.services";
import {FormBuilder} from "@angular/forms";
import {AuthServices} from "../../_services/auth.services";
import swal from "sweetalert2";
import {InvestorServices} from "../../_services/investor.services";
import {Applicants, Application} from "../../_models/investor.model";
declare var $:any;

@Component({
    selector:'app-evaluation-details',
    templateUrl:'./evaluationDetails.component.html'
})
export class EvaluationDetailsComponent implements OnInit{
    public investorID:string;
    public promotionID:string;
    public promotionUnitID:string;
    public currentUsername:string;
    public currentRole:string;
    public teamID:string=null;
    public evaluationPoint:any;
    public evaluationData:any;
    public allEvaluationPoint:any;
    public criteria:any;
    public criteriaList:any[]=[];
    public invCriteria:any;
    public invCriteriaList:any[]=[];
    public appQuery:any;
    public investorLists:any[]=[];
    public investor:any;
    public team:any;
    public teamsList:any[]=[];
    public evaluator:any;
    public member:any;
    public memberList:any[]=[];
    public teamCriteria:any;
    public teamCriteriaList:any[]=[];
    public teamCriteriaMem:any;
    user:any;

    public promotionUnit:any;

    public evaluationResult:any;
    public subEvlResult:any;
    public evaluationResultList:any[]=[];
    public fullname:string;
    public promotionResult: any;
    public teamResult: any;
    public loading:boolean=false;
    public applicants:Applicants;
    application:Application[];
    public applicantList:any[]=[];
    public investorName:string;
    constructor(public activatedRoute:ActivatedRoute, public bidServices:BidServices, public authService:AuthServices, public investorService:InvestorServices, public router:Router){}
    ngOnInit(){
        this.loading=true;
        this.promotionID=this.activatedRoute.snapshot.params['prom_id'];
        this.investorID=this.activatedRoute.snapshot.params['investor_id'];
        this.promotionUnitID=this.activatedRoute.snapshot.params['promUnit_id'];
        this.currentUsername=JSON.parse(<string>localStorage.getItem('username'));
        this.currentRole=JSON.parse(<string>localStorage.getItem('role'));

        this.investor={
            id:'',
            name:''
        };
        this.team={
            id:'',
            name:'',
            member:[]
        };
        this.criteria={
            invName:'',
            val:[],
            sumVal:[],
            total:null

        };
        this.invCriteria={
            evaluator:'',
            val:[],
            sum:''
        };
        this.evaluator={
            username:'',
            val:''
        };
        this.member={
            username:'',
            fullname:''
        };

        this.teamCriteria={
            teamName:'',
            teamVal:[],
            teamMember:null
        };
        this.teamCriteriaMem={
            username:'',
            fullname:''
        };
        this.evaluationPoint={
            val:'',
            promId:'',
            teamId:'',
            investorId:'',
            evaluatorUsername:''
        };
        this.evaluationResult={
            criteriaName:'',
            maxVal:'',
            weight:'',
            val:'',
            subCriteria:[]
        };
        this.subEvlResult={
            criteriaName:'',
            maxVal:'',
            weight:'',
            val:''
        };
        this.promotionResult={
            evaluatorName:'',
            evaWeight:'',
            teamName:'',
            result:null
        };
        this.appQuery={
            promoID:this.promotionID,
            investorID:'',
            promoUnitID:this.promotionUnitID
        };
        this.applicants={
            submittedDate:null,
            investorName:'',
            nationality:'',
            investmentType:[],
            capital:null,
            contact:null,
            promoID:'',
            investorID:'',
            promotionUnitId:''
        };
        this.getAllEvaluationPoint();
        this.getCriteria();
        this.getEvaResult();
        this.getPromotionUnit();
        this.getApplications();

    }

    // getEvaluationPoint(){
    //     this.bidServices.getEvaluationPoint(this.promotionID, this.teamID, this.currentUsername, this.investorID).subscribe(res=>{
    //         this.evaluationPoint=res;
    //     },e=>{
    //         swal({
    //             type: 'error', title: 'Oops...', text: e.message
    //         });
    //     })
    // }

    getAllEvaluationPoint(){
        this.bidServices.getAllEvaluationPoint(this.promotionID).subscribe(res=>{
            this.allEvaluationPoint=res;
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }
    getEvaResult() {
        this.bidServices.getEvaluationSummary(this.promotionUnitID, this.promotionID).subscribe(res => {
            this.teamResult = res;
        }, e => {
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }
    getCriteria(){


        this.investorService.searchApplication(this.appQuery).subscribe(res=>{
            for(const appList of res.result){
                const investorProfile=JSON.parse(appList.invProfile.defaultProfile);
                this.investor.id=appList.invProfile.id;
                this.investor.name=investorProfile.Name;

                this.investorLists.push(this.investor);
                this.investor={
                    id:'',
                    name:''
                };
            }
            this.bidServices.getPromotionUnit(this.promotionUnitID, this.promotionID).subscribe(res2=>{

                for(const team of res2.evalTeams){
                    for(const user of team.members){
                        this.authService.getUser(user.userName).subscribe(res4=>{
                            this.member.fullname=res4.fullName;
                            this.member.username=user.userName;
                            this.memberList.push(this.member);
                            this.member={
                                username:'',
                                fullname:''
                            }
                        });

                    }
                    this.team.id=team.id;
                    this.team.name=team.teamName;
                    this.team.member=team.members;

                    this.teamsList.push(this.team);
                    this.team={
                        id:'',
                        name:'',
                        member:[]
                    };
                }
                this.bidServices.getAllEvaluationPoint(this.promotionUnitID).subscribe(res3=>{
                    this.allEvaluationPoint=res3;

                    if(this.investorID !=null || this.investorID != undefined) {
                        for (const ur of this.memberList) {
                            for (const tm of this.teamsList) {
                                let value = null;
                                for (const val of this.allEvaluationPoint) {
                                    if (this.investorID == val.investorId) {

                                        if (tm.id == val.teamId && ur.username == val.userName) {
                                            value = val.val;

                                        }

                                    }
                                }

                                this.invCriteria.val.push(value);

                            }
                            this.invCriteria.evaluator=ur.fullname;
                            this.invCriteriaList.push(this.invCriteria);
                            this.invCriteria={
                                evaluator:'',
                                val:[],
                                sum:''
                            }
                        }
                    }else{
                        for(const inv of this.investorLists) {
                            let total=0;
                            for (const tm of this.teamsList) {
                                let sumVal=0;


                                for (const val of this.allEvaluationPoint) {

                                    if (val.investorId == inv.id) {
                                        for (const ur of tm.member) {
                                            let value = null;
                                            if (ur.userName == val.userName) {
                                                value = val.val;
                                                this.criteria.val.push(value);
                                            }
                                        }
                                        if (tm.id == val.teamId) {
                                            sumVal = sumVal + val.val;
                                        }
                                    }

                                }
                                sumVal=sumVal/tm.member.length;
                                this.criteria.sumVal.push(sumVal);
                                total=total+sumVal;
                            }
                            this.criteria.total=total;
                            this.criteria.invName = inv.name;
                            this.criteriaList.push(this.criteria);
                            this.criteria = {
                                invName: '', val: [], sumVal:[]
                            };
                        }

                        for (const tm of this.teamsList) {

                            for(const inv of this.investorLists) {
                                for (const val of this.allEvaluationPoint) {
                                    if (tm.id == val.teamId) {
                                        if (val.investorId == inv.id) {
                                            for (const ur of tm.member) {
                                                let value = null;
                                                if (ur.userName == val.userName) {
                                                    value = val.val;
                                                    this.evaluationPoint.val=value;
                                                    this.evaluationPoint.promId=this.promotionUnitID;
                                                    this.evaluationPoint.investorId=val.investorId;
                                                    this.evaluationPoint.teamId=val.teamId;
                                                    this.evaluationPoint.evaluatorUsername=val.userName;
                                                    this.criteria.val.push(this.evaluationPoint);
                                                    this.evaluationPoint={
                                                        val:'',
                                                        promId:'',
                                                        teamId:'',
                                                        investorId:'',
                                                        evaluatorUsername:''
                                                    }

                                                }

                                            }
                                        }
                                    }



                                }
                                this.criteria.invName = inv.name;
                                this.teamCriteria.teamVal.push(this.criteria);
                                this.criteria = {
                                    invName: '', val: [], sumVal: []
                                };

                            }

                            this.teamCriteria.teamMember=tm.member;
                            this.teamCriteriaMem={username:''};
                            this.teamCriteria.teamName=tm.name;
                            this.teamCriteriaList.push(this.teamCriteria);
                            this.teamCriteria={
                                teamName:'',
                                teamVal:[],
                                memberList:[]

                            }
                        }

                        console.log(this.teamCriteriaList);
                    }

                })
            });
            this.loading=false;
        })

    }

    approveEvaluation(){
        this.bidServices.AutoAcceptApplication(this.promotionUnitID, this.promotionID).subscribe(res=>{
            swal({
                type: 'success', title: 'Success', text: 'You Have Successfully Approve Evaluation'
            }).then(value => {
                this.router.navigate(['/admin/manageBid']);
            })
        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }

    public detail(promId:string, teamID:string, username:string, investorId:string):void{
        this.bidServices.getEvaluationData(promId, teamID, username, investorId).subscribe(res=>{
            this.evaluationData=res;
            this.evaluationResultList=[];
            this.bidServices.getPromotionUnit(this.promotionUnitID, this.promotionID).subscribe(res2=>{
                this.promotionUnit=res2;
                for(const evaTeam of this.promotionUnit.evalTeams){
                    if(this.evaluationData.evaluationTeamID==evaTeam.id){
                        for(const ur of evaTeam.members){
                            if(this.evaluationData.evaluatorUserName==ur.userName){
                                for(const cri of evaTeam.criterion){
                                    for(const result of this.evaluationData.result){
                                        if(cri.id==result.id){
                                            if(cri.cubCriterion != undefined && cri.cubCriterion.length > 0 && result.subResult.length > 0 ){
                                                for(const subCri of cri.cubCriterion){
                                                    for(const subRes of result.subResult){
                                                        if(subCri.id == subRes.id){
                                                            this.subEvlResult.criteriaName=subCri.name;
                                                            this.subEvlResult.maxVal=subCri.maxVal;
                                                            this.subEvlResult.weight=subCri.weight;
                                                            this.subEvlResult.val=subRes.val;
                                                            this.evaluationResult.subCriteria.push(this.subEvlResult);
                                                            this.subEvlResult={
                                                                criteriaName:'',
                                                                maxVal:'',
                                                                weight:'',
                                                                val:''
                                                            };

                                                        }
                                                    }
                                                }

                                            }
                                            this.evaluationResult.criteriaName=cri.name;
                                            this.evaluationResult.maxVal=cri.maxVal;
                                            this.evaluationResult.weight=cri.weight;
                                            this.evaluationResult.val=result.val;
                                            this.evaluationResultList.push(this.evaluationResult);
                                            this.evaluationResult={
                                                criteriaName:'',
                                                maxVal:'',
                                                weight:'',
                                                val:'',
                                                subCriteria:[]
                                            };
                                        }
                                    }
                                }
                                this.promotionResult.evaluatorName=ur.userName;
                                this.promotionResult.teamName=evaTeam.teamName;
                                this.promotionResult.evaWeight=evaTeam.weight;
                                this.promotionResult.result=this.evaluationResultList;
                            }
                        }
                    }
                }
                $('#view-evaluation-data-dialog').modal('show');
            }, e=>{
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });

        },e=>{
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        })
    }

    public getPromotionUnit(){
        this.bidServices.getPromotionUnit(this.promotionUnitID, this.promotionID).subscribe(pro=>{
            this.promotionUnit=pro;
        });
    }
    public getApplications(){
        this.investorService.searchApplication(this.appQuery).subscribe(res=>{
            this.application=res.result;
            for(const appList of this.application){
                
                const investor=JSON.parse(appList.invProfile.defaultProfile);
                if(this.investorID==appList.invProfile.id){
                    this.investorName=investor.Name;
                }
                this.applicants.investorName=investor.Name;
                this.applicants.nationality=investor.Nationality;
                this.applicants.investorID=appList.invProfile.id;
                this.applicants.submittedDate=appList.applicationTime;
                this.applicants.investmentType=appList.investmentTypes;
                this.applicants.contact=appList.contactAddress;
                this.applicants.capital=appList.proposedCapital;
                this.applicants.promoID=appList.promoID;
                this.applicants.promotionUnitId=appList.promotionUnitId;

                this.applicantList.push(this.applicants);
                this.applicants={
                    submittedDate:null,
                    investorName:'',
                    nationality:'',
                    investmentType:[],
                    capital:null,
                    contact:null,
                    promoID:'',
                    investorID:'',
                    promotionUnitId:''
                };
            }
        })
    }
    
    getDataByInvestor(inv_id:string){
        this.router.navigate([`admin/eva-details/${this.promotionID}/${this.promotionUnitID}/${inv_id}`]);
        this.investorID=this.activatedRoute.snapshot.params['investor_id'];
        
    }
}
