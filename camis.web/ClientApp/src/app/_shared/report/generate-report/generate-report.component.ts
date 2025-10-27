import {FormGroup, AbstractControl, FormBuilder, FormsModule, FormControl, ReactiveFormsModule} from '@angular/forms';
import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {FormArray, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {LandDataService} from '../../../_services/land-data.service';
import {FarmApiService} from '../../../_services/farm-api.service';
import {ReportAPIService} from '../../../_services/report-api.service';
import {ReportRequestModel, ReportResponseModel} from '../report.model';
import {DialogService} from '../../dialog/dialog.service';
import {Accessablity, MoistureSource, ExistingLandUse} from '../../land-bank/land.model';
import {CommonModule} from "@angular/common";
import {ProjectModule} from "../../project/project.module";
import dialog from "../../dialog";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {ExportService} from "../../../_services/export.service";

@Component({
  selector: "app-generate-report",
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ProjectModule],
  templateUrl: "./generate-report.component.html",
  styleUrls: ["./generate-report.component.css"],
  encapsulation: ViewEncapsulation.None
})

export class GenerateReportComponent implements OnInit {

  reportForm: FormGroup;
  firstName: AbstractControl;
  reportType: any;
  filteredBy: any;

  request: ReportRequestModel;
  response: ReportResponseModel;
  html: string;

  reportTypes: any[] = [];
  landTypes: any[] = [];
  regions: any[] = [];
  investmentTypes: any[] = [];
  investorOrigins: any[] = [];
  investorOrgTypes: any[] = [];
  farmTypes: any[] = [];
  farms: any[] = [];
  accessiblitiesList: Accessablity[] = [];
  moistureSourceList: MoistureSource[] = [];
  existingLandList: ExistingLandUse[] = [];

  woredaList: any[] = [];
  zoneList: any[] = [];
  filterReports: number[] = [1, 2, 3, 4, 7, 8, 9, 10, 12, 13, 20, 21];
  summerizedReports: number[] = [1, 4, 5, 6, 7, 8, 9, 14, 18, 19, 21];
  regionError: string = "";

  constructor(private router: Router, public formBuilder: FormBuilder,
              public reportService: ReportAPIService,
              public landService: LandDataService,
              public farmService: FarmApiService,
              private dialog: DialogService,
              private keyCase: ObjectKeyCasingService,
              private exportService: ExportService,) {
    this.reportForm = this.formBuilder.group({
      title: [''],
      firstName: [''],
      selectedReportType: ['', [Validators.required]],
      landType: ['0'],
      region: [''],
      woreda: [''],
      zone: [''],
      filteredBy: [''],
      summerizedBy: [''],
      dates: this.formBuilder.array([
        this.initdate(),
      ]),

      farmSizes: this.formBuilder.array([
        this.initSize(),
      ]),
      fromDate: [''],
      endDate: [''],
      farmId: [''],
      startYear: [''],
      endYear: [''],


    });
    this.firstName = this.reportForm.controls['firstName'].value;
    this.reportType = this.reportForm.controls['selectedReportType'];
    this.filteredBy = this.reportForm.controls['filteredBy'];
  }


  ngOnInit() {
    this.landService.getLandType().subscribe(data => {
      this.keyCase.camelCase(data);
      this.landTypes = data.filter(d => d.id !== 1);
    });
    this.reportService.getAllRegions().subscribe(data => {
      this.keyCase.camelCase(data);
      this.regions = data
    });
    this.reportService.getReportTypes().subscribe(data => {
      this.keyCase.camelCase(data);
      this.reportTypes = data;
    })

    this.landService.getInvestmentType().subscribe(data => {
      this.investmentTypes = data;
      this.investmentTypes = data;
    })
    this.farmService.getAllFarmOperatorOrigins().subscribe(data => {
      this.keyCase.camelCase(data);
      this.investorOrigins = data;
    })

    this.farmService.getAllFarmTypes().subscribe(data => {
      this.keyCase.camelCase(data);
      this.farmTypes = data
    })

    this.farmService.getAllFarmOperatorTypes().subscribe(data => {
      this.keyCase.camelCase(data);
      this.investorOrgTypes = data
    })

    this.landService.getAccessiblity().subscribe(data => {
      this.keyCase.camelCase(data);
      this.accessiblitiesList = data;
    })

    this.landService.getMoistureSource().subscribe(data => {
      this.keyCase.camelCase(data);
      this.moistureSourceList = data;
    });

    this.landService.getExistingLandUse().subscribe(data => {
      this.keyCase.camelCase(data);
      this.existingLandList = data;
    })


  }

  getAllFarms() {
    this.reportService.getAllFarms().subscribe(data => {
      this.keyCase.camelCase(data);
      this.farms = data;
      //console.log(data);
    })
  }

  initdate() {
    return this.formBuilder.group({
      date: [''],
    });
  }

  initSize() {
    return this.formBuilder.group({
      size: ['']
    });
  }

