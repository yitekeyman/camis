import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FullScreenService {
  private isFullScreenSubject = new BehaviorSubject<boolean>(false);
  public isFullScreen$ = this.isFullScreenSubject.asObservable();

  enterFullScreen() {
    this.isFullScreenSubject.next(true);
  }

  exitFullScreen() {
    this.isFullScreenSubject.next(false);
  }

  toggleFullScreen() {
    this.isFullScreenSubject.next(!this.isFullScreenSubject.value);
  }
}
