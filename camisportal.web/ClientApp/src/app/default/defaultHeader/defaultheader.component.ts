import {Component, OnInit} from "@angular/core";
import {IMainShellRoute} from "../../shared/interface";

@Component({
    selector:'app-default',
    templateUrl:'./defaultheader.component.html'
})
export class DefaultheaderComponent{
    routes:IMainShellRoute[]=[
        {
            route:'bids',
            title:'Investment Opportunities',
            icon:'',
            click:'',
            child:null
        },
        {
            route:'register',
            title:'Register',
            icon:'',
            click:'',
            child:null
        },{
            route:'login',
            title:'Log In',
            icon:'',
            click:'',
            child:null
        }
    ];
    
    ngOnInit():void{
        
    }
}