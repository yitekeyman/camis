import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {DocumentListComponent} from './document-list/document-list.component';
import {DocumentSelectorComponent} from './document-selector/document-selector.component';
import { SingleDocumentSelectorComponent } from './single-document-selector/single-document-selector.component';
import { DocumentDetailComponent } from './document-detail/document-detail.component';

@NgModule({
    declarations: [
        DocumentDetailComponent,
        DocumentListComponent,
        DocumentSelectorComponent,
        SingleDocumentSelectorComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
    ],
    exports: [
        DocumentDetailComponent,
        DocumentListComponent,
        DocumentSelectorComponent,
        SingleDocumentSelectorComponent,
    ],
    providers: []
})
export class DocumentModule { }
