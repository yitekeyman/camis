import {Component, OnInit} from '@angular/core';
import {DialogService} from '../dialog.service';
import {Observable} from 'rxjs';
import {IDialog, IDialogButton} from '../interfaces';
import {CommonModule} from "@angular/common";

@Component({
  selector: 'app-dialog',
  imports:[CommonModule],
  templateUrl: './dialog.component.html',
  styleUrls: ['./dialog.component.css']
})
export class DialogComponent implements OnInit {

  constructor(private service: DialogService) { }

  ngOnInit() {
  }

  get dialog$(): Observable<IDialog | undefined> {
    return this.service.dialog$;
  }

  close(): void {
    this.service.close();
  }


  handleClickFor(btn: IDialogButton) {
    if (btn.onClick) {
      btn.onClick();
    }

    if (btn.noCloseOnClick !== true) {
      this.close();
    }
  }

}
