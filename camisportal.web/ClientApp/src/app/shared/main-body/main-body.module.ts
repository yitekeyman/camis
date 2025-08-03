import {NgModule} from "@angular/core";
import {MainBodyComponent} from "./main-body.component";
import {CommonModule} from "@angular/common";
import {RouterModule} from "@angular/router";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

@NgModule({
    declarations:[MainBodyComponent],
    imports:[CommonModule,RouterModule, FormsModule, ReactiveFormsModule,],
    exports:[MainBodyComponent],
    providers:[]
})
export class MainBodyModule {
    
}