import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IDocument} from '../interfaces';
import {IDocumentListOpenEvent} from './interfaces';
import {configs} from '../../../app-config';

@Component({
    selector: 'app-document-list',
    templateUrl: 'document-list.component.html',
})
export class DocumentListComponent implements OnInit {

    /* Inputs */
    @Input('title') title = 'Documents';
    @Input('documents') documents: IDocument[] = [];

    /* Output */
    @Output('open') open = new EventEmitter<IDocumentListOpenEvent>();


    constructor () {
    }

    ngOnInit(): void {
    }


    getLink(doc: IDocument): string {
      if (doc.overrideFilePath) {
        return doc.overrideFilePath;
      }

      return !doc.id && doc.mimetype && doc.file ?
            `javascript:;` :
            `${configs.url}Document/DocumentFile/${doc.id}`;
    }

    openDoc(e: any, doc: IDocument): void {
        if (!doc.overrideFilePath && !doc.id && doc.mimetype && doc.file) {
            e.preventDefault();
            window.open(`data:${doc.mimetype};base64,${doc.file}`, '_blank');
        }

        this.open.emit({ document: doc });
    }

}
