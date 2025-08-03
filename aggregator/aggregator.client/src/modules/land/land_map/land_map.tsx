import React, { Component }  from 'react';

import OlMap from 'ol/Map';
import OlView from 'ol/View';
import { fromLonLat } from 'ol/proj';
import OLTileWMS from "ol/source/TileWMS";
import OlTileLayer from 'ol/layer/Tile';
import { register } from 'ol/proj/proj4';
import GeoJSON from 'ol/format/GeoJSON'
import WKT from 'ol/format/WKT';
import { OSM, Vector as VectorSource } from 'ol/source';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import { Fill, Stroke, Style } from 'ol/style';

import LandService from '../../../_setup/services/land.service';
import {Api} from '../../../_setup/services/api';
import Projection from 'ol/proj/Projection';
import * as proj4 from 'proj4';
import './land_map.scss';

//declare var proj4: proj4;

export default class Map extends Component<any, any>{

    //proj4 : any;
    view: OlView;
    map: OlMap;
    land: LandService;
    api: Api;
    mapConfig: any;
  
  
    nrlais_features: VectorSource;
    nrlais_style: any;
  
    workflow_features: VectorSource;
    workflow_style: any;
  
    split_features: VectorSource;
    split_style: any;
  
    zoomMargin: number = 1.3;
    mapRef : React.RefObject<any>;

    constructor(props : any){
        super(props);

        this.land = new LandService(this.props);
        this.api = new Api(this.props,'Map');

        this.mapRef = React.createRef();
        this.view = new OlView();
        this.map = new OlMap({});
        this.nrlais_features = new VectorSource();
        this.workflow_features = new VectorSource();
        this.split_features = new VectorSource();


    }

    componentDidMount(){
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
        //console.log(proj4);
        proj4.default.defs('EPSG:20137', '+proj=utm +zone=37 +ellps=clrk80 +units=m +no_defs')
        //proj4.defs('EPSG:20137', '+proj=utm +zone=37 +ellps=clrk80 +units=m +no_defs');
    
        register(proj4.default);
        this.mapConfig =
          {
            server: `http://${window.location && window.location.hostname || 'localhost'}:8080/`,
            layers: [
              {
                name: "Country",
                wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:ne_10m_admin_0_countries"
              },
              {
                name: "Region",
                wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_regions"
              },
              {
                name: "Woreda",
                wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_woredas"
              },
              {
                name: "Kebele",
                wms: "nrlais/wms?service=WMS&version=1.1.0&request=GetMap&layers=nrlais:t_kebeles"
              },
              {
                name: "Land",
                wms: "camis/wms?service=WMS&version=1.1.0&request=GetMap&layers=layers=camis:v_gs_land"
              },
            ]
          };

        this.view = new OlView({
          center: [335320.696579432, 1294832.60257192],
          resolution: 300,
          projection:  'EPSG:20137',
          maxResolution: 10000,
          minResolution: 0.1,
        });
        var layers = [];
        for (let l of this.mapConfig.layers) {
          var wmsLayer = new OlTileLayer({
            source: new OLTileWMS({
              url: this.mapConfig.server + l.wms,
              params: {},
              serverType: 'geoserver'
            })
          });
          layers.push(wmsLayer);
        }
    
        layers.push(this.defineNrlaisLayer());
        layers.push(this.defineWorkFlowLayer());
        layers.push(this.defineSplitLayer());
    
        this.map = new OlMap({
          target: 'camis_map',
          layers: layers,
          view: this.view
        });
        this.zoomToExtent();
      }


      createMap() {

    }
  
    zoomToExtent() {
   var self = this;
    this.api.get("/GetLandMapBound").then((response) => {
        console.log(response);
        console.log(this.mapRef.current);
        var bbox : any = response.data;
        if(bbox != null){
          var dw = this.mapRef.current.offsetWidth;
          var dh = this.mapRef.current.offsetHeight;
          var hres = (bbox.x2 - bbox.x1) * this.zoomMargin / dw;
          var vres = (bbox.y2 - bbox.y1) * this.zoomMargin / dh;
          var res = (vres > hres ? vres : hres)*1.2;
          // this.view.animate({
          //   center : [35,10],
          //   resolution : res,
          //   //center : [(bbox.x2 - bbox.x1) / 2, (bbox.y2 + bbox.y1) / 2]
          // })
          // this.view.animate({
          //   center: [(bbox.x2 + bbox.x1) / 2, (bbox.y2 + bbox.y1) / 2],
          //   resolution: res,
            
          //   //projection: new Projection({ code : 'EPSG:20137'}) ,
          // });
        }
     
    })
    }

    zoomToSetExtent(ext: any) {
      var dw = this.mapRef.current.offsetWidth;
      var dh = this.mapRef.current.offsetHeight;
      var hres = (ext[2] - ext[0]) * this.zoomMargin / dw;
      var vres = (ext[3] - ext[1]) * this.zoomMargin / dh;
      var res = vres > hres ? vres : hres;
      this.view.animate({
        center: [(ext[2] + ext[0]) / 2, (ext[3] + ext[1]) / 2],
        resolution: res,

       // projection: "EPSG:20137",
      });
    }

    zoomToParcel(upin: String, ready: any) {
      ready();
    }
    
    public setNrlaisParcel(upin: String) {
      if (upin == null)
        return;
      var wms_url = `/WfsGet?service=WFS&version=1.0.0&request=GetFeature&typeName=nrlais:nrlais_inventory.t_parcels&maxFeatures=50&outputFormat=application/json&CQL_FILTER=upid='${upin}'`;
      this.api.get(wms_url).then((response) => {
        var d : any = response.data;
        if (d.error)
        console.log(`Error reading geojson for upin:${upin}\n${d.error}`);
      else {
        var fs = (new GeoJSON()).readFeatures(d.response);
        this.nrlais_features.clear();
        this.nrlais_features.addFeatures(fs);
        var extent = this.nrlais_features.getExtent();
        this.zoomToSetExtent(extent);
      }
      })
    }
  
    public setWorkFlowGeomByWKT(wkt: String) {
  
      var fs = (new WKT()).readFeatures(wkt);
      this.workflow_features.clear();
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

    render(){
        return(
            <div id="camis_map" ref={this.mapRef}></div>
        )
    }

}