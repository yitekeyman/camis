import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {RouterModule} from "@angular/router";
import {PromotionDetailsComponent} from "./promotion_details.component";
import {PromotionUnitDetailsComponent} from "./promotionUnitDetail/promotionUnit_details.component";
import {MapComponent} from "./map/map.component";

@NgModule({
    declarations: [
        PromotionDetailsComponent,
        PromotionUnitDetailsComponent,
        MapComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
    ],
    exports: [
        PromotionDetailsComponent,
        PromotionUnitDetailsComponent,
        MapComponent
    ],
    providers: []
})
export class PromotionDetailsModule {
    
}