import { RouteReuseStrategy, ActivatedRouteSnapshot, DetachedRouteHandle, OutletContext } from '@angular/router';


interface RouteStorageObject {
    snapshot: ActivatedRouteSnapshot;
    handle: DetachedRouteHandle;
}

export class CustomRouteReuseStrategyinterface extends RouteReuseStrategy {


    storedRoutes: {[key: string]: RouteStorageObject } = {};
    private acceptedRoutes: string[] = ['land-clerk/land-dashboard', 'land-supervisor/land-dashboard'];
    previousUrl = '';

    public shouldDetach(route: ActivatedRouteSnapshot): boolean {

        const path = this.getPath(route);
        this.previousUrl = path;

        if (this.acceptedRoutes.indexOf(path) > -1) {
            return true;
        } else {
            return false;
        }
    }
    public store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle): void {
        const storedRoute: RouteStorageObject = {
            snapshot: route,
            handle: handle
        };
        const path = this.getPath(route);
        this.storedRoutes[path] = storedRoute;
    }
    public shouldAttach(route: ActivatedRouteSnapshot): boolean {
        const path = this.getPath(route);
        // tslint:disable-next-line:max-line-length
        const canAttach: boolean = !!route.routeConfig && !!this.storedRoutes[path] && (this.previousUrl == 'land-clerk/land-dashboard/land-detail/:landID' || this.previousUrl == 'land-supervisor/land-dashboard/land-detail/:landID');
        if (canAttach) {
            const willAttach = true;
            const paramsMatch: boolean = this.compareObjects(route.params, this.storedRoutes[path].snapshot.params);
            // tslint:disable-next-line:max-line-length
            const queryParamsMatch: boolean = this.compareObjects(route.queryParams, this.storedRoutes[path].snapshot.queryParams);

            // tslint:disable-next-line:max-line-length

            return paramsMatch && queryParamsMatch;
        } else { return false; }

    }
    public retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle {
        const path = this.getPath(route);
        if (!route.routeConfig || !this.storedRoutes[path]) { return null; }

        return this.storedRoutes[path].handle;
    }
    public shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
        // tslint:disable-next-line:max-line-length

        // if (curr.routeConfig.component.name === 'LandDashboardComponent') {
            return future.routeConfig === curr.routeConfig;

        // }
    }

    private compareObjects(base: any, compare: any): boolean {
        for (const baseProperty in base) {
            if (compare.hasOwnProperty(baseProperty)) {
                switch (typeof base[baseProperty]) {
                    case 'object':
                        // tslint:disable-next-line:max-line-length
                        if (typeof compare[baseProperty] !== 'object' || !this.compareObjects(base[baseProperty], compare[baseProperty])) { return false; } break;
                    case 'function':
                        // tslint:disable-next-line:max-line-length
                        if (typeof compare[baseProperty] !== 'function' || base[baseProperty].toString() !== compare[baseProperty].toString()) { return false; } break;
                    default:
                        if (base[baseProperty] != compare[baseProperty] ) { return false; }
                }
            } else {
                return false;
            }
        }

        return true;
    }

    public deactivateOutlet(handle: DetachedRouteHandle): void {
        const contexts: Map<string, OutletContext> = handle['contexts'];
        contexts.forEach((context: OutletContext, key: string) => {
            if (context.outlet) {
                context.outlet.deactivate();
                context.children.onOutletDeactivated();
            }
        });
    }
    private getPath(route: ActivatedRouteSnapshot) {
        return route.pathFromRoot
            // tslint:disable-next-line:no-shadowed-variable
            .map(route => route.routeConfig && route.routeConfig.path)
            .filter(path => !!path)
            .join('/');
    }
}
