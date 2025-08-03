import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IDocument} from '../interfaces';
import {IDocumentSelectorChangeEvent} from './interfaces';

@Component({
    selector: 'app-document-selector',
    templateUrl: 'document-selector.component.html',
})
export class DocumentSelectorComponent implements OnInit {

    /* Inputs */
    @Input('title') title = 'Documents';
    @Input('documents') documents: IDocument[] = [];
    @Input('max') max: number = Number.MAX_SAFE_INTEGER;

    /* Outputs */
    @Output('change') changeNotification = new EventEmitter<IDocumentSelectorChangeEvent>();

    add_file = '';
    add_ref = '';
    add_date: string = new Date().toISOString().slice(0, 10);
    add_note = '';

    file_data?: string;
    file_mime?: string;

    constructor () {
    }

    ngOnInit(): void {
    }


    removeDoc(index: number): void {
        this.documents.splice(index, 1);

        this.changeNotification.emit({ documents: this.documents });
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

    addDocument(): void {
        this.documents.push({
            file: this.file_data,
            mimetype: this.file_mime,
            filename: this.add_file.replace(/\\/gi, '/').split('/').pop(),
            ref: this.add_ref,
            date: new Date(this.add_date).getTime(),
            note: this.add_note
        });

        this.changeNotification.emit({ documents: this.documents });

        this.clear();
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
