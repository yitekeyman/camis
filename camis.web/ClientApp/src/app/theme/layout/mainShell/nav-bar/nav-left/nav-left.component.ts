// Angular import
import {Component, Output, EventEmitter, OnInit, OnDestroy} from '@angular/core';
import {Subscription} from "rxjs";
import {AdminServices} from "../../../../../_services/admin.Services";

@Component({
  selector: 'app-nav-left',
  templateUrl: './nav-left.component.html',
  styleUrls: ['./nav-left.component.scss']
})
export class NavLeftComponent implements OnInit, OnDestroy {
  // public props
  @Output() NavCollapsedMob = new EventEmitter();

  public regionName: string="Region";
  public regionCode: string="Code";
public utmZone: string="37";
  private subscriptions: Subscription = new Subscription();

  constructor(private adminService: AdminServices) {}
ngOnInit() {
  this.subscriptions.add(
    this.adminService.regionName$.subscribe(value => this.regionName = value)
  );
  this.subscriptions.add(
    this.adminService.regionCode$.subscribe(value => this.regionCode = value)
  );
  this.subscriptions.add(
    this.adminService.utmZone$.subscribe(value => this.utmZone = value)
  );
}
  ngOnDestroy() {
    // Clean up subscriptions to avoid memory leaks
    this.subscriptions.unsubscribe();
  }
}
