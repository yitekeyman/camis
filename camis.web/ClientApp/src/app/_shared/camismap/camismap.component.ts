import {Component, Input, OnInit, ElementRef, OnDestroy, AfterViewInit, Output, EventEmitter} from '@angular/core';
import {HttpClient, HttpClientModule} from '@angular/common/http';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

// OpenLayers imports
import OlMap from 'ol/Map';
import OlView from 'ol/View';
import OLTileWMS from 'ol/source/TileWMS';
import {register} from 'ol/proj/proj4';
import GeoJSON from 'ol/format/GeoJSON';
import WKT from 'ol/format/WKT';
import {Vector as VectorSource} from 'ol/source';
import {Tile as TileLayer, Vector as VectorLayer} from 'ol/layer';
import {Fill, Stroke, Style} from 'ol/style';
import XYZ from 'ol/source/XYZ';
import Feature from 'ol/Feature';
import Geometry from 'ol/geom/Geometry';

// Services
import {ApiService} from '../../_services/api.service';
import {LandDataService} from '../../_services/land-data.service';
import {FullScreenService} from "../../_services/full-screen.service";

declare var proj4: any;

interface WmsLayerConfig {
  name: string;
  layerName: string;
  visible: boolean;
}

interface BoundingBox {
  x1: number;
  y1: number;
  x2: number;
  y2: number;
}

