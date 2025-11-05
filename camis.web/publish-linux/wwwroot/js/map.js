/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


function lmap() {

    var uuid;


    proj4.defs("EPSG:20137", "+proj=utm +zone=37 +ellps=clrk80 +towgs84=-166,-15,204,0,0,0,0 +units=m +no_defs");
    var extentStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: 'red',
            width: 1.5
        }),
        fill: new ol.style.Fill({
            color: 'rgba(255, 255, 255, 0.5)'
        })
    });

    var extentSource = new ol.source.Vector({});
    var extentLayer = new ol.layer.Vector({
        projection: 'EPSG:20137',
        source: extentSource
    });


    var format = 'image/png';
    var boundStatic = [-170185.4375, 355913.125,
       1502477.75, 1653799.875];

    var bounds = [803702.8197659142, 1058204.3324224968,
        818919.4198037051, 1067076.8324445318];

    var mousePositionControl = new ol.control.MousePosition({
        className: 'custom-mouse-position',
        target: document.getElementById('location'),
        coordinateFormat: ol.coordinate.createStringXY(5),
        undefinedHTML: '&nbsp;'
    });
    var untiled = new ol.layer.Image({
        source: new ol.source.ImageWMS({
            ratio: 1,
            url: 'http://map-server:8080/geoserver/ethio_map/wms',
            params: {
                'FORMAT': format,
                'VERSION': '1.1.1',
                STYLES: '',
                LAYERS: 'ethio_map:ethiopia'
            }
        })
    });

    var tiledE = new ol.layer.Tile({
        visible: true,
        source: new ol.source.TileWMS({
            url: 'http://map-server:8080/geoserver/ethio_map/wms',
            params: {
                'FORMAT': format,
                'VERSION': '1.1.1',
                tiled: true,
                STYLES: '',
                LAYERS: 'ethio_map:ethiopia',
                tilesOrigin: -170185.4375 + "," + 355913.125
            }
        })
    });

    var tiled = new ol.layer.Tile({
        visible: true,
        source: new ol.source.TileWMS({
            url: 'http://map-server:8080/geoserver/ethio_map/wms',
            params: {
                'FORMAT': format,
                'VERSION': '1.1.1',
                tiled: true,
                STYLES: '',
                LAYERS: 'ethio_map:ethiopiaboundary',
                tilesOrigin: -170168.390625 + "," + 370040.375
            }
        })
    });

    var untiledL = new ol.layer.Image({
        source: new ol.source.ImageWMS({
            ratio: 1,
            url: 'http://map-server:8080/geoserver/ethio_map/wms',
            params: {
                'FORMAT': format,
                'VERSION': '1.1.1',
                STYLES: '',
                LAYERS: 'ethio_map:intaps_land'
            }
        })
    });

    var arerial = new ol.layer.Image({
        source: new ol.source.ImageWMS({
            ratio: 1,
            url: 'http://map-server:8080/geoserver/ethio_map/wms',
            params: {
                'FORMAT': format,
                'VERSION': '1.1.1',
                STYLES: '',
                LAYERS: 'ethio_map:aerial'
            }
        })
    });


    var projection = new ol.proj.Projection({
        code: 'EPSG:20137',
        units: 'm',
        axisOrientation: 'neu',
        global: false
    });

    var vectorSource = new ol.source.Vector({
        format: new ol.format.GeoJSON(),
        url: function (extent) {
            return 'http://map-server:8080/geoserver/ethio_map/wfs?service=WFS&' +
                'version=1.1.0&request=GetFeature&typename=ethio_map:developed_land&' +
                'outputFormat=application/json&srsname=EPSG:20137&' +
                'bbox=' + extent.join(',') + ',EPSG:4326';
        },
        strategy: ol.loadingstrategy.bbox
    });

    var vectorSource1 = new ol.source.Vector({
        format: new ol.format.GeoJSON(),
        url: function (extent) {
            return 'http://map-server:8080/geoserver/ethio_map/wfs?service=WFS&' +
                'version=1.1.0&request=GetFeature&typename=ethio_map:in_land_bank&' +
                'outputFormat=application/json&srsname=EPSG:20137&' +
                'bbox=' + extent.join(',') + ',EPSG:4326';
        },
        strategy: ol.loadingstrategy.bbox
    });

    var vectorSource2 = new ol.source.Vector({
        format: new ol.format.GeoJSON(),
        url: function (extent) {
            return 'http://map-server:8080/geoserver/ethio_map/wfs?service=WFS&' +
                'version=1.1.0&request=GetFeature&typename=ethio_map:transferd_land&' +
                'outputFormat=application/json&srsname=EPSG:20137&' +
                'bbox=' + extent.join(',') + ',EPSG:4326';
        },
        strategy: ol.loadingstrategy.bbox
    });

    var purgatorySource = new ol.source.Vector({

        format: new ol.format.GeoJSON(),
        url: function (extent) {
            return 'http://map-server:8080/geoserver/ethio_map/wfs?service=WFS&' +
                'version=1.1.0&request=GetFeature&typename=ethio_map:land_purgatory&' +
                'outputFormat=application/json&srsname=EPSG:20137&' +
                'bbox=' + extent.join(',') + ',EPSG:4326';
        },
        strategy: ol.loadingstrategy.bbox
    });

    var splitSource = new ol.source.Vector({

        format: new ol.format.GeoJSON(),
        url: function (extent) {
            return 'http://map-server:8080/geoserver/ethio_map/wfs?service=WFS&' +
                'version=1.1.0&request=GetFeature&typename=ethio_map:land_split&' +
                'outputFormat=application/json&srsname=EPSG:20137&' +
                'bbox=' + extent.join(',') + ',EPSG:4326';
        },
        strategy: ol.loadingstrategy.bbox
    });







    var vec = new ol.layer.Vector({
        source: vectorSource,
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'fuchsia',
                width: 2
            })
        })
    });

    var vec1 = new ol.layer.Vector({
        source: vectorSource1,
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'greenyellow',
                width: 2
            })
        })
    });

    var vec2 = new ol.layer.Vector({
        source: vectorSource2,
        style: new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: 'firebrick',
                width: 2
            })
        })
    });

    var vecTemp = new ol.layer.Vector({
        source: purgatorySource
    });

    var vecSplit = new ol.layer.Vector({
        source: splitSource
    });

    function ftOverlay(m) {

        return new ol.layer.Vector({
            source: new ol.source.Vector(),
            map: m,
            style: function (feature) {
                // highlightStyle.getText().setText(feature.get('name'));
                return highlightStyle;
            }
        });
    }


    function mainMap() {
        var container = document.getElementById('popup');
        var content = document.getElementById('popup-content');
        var closer = document.getElementById('popup-closer');

        var overlay = new ol.Overlay({
            element: container,
            autoPan: true,
            autoPanAnimation: {
                duration: 250
            }
        });


        closer.onclick = function () {
            overlay.setPosition(undefined);
            closer.blur();
            return false;
        };

       
        var map = loadMap('map_preview', [untiled, arerial, vec, vec1, vec2]);
        map.getView().fit(bounds, map.getSize());
            map.addOverlay(overlay);


        var map2 = new ol.Map({
            controls: ol.control.defaults({
                attribution: false
            }).extend([mousePositionControl]),
            target: 'map_preview2',
            layers: [
                untiled,
                extentLayer
            ],
            view: new ol.View({
                projection: projection
            })
        });
        map2.getView().fit(boundStatic, map.getSize());

       

        var highlightOverlay = new ol.layer.Vector({
            // style: (customize your highlight style here),
            style: new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 255, 255, 1.0)',
                    width: 2
                })
            }),
            source: new ol.source.Vector(),
            map: map
        });

        map.on('moveend', function (evt) {
           // var res = evt.target.get('resolution');
           // mapLink(map, evt);
            addExtent(map);

        });

        map.getView().on('change:resolution', function (evt) {
          var res = evt.target.get('resolution');
            mapLink(map, res);
           
        });

      var mClick =   map.on('singleclick', function (evt) {

            var cfeat = displayFeatureInfo(evt.pixel);
          console.log(cfeat);
          var sp = cfeat.id_.split(".");
          uuid = sp[1];
          var coordinate = evt.coordinate;
            var hdms = ol.coordinate.toStringHDMS(ol.proj.transform(
                coordinate, 'EPSG:20137', 'EPSG:4326'));
            content.innerHTML = '<p>Land Type: ' + cfeat.values_.land_type + '</p>\n\
                                          <p>Description: '+ cfeat.values_.description + '</p>\n\
                                          <p>Area(hec): '+ cfeat.values_.area + '</p><code>' + hdms +
                '</code>\n'
                + '<a href="\#\landBank/' + sp[1]+'">detail</a>';
            overlay.setPosition(coordinate);

            content.querySelectorAll("button#inner_id")[0].addEventListener("click", function () {

                //alert(uuid);

                
            });
        });

        

        var displayFeatureInfo = function (pixel) {
            var feature = map.forEachFeatureAtPixel(pixel, function (feature) {
                return feature;
            });
            return feature;

        };



        function zoomFeature(uuid) {
            highLight(uuid, true, feat, map, ftOverlay(map), 9);
        }


        var landBankData;
        var transferedData;
        var unidentifiedData;
        var ID;
        $('#in_land_bank').on('click', function (event) {
            if ($('#in_land_bank').is(":checked")) {
                map.addLayer(vec1);
                $("#nodelist1").html(landBankData);
            }

            else {
                if (landBankData === undefined) {
                    landBankData = $("#nodelist1").html();
                }


                //console.log(tdata);
                var layers = map.getLayers().array_;
                for (var i = 0; i < layers.length; i++) {
                    if (layers[i] === vec1)
                        map.removeLayer(layers[i]);
                }
                $("#nodelist1").html('');
                //


            }
        });

        $('#transfered').on('click', function (event) {

            if ($('#transfered').is(":checked")) {

                map.addLayer(vec);
                $("#nodelist").html(transferedData);
                row1 = [];
                $('#tclick tr').each(function () {
                    row1.push($(this));
                });
                $('#nextt').on('click', function (evetn) {
                    next(row1);
                });

                $('#prevt').on('click', function (event) {
                    previous(row1);
                });
                $('#counting_transfered').on("click", 'button', function () {
                    var currentId = $(this).attr('id');
                    traverse(row1, currentId);

                });


            }
            else {
                if (transferedData === undefined) {
                    transferedData = $("#nodelist").html();
                }
                var layers = map.getLayers().array_;
                for (var i = 0; i < layers.length; i++) {
                    if (layers[i] === vec)
                        map.removeLayer(layers[i]);
                }

                $("#nodelist").html("");

            }


        });

        $('#unidentified').on('change', function (event) {
            if ($('#unidentified').is(":checked")) {
                map.addLayer(vec2);
                $("#nodelist2").html(unidentifiedData);

            }
            else {
                if (unidentifiedData === undefined) {
                    unidentifiedData = $("#nodelist2").html();
                }

                var layers = map.getLayers().array_;
                for (var i = 0; i < layers.length; i++) {
                    if (layers[i] === vec2)
                        map.removeLayer(layers[i]);
                }
                $("#nodelist2").html("");
            }

        });

        $('#aerial').on('change', function (event) {
       
            if ($('#aerial').is(":checked")) {
               
                map.addLayer(arerial);
            }
            else {
                map.removeLayer(arerial);
            }

        });



        return { highLight: zoomFeature, mapClick: mClick };
    }





    var highlightStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: '#f00',
            width: 1
        }),
        fill: new ol.style.Fill({
            color: 'rgb(0,0,255)'
        })
    });

    var parser = new ol.format.WMSGetFeatureInfo();


    function loadMap(id,arr) {
        return new ol.Map({
            controls: ol.control.defaults({
                attribution: false
            }).extend([mousePositionControl]),
           // overlays: [overlay],
            target: id,
            layers:arr,
            view: new ol.View({
                projection: projection
            })
        });
        
    }


    function mapLink(m, res) {

        var units = m.getView().getProjection().getUnits();
        var dpi = 25.4 / 0.28;
        var mpu = ol.proj.METERS_PER_UNIT[units];
        var scale = res * mpu * 39.37 * dpi;
            if (scale >= 9500 && scale <= 950000) {
                scale = Math.round(scale / 1000) + "K";
            } else if (scale >= 950000) {
                scale = Math.round(scale / 1000000) + "M";
            } else {
                scale = Math.round(scale);
        }
        
            document.getElementById('scale').innerHTML = "Scale = 1 : " + scale;
            
       
    }


    function addExtent(m) {
        // Remove the previous polygon
        extentSource.clear();
        // Get the extent of the map and store it in an array of coordinates 
        var extentMap = m.getView().calculateExtent(m.getSize());
        var bottomLeft = ol.proj.transform(ol.extent.getBottomLeft(extentMap),
            'EPSG:20137', 'EPSG:4326');
        var topRight = ol.proj.transform(ol.extent.getTopRight(extentMap),
            'EPSG:20137', 'EPSG:4326');
        var bottomRight = ol.proj.transform(ol.extent.getBottomRight(extentMap),
            'EPSG:20137', 'EPSG:4326');
        var topLeft = ol.proj.transform(ol.extent.getTopLeft(extentMap),
            'EPSG:20137', 'EPSG:4326');
        var ring = [
            [bottomLeft[0], bottomLeft[1]],
            [topLeft[0], topLeft[1]],
            [topRight[0], topRight[1]],
            [bottomRight[0], bottomRight[1]]
        ];
        // Create a polygon based on the array of coordinates
        var polygon = new ol.geom.Polygon([ring]);
        polygon.transform('EPSG:4326', 'EPSG:20137');
        // Add the polygon to the layer and style it
        var feature = new ol.Feature(polygon);
        extentSource.addFeature(feature);
        feature.setStyle(extentStyle);

    }
    var feat = [];
    var tempFeat = [];
    var splitFeat = [];
    var extent;
    var highlight;
    vectorSource.on('change', function (evt) {
        pushFeatures(vectorSource, evt,feat);
    });

    vectorSource1.on('change', function (evt) {
        pushFeatures(vectorSource1, evt,feat);
    });
    vectorSource2.on('change', function (evt) {
        pushFeatures(vectorSource2, evt,feat);
    });

  
    function pushFeatures(vs, evt, ft) {
        var source = evt.target;
            if (source.getState() === 'ready') {
                for (var i = 0; i < vs.getFeatures().length; i++) {
                    ft.push(vs.getFeatures()[i]);
                }
            }
        
    }






    function highLight(uuid, zoom,ft,m,fo,z) {
        console.log("called from angular winterspell");
        console.log(uuid);

        setTimeout(function () {

            var feature;
                for (var i = 0; i < ft.length; i++) {
                    var splitted = ft[i].id_.split(".");
                    if (splitted[1] == uuid) {
                        console.log(splitted[1]);
                        feature = ft[i];
                        console.log(feature);
                        if (feature !== highlight) {
                            if (highlight) {
                                //fo.getSource().removeFeature(highlight);
                            }
                            if (feature) {
                                console.log(fo);
                                fo.getSource().addFeature(feature);
                            }
                            highlight = feature;
                        }

                        extent = feature.getGeometry().getExtent();
                        console.log("this is Extent" + extent);
                        if (zoom === true) {
                            var center = ol.extent.getCenter(extent);
                            m.setView(new ol.View({
                                projection: 'EPSG:20137',
                                center: [center[0], center[1]],
                                zoom: z
                            }));
                            addExtent(m);
                        }
                        else {


                            //  var poly = new ol.geom.Polygon([extent]);
                            // poly.transform('EPSG:4326', 'EPSG:20137');
                            //var singleFeature = new ol.Feature(poly);

                           // prepareSingleLayer(feature, 'map');
                            return extent;
                        }



                    }
                }
            

        }, 400);

    }


    return {
        singleFeature: loadSinglFeature, tempLand: loadTempLand, mainLandBank: mainMap, splitLand: loadSplitLand, id: uuid
    };


    $('a[href ="#view"]').hover(function () {
        $(this)
            .toggleClass("view");
    });



    function loadSinglFeature(uuid) {
        console.log(uuid);
        //   highLight(uuid, false, feat,  , 12);
        var map = loadMap('map', [untiled, vec, vec1, vec2]);
        map.getView().fit(bounds, map.getSize());
        highLight(uuid, true, feat, map, ftOverlay(map), 16);
        
    }



    function loadTempLand(id) {
        
        var map4 = loadMap('temp_map', [untiled, arerial, vec, vec1, vec2, vecTemp]);
        map4.getView().fit(bounds, map4.getSize());

            purgatorySource.on('change', function (evt) {

                var source = evt.target;
                if (source.getState() === 'ready') {
                    for (var i = 0; i < purgatorySource.getFeatures().length; i++) {
                        tempFeat.push(purgatorySource.getFeatures()[i]);
                    }
                    console.log(tempFeat);
                }
        });
        highLight(id, true, tempFeat, map4, ftOverlay(map4),16);
     

    }

    function loadSplitLand(id) {
        var map5 = loadMap('split_map', [untiled, arerial, vecSplit]);
        map5.getView().fit(bounds, map5.getSize());

        splitSource.on('change', function (evt) {

            var source = evt.target;
            if (source.getState() === 'ready') {
                for (var i = 0; i < splitSource.getFeatures().length; i++) {
                    splitFeat.push(splitSource.getFeatures()[i]);
                }
            }
        });
        highLight(id, true, splitFeat, map5, ftOverlay(map5), 16);


    }


    $('a[href ="#details"]').hover(function () {
        $(this)
            .toggleClass("details");
    });


   

}