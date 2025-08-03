import {Component, OnInit} from '@angular/core';
import {IMainShellRoute} from '../../_shared/main-shell/interfaces';

@Component({
  selector: 'app-land-certificate-issuer',
  templateUrl: 'land-certificate-issuer.component.html'
})
export class LandCertificateIssuerComponent implements OnInit {

  routes: IMainShellRoute[] = [
    {
      route: 'dashboard',
      title: 'Pending Tasks',
      iconStyle: 'fa-tasks'
    },
    {
      route: 'farms',
      title: 'Search Farms and Farm Owners',
      iconStyle: 'fa-search'
    },
  ];

  constructor () {
  }

  ngOnInit(): void {
  }

}
