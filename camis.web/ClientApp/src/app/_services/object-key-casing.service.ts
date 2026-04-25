import {Injectable} from '@angular/core';
import {DatePipe} from "@angular/common";

@Injectable()
export class ObjectKeyCasingService {
  camelCase(obj: any): void {
    for (let key in obj) {
      if (obj.hasOwnProperty(key)) {
        key = key.toString();
        const newKey = key.substr(0, 1).toLowerCase() + key.substr(1);

        const oldVal = obj[key];
        delete obj[key];
        obj[newKey] = oldVal;

        if (typeof obj[newKey] == 'object') {
          this.camelCase(obj[newKey]);
        }
      }
    }
  }

  PascalCase(obj: any): void {
    for (let key in obj) {
      if (obj.hasOwnProperty(key)) {
        key = key.toString();
        const newKey = key.substr(0, 1).toUpperCase() + key.substr(1);

        const oldVal = obj[key];
        delete obj[key];
        obj[newKey] = oldVal;

        if (typeof obj[newKey] == 'object') {
          this.PascalCase(obj[newKey]);
        }
      }
    }
  }
  convertDate(date: any) {
    let currentDate = new Date();
    let dates = new Date(date);
    let dateTimePipe = new DatePipe("en-US");
    let ret = "";
    if (dateTimePipe.transform(currentDate, 'MMM dd, yyyy') == dateTimePipe.transform(date, 'MMM dd, yyyy')) {
      ret = dateTimePipe.transform(date, 'hh:mm:ss aa ');
    } else if (dateTimePipe.transform(currentDate, 'W MMM yyyy') == dateTimePipe.transform(date, 'W MMM yyyy')) {
      ret = dateTimePipe.transform(date, 'EE hh:mm:ss aa');
    } else if (dateTimePipe.transform(currentDate, 'yyyy') == dateTimePipe.transform(date, 'yyyy')) {
      ret = dateTimePipe.transform(date, 'MMM dd hh:mm:ss aa');
    } else {
      ret = dateTimePipe.transform(date, 'MMM dd, yyyy');
    }

    return ret;
  }
}
