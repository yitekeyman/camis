import {Component, Input, OnInit, ViewChild} from "@angular/core";
import swal from "sweetalert2";
import {BidServices} from "../../../_services/bid.services";
@Component({
    selector:'app-map',
    templateUrl:'./map.component.html'
})

export class MapComponent implements OnInit{
    @Input('promoID') promotion_ID:string;
    @Input('promoUnitID') promotionUnit_ID:string;

    @ViewChild('gmap') set mapElement(mapel: any) {
        if(mapel && this.map!=undefined)
            this.initMap(mapel);
    };
    map: google.maps.Map;
    polygon: any;
    map_center: any;
    map_bound: any;
    
    constructor(public bidServices:BidServices){}
    ngOnInit(){
        
       this.getLandData();
    }
    initMap(mapel: any): void {
        this.map = new google.maps.Map(mapel.nativeElement, {
            zoom: 5,
            center: new google.maps.LatLng(24.886, -70.268),
            mapTypeId: google.maps.MapTypeId.SATELLITE
        });
       

    }
    
    getLandData(){
        this.bidServices.getLandDetails(this.promotionUnit_ID,this.promotion_ID).subscribe(res => {
            if (this.map && res.polygon && res.polygon.length > 0) {
                // Define the LatLng coordinates for the polygon's path.

                this.map_center = new google.maps.LatLng(res.location.lat, res.location.lng);
                // Construct the polygon.
                this.polygon = [];
                for (var p in res.polygon) {
                    var mp = new google.maps.Polygon({
                        paths: res.polygon[0],
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: '#FF0000',
                        fillOpacity: 0.1
                    });
                    this.polygon.push(mp);
                }
                this.map_bound = new google.maps.LatLngBounds(new google.maps.LatLng(res.bottomLeft.lat, res.bottomLeft.lng),
                    new google.maps.LatLng(res.topRight.lat, res.topRight.lng));

                if (this.polygon) {
                    for (var i  in this.polygon)
                        this.polygon[i].setMap(this.map);
                }
                //if (this.map_center)
                //  this.map.setCenter(this.map_center);
                if (this.map_bound)
                    this.map.fitBounds(this.map_bound);

            }
        }, e => {
            swal({
                type: 'error', title: 'Oops...', text: e.message
            });
        });
    }
}