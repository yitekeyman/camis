import {Component, OnInit} from "@angular/core";

@Component({
    selector:'app-admin-dashboard',
    templateUrl:'./adminDashboard.component.html'
})
export class AdminDashboardComponent implements OnInit{
    
    public userRole:number;
    ngOnInit(){
        this.userRole=parseInt(localStorage.getItem('role'));
    }
}