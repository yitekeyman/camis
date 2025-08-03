import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {BidServices} from "./bid.services";
import {PromotionUnit} from "../_models/bid.model";
import {Application} from "../_models/investor.model";
import {AuthServices} from "./auth.services";
import {r} from "@angular/core/src/render3";

@Injectable()
export class ResolverServices implements Resolve<PromotionUnit>{
    constructor(public bidService:BidServices, public authService:AuthServices){}
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot){
        return this.bidService.getPromotion(<string>route.paramMap.get("prom_id"));
    }
    resolve2(route:ActivatedRouteSnapshot, state:RouterStateSnapshot){
        return this.bidService.getApplication(<string>route.paramMap.get("prom_id"), <string>route.paramMap.get("investor_id"));
    }
    resolveUser(route:ActivatedRouteSnapshot, state:RouterStateSnapshot){
        return this.authService.getUser(<string>route.paramMap.get("username"));
    }
}