# -*- coding: utf-8 -*-
from qgis.core import *
from qgis.PyQt.QtCore import QSettings, QTranslator, QCoreApplication, Qt
from osgeo import ogr
from .CMSSBrowser import CMSSBrowser

# Helper function for JavaScript execution (WebEngine vs WebKit)
def executeJavaScript(page, script):
    """Execute JavaScript in a web page, handling both WebEngine and WebKit."""
    if hasattr(page, 'runJavaScript'):
        # QtWebEngine
        page.runJavaScript(script)
    else:
        # QtWebKit
        page.mainFrame().evaluateJavaScript(script)


class CMSSURLProcessor:

    def __init__(self, cmss, page):
        self.cmss = cmss
        self.page = page
        self.selectionMode = False
        self.maxID = 0
        self.layer = None
        self.fid_dict = {}
        self.id_dict = {}
        self.b = None

    def unload(self):
        if self.b:
            self.cmss.iface.removeDockWidget(self.b)
        self.unloadLayer()

    def unloadLayer(self):
        if self.layer:
            print('unloading layer')
            self.layer.commitChanges()
            # QGIS 3.x uses QgsProject instead of QgsMapLayerRegistry
            QgsProject.instance().removeMapLayer(self.layer)
            self.layer = None
            self.fid_dict = {}

    def processURL(self, path, query):
        print("request: " + path)
        if path == '/cmss/qcmd/showMessage':
            self.cmss.showInformationMessage('CAMIS-v2 QGIS', query['msg'])
            return True
        if path == '/cmss/qcmd/loadSplit':
            try:
                self.loadParcelGeometry(query['task_uid'])
            except Exception as ex:
                self.cmss.showCriticalMessage('Failed to load parcel geometries for the task', str(ex))
            return True
        if path == '/cmss/qcmd/unloadTask':
            self.unloadLayer()
            return True

        if path == '/cmss/qcmd/set_selection_mod':
            self.cmss.iface.actionSelect().trigger()
            self.selectionMode = True
            return True

        if path == '/cmss/qcmd/open_url':
            if not self.b:
                self.b = CMSSBrowser(self.cmss, query['url'])
                self.cmss.iface.addDockWidget(Qt.NoDockWidgetArea, self.b)
            else:
                self.b.setUrl(query['url'])
            self.b.show()
            return True

        if path == '/cmss/qcmd/label_geom':
            print(str(query))
            i = 0
            while ('id' + str(i)) in query.keys():
                id_val = query['id' + str(i)]
                label = query['lb' + str(i)]
                feature = next(self.layer.getFeatures(QgsFeatureRequest(QgsExpression("id=" + id_val))), None)
                if feature:
                    self.layer.changeAttributeValue(feature.id(), 1, label)
                else:
                    print('Feature for label not found id:' + id_val)
                i = i + 1

            self.layer.triggerRepaint()
            return True

        if path == '/cmss/qcmd/get_data/':
            if not self.layer:
                return True

            if 'id' in query:
                qid = query['id']
                print('get_data single: ' + qid)
                fid = self.fid_dict.get(qid)
                if fid:
                    print('get_data single fid: ' + str(fid))
                    f = next(self.layer.getFeatures(QgsFeatureRequest(int(qid))))
                    oneitem = "{id:" + str(f.attribute('id')) + ",area:" + str(f.geometry().area()) + ",wkt:'" + f.geometry().asWkt() + "'}"
                    # Use helper function for JS execution
                    executeJavaScript(self.page, 'setGeomData(' + oneitem + ')')
                else:
                    print('fid not found')
                return True
            print('get_data array')
            data = ''
            for f in self.layer.getFeatures():
                oneitem = "{id:" + str(f.attribute('id')) + ",area:" + str(f.geometry().area()) + ",wkt:'" + f.geometry().asWkt() + "'}"
                if data == '':
                    data = oneitem
                else:
                    data = data + ',' + oneitem
            executeJavaScript(self.page, 'setGeomData([' + data + '])')
            return True

        return False

    def loadParcelGeometry(self, taskid):
        res = self.cmss.invokeServer('/api/cmss/GetTaskGeom?taskid=' + taskid, None)
        if res['error']:
            raise Exception('Error getting task geometry from server\n' + res['error'])
        geom_list = res['res']
        if not isinstance(geom_list, list):
             geom_list = [geom_list]
        
        uri = "MultiPolygon?crs=epsg:20137&field=id:integer&field=label:string"
        self.unloadLayer()

        self.layer = QgsVectorLayer(uri, "CAMIS-v2_T_Geometries", "memory")
        styleFile = self.cmss.plugin_dir + '/task_geom.qml'
        self.layer.loadNamedStyle(styleFile)

        # Use QgsProject instead of QgsMapLayerRegistry
        QgsProject.instance().addMapLayer(self.layer)
        self.layer.startEditing()

        self.maxID = 0
        extents = []
        for geom_data in geom_list:
            feature = QgsFeature()
            raw_wkt = geom_data['geom']
    
            # Strip SRID prefix (e.g., "SRID=20137;MultiPolygon(...)" -> "MultiPolygon(...)")
            if ';' in raw_wkt and raw_wkt.upper().startswith('SRID='):
                wkt = raw_wkt.split(';', 1)[1]
            else:
                wkt = raw_wkt
            
            gm = QgsGeometry.fromWkt(wkt)
            if gm.isNull():
                print(f"Warning: Failed to parse WKT for id {geom_data['id']}")
                continue
                
            feature.setGeometry(gm)
            id_val = geom_data['id']
            if id_val > self.maxID:
                self.maxID = id_val
            
            label = geom_data.get('label', f"parcel-{id_val}")
            feature.setAttributes([id_val, label])
            self.layer.addFeature(feature)
            self.fid_dict[str(id_val)] = feature.id()
            extents.append(feature.geometry().boundingBox())
            executeJavaScript(self.page, 'renderSplitParcelsList()')
        self.layer.commitChanges()
        self.layer.startEditing()

        self.layer.featureAdded.connect(self.featureAdded)
        self.layer.featureDeleted.connect(self.featureDeleted)
        self.layer.geometryChanged.connect(self.geomChanged)
        self.layer.selectionChanged.connect(self.selectionChanged)

        if extents:
            combined_extent = QgsRectangle()
            for ext in extents:
                combined_extent.combineExtentWith(ext)
            self.cmss.iface.mapCanvas().setExtent(combined_extent)
        else:
            print('Empty extent, trying kebele')
            res = self.cmss.invokeServer('/api/task_kebele?task_uid=' + taskid, None)
            if res['error'] is None:
                features = self.cmss.kebele_layer.getFeatures(QgsFeatureRequest(QgsExpression("nrlais_kebeleid='" + res['res'] + "'")))
                f = next(features, None)
                if f:
                    self.cmss.iface.mapCanvas().setExtent(f.geometry().boundingBox())
       
        
    # The beforeCommit method has been removed entirely

    def onLayerChanged(self, type_val, id_val):
        executeJavaScript(self.page, 'layerChanged(' + str(type_val) + ',' + str(id_val) + ',' + str(self.layer.featureCount()) + ')')

    def featureAdded(self, fid):
        self.maxID = self.maxID + 1
        self.layer.changeAttributeValue(fid, 0, self.maxID)
        self.layer.changeAttributeValue(fid, 1, f"parcel-{self.maxID}")
        self.fid_dict[str(self.maxID)] = fid
        self.onLayerChanged(1, self.maxID)

    def featureDeleted(self, fid):
        self.onLayerChanged(2, -1)

    def geomChanged(self, fid, geom):
        id_val = next(self.layer.getFeatures(QgsFeatureRequest(fid))).attribute('id')
        print('modified: ' + str(fid))
        self.onLayerChanged(3, id_val)

    def selectionChanged(self, selected, deselected, clearAndSelect):
        if not self.selectionMode:
            return
        if len(selected) == 1:
            f = next(self.layer.getFeatures(QgsFeatureRequest(selected[0])))
            executeJavaScript(self.page, 'featureSelected(' + str(f.attribute('id')) + ')')