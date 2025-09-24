// Angular import
import {Component, OnInit} from '@angular/core';
import {Router, RouterModule} from '@angular/router';

// third party import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import {AdminServices} from "../../../../../_services/admin.Services";

@Component({
  selector: 'app-nav-right',
  imports: [RouterModule, SharedModule],
  templateUrl: './nav-right.component.html',
  styleUrls: ['./nav-right.component.scss']
})
export class NavRightComponent implements OnInit{
  public username:string;
  public role:number;
  public roleName:string;
  constructor(private  adminService:AdminServices, private router:Router) {
  }
  ngOnInit() {
    this.username=localStorage.getItem('username');
    this.roleName=localStorage.getItem('roleName');
  }
  logout(event: Event) {
    event.preventDefault();
    this.adminService.logout();
    this.router.navigate(['login']);
  }
}
