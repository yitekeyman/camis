import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AdminDashboardComponent} from './adminDashboard.component';
import {RouterModule} from '@angular/router';
import {AdminDashboardService} from './adminDashboard.service';


@NgModule({
    imports: [CommonModule, RouterModule],
    declarations: [AdminDashboardComponent],
    exports: [AdminDashboardComponent],
    providers: [AdminDashboardService]
})
export class AdminDashboardModule {

}
