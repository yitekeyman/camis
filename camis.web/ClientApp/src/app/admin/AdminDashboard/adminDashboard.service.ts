import {Injectable} from '@angular/core';
import {ApiService} from '../../_services/api.service';


@Injectable()
export class AdminDashboardService {
  url: string;
    constructor(public apiService: ApiService) {

    }

    public getAudits() {
        return this.apiService.get('audit/getaudit');
    }
}
