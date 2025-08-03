import { FormGroup, AbstractControl, FormBuilder, FormsModule, FormControl } from '@angular/forms';import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormArray,Validators } from '@angular/forms';
import {Router} from '@angular/router';
import swal from 'sweetalert2';
import { LandDataService } from '../../../_services/land-data.service';
import { FarmApiService } from '../../../_services/farm-api.service';
import { ReportAPIService } from '../../../_services/report-api.service';
import { ReportRequestModel, ReportResponseModel } from '../report.model';
import { DialogService } from '../../dialog/dialog.service';
import { Accessablity, MoistureSource, ExistingLandUse } from '../../land-bank/land.model';

@Component({
    selector : "app-generate-report",
    templateUrl : "./generate-report.component.html",
    styleUrls : ["./generate-report.component.css"],
    encapsulation : ViewEncapsulation.None
})

export class GenerateReportComponent implements OnInit {

    reportForm : FormGroup;
    firstName : AbstractControl;
    reportType : any;
    filteredBy : any;

    request : ReportRequestModel;
    response : ReportResponseModel;
    html : string;

    reportTypes : any[] = [];
    landTypes : any[] = [];
    regions : any[] = [];
    investmentTypes : any[] = [];
    investorOrigins : any[] = [];
    investorOrgTypes : any[] = [];
    farmTypes : any[] = [];
    farms : any[] = [];
    accessiblitiesList: Accessablity[] = [];
    moistureSourceList: MoistureSource[] = [];
    existingLandList: ExistingLandUse[] = [];

    woredaList : any[] = [];
    zoneList : any[] = [];
    filterReports  : number[] = [1,2,3,4,7,8,9,10,12,13,20,21];
    summerizedReports : number[] = [1,4,5,6,7,8,9,14,18,19,21];
    regionError:string="";

    constructor(private router : Router, public formBuilder : FormBuilder,
        public reportService : ReportAPIService, 
        public landService : LandDataService,
        public farmService : FarmApiService,
        private dialog : DialogService)
    {
        this.reportForm = this.formBuilder.group({
            title : [''],
            firstName : [''],
            selectedReportType : ['',[Validators.required]],
            landType : ['0'],
            region : [''],
            woreda : [''],
            zone : [''],
            filteredBy : [''],
            summerizedBy : [''],
            dates: this.formBuilder.array([
                this.initdate(),
                ]),
            
            farmSizes : this.formBuilder.array([
                this.initSize(),
            ]),
            fromDate : [''],
            endDate : [''],
            farmId : [''],
            startYear : [''],
            endYear : [''], 


        });
        this.firstName = this.reportForm.controls.firstName.value;
        this.reportType = this.reportForm.controls.selectedReportType;
        this.filteredBy = this.reportForm.controls.filteredBy;
    }



    ngOnInit(){
        this.landService.getLandType().subscribe(data => {
            this.landTypes = data.filter(d => d.id !== 1);
        });
        this.reportService.getAllRegions().subscribe(data => {
            this.regions = data
        });
        this.reportService.getReportTypes().subscribe(data => {
            this.reportTypes = data;
        })

        this.landService.getInvestmentType().subscribe(data => {
            this.investmentTypes = data;
        })
        this.farmService.getAllFarmOperatorOrigins().subscribe(data => {
            this.investorOrigins = data;
        })

        this.farmService.getAllFarmTypes().subscribe(data => {
            this.farmTypes = data
        })

        this.farmService.getAllFarmOperatorTypes().subscribe(data => {
            this.investorOrgTypes = data
        })

        this.landService.getAccessiblity().subscribe(data => {
            this.accessiblitiesList = data;
        })

        this.landService.getMoistureSource().subscribe(data => {
            this.moistureSourceList = data;
        });

        this.landService.getExistingLandUse().subscribe(data => {
            this.existingLandList = data;
        })

        
    }

    getAllFarms(){
        this.reportService.getAllFarms().subscribe(data => {
            this.farms = data;
            console.log(data);
        })
    }

    initdate() {
        return this.formBuilder.group({
        date: [''],
        });
        }

    initSize(){
        return this.formBuilder.group({
            size : ['']
        });
    }

