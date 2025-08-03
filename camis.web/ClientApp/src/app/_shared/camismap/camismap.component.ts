import {Component, Input, OnInit, ElementRef} from '@angular/core';
import {Observable} from 'rxjs/Observable';
//Open layer
import OlMap from 'ol/Map';
import OlView from 'ol/View';
import {fromLonLat} from 'ol/proj';
import OLTileWMS from "ol/source/TileWMS";
import OlTileLayer from 'ol/layer/Tile';
import {register} from 'ol/proj/proj4';
import GeoJSON from 'ol/format/GeoJSON'
import WKT from 'ol/format/WKT';
import {OSM, Vector as VectorSource} from 'ol/source';
import {Tile as TileLayer, Vector as VectorLayer} from 'ol/layer';
import {Fill, Stroke, Style} from 'ol/style';
import XYZ from 'ol/source/XYZ';

//End: Open Layer
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ApiService} from '../../_services/api.service';
import {} from 'googlemaps';


import {LandDataService} from '../../_services/land-data.service';
import {$} from 'protractor';
import {fail} from 'assert';


declare var proj4: any;

@Component({
  selector: 'app-camis-map',
  templateUrl: './camismap.component.html',
  styleUrls: ['./camismap.component.css']
})


export class CamisMapComponent implements OnInit {

  view: OlView;
  map: OlMap;
  land: LandDataService;
  api: ApiService;
  mapConfig: any;


  nrlais_features: VectorSource;
  nrlais_style: any;

  workflow_features: VectorSource;
  workflow_style: any;

  split_features: VectorSource;
  split_style: any;

  zoomMargin: number = 1.3;
  mapType: string = 'satellite';
  backTo: string = 'Kebele';
  layer = [];

  constructor(public _http: HttpClient, land: LandDataService, api: ApiService, private el: ElementRef) {
    this.api = api;
    this.land = land;
    if (localStorage.getItem('mapType') != null) {
      this.mapType = localStorage.getItem('mapType');
    }
    if (localStorage.getItem('backTo') != null) {
      this.backTo = localStorage.getItem('backTo');
    }
  }

  @Input('id')
  id = 1;

  ngOnInit() {
    this.initMap();
  }

  defineNrlaisLayer() {
    this.nrlais_features = new VectorSource({
      features: []
    });
    this.nrlais_style = new Style({
      stroke: new Stroke({
        color: 'blue',
        lineDash: [4],
        width: 3
      }),
      fill: new Fill({
        color: 'rgba(0, 0, 255, 0.1)'
      })
    });
    var nrlais_features_layer = new VectorLayer({
      source: this.nrlais_features,
      style: () => this.nrlais_style
    });
    return nrlais_features_layer;
  }

  defineWorkFlowLayer(): any {
    this.workflow_features = new VectorSource({
      features: []
    });

    this.workflow_style = new Style({
      stroke: new Stroke({
        color: 'blue',
        lineDash: [4],
        width: 3
      }),
      fill: new Fill({
        color: 'rgba(0, 355, 255, 0.1)'
      })
    });

    var workflow_features_layer = new VectorLayer({
      source: this.workflow_features,
      style: () => this.workflow_style
    });
    return workflow_features_layer;
  }

  defineSplitLayer(): any {
    this.split_features = new VectorSource({
      features: []
    });

    this.split_style = new Style({
      stroke: new Stroke({
        color: 'red',
        width: 3
      }),
      fill: new Fill({
        color: 'rgba(255, 0, 0, 0.1)'
      })
    });

    var ret = new VectorLayer({
      source: this.split_features,
      style: () => this.split_style
    });
    return ret;
  }

  initMap() {
    proj4.defs('EPSG:20137', '+proj=utm +zone=37 +ellps=clrk80 +units=m +no_defs');

    register(proj4);
    this.mapConfig =
      {
        server: `http://${window.location && window.location.hostname || 'localhost'}:8080/`,
        layers: [
          {
            name: "Country",
            wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:ne_10m_admin_0_countries",
            type:"text/xml"
          },
          {
            name: "Region",
            wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_regions",
            type:"text/xml"
          },
          {
            name: "Woreda",
            wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_woredas",
            type:"text/xml"
          },
          {
            name: "Kebele",
            wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_kebeles",
            type:"text/xml"
          },
          {
            name: "Land",
            wms: "camis/wms?service=WMS&version=1.1.0&request=GetMap&layers=layers=camis:v_gs_land",
            type:"text/xml"
          },
        ],
      };

    this.view = new OlView({
      center: [335320.696579432, 1294832.60257192],
      resolution: 300,
      projection: 'EPSG:20137',
      maxResolution: 10000,
      minResolution: 0.1,

    });
    this.buildLayers();

    this.map = new OlMap({
      target: 'camis_map',
      layers: this.layer,
      view: this.view,
    });
    this.zoomToExtent();
  }

