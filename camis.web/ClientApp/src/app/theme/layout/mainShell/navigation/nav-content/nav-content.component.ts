// Angular import
import { Component, OnInit, output, inject } from '@angular/core';
import { Location } from '@angular/common';
import { RouterModule } from '@angular/router';

//theme version
import { environment } from 'src/environments/environment';

// project import
import { NavigationItem } from '../navigation';

import { NavCollapseComponent } from './nav-collapse/nav-collapse.component';
import { NavGroupComponent } from './nav-group/nav-group.component';
import { NavItemComponent } from './nav-item/nav-item.component';

// NgScrollbarModule
import { SharedModule } from 'src/app/theme/shared/shared.module';

@Component({
  selector: 'app-nav-content',
  imports: [RouterModule, NavCollapseComponent, NavGroupComponent, NavItemComponent, SharedModule],
  templateUrl: './nav-content.component.html',
  styleUrl: './nav-content.component.scss'
})
export class NavContentComponent implements OnInit {
  private location = inject(Location);

  // public props
  NavCollapsedMob = output();
  SubmenuCollapse = output();
  private role:any;
  // version
  title = 'Demo application for version numbering';
  currentApplicationVersion = environment.appVersion;

  navigations!: NavigationItem[];
  windowWidth: number;

  // Constructor
  constructor() {
    this.role = localStorage.getItem('role');
    const defaultChildren:NavigationItem[]=[
      {
        id: 'default',
        title: 'Dashboard',
        type: 'item',
        classes: 'nav-item',
        url: '/default/dashboard',
        icon: 'ti ti-dashboard',
        breadcrumbs: false
      },
      {
        id: 'pending-task',
        title: 'Pending Task',
        type: 'item',
        classes: 'nav-item',
        url: '/default/pending-task',
        icon: 'ti ti-list-details',
        breadcrumbs: false
      }
    ];
    if (['1','3', '5', '6', '7','8','9'].includes(this.role || '')) {
      defaultChildren.push( {
        id: 'land-report',
        title: 'Reports',
        type: 'item',
        classes: 'nav-item',
        url: '/default/reports',
        icon: 'ti ti-report'
      })
    }
    const NavigationItems: NavigationItem[] = [
      {
        id: 'dashboard',
        title: 'CAMIS Home',
        type: 'group',
        icon: 'icon-navigation',
        children: defaultChildren
      }
    ];

// Add Admin section for role '1'
    if (this.role === '1') {
      NavigationItems.push({
        id: 'admin',
        title: 'System Administration',
        type: 'group',
        icon: 'icon-navigation',
        children: [
          {
            id: 'users',
            title: 'User Management',
            type: 'item',
            icon: 'ti ti-users',
            url: '/admin/user-management',
            classes: 'nav-item',
            breadcrumbs: true
          },
          {
            id: 'activity-log',
            title: 'Activity Log',
            type: 'item',
            icon: 'ti ti-device-desktop-analytics',
            url: '/admin/activity-log',
            classes: 'nav-item',
            breadcrumbs: true
          },
          {
            id: 'configuration-admin',
            title: 'Configuration',
            type: 'collapse',
            icon: 'ti ti-settings-check',
            children: [
              {
                id: 'activity-template',
                title: 'Activity Template',
                type: 'item',
                url: '/admin/activity-template',
                icon: 'ti ti-file-diff',
                classes: 'nav-item'
              },
              {
                id: 'sys-config',
                title: 'System Config',
                type: 'item',
                url: '/admin/sys-config',
                icon: 'ti ti-database-cog',
                classes: 'nav-item'
              }
            ]
          }
        ]
      });
    }

// Add Land Bank section for roles 3,4,5,6,7
    if (['3', '4', '5', '6', '7'].includes(this.role || '')) {
      const landBankChildren: NavigationItem[] = [
        {
          id: 'search-land',
          title: 'Search Parcel',
          type: 'item',
          classes: 'nav-item',
          url: '/land-bank/search-parcel',
          icon: 'ti ti-map-search'
        }
      ];

      // Add Register Land only for role '2'
      if (this.role === '4') {
        landBankChildren.push({
          id: 'register-land',
          title: 'Register New Parcel',
          type: 'item',
          classes: 'nav-item',
          url: '/land-bank/register-parcel',
          icon: 'ti ti-map-plus'
        });
      }
      NavigationItems.push({
        id: 'land-bank',
        title: 'Land Bank',
        type: 'group',
        icon: 'icon-navigation',
        children: landBankChildren
      });
    }

// Add Farm Management section for roles 2,3
    if (['2', '3','6','7'].includes(this.role || '')) {
      const farmManagementChildren: NavigationItem[] = [
        {
          id: 'search-farm',
          title: 'Search Farm & Owners',
          type: 'item',
          url: '/farm-management/search-farm',
          classes: 'nav-item',
          icon: 'ti ti-building-cottage'
        }
      ];

      // Add Register Farm only for role '2'
      if (this.role === '2') {
        farmManagementChildren.push({
          id: 'register-farm',
          title: 'Register Farm & Owners',
          type: 'item',
          classes: 'nav-item',
          url: '/farm-management/fc/farm/registration/new',
          icon: 'ti ti-tractor',
        });
      }

      NavigationItems.push({
        id: 'farm-management',
        title: 'Farm Management',
        type: 'group',
        icon: 'icon-navigation',
        children: farmManagementChildren
      });
    }
    if (['8', '9','10'].includes(this.role || '')) {
      const mneChildren: NavigationItem[] = [
        {
          id: 'search-farm',
          title: 'Search Farm & Owners',
          type: 'item',
          url: '/mne/search-farm',
          classes: 'nav-item',
          icon: 'ti ti-building-cottage'
        }
      ];
      NavigationItems.push({
        id: 'mne-management',
        title: 'M&E Management',
        type: 'group',
        icon: 'icon-navigation',
        children: mneChildren
      });
    }

    this.navigations = NavigationItems;
    this.windowWidth = window.innerWidth;
  }

  // Life cycle events
  ngOnInit() {
    if (this.windowWidth < 1025) {
      setTimeout(() => {
        (document.querySelector('.coded-navbar') as HTMLDivElement).classList.add('menupos-static');
      }, 500);
    }
  }

  fireOutClick() {
    let current_url = this.location.path();
    // eslint-disable-next-line
    // @ts-ignore
    if (this.location['_baseHref']) {
      // eslint-disable-next-line
      // @ts-ignore
      current_url = this.location['_baseHref'] + this.location.path();
    }
    const link = "a.nav-link[ href='" + current_url + "' ]";
    const ele = document.querySelector(link);
    if (ele !== null && ele !== undefined) {
      const parent = ele.parentElement;
      const up_parent = parent?.parentElement?.parentElement;
      const last_parent = up_parent?.parentElement;
      if (parent?.classList.contains('coded-hasmenu')) {
        parent.classList.add('coded-trigger');
        parent.classList.add('active');
      } else if (up_parent?.classList.contains('coded-hasmenu')) {
        up_parent.classList.add('coded-trigger');
        up_parent.classList.add('active');
      } else if (last_parent?.classList.contains('coded-hasmenu')) {
        last_parent.classList.add('coded-trigger');
        last_parent.classList.add('active');
      }
    }
  }
}
