import { Component, Input, OnInit, ElementRef, Output, OnChanges, SimpleChanges } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import 'rxjs/add/operator/catch';

import { $ } from 'protractor';
import { fail } from 'assert';

declare var proj4: any;
@Component({
  selector: 'app-camis-report',
  templateUrl: './report.component.html'
})

export class ReportComponent implements OnInit, OnChanges {

  private _rep = '';

  @Input()
  set rep(r: string) {
    this._rep = (r && r.trim()) || '';
    if (this._rep && this._rep != "")
      this.generateReport(this._rep, '');

  }

  get rep(): string { return this._rep; }

  ngOnChanges(changes: SimpleChanges): void {
  }
  constructor(public _http: HttpClient, private el: ElementRef) {
  }

  ngOnInit() {
  }
  generateReport(rep: String, par: String) {
    

      this._http.post('/api/report/generatereport?report_name=' + this.rep, par,
        {
          headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' }),
          responseType: 'text'
        }
      ).subscribe(x => {
        this.el.nativeElement.innerHTML  = x;
        },
        e => {
          this.el.nativeElement.innerHTML = "Failed to load report";
        });
    
  }
}
