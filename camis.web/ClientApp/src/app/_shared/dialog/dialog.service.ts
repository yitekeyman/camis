import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IDialog } from './interfaces';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  private _dialog?: IDialog;

  constructor() { }

  get dialog$(): Observable<IDialog | undefined> {
    return Observable.create(observer => {
      observer.next(this._dialog);
      observer.complete();
    });
  }

  open(dialog: IDialog): void {
    this._dialog = dialog;
  }

  close(): void {
    this._dialog = undefined;
  }


  error(e: any, overrideDialog?: IDialog): void {
    this.open(Object.assign({
      title: 'Error!',
      message: e && (e.message || JSON.stringify(e)) || 'Unknown error.',
      buttons: [
        { name: 'Close', className: 'btn-danger',  }
      ],
      titleClassName: 'text-danger',
      messageClassName: 'text-warning',
    }, overrideDialog));
  }
}
