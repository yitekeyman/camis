import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogComponent } from './dialog/dialog.component';
import { DialogService } from './dialog.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [DialogComponent],
  providers: [DialogService],
  exports: [
    DialogComponent,
  ]
})
export class DialogModule { }
