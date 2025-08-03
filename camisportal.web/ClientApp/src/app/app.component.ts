import { Component } from '@angular/core';
import {ApiService, ErrorMsg} from "./_services/api.services";
import {Router} from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
    public errData : {msg: '', title: ''};
    constructor(public apiService: ApiService, public router: Router){
        ApiService.apiEvent.subscribe((errMsg: ErrorMsg) => {
            

            if (errMsg.msg === "User not logedin or sesssion has expired"){ //session expired head to login  page
                this.router.navigate(["/default/login"]);
                localStorage.setItem('username','');

            }
        })
    }
}