@Component({
  selector: 'app-camis-map',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, HttpClientModule],
  templateUrl: './camismap.component.html',
  styleUrls: ['./camismap.component.css']
})
export class CamisMapComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() id = 1;

  private view!: OlView;
  private map!: OlMap;

  // Vector sources
  private nrlaisSource!: any;
  private workflowSource!: any;
  private splitSource!: any;
  private environmentalChangeSource!: any;

  // Vector layers - use any to avoid complex generic issues
  private environmentalChangeLayer!: any;

  // Styles
  private nrlaisStyle!: Style;
  private workflowStyle!: Style;
  private splitStyle!: Style;

  // Configuration
  zoomMargin = 1.3;
  mapType = 'satellite';
  backTo = 'Kebele';
  utmZone=localStorage.getItem("UTM");
  private layers: any[] = [];

  // Connectivity monitoring
  private onlineListener!: () => void;
  private offlineListener!: () => void;
  private isUsingOfflineLayer = false;
  isFullScreen = false;

  // Cache busting for WMS layers
  private cacheBuster = Date.now();

  // Environmental change detection
  public changeDetectionActive = false;

  constructor(
    private http: HttpClient,
    private land: LandDataService,
    private api: ApiService,
    private elementRef: ElementRef,
    private fullScreenService: FullScreenService
  ) {
    this.loadSettings();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.forceMapRefresh();
    }, 1000);
  }

  ngOnInit(): void {
    console.log('Initializing map component, online status:', navigator.onLine);
    this.initializeVectorSources();
    //this.initializeEnvironmentalChangeLayer();
    this.initMap();
    this.setupResizeHandler();
    this.setupConnectivityMonitoring();
    setTimeout(() => {
      console.log('Map component fully initialized and ready');
    }, 500);
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.setTarget(null);
    }
    window.removeEventListener('resize', this.onWindowResize.bind(this));
    if (this.onlineListener) {
      window.removeEventListener('online', this.onlineListener);
    }
    if (this.offlineListener) {
      window.removeEventListener('offline', this.offlineListener);
    }
  }

  private initializeEnvironmentalChangeLayer(): void {
    this.environmentalChangeSource = new VectorSource({
      features: []
    });

    // Style function with any type to avoid complex type issues
    const styleFunction = (feature: any) => {
      const eventType = feature.get('eventType');
      let color = '#FF0000';

      switch (eventType) {
        case 'VEGETATION_LOSS':
          color = '#FF0000';
          break;
        case 'VEGETATION_GROWTH':
          color = '#00FF00';
          break;
        case 'WATER_INCREASE':
          color = '#0000FF';
          break;
        case 'WATER_DECREASE':
          color = '#FFA500';
          break;
        case 'URBANIZATION':
          color = '#808080';
          break;
      }

      const severity = feature.get('severity');
      let strokeWidth = 2;
      let fillOpacity = 0.3;

      switch (severity) {
        case 'SEVERE':
          strokeWidth = 4;
          fillOpacity = 0.6;
          break;
        case 'HIGH':
          strokeWidth = 3;
          fillOpacity = 0.5;
          break;
        case 'MODERATE':
          strokeWidth = 2;
          fillOpacity = 0.4;
          break;
        default:
          strokeWidth = 1;
          fillOpacity = 0.3;
      }

      return new Style({
        stroke: new Stroke({
          color: color,
          width: strokeWidth
        }),
        fill: new Fill({
          color: this.hexToRgba(color, fillOpacity)
        })
      });
    };

    this.environmentalChangeLayer = new VectorLayer({
      source: this.environmentalChangeSource,
      style: styleFunction
    });
    this.environmentalChangeLayer.set('name', 'environmental-changes');
  }

  private hexToRgba(hex: string, opacity: number): string {
    hex = hex.replace('#', '');
    const r = parseInt(hex.substring(0, 2), 16);
    const g = parseInt(hex.substring(2, 4), 16);
    const b = parseInt(hex.substring(4, 6), 16);
    return `rgba(${r}, ${g}, ${b}, ${opacity})`;
  }

  private setupResizeHandler(): void {
    window.addEventListener('resize', this.onWindowResize.bind(this));
  }

  private onWindowResize(): void {
    setTimeout(() => {
      if (this.map) {
        this.map.updateSize();
      }
    }, 250);
  }

  private setupConnectivityMonitoring(): void {
    this.onlineListener = () => {
      console.log('Internet connection restored');
      this.isUsingOfflineLayer = false;
      setTimeout(() => {
        this.rebuildLayers();
      }, 1000);
    };

    this.offlineListener = () => {
      console.log('Internet connection lost');
      this.isUsingOfflineLayer = true;
      setTimeout(() => {
        this.rebuildLayers();
      }, 100);
    };

    window.addEventListener('online', this.onlineListener);
    window.addEventListener('offline', this.offlineListener);
  }

  private rebuildLayers(): void {
    console.log('Rebuilding layers, online:', navigator.onLine);
    this.cacheBuster = Date.now();
    this.buildLayers();
    if (this.map) {
      this.map.setLayers(this.layers);
      this.map.updateSize();
    }
  }

  private loadSettings(): void {
    this.mapType = localStorage.getItem('mapType') || 'satellite';
    this.backTo = localStorage.getItem('backTo') || 'Kebele';
  }

  private initializeVectorSources(): void {
    this.nrlaisSource = new VectorSource({features: []});
    this.nrlaisStyle = new Style({
      stroke: new Stroke({color: 'blue', lineDash: [4], width: 3}),
      fill: new Fill({color: 'rgba(0, 0, 255, 0.1)'})
    });

    this.workflowSource = new VectorSource({features: []});
    this.workflowStyle = new Style({
      stroke: new Stroke({color: 'green', lineDash: [4], width: 3}),
      fill: new Fill({color: 'rgba(0, 255, 0, 0.1)'})
    });

    this.splitSource = new VectorSource({features: []});
    this.splitStyle = new Style({
      stroke: new Stroke({color: 'red', width: 3}),
      fill: new Fill({color: 'rgba(255, 0, 0, 0.1)'})
    });
  }

  private initMap(): void {
    this.setupProjection();
    this.createView();
    this.buildLayers();
    this.createMap();
    this.zoomToExtent();
    this.setupChangeInteraction();
  }

  private setupProjection(): void {
    proj4.defs('EPSG:20137', '+proj=utm +zone='+this.utmZone+' +ellps=clrk80 +units=m +no_defs');
    register(proj4);
  }

  private createView(): void {
    this.view = new OlView({
      center: [335320.696579432, 1294832.60257192],
      zoom: 10,
      resolution: 300,
      projection: 'EPSG:20137',
      maxResolution: 10000,
      minResolution: 0.1,
    });
  }

  private createMap(): void {
    const mapElement = this.elementRef.nativeElement.querySelector('#camis_map');
    console.log('Map element:', mapElement);

    if (!mapElement) {
      console.error('Map element #camis_map not found!');
      return;
    }

    this.map = new OlMap({
      target: mapElement,
      layers: this.layers,
      view: this.view,
    });

    console.log('Map created with layers:', this.layers.length);
  }

  private buildLayers(): void {
    console.log('Building layers, online status:', navigator.onLine);
    this.layers = [];

    this.addBaseLayer();
    this.addWmsLayers();
    this.addVectorLayers();
    this.addEnvironmentalChangeLayer();

    console.log('Total layers built:', this.layers.length);
  }

  private addEnvironmentalChangeLayer(): void {
    if (this.environmentalChangeLayer) {
      this.layers.push(this.environmentalChangeLayer);
    }
  }

  private addBaseLayer(): void {
    if (!this.mapType) {
      console.warn('No map type specified');
      return;
    }

    if (!navigator.onLine) {
      console.log('Offline detected, using offline base layer');
      this.addOfflineBaseLayer();
      this.isUsingOfflineLayer = true;
      return;
    }

    console.log('Online detected, using Google Maps base layer');
    this.addOnlineBaseLayer();
    this.isUsingOfflineLayer = false;
  }

  private addOnlineBaseLayer(): void {
    const googleUrls: { [key: string]: string } = {
      satellite: 'https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}',
      hybrid: 'https://mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
      roadmap: 'https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}'
    };

    const url = googleUrls[this.mapType] || googleUrls['roadmap'];
    console.log('Using Google Maps URL:', url);

    const baseLayer = new TileLayer({
      source: new XYZ({
        url,
        attributions: 'Google Maps',
        tileLoadFunction: (tile: any, src: string) => {
          const imageTile = tile;
          const img = imageTile.getImage();
          img.src = src;
          img.onerror = () => {
            console.warn('Google Maps tile failed to load, falling back to offline layer');
            this.isUsingOfflineLayer = true;
            this.fallbackToOfflineBaseLayer();
          };
        }
      })
    });

    baseLayer.set('name', 'google-base-layer');
    this.layers.push(baseLayer);
    console.log('Added online base layer');
  }

  private addOfflineBaseLayer(): void {
    console.log('Creating offline base layer');

    const offlineLayer = new TileLayer({
      source: new XYZ({
        attributions: 'Offline Base Map',
        tileLoadFunction: (tile: any, src: string) => {
          const imageTile = tile;
          const canvas = document.createElement('canvas');
          canvas.width = 256;
          canvas.height = 256;
          const context = canvas.getContext('2d');
          if (context) {
            context.fillStyle = '#f8f8f8';
            context.fillRect(0, 0, 256, 256);

            context.strokeStyle = '#e0e0e0';
            context.lineWidth = 1;

            for (let i = 0; i < 256; i += 16) {
              context.beginPath();
              context.moveTo(i, 0);
              context.lineTo(i, 256);
              context.stroke();

              context.beginPath();
              context.moveTo(0, i);
              context.lineTo(256, i);
              context.stroke();
            }

            context.fillStyle = '#666666';
            context.font = 'bold 16px Arial';
            context.textAlign = 'center';
            context.fillText('Offline Map', 128, 128);

            context.font = '12px Arial';
            context.fillText('No internet connection', 128, 150);
          }
          imageTile.getImage().src = canvas.toDataURL();
        }
      })
    });

    offlineLayer.set('name', 'offline-base-layer');
    this.layers.push(offlineLayer);
    console.log('Added offline base layer');
  }

  private fallbackToOfflineBaseLayer(): void {
    console.log('Falling back to offline base layer');

    this.layers = this.layers.filter(layer => {
      const layerName = layer.get('name');
      return !(layerName === 'google-base-layer' || layerName === 'offline-base-layer');
    });

    this.addOfflineBaseLayer();
    this.isUsingOfflineLayer = true;

    if (this.map) {
      this.map.setLayers(this.layers);
      this.map.updateSize();
      console.log('Map updated with offline layer');
    }
  }

  private addWmsLayers(): void {
    const wmsConfigs: WmsLayerConfig[] = [
      {name: 'Counter', layerName: 'nrlais:ne_10m_admin_0_countries', visible: true},
      {
        name: 'Region',
        layerName: 'nrlais:t_regions',
        visible: this.backTo === 'Region' || this.backTo === 'Woreda' || this.backTo === 'Kebele' || this.backTo === 'Land'
      },
      {
        name: 'Woreda',
        layerName: 'nrlais:t_woredas',
        visible: this.backTo === 'Woreda' || this.backTo === 'Kebele' || this.backTo === 'Land'
      },
      {name: 'Kebele', layerName: 'nrlais:t_kebeles', visible: this.backTo === 'Kebele' || this.backTo === 'Land'},
      {name: 'Land', layerName: 'camis:v_gs_land', visible: this.backTo === 'Land'}
    ];

    const visibleConfigs = wmsConfigs.filter(config => config.visible);
    console.log('Adding WMS layers:', visibleConfigs.map(c => c.name));

    visibleConfigs.forEach(config => this.createCacheFreeWmsLayer(config));
  }

  private createCacheFreeWmsLayer(config: WmsLayerConfig): void {
    try {
      console.log('Creating cache-free WMS layer:', config.layerName);

      const wmsSource = new OLTileWMS({
        url: '/geoserver/wms',
        params: {
          'LAYERS': config.layerName,
          'TILED': true,
          'VERSION': '1.1.1',
          'FORMAT': 'image/png',
          'TRANSPARENT': true,
          '_t': this.cacheBuster
        },
        serverType: 'geoserver',
        crossOrigin: 'anonymous',
        cacheSize: 0
      });

      const wmsLayer = new TileLayer({
        source: wmsSource,
        visible: true,
        opacity: 0.7
      });

      wmsLayer.set('name', `wms-${config.name.toLowerCase()}`);

      wmsSource.on('tileloaderror', (error: any) => {
        console.error(`Failed to load WMS layer: ${config.layerName}`, error);
      });

      this.layers.push(wmsLayer);
      console.log(`WMS layer ${config.name} added successfully`);
    } catch (error) {
      console.error(`Error creating layer ${config.name}:`, error);
    }
  }

  private addVectorLayers(): void {
    console.log('Adding vector layers');

    this.layers.push(
      this.createVectorLayer(this.nrlaisSource, this.nrlaisStyle, 'nrlais-vector'),
      this.createVectorLayer(this.workflowSource, this.workflowStyle, 'workflow-vector'),
      this.createVectorLayer(this.splitSource, this.splitStyle, 'split-vector')
    );

    console.log('Vector layers added');
  }

  private createVectorLayer(source: any, style: Style, name: string) {
    const layer = new VectorLayer({
      source,
      style: () => style
    });
    layer.set('name', name);
    return layer;
  }

  private zoomToExtent(): void {
    console.log('Zooming to extent');

    this.api.get('map/GetLandMapBound').subscribe({
      next: (response: any) => {
        console.log('Raw API response:', response);
        let bbox: BoundingBox;

        if (this.isBoundingBox(response)) {
          bbox = response;
        } else if (response.data && this.isBoundingBox(response.data)) {
          bbox = response.data;
        } else if (response.bbox || response.bounds) {
          const bboxData = response.bbox || response.bounds;
          bbox = {
            x1: bboxData.minX || bboxData.x1 || bboxData.left,
            y1: bboxData.minY || bboxData.y1 || bboxData.bottom,
            x2: bboxData.maxX || bboxData.x2 || bboxData.right,
            y2: bboxData.maxY || bboxData.y2 || bboxData.top
          };
        } else {
          console.warn('Unexpected API response format:', response);
          this.useDefaultView();
          return;
        }

        console.log('Processed bounding box:', bbox);

        if (!this.isValidBoundingBox(bbox)) {
          console.warn('Invalid bounding box received, using default view');
          this.useDefaultView();
          return;
        }

        const resolution = this.calculateResolution(bbox);

        if (isNaN(resolution) || !isFinite(resolution)) {
          console.warn('Invalid resolution calculated, using default view');
          this.useDefaultView();
          return;
        }

        this.view.animate({
          center: [(bbox.x2 + bbox.x1) / 2, (bbox.y2 + bbox.y1) / 2],
          resolution,
          duration: 1000
        });
        console.log('Zoom animation started with resolution:', resolution);
      },
      error: (error) => {
        console.error('Failed to get map bounds:', error);
        this.useDefaultView();
      }
    });
  }

  private isValidBoundingBox(bbox: BoundingBox): boolean {
    return (
      bbox &&
      typeof bbox.x1 === 'number' && !isNaN(bbox.x1) &&
      typeof bbox.x2 === 'number' && !isNaN(bbox.x2) &&
      typeof bbox.y1 === 'number' && !isNaN(bbox.y1) &&
      typeof bbox.y2 === 'number' && !isNaN(bbox.y2) &&
      bbox.x2 > bbox.x1 &&
      bbox.y2 > bbox.y1
    );
  }

  private isBoundingBox(obj: any): obj is BoundingBox {
    return (
      obj &&
      typeof obj.x1 === 'number' &&
      typeof obj.x2 === 'number' &&
      typeof obj.y1 === 'number' &&
      typeof obj.y2 === 'number'
    );
  }

  private useDefaultView(): void {
    console.log('Using default map view');
    this.view.animate({
      center: [335320.696579432, 1294832.60257192],
      zoom: 10,
      duration: 1000
    });
  }

  private calculateResolution(bbox: BoundingBox): number {
    const mapElement = this.elementRef.nativeElement.querySelector('#camis_map');

    if (!mapElement) {
      console.warn('Map element not found for resolution calculation');
      return 300;
    }

    const width = mapElement.clientWidth || 800;
    const height = mapElement.clientHeight || 600;

    console.log('Map dimensions:', {width, height});
    console.log('Bounding box dimensions:', {
      width: bbox.x2 - bbox.x1,
      height: bbox.y2 - bbox.y1
    });

    const bboxWidth = bbox.x2 - bbox.x1;
    const bboxHeight = bbox.y2 - bbox.y1;

    if (bboxWidth <= 0 || bboxHeight <= 0 || !isFinite(bboxWidth) || !isFinite(bboxHeight)) {
      console.warn('Invalid bounding box dimensions:', {bboxWidth, bboxHeight});
      return 300;
    }

    const horizontalRes = (bboxWidth * this.zoomMargin) / width;
    const verticalRes = (bboxHeight * this.zoomMargin) / height;

    const resolution = Math.max(horizontalRes, verticalRes) * 1.2;
    console.log('Calculated resolution:', resolution);
    return resolution;
  }

  private zoomToSetExtent(extent: number[]): void {
    if (!extent || extent.length !== 4 ||
      extent.some(val => isNaN(val) || !isFinite(val)) ||
      extent[0] >= extent[2] || extent[1] >= extent[3]) {
      console.warn('Invalid extent provided for zoom:', extent);
      return;
    }

    const mapElement = this.elementRef.nativeElement.querySelector('#camis_map');
    const width = mapElement?.clientWidth || 800;
    const height = mapElement?.clientHeight || 600;

    const extentWidth = extent[2] - extent[0];
    const extentHeight = extent[3] - extent[1];

    const horizontalRes = (extentWidth * this.zoomMargin) / width;
    const verticalRes = (extentHeight * this.zoomMargin) / height;

    const resolution = Math.max(horizontalRes, verticalRes);

    this.view.animate({
      center: [(extent[2] + extent[0]) / 2, (extent[3] + extent[1]) / 2],
      resolution,
      duration: 1000
    });
  }

  // Environmental Change Detection Methods


  private setupChangeInteraction(): void {
    this.map.on('click', (event: any) => {
      const features = this.map.getFeaturesAtPixel(event.pixel);

      if (features && features.length > 0) {
        // Find the first environmental change feature
        const changeFeature = features.find((f: any) => {
          const eventType = f.get('eventType');
          return eventType && typeof eventType === 'string';
        });

      }
    });
  }

  private showChangePopup(feature: any): void {
    const eventType = feature.get('eventType');
    const severity = feature.get('severity');
    const parcelUpid = feature.get('parcelUpid');
    const changeAmount = feature.get('changeAmount');
    const confidence = feature.get('confidence');

    console.log('Change Feature Clicked:', {
      eventType,
      severity,
      parcelUpid,
      changeAmount,
      confidence
    });

    const message = `
      Environmental Change Detected:
      Type: ${eventType}
      Severity: ${severity}
      Parcel: ${parcelUpid}
      Change: ${changeAmount?.toFixed(4)}
      Confidence: ${((confidence || 0) * 100).toFixed(1)}%
    `;

    alert(message);
  }

  public clearEnvironmentalChanges(): void {
    this.environmentalChangeSource.clear();
    console.log('Cleared environmental changes from map');
  }

  public toggleChangeLayerVisibility(): void {
    if (this.environmentalChangeLayer) {
      const currentVisibility = this.environmentalChangeLayer.getVisible();
      this.environmentalChangeLayer.setVisible(!currentVisibility);
      console.log('Environmental change layer visibility:', !currentVisibility);
    }
  }

  // Public methods
  public googleMapSetting(): void {
    console.log('Updating map settings:', {mapType: this.mapType, backTo: this.backTo});

    localStorage.setItem('mapType', this.mapType);
    localStorage.setItem('backTo', this.backTo);

    this.rebuildLayers();
  }

  public setNrlaisParcel(upin: string): void {
    if (!upin) return;

    console.log('Setting NRLais parcel for UPIN:', upin);

    const wmsUrl = `map/WfsGet?service=WFS&version=1.0.0&request=GetFeature&typeName=nrlais:nrlais_inventory.t_parcels&maxFeatures=50&outputFormat=application/json&CQL_FILTER=upid='${upin}'`;

    this.api.get(wmsUrl).subscribe({
      next: (data: any) => {
        if (data.error) {
          console.error(`Error reading geojson for upin:${upin}`, data.error);
          return;
        }

        const features = new GeoJSON().readFeatures(data.response);
        console.log(`Loaded ${features.length} features for parcel ${upin}`);

        this.nrlaisSource.clear();
        this.nrlaisSource.addFeatures(features);

        const extent = this.nrlaisSource.getExtent();
        if (extent[0] !== Infinity && extent[1] !== Infinity) {
          this.zoomToSetExtent(extent);
        } else {
          console.warn('Invalid extent for parcel, skipping zoom');
        }
      },
      error: (error) => {
        console.error(`Failed to load parcel for upin:${upin}`, error);
      }
    });
  }

  public setWorkFlowGeomByWKT(wkt: string): void {
    console.log('Setting workflow geometry from WKT');

    this.workflowSource.clear();
    const features = new WKT().readFeatures(wkt);
    this.workflowSource.addFeatures(features);

    const extent = this.workflowSource.getExtent();
    if (extent[0] !== Infinity && extent[1] !== Infinity) {
      this.zoomToSetExtent(extent);
    }
  }

  public setSplitGeomsByWKT(wkts: string[]): void {
    console.log('Setting split geometries from WKT array, count:', wkts.length);

    this.splitSource.clear();
    wkts.forEach(wkt => {
      const features = new WKT().readFeatures(wkt);
      this.splitSource.addFeatures(features);
    });

    const extent = this.splitSource.getExtent();
    if (extent[0] !== Infinity && extent[1] !== Infinity) {
      this.zoomToSetExtent(extent);
    }
  }

  public toggleFullScreen(): void {
    this.isFullScreen = !this.isFullScreen;

    if (this.isFullScreen) {
      this.enterFullScreen();
    } else {
      this.exitFullScreen();
    }
  }

  private enterFullScreen(): void {
    const mapContainer = this.elementRef.nativeElement.querySelector('.map-container');
    if (mapContainer) {
      mapContainer.classList.add('full-screen');
    }

    setTimeout(() => {
      if (this.map) {
        this.map.updateSize();
      }
    }, 300);
  }

  private exitFullScreen(): void {
    const mapContainer = this.elementRef.nativeElement.querySelector('.map-container');
    if (mapContainer) {
      mapContainer.classList.remove('full-screen');
    }

    setTimeout(() => {
      if (this.map) {
        this.map.updateSize();
      }
    }, 300);
  }


  public forceMapRefresh(): void {
    console.log('Forcing complete map refresh');
    this.rebuildLayers();
  }
}
