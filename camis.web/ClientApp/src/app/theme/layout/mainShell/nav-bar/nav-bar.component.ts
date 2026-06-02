import { Component, output, OnDestroy, HostListener } from '@angular/core';
import { BerryConfig } from 'src/app/app-config';
import { NavLeftComponent } from './nav-left/nav-left.component';
import { NavLogoComponent } from './nav-logo/nav-logo.component';
import { NavRightComponent } from './nav-right/nav-right.component';

@Component({
  selector: 'app-nav-bar',
  imports: [NavLogoComponent, NavLeftComponent, NavRightComponent],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnDestroy {
  NavCollapse = output();
  NavCollapsedMob = output();

  navCollapsed: boolean;
  windowWidth: number;
  navCollapsedMob: boolean;

  constructor() {
    this.windowWidth = window.innerWidth;
    this.navCollapsed = this.windowWidth >= 1025 ? BerryConfig.isCollapse_menu : false;
    this.navCollapsedMob = false;
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.windowWidth = window.innerWidth;
    // Re-evaluate navCollapsed when crossing the desktop breakpoint
    const wasDesktop = this.navCollapsed !== undefined; // or store previous
    const isDesktop = this.windowWidth >= 1025;
    if (isDesktop) {
      // On desktop, use configured collapse state
      this.navCollapsed = BerryConfig.isCollapse_menu;
    } else {
      // On mobile, ensure it's expanded (or keep whatever makes sense)
      this.navCollapsed = false;
    }
  }

  navCollapse() {
    if (this.windowWidth >= 1025) {
      this.navCollapsed = !this.navCollapsed;
      this.NavCollapse.emit();
    }
  }

  navCollapseMob() {
    if (this.windowWidth < 1025) {
      this.NavCollapsedMob.emit();
    }
  }

  ngOnDestroy() {
    // No explicit unsubscribe needed for @HostListener, but keep if using RxJS
  }
}
