import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IDocument} from '../interfaces';
import {ISingleDocumentSelectorChangeEvent} from './interfaces';

@Component({
  selector: 'app-single-document-selector',
  templateUrl: './single-document-selector.component.html'
})
export class SingleDocumentSelectorComponent implements OnInit {

  /* Inputs */
  @Input('title') title = 'Documents';
  @Input('document') document?: IDocument;

  /* Outputs */
  @Output('change') changeNotification = new EventEmitter<ISingleDocumentSelectorChangeEvent>();

  add_file = '';
  add_ref = '';
  add_date: string = new Date().toISOString().slice(0, 10);
  add_note = '';

  file_data?: string;
  file_mime?: string;


  constructor() { }

  ngOnInit() {
  }


  readFile(e: any): void {
    const file = e.target.files[0] as Blob;

    if (!file) {
      this.file_data = undefined;
      return;
    }

    const reader = new FileReader();
    reader.onload = ev => this.file_data = btoa((ev.target as any).result);
    reader.readAsBinaryString(file);

    this.file_mime = file.type;
  }


  changeDocument($event?: any): void {
    if ($event) {
      this.readFile($event);
      setTimeout(() => this.changeDocument(), 300);
    }

    this.document = !this.add_file || !this.add_ref || !this.add_date ?
      null :
      {
        file: this.file_data,
        mimetype: this.file_mime,
        filename: this.add_file.replace(/\\/gi, '/').split('/').pop(),
        ref: this.add_ref,
        date: new Date(this.add_date).getTime(),
        note: this.add_note
      };

    this.changeNotification.emit({ document: this.document });
  }

  clear(): void {
    this.add_file = '';
    this.add_ref = '';
    this.add_date = new Date().toISOString().slice(0, 10);
    this.add_note = '';

    this.file_data = undefined;
    this.file_mime = undefined;
  }
}
