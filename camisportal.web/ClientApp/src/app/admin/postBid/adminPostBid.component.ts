import { Component, OnInit, ViewChild } from "@angular/core";
import {AbstractControl, Form, FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {
    EvaluationCriterionModel,
    EvaluationTeamMemberModel,
    EvaluationTeamModel, IdocumentPriview,
    PromotionDocModel,
    PromotionModel,
    PromotionUnit
} from "../../_models/bid.model";
import { BidServices } from "../../_services/bid.services";
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from "@angular/router";
import { AuthServices } from "../../_services/auth.services";
import swal from "sweetalert2";
import { DatePipe } from "@angular/common";
import { } from '@types/googlemaps';
import dialog from "../../shared/loader/loader_dialog";
import {configs} from "../../../../../../camis.web/ClientApp/src/app/app-config";
import {ListServices} from "../../_services/list.services";
import {el} from "@angular/platform-browser/testing/src/browser_util";

declare var $: any;
@Component({
  selector: 'app-post-bid',
  templateUrl: './adminPostBid.component.html'

})
export class AdminPostBidComponent implements OnInit {

  public bidRegForm: FormGroup;
  public evaluationTeamForm: FormGroup;
  public evaluationCriteriaForm: FormGroup | any;
  public promotionParcelForm: FormGroup;
  public promotionModel: PromotionModel;
  public promotionDocModel: PromotionDocModel;
  public promotionPicModel: PromotionDocModel;
  public evaTeamMemberModel: EvaluationTeamMemberModel;
  public evaTeamModel: EvaluationTeamModel;
  public evaCriteriaModel: EvaluationCriterionModel;
  public promotionUnit: any;
  public landUPID: string;
  public landData: any;
  public step: number | 0;
  public step1Class: string;
  public step2Class: string | "disabled";
  public step3Class: string | "disabled";
  public step4Class: string | "disabled";
  public lat: number = 8.9806;
  public lng: number = 38.7578;
  users: any[] = [];
  imageList: any[];
  selectedUser: any[];
  selectedMember: any;
  member: any;
  uname: string | '';
  selectedTeam: any;
  team: any;
  selectedValueList: any;
  criteriaIndex: number | null;
  teamIndex: number | null
  cubCriterionIndex:number=null;
  cubCriterionOneIndex:number=null;
  cubCriterionTwoIndex:number=null;

  validParcel: boolean;
  promotionID: string;
  isRegister: boolean | true;
  isEdit: any;
  isAddVariable=" ";
  isAdd:boolean|false;
  title: string;
  selectedRegion:any;

  public action:string;
  public promotionUnitID:string;

  @ViewChild('gmap') set mapElement(mapel: any) {
    if(mapel && this.map!=undefined)
      this.initMap(mapel);
  };

  map: google.maps.Map;
  polygon: any;
  map_center: any;
  map_bound: any;

    accessablity:any[]=[];
    accessablityList:any[]=[];
    agroEcoZone:any[]=[];
    agroEcoZoneList:any[]=[];
    exisingLandUse:any[]=[];
    exisingLandUseList:any[]=[];
    investTypes:any[]=[];
    topography:any[]=[];
    moistureSource:any[]=[];
    topographyList:any[]=[];
    moistureSourceList:any[]=[];
    investmentTypeList:any[]=[];
    monthesList: any[] = [];
    soilTestsList:any[]=[];
    climates: any[] = [];
    soilTests:any[]=[];
    public lData:any;

  constructor(public fb: FormBuilder, public bidService: BidServices, public router: Router, public authServices: AuthServices, public activeRouter: ActivatedRoute, public listServices:ListServices) {
    this.bidRegForm = this.fb.group({
      bidTitle: ['', Validators.required],
      bidStartDate: ['', [Validators.required, AdminPostBidComponent.startDateValidator]],
      bidDeadline: ['', [Validators.required, AdminPostBidComponent.endDateValidator]],
      BidRefNo: ['', Validators.required],
      bidSummery: ['', Validators.required],
      bidDetails: ['', Validators.required],
      docRef: ['', Validators.required],
      physicalAddress: ['', Validators.required],
      investmentTypes: ['', Validators.required]
    });
    this.evaluationTeamForm = this.fb.group({
      members: this.fb.array([], Validators.required),
      teamName: ['', Validators.required],
      weight: [1, [Validators.required, Validators.min(0)]]
    });

    this.evaluationCriteriaForm = this.fb.group({
      valueList: this.fb.array([]),
      name: ['', Validators.required],
      maxVal: ['', [Validators.required, Validators.min(0)]],
      weight: [1, [Validators.required, Validators.min(0)]]
    });

    this.promotionParcelForm = this.fb.group({
      parcelUPIN: ['', Validators.required]
    })

  }
  intiMember() {
    return this.fb.group({
      userName: this.uname,
      weight: [1, [Validators.required,Validators.min(0)]]
    });
  };


  intiValueList() {
    return this.fb.group({
      name: ['', Validators.required],
      value: ['', Validators.required]
    })
  };

  get members() {
    return this.evaluationTeamForm.get('members') as FormArray;
  }

  addMember() { 
    this.members.push(this.intiMember());

  }

  get valueList() {
    return this.evaluationCriteriaForm.get('valueList') as FormArray;
  }

  addValueListForm() {
    this.valueList.push(this.intiValueList());
  }

  initMap(mapel: any): void {
    this.map = new google.maps.Map(mapel.nativeElement, {
      zoom: 5,
      center: new google.maps.LatLng(24.886, -70.268),
      mapTypeId: google.maps.MapTypeId.SATELLITE
    });
    if (this.polygon) {
      for (var i  in this.polygon)
        this.polygon[i].setMap(this.map);
    }
    //if (this.map_center)
    //  this.map.setCenter(this.map_center);
    if (this.map_bound)
      this.map.fitBounds(this.map_bound);

  }
  public ngOnInit(): void {
    dialog.loading();
    this.selectedRegion=JSON.parse(localStorage.getItem('region'));
    this.getUser();

    this.promotionID = this.activeRouter.snapshot.params["prom_id"];
    this.promotionUnitID=this.activeRouter.snapshot.params["promUnit_id"];
    this.action=this.activeRouter.snapshot.params["status"];



    if (!this.step) {
      this.step = 1;
      this.step1Class = "selected";
      this.step2Class = "disabled";
      this.step3Class = "disabled";
      this.step4Class = "disabled";
    }
 
    this.selectedValueList = {
      value: []
    };
    this.selectedMember = {
      member: []
    };
    this.selectedTeam = {
      team: []
    };
    this.promotionDocModel = {
      id: '',
      data: '',
      mime: '',
      docRef: '',
      desc:''  
    };

    this.promotionModel = {
      id: '',
      applyDateFrom: null,
      applyDateTo: null,
      title: '',
      promotionRef: '',
      summery: '',
      region: '',
      description: '',
      postedOn: new Date(),
      status: 0,
      physicalAddress: '',
      investmentTypes: []
    };

    this.promotionPicModel = {
      id: '',
      data: '',
      docRef: '',
      mime: '',
        desc:''  
    };
    this.evaTeamMemberModel = {
      id: '',
      userName: '',
      weight: 1,
    };
    this.evaCriteriaModel = {
      id: '',
      name: '',
      maxVal: 1,
      weight: 1,
      valueList: [],
      cubCriterion: []
    };
    this.evaTeamModel={
      id: '',
      teamName: '',
      weight: 1,
      criterion: [],
      members: []

    };
    this.promotionUnit = {
      id: '',
      landUPIN: '',
      promotion: this.promotionModel,
      documents: [],
      pictures: [],
      evalTeams: []
    };

    if (this.promotionID != "" && this.promotionID != undefined) {
      if(this.action!="" && this.action!=undefined){
        this.title='Add Promotion Unit';
        this.isRegister=false;
        this.isEdit=false;
        this.isAdd=true;
        this.bidRegForm.get("bidTitle").disable();
          this.bidRegForm.get("bidStartDate").disable();
          this.bidRegForm.get("BidRefNo").disable();
          this.bidRegForm.get("bidDeadline").disable();
          this.bidRegForm.get("physicalAddress").disable();
          this.bidRegForm.get("investmentTypes").disable();
          this.bidRegForm.get("bidSummery").disable();
          this.bidRegForm.get("bidDetails").disable();
          this.bidService.getPromotion(this.promotionID).subscribe(res => {
              let dateTimePipe = new DatePipe("en-US");

              this.promotionModel = res.promotionUnit[0].promotion;
              this.promotionModel.applyDateTo = dateTimePipe.transform(res.applyDateTo, 'yyyy-MM-dd');
              this.promotionModel.applyDateFrom = dateTimePipe.transform(res.applyDateFrom, 'yyyy-MM-dd');
          });
      }else {
          this.title = 'Edit Promotion Information';
          this.isRegister = false;
          this.isAdd=false;
          this.isEdit = true;
          this.bidService.getPromotionUnit(this.promotionUnitID, this.promotionID).subscribe(res => {
              let dateTimePipe = new DatePipe("en-US");
              this.promotionUnit = res;
              this.promotionModel = this.promotionUnit.promotion;
              this.landUPID = this.promotionUnit.landUPIN;
              this.landData=this.promotionUnit.landData;
              this.lData=this.promotionUnit.landData;
              this.getParcelDataLookup();
              this.promotionDocModel.docRef = this.promotionUnit.documents[0].docRef;
              this.promotionDocModel.data = this.promotionUnit.documents[0].data;
              this.promotionDocModel.mime = this.promotionUnit.documents[0].mime;
              this.promotionDocModel.desc=this.promotionUnit.documents[0].desc;
              this.promotionModel.applyDateTo = dateTimePipe.transform(this.promotionUnit.promotion.applyDateTo, 'yyyy-MM-dd');
              this.promotionModel.applyDateFrom = dateTimePipe.transform(this.promotionUnit.promotion.applyDateFrom, 'yyyy-MM-dd');
          });
      }


    }
    else {
      this.title = 'Post Promotion Information';
      this.isRegister = true;
      this.isEdit = false;
      this.isAdd=false;
    }
    dialog.close();
  }
  public getUser() {
    this.authServices.getAllSystemUsers().subscribe(res => {
      for (const user of res) {
        if (user.role == 4 && this.selectedRegion==user.region && user.active==true) {
          this.users.push(user);
        }
      }
    });
  }

  //promotion Details

  public bidDocUploadTrigger(): void {
    $("#uploadBidDoc").click();
  }

  public handleDocumentUpload(evt: any): void {
    let files = evt.target.files;
    let fileName = $('#uploadBidDoc').val();
    let file = files[0];


    if (files && file) {

      let reader = new FileReader();

      this.promotionDocModel.mime = file.type;
      this.promotionDocModel.desc=fileName.replace(/\\/gi, '/').split('/').pop();
      reader.onload = this.handleDocumentOnLoad.bind(this);

      reader.readAsBinaryString(file);
      $('#uploadBidDocLabel').val(fileName);
    }
  }

  private handleDocumentOnLoad(readerEvt: any): void {
    let binaryString = readerEvt.target.result;
    this.promotionDocModel.data = btoa(binaryString);
  }

  public savePromotion() {
    this.promotionUnit.promotion = this.promotionModel;
    if (this.promotionUnit.documents.length >= 1) {
      this.promotionUnit.documents = [];
      this.promotionUnit.documents.push(this.promotionDocModel);
    } else {
      this.promotionUnit.documents.push(this.promotionDocModel);
    }

    this.step2Class = "selected";
    this.step1Class = "disabled";
    this.step = 2;
    if( this.promotionParcelForm.controls['parcelUPIN'].value != ''){
      this.getParcelData();
    }
  }


  //promotion Land
  public imgUploadTrigger(): void {
    $("#uploadImageDoc").click();
  }

  addImage() {
    this.promotionUnit.pictures.push(this.promotionPicModel);
    this.promotionPicModel = {
      id: '',
      data: '',
      docRef: '',
      mime: '',
      desc:''  
    };
  }
  public handleImageUpload(evt: any): void {
    let files = evt.target.files;
    let file = files[0];


    if (files && file) {

      let reader = new FileReader();

      this.promotionPicModel.mime = file.type;
      reader.onload = this.handleImageOnLoad.bind(this);
      reader.readAsBinaryString(file);
    }

  }

  private handleImageOnLoad(readerEvt: any): void {
    let binaryString = readerEvt.target.result;
    this.promotionPicModel.data = btoa(binaryString);
    this.addImage();
  }

  backToStep1() {
    this.step = 1;
    this.step1Class = 'selected';
    this.step2Class = 'disabled';
  }
  getParcelData() {
      dialog.loading();
    const upin = this.promotionParcelForm.controls['parcelUPIN'].value;
    this.bidService.getLandInformation(upin).subscribe(res => {
      this.landData = res.landData;
      this.lData=res.landData;
       this.getParcelDataLookup();

      if (this.map && res.polygon && res.polygon.length > 0) {
        // Define the LatLng coordinates for the polygon's path.

        this.map_center = new google.maps.LatLng(res.location.lat, res.location.lng);
        // Construct the polygon.
        this.polygon = [];
        for (var p in res.polygon) {
          var mp = new google.maps.Polygon({
            paths: res.polygon[0],
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.35
          });
          this.polygon.push(mp);
        }
        this.map_bound = new google.maps.LatLngBounds(new google.maps.LatLng(res.bottomLeft.lat, res.bottomLeft.lng),
          new google.maps.LatLng(res.topRight.lat, res.topRight.lng));
      }
      dialog.close();
    }, e => {
      swal({
        type: 'error', title: 'Oops...', text: e.message
      });
    })
  }

  savePromotionParcel() {
    if(this.promotionUnit.pictures.length < 1){
        swal({
            title: 'Are you sure?',
            text: "You want to next this page? you haven't upload promotion pictures",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Next it!'
        }).then((result) => {
            if (result.value) {
                this.step = 3;
                this.step3Class = 'selected';
                this.step2Class = 'disabled';
            }
        })
    }
    else{
        this.step = 3;
        this.step3Class = 'selected';
        this.step2Class = 'disabled';
    }
    
  }


  //promotion evaluation team member

  addUser(username: any, index: number) {
    this.uname = username.userName;
    this.addMember();
    this.selectedMember.member.push(username);
    this.selectedUser = this.selectedMember.member;
    this.users.splice(index, 1);
  }

  removeUser(username: any, index: number) {
    this.users.push(username);
    this.selectedUser.splice(index, 1);
  }

  removeTeam(index: number) {
    this.promotionUnit.evalTeams.splice(index, 1);
  }
  saveEvaTeamMember() {
    this.evaTeamModel = this.evaluationTeamForm.value;
    this.selectedTeam.team.push(this.evaTeamModel);
    this.promotionUnit.evalTeams.push(this.evaTeamModel);
    this.evaluationTeamForm.reset();
    this.evaluationTeamForm.controls['weight'].setValue(1);
    this.evaluationTeamForm.controls['members'] = this.fb.array([]);
    this.selectedUser.length = 0;
    this.selectedMember.member.length = 0;
    this.users.length = 0;
    return this.getUser();
  }

  backToStep3() {
    this.step = 3;
    this.step3Class = 'selected';
    this.step4Class = 'disabled';
  }

  backToStep2() {
    this.step = 2;
    this.step2Class = 'selected';
    this.step3Class = 'disabled';
  }

  //evaluation criteria
  public addEvaluationCriteria(index: number) {
    $('#add-criteria-dialog').modal('show');
    this.teamIndex = index;
  }

  public addSubCriteria(team: number, criteria: number) {
    $('#add-criteria-dialog').modal('show');
    this.teamIndex = team;
    this.criteriaIndex = criteria
  }
  public addSubCriOne(team:number, criteria:number, cubCriterion){
      $('#add-criteria-dialog').modal('show');
      this.teamIndex = team;
      this.criteriaIndex = criteria;
      this.cubCriterionIndex=cubCriterion;
  }
  public addSubCriTwo(team:number, criteria:number, cubCriterion:number, cubCriterionOne:number){
      $('#add-criteria-dialog').modal('show');
      this.teamIndex = team;
      this.criteriaIndex = criteria;
      this.cubCriterionIndex=cubCriterion;
      this.cubCriterionOneIndex=cubCriterionOne;
  }

    public addSubCriThree(team:number, criteria:number, cubCriterion:number, cubCriterionOne:number, cubCriterionTwo:number){
        $('#add-criteria-dialog').modal('show');
        this.teamIndex = team;
        this.criteriaIndex = criteria;
        this.cubCriterionIndex=cubCriterion;
        this.cubCriterionOneIndex=cubCriterionOne;
        this.cubCriterionTwoIndex=cubCriterionTwo
    }
  public addValueList() {
    //this.addValueListForm();

    const control = <FormArray>this.evaluationCriteriaForm.controls['valueList'];
    control.push(this.intiValueList());

  }
  public saveValueList() {
    const evalCriteria = this.evaluationCriteriaForm.value;
      if(this.teamIndex!=null && this.criteriaIndex != null && this.cubCriterionIndex != null && this.cubCriterionOneIndex!=null && this.cubCriterionTwoIndex !=null){
          if (this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion[this.cubCriterionTwoIndex].cubCriterion)
              this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion[this.cubCriterionTwoIndex].cubCriterion.push(evalCriteria);
          else
              this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion[this.cubCriterionTwoIndex].cubCriterion = [evalCriteria];

          this.resetValueList();
          $('#add-criteria-dialog').modal('hide');
          this.criteriaIndex = null;
          this.teamIndex = null;
          this.cubCriterionIndex=null;
          this.cubCriterionOneIndex=null;
          this.cubCriterionTwoIndex=null;
      }
    else if(this.teamIndex!=null && this.criteriaIndex != null && this.cubCriterionIndex != null && this.cubCriterionOneIndex!=null){
        if (this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion)
            this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion.push(evalCriteria);
        else
            this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion[this.cubCriterionOneIndex].cubCriterion = [evalCriteria];

        this.resetValueList();
        $('#add-criteria-dialog').modal('hide');
        this.criteriaIndex = null;
        this.teamIndex = null;
        this.cubCriterionIndex=null;
        this.cubCriterionOneIndex=null;
    }
    else if(this.teamIndex!=null && this.criteriaIndex != null && this.cubCriterionIndex != null){
        if (this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion)
            this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion.push(evalCriteria);
        else
            this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion[this.cubCriterionIndex].cubCriterion = [evalCriteria];

        this.resetValueList();
        $('#add-criteria-dialog').modal('hide');
        this.criteriaIndex = null;
        this.teamIndex = null;
        this.cubCriterionIndex=null;
    }
    else if (this.teamIndex != null && this.criteriaIndex != null) {
      if (this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion)
        this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion.push(evalCriteria);
      else
        this.promotionUnit.evalTeams[this.teamIndex].criterion[this.criteriaIndex].cubCriterion = [evalCriteria];

      this.resetValueList();
      $('#add-criteria-dialog').modal('hide');
      this.criteriaIndex = null;
      this.teamIndex = null;
    }
    else {
      if (this.promotionUnit.evalTeams[this.teamIndex].criterion)
        this.promotionUnit.evalTeams[this.teamIndex].criterion.push(evalCriteria);
      else
        this.promotionUnit.evalTeams[this.teamIndex].criterion = [evalCriteria];

      this.resetValueList();
      $('#add-criteria-dialog').modal('hide');
      this.teamIndex = null;
    }


  }
  public resetValueList() {
    this.evaluationCriteriaForm.reset();
    this.evaluationCriteriaForm.controls['weight'].setValue(1);
    const formValueList = this.evaluationCriteriaForm.get('valueList') as FormArray;
    while (formValueList.length !== 0) {
      formValueList.removeAt(0);
    }
  }

  public removeValueList(index:number){
      const formValueList = this.evaluationCriteriaForm.get('valueList') as FormArray;
      formValueList.removeAt(index);
  }
  public removeCriteria(team: number, criteria: number) {
    this.promotionUnit.evalTeams[team].criterion.splice(criteria, 1);
  }
  public removeSubCriteria(team:number, criteria:number, subCriteria:number){
    this.promotionUnit.evalTeams[team].criterion[criteria].cubCriterion.splice(subCriteria, 1);
  }
  public removeSubCriOne(team:number, criteria:number, subCriteria:number, subCriteriaOne:number){
      this.promotionUnit.evalTeams[team].criterion[criteria].cubCriterion[subCriteria].cubCriterion.splice(subCriteriaOne, 1);
  }
  public removeSubCriTwo(team:number, criteria:number, subCriteria:number, subCriteriaOne:number, subCriteriaTwo:number){
      this.promotionUnit.evalTeams[team].criterion[criteria].cubCriterion[subCriteria].cubCriterion[subCriteriaOne].cubCriterion.splice(subCriteriaTwo, 1);
  }
    public removeSubCriThree(team:number, criteria:number, subCriteria:number, subCriteriaOne:number, subCriteriaTwo:number, subCriterionThree){
        this.promotionUnit.evalTeams[team].criterion[criteria].cubCriterion[subCriteria].cubCriterion[subCriteriaOne].cubCriterion[subCriteriaTwo].cubCriterion.splice(subCriterionThree, 1);
    }
  saveCommute() {
    this.step = 4;
    this.step4Class = 'selected';
    this.step3Class = 'disabled';
  }



  public postBid(): void {
    dialog.loading();
      //this.promotionUnit.landData=this.lData;
    if (this.promotionUnit.promotion.id != '') {
        if(this.action !="" && this.action != undefined){
            this.bidService.registerPromotion(this.promotionUnit).subscribe(res => {
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You Have Successfully Post Promotion',
                    showConfirmButton: false,
                    timer: 1500
                });

                this.router.navigate(['admin/manageBid']);

            }, e => {
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });
        }
        else{
          this.promotionUnit.landData="";
            this.bidService.updatePromotion(this.promotionUnit).subscribe(res => {
                dialog.close();
                swal({
                    type: 'success',
                    title: 'You Have Successfully Updated Promotion',
                    showConfirmButton: false,
                    timer: 1500
                });

                this.router.navigate(['admin/manageBid']);

            }, e => {
                swal({
                    type: 'error', title: 'Oops...', text: e.message
                });
            });
        }

    }
    else {
      this.bidService.registerPromotion(this.promotionUnit).subscribe(res => {
        dialog.close();
        swal({
          type: 'success',
          title: 'You Have Successfully Post Promotion',
          showConfirmButton: false,
          timer: 1500
        });

        this.router.navigate(['admin/manageBid']);

      }, e => {
        swal({
          type: 'error', title: 'Oops...', text: e.message
        });
      });
    }

  }

  openDoc(e:any, doc:IdocumentPriview, promID:string, promUnitID:string){
      doc.overrideFilePath=`${configs.url}Bid/GetPromotionDocument?promId=${promID}&promUnitId=${promUnitID}`;
      if (!doc.overrideFilePath && doc.mime && doc.data) {
          e.preventDefault();
          window.open(`data:${doc.mime};base64,${doc.data}`, '_blank', );
      }

      $("#view-document-dialog").modal("show");
      $("#docIframe").attr("src", doc.overrideFilePath);
  }

  changeDoc(){
    this.promotionDocModel.data='';
    this.promotionDocModel.mime='';
    this.bidDocUploadTrigger();
  }
    getArea(area:any) {
        return Math.round(area / 10) / 1000 + ' hec';
    }

    getParcelDataLookup(){
        this.listServices.getAccessbility().subscribe(a=>{
            this.accessablityList=a;
            this.accessablity=[];
            for(let ac of this.landData['accessablity']){
                for(const accList of this.accessablityList){
                    if(ac==accList['id']){
                        ac=accList['name'];
                        this.accessablity.push(ac);
                    }
                }
            }
            this.landData.accessablity=this.accessablity;
        });
        this.listServices.getAgroEcoZone().subscribe(ag=>{
            this.agroEcoZoneList=ag;
            this.agroEcoZone=[];
            for(let agr of this.landData['agroEchologyZone']){
                for(const agrList of this.agroEcoZoneList){
                    if(agr.agroType==agrList['id']){
                        agr['agroType']=agrList['name'];
                        this.agroEcoZone.push(agr);
                    }
                }
            }
            this.landData.agroEchologyZone=this.agroEcoZone;
        });
        this.listServices.getExistingLandUse().subscribe(elu=>{
            this.exisingLandUseList=elu;
            this.exisingLandUse=[];
            for(let el of this.landData.existLandUse){
                for(const elList of this.exisingLandUseList){
                    if(el==elList['id']){
                        this.exisingLandUse.push(elList['name']);
                    }
                }
            }
            this.landData.existLandUse=this.exisingLandUse;
        });
        this.listServices.getMoistureSource().subscribe(ms=>{
            this.moistureSourceList=ms;
            this.moistureSource=null;
            for(const msList of this.moistureSourceList){
                if(msList['id']==this.landData.moistureSource){
                    this.moistureSource=msList['name'];
                }
            }
            this.landData.moistureSource=this.moistureSource;
        });
        this.listServices.getInvetmentType().subscribe(inv=>{
            this.investmentTypeList=inv;
            this.investTypes=[];
            for(const invt of this.landData['investmentType']){
                for(const invtList of this.investmentTypeList){
                    if(invt == invtList['id']){
                        this.investTypes.push(invtList['name']);
                    }
                }
            }
            this.landData.investmentType=this.investTypes;
        });
        this.listServices.getTopography().subscribe(t=>{
            this.topographyList=t;
            this.topography=[];
            for(let top of this.landData['topography']){
                for(const topList of this.topographyList){
                    if(top.topographyType==topList['id']){
                        top['topographyType']=topList['name'];
                        this.topography.push(top);
                    }
                }
            }
            this.landData.topography=this.topography;
        });
        this.listServices.getMonth().subscribe(m=>{
            this.monthesList=m;
            this.climates=[];
            for(let climate of this.landData['climate']){
                for (const climateList of this.monthesList) {
                    if (climate.month === climateList['id']) {
                        climate['month'] = climateList['name'];
                        this.climates.push(climate);
                    }
                }
            }
            this.landData.climate=this.climates;
        });

        this.listServices.getSoilTest().subscribe(s=>{
            this.soilTestsList=s;
            this.soilTests=[];
            for(let test of this.landData['soilTests']){
                for(const testList of this.soilTestsList){
                    if(test.testType===testList['id']){
                        test['testType']=testList['name'];
                        this.soilTests.push(test);
                    }
                }
            }
            this.landData.soilTests=this.soilTests;
        });
    }
    
    static startDateValidator(abs:AbstractControl){
      const control=abs.parent;
        let postedDate=new Date();
        let dateTimePipe = new DatePipe("en-US");
        let date = dateTimePipe.transform(postedDate, 'yyyy-MM-dd');
        let currentDate=new Date(date).getTime();
      if(control) {
          const startDate = control.get("bidStartDate");
          let startDateValue = null;
          let date;
          if (startDate) {
              startDateValue = startDate.value;
              date=new Date(startDateValue).getTime();

          }
          if (date < currentDate) {
              return {invalidDate: true}
          } else {
              return null;
          }
      }
      else{
        return null;
      }
      
    }
    static endDateValidator(abs:AbstractControl){
        const control=abs.parent;

        let postedDate=new Date();
        let dateTimePipe = new DatePipe("en-US");
        let date = dateTimePipe.transform(postedDate, 'yyyy-MM-dd');
        let currentDate=new Date(date).getTime();
        if(control) {
            const endDate = control.get("bidDeadline");
            let endDateValue = null;
            let date;
            if (endDate) {
                endDateValue = endDate.value;
                date=new Date(endDateValue).getTime();

            }
            if (date < currentDate) {
                return {invalidEndDate: true}
            } else {
                return null;
            }
        }
        else{
            return null;
        }

    }

    delImage(index:number){
     this.promotionUnit.pictures.splice(index,1);
    }
    
}