    yaerList(){
        var arr = [];
        for (let index = 2000; index < 2025; index++) {
           arr.push(index);
            
        }
        return arr;
    }
    
        addDate() {
        const control = <FormArray>this.reportForm.controls['dates'];
        control.push(this.initdate());
        }

        addSizeField(){
            const control = <FormArray>this.reportForm.controls['farmSizes'];
            control.push(this.initSize());
        }

        removeDate(i: number) {
        const control = <FormArray>this.reportForm.controls['dates'];
        control.removeAt(i);
        }

        removeSizeField(i : number){
            const control = <FormArray>this.reportForm.controls['farmSizes'];
            control.removeAt(i);
        }

        // onSubmit({ value, valid }: { value: User, valid: boolean }) {
        //     console.log(value, valid);
        //   }


    buttonClicked(){
        swal({allowOutsideClick: false});
        swal.disableButtons();
        swal.showLoading();

        var vals = this.reportForm.value;
        console.log(vals);
        this.request = {
            selectedReportType : vals.selectedReportType,
            region : vals.region,
            dates : vals.dates,
            farmSizes : vals.farmSizes,
            woreda : vals.woreda,
            zone  : vals.zone,
            filteredBy : vals.filteredBy != "" ? vals.filteredBy : "0",
            summerizedBy : vals.summerizedBy != "" ? vals.summerizedBy : "1",
            endDate : vals.endDate,
            fromDate : vals.fromDate,
            endYear : vals.endYear,
            startYear : vals.startYear,
            farmId : vals.farmId
            
        };
        this.response = null;
        console.log(this.request);
        this.reportService.getReport(this.request).subscribe(data => {
            this.html = data;
            console.log(data);
            swal.close();
        }, (err) => {
            this.dialog.error(err);
            swal.close();

        })
    }

    reportIsTypeOf(x : number[]){
        var val = parseInt(this.reportType.value);
        return  x.includes(val);
    }

    isFilteredBy(x : number[]){
        var val = parseInt(this.filteredBy.value);
        return x.includes(val);
    }

    generate(){

    }

    reportTypeChangeHandler(id: number){
        console.log(id);
        if(parseInt(this.reportType.value) == 11) { this.getAllFarms();}
        this.html = "";
    }

    regionSelected(value : string){

        this.reportService.getZones(value).subscribe((data) => {
            console.log(data);
            this.zoneList = data;
        })

  
    }

    zoneSelected(value : string){
        this.reportService.getWoredas(value).subscribe((data) => {
            console.log(data);
            this.woredaList = data;
        })
    }

    GetLandType(id : any){
        var res = this.landTypes.filter(function(val){
            return val.id == id;
        })[0];  
        return res.name;
    }

    GetArea(area) {
        return Math.round(area / 10) / 1000 + ' ha';
      }

      GetAccessiblity(id : number){
          var res = this.accessiblitiesList.filter(function(val){
              return val.id == id;
          })[0];
          return res.name;

      }

      GetMoistureSource(id : number){
        var res = this.moistureSourceList.filter(function(val){
            return val.id == id;
        })[0];
        return res.name;
      }

      GetInvestmentType(id : number){
        var res = this.investmentTypes.filter(function(val){
            return val.id == id;
        })[0];
        return res.name;
      }

      GetExistingUse(id : number){
        var res = this.existingLandList.filter(function(val){
            return val.id == id;
        })[0];
        return res.name;
      }

      DownloadData(){
        this.reportService.downloadReport(this.response).subscribe( (data : Blob) => {
            var a = document.createElement("a");
            a.href = URL.createObjectURL(data);
            a.download = `${new Date().getTime()}_report.doc`;
            a.click();
        }, (err) => {
            this.dialog.error(err);
           

        })
      }

      downloadReport(){
        var printPreview = window.open('CAMIS');
        var printDocument = printPreview.document;
        printDocument.open();
        printDocument.write(
                   "<html> <head>"+
                   document.head.innerHTML + "</head> " +
                       document.getElementById("report_html").innerHTML +
                   "</html>");
        printDocument.close();
      }

      filterBySelected(value){
        alert(value);
      }
}