  createMap() {

  }

  zoomToExtent() {

    this.api.get("map/GetLandMapBound").subscribe(bbox => {
      var dw = this.el.nativeElement.firstChild.clientWidth;
      var dh = this.el.nativeElement.firstChild.clientHeight;
      var hres = (bbox.x2 - bbox.x1) * this.zoomMargin / dw;
      var vres = (bbox.y2 - bbox.y1) * this.zoomMargin / dh;
      var res = (vres > hres ? vres : hres) * 1.2;
      this.view.animate({
        center: [(bbox.x2 + bbox.x1) / 2, (bbox.y2 + bbox.y1) / 2],
        resolution: res,
        projection: "EPSG:20137",
      });
    });
  }

  zoomToSetExtent(ext: any) {
    var dw = this.el.nativeElement.firstChild.clientWidth;
    var dh = this.el.nativeElement.firstChild.clientHeight;
    var hres = (ext[2] - ext[0]) * this.zoomMargin / dw;
    var vres = (ext[3] - ext[1]) * this.zoomMargin / dh;
    var res = vres > hres ? vres : hres;
    this.view.animate({
      center: [(ext[2] + ext[0]) / 2, (ext[3] + ext[1]) / 2],
      resolution: res,
      projection: "EPSG:20137",
    });
  }

  zoomToParcel(upin: String, ready: any) {
    ready();
  }

  public setNrlaisParcel(upin: String) {
    if (upin == null)
      return;
    var wms_url = `map/WfsGet?service=WFS&version=1.0.0&request=GetFeature&typeName=nrlais:nrlais_inventory.t_parcels&maxFeatures=50&outputFormat=application/json&CQL_FILTER=upid='${upin}'`;
    this.api.get(wms_url).subscribe(d => {
      if (d.error)
        console.log(`Error reading geojson for upin:${upin}\n${d.error}`);
      else {
        var fs = (new GeoJSON()).readFeatures(d.response);
        this.nrlais_features.clear();
        this.nrlais_features.addFeatures(fs);
        var extent = this.nrlais_features.getExtent();
        this.zoomToSetExtent(extent);
      }
    });
  }

  public setWorkFlowGeomByWKT(wkt: String) {
    this.workflow_features.clear();
    var fs = (new WKT()).readFeatures(wkt);
    this.workflow_features.addFeatures(fs);
    var extent = this.workflow_features.getExtent();
    this.zoomToSetExtent(extent);
  }

  public setSplitGeomsByWKT(wkts: String[]) {
    this.split_features.clear();
    wkts.forEach(wkt => {
      var fs = (new WKT()).readFeatures(wkt);
      this.split_features.addFeatures(fs);
    });
    var extent = this.split_features.getExtent();
    this.zoomToSetExtent(extent);
  }

  public buildLayers() {
    var layers = [];
    let mapTypes = '';
    if (this.mapType == 'satellite') {
      mapTypes = new XYZ({
        url: 'http://mt0.google.com/vt/lyrs=s&hl=en&x={x}&y={y}&z={z}'
      })
    } else if (this.mapType == 'roadmap') {
      mapTypes = new XYZ({
        url: 'http://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}'
      })
    } else if (this.mapType == 'hybrid') {
      mapTypes = new XYZ({
        url: 'http://mt0.google.com/vt/lyrs=y&hl=en&x={x}&y={y}&z={z}'
      })
    } else {
      mapTypes = '';
    }
    for (let l of this.mapConfig.layers) {
      var wmsLayer = new OlTileLayer({
        source: new OLTileWMS({
          url: this.mapConfig.server + l.wms,
          params: {},
          serverType: 'geoserver'
        })
      });

      if (this.mapType != "") {
        if (l.name == this.backTo && mapTypes != '') {
          layers.push(new TileLayer({
            source: mapTypes
          }))

        }
        if ((this.backTo == "Region" && l.name == "Region") || (this.backTo == "Woreda" && l.name == "Woreda") || (this.backTo == "Kebele" && l.name == "Kebele")) {
          layers.push(wmsLayer);
        }

        if (l.name == "Land") {
          layers.push(wmsLayer);
        }
      } else {
        layers.push(wmsLayer);
      }
    }


    layers.push(this.defineNrlaisLayer());
    layers.push(this.defineWorkFlowLayer());
    layers.push(this.defineSplitLayer());
    this.layer = layers;
  }

  public googleMapSetting() {
    localStorage.setItem('mapType', this.mapType);
    localStorage.setItem('backTo', this.backTo);
    this.buildLayers();
    document.querySelector('div#camis_map').innerHTML = "";
    this.map = new OlMap({
      target: 'camis_map',
      layers: this.layer,
      view: this.view,
    });
    this.zoomToExtent();
  }
}
