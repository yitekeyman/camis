import {Route, RouterModule} from "@angular/router";
import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";

const routes:Route[]=[
    {path:'', pathMatch:'full', redirectTo:'default'},
    {path:'default', loadChildren:'app/default/default.module#DefaultModule'},
    {path:'investor', loadChildren:'app/investor/investor.module#InvestorModule'},
    {path:'admin', loadChildren:'app/admin/admin.module#AdminModule'}
];

@NgModule({
    imports:[
        CommonModule,
        RouterModule.forRoot(routes)
    ],
    exports:[RouterModule],
    declarations:[]
})

export class AppRoutingModule {
    
}