  yaerList() {
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

  addSizeField() {
    const control = <FormArray>this.reportForm.controls['farmSizes'];
    control.push(this.initSize());
  }

  removeDate(i: number) {
    const control = <FormArray>this.reportForm.controls['dates'];
    control.removeAt(i);
  }

  removeSizeField(i: number) {
    const control = <FormArray>this.reportForm.controls['farmSizes'];
    control.removeAt(i);
  }

  // onSubmit({ value, valid }: { value: User, valid: boolean }) {
  //     console.log(value, valid);
  //   }


  buttonClicked() {
    dialog.loading();

    var vals = this.reportForm.value;
    console.log(vals);
    const dates = vals.dates ? vals.dates.map((d: any) => ({
      date: d.date ? new Date(d.date).toISOString() : null
    })).filter((d: any) => d.date !== null) : [];

    // Prepare farm sizes properly
    const farmSizes = vals.farmSizes ? vals.farmSizes.map((s: any) => ({
      size: s.size ? parseFloat(s.size) : 0
    })).filter((s: any) => s.size > 0) : [];
    this.request = {
      selectedReportType: parseInt(vals.selectedReportType),
      region: vals.region || null,
      regions: vals.regions || null, // Add this if needed
      zone: vals.zone || null,
      woreda: vals.woreda || null,
      dates: dates.length > 0 ? dates : null,
      farmSizes: farmSizes.length > 0 ? farmSizes : null,
      filteredBy: vals.filteredBy != "" ? parseInt(vals.filteredBy) : 0,
      summerizedBy: vals.summerizedBy != "" ? parseInt(vals.summerizedBy) : 1,
      fromDate: vals.fromDate ? new Date(vals.fromDate).toISOString() : null,
      endDate: vals.endDate ? new Date(vals.endDate).toISOString() : null,
      farmId: vals.farmId || null,
      startYear: vals.startYear ? parseInt(vals.startYear) : 0,
      endYear: vals.endYear ? parseInt(vals.endYear) : 0

    };
    this.response = null;
    //console.log(this.request);
    //this.keyCase.PascalCase(this.request);
    this.reportService.getReport(this.request).subscribe(data => {
     // this.keyCase.camelCase(data);
      this.html = data;
      //console.log(data);
      dialog.close();
    }, dialog.error)
  }

  reportIsTypeOf(x: number[]) {
    var val = parseInt(this.reportType.value);
    return x.includes(val);
  }

  isFilteredBy(x: number[]) {
    var val = parseInt(this.filteredBy.value);
    return x.includes(val);
  }

  generate() {

  }

  reportTypeChangeHandler(id: number) {
    console.log(id);
    if (parseInt(this.reportType.value) == 11) {
      this.getAllFarms();
    }
    this.html = "";
  }

  regionSelected(value: string) {
    dialog.loading();
    this.reportService.getZones(value).subscribe((data) => {
      this.keyCase.camelCase(data);
      this.zoneList = data;
      dialog.close();
    }, dialog.error)
  }

  zoneSelected(value: string) {
    dialog.loading();
    this.reportService.getWoredas(value).subscribe((data) => {
      this.keyCase.camelCase(data);
      this.woredaList = data;
      dialog.close();
    }, dialog.error)
  }

  GetLandType(id: any) {
    var res = this.landTypes.filter(function (val) {
      return val.id == id;
    })[0];
    return res.name;
  }

  GetArea(area) {
    return Math.round(area / 10) / 1000 + ' ha';
  }

  GetAccessiblity(id: number) {
    var res = this.accessiblitiesList.filter(function (val) {
      return val.id == id;
    })[0];
    return res.name;

  }

  GetMoistureSource(id: number) {
    var res = this.moistureSourceList.filter(function (val) {
      return val.id == id;
    })[0];
    return res.name;
  }

  GetInvestmentType(id: number) {
    var res = this.investmentTypes.filter(function (val) {
      return val.id == id;
    })[0];
    return res.name;
  }

  GetExistingUse(id: number) {
    var res = this.existingLandList.filter(function (val) {
      return val.id == id;
    })[0];
    return res.name;
  }

  DownloadData() {
    this.reportService.downloadReport(this.response).subscribe((data: Blob) => {
      var a = document.createElement("a");
      a.href = URL.createObjectURL(data);
      a.download = `${new Date().getTime()}_report.doc`;
      a.click();
    }, (err) => {
      this.dialog.error(err);


    })
  }

  downloadReport() {
    var printPreview = window.open('CAMIS');
    var printDocument = printPreview.document;
    printDocument.open();
    printDocument.write(
      "<html> <head>" +
      document.head.innerHTML + "</head> " +
      document.getElementById("report_html").innerHTML +
      "</html>");
    printDocument.close();
  }

  filterBySelected(value) {
    alert(value);
  }
  exportToExcel(): void {
    this.exportService.exportToExcel('report_html', 'CAMIS_Report');
  }

  exportToPdf(): void {
    dialog.loading();
    this.exportService.exportToPdf('report_html', 'CAMIS_Report')
      .then(() => {
        dialog.close();
      })
      .catch(error => {
        dialog.close();
        return dialog.error('Error generating PDF: ' + error);
      });
  }

  exportToCsv(): void {
    this.exportService.exportToCsv('report_html', 'CAMIS_Report');
  }

  printReport(): void {
    this.exportService.printDivAlternative('report_html');
  }
}

