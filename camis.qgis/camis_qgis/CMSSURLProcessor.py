# -*- coding: utf-8 -*-
from qgis.core import *
from PyQt4.QtCore import QSettings, QTranslator, qVersion, QCoreApplication, Qt
from osgeo import ogr
from CMSSBrowser import CMSSBrowser
class CMSSURLProcessor:

    def __init__(self,cmss,page):
        self.cmss=cmss
        self.page=page
        self.selectionMode=False
        self.maxID=0
        self.layer=None
        self.fid_dict={}
        self.id_dict={};
        self.b=None

    def unload(self):
        if  self.b:
            self.cmss.iface.removeDockWidget(self.b)
        self.unloadLayer()

    def unloadLayer(self):
        if self.layer:
            print('unloading layer')
            self.layer.commitChanges()
            QgsMapLayerRegistry.instance().removeMapLayer(self.layer)
            self.layer=None
            self.fid_dict={}

    def processURL(self,path,query):
        print("request: "+ path)
        if path=='/cmss/qcmd/showMessage':
            self.cmss.showInformationMessage('CMSS 2',query['msg'])
            return True
        if path=='/cmss/qcmd/loadSplit':
            try:
                self.loadParcelGeometry(query['task_uid'])
            except Exception as ex:
                self.cmss.showCriticalMessage('Failed to load parcel geometries for the task',str(ex))
            return True
        if path=='/cmss/qcmd/unloadTask':
            self.unloadLayer()
            return True

        if path=='/cmss/qcmd/set_selection_mod':
            self.cmss.iface.actionSelect().trigger()
            self.selectionMode=True
            return True

        if path=='/cmss/qcmd/open_url':
            if not(self.b):
                self.b=CMSSBrowser(self.cmss,query['url'])
                self.cmss.iface.addDockWidget(Qt.NoDockWidgetArea, self.b)
            else:
                self.b.setUrl(query['url'])
            self.b.show()
            return True

        if path=='/cmss/qcmd/label_geom':
            print(str(query))
            i=0
            while ('id'+str(i)) in query.keys():
                id=query['id'+str(i)]
                label=query['lb'+str(i)]
                #print('Label '+id+' - '+label)
                feature=next(self.layer.getFeatures(QgsFeatureRequest(QgsExpression("id="+id))),None)
                if feature:
                    #fid=self.fid_dict[id]
                    #print('Label fide'+str(fid))
                    #self.layer.changeAttributeValue(fid,0,id)
                    self.layer.changeAttributeValue(feature.id(),1,label)
                    #feature.setAttribute(1,label)
                else:
                    print('Feature for label not found id:'+id)
                i=i+1

            self.layer.triggerRepaint()

            return True

        if path=='/cmss/qcmd/get_data/':
            if not (self.layer):
                return True

            if 'id' in query:
                qid=query['id']
                print('get_data single: '+qid)
                fid=self.fid_dict[qid];
                if fid:
                    print('get_data single fid: '+str(fid))
                    f=next(self.layer.getFeatures(QgsFeatureRequest(int(qid))))
                    oneitem="{id:"+str(f.attribute('id'))+",area:"+str(f.geometry().area())+",wkt:'"+f.geometry().exportToWkt()+"'}"
                    self.page.mainFrame().evaluateJavaScript('setGeomData('+oneitem+')')
                else:
                    print('fid not found')
                return True
            print('get_data array')
            data=''
            for f in self.layer.getFeatures():
                oneitem="{id:"+str(f.attribute('id'))+",area:"+str(f.geometry().area())+",wkt:'"+f.geometry().exportToWkt()+"'}"
                if data=='':
                    data=oneitem
                else:
                    data=data+','+oneitem
            self.page.mainFrame().evaluateJavaScript('setGeomData(['+data+'])')
            return True

        return False

    def loadParcelGeometry(self,taskid):
        res=self.cmss.invokeServer('/api/cmss/GetTaskGeom?taskid='+taskid,None)
        if res['error']:
            raise 'Erorr getting task geometry from server\n'+res['error']
        arr=res['res']
        uri = "MultiPolygon?crs=epsg:20137&field=id:integer&field=label:string";
        self.unloadLayer()

        self.layer = QgsVectorLayer(uri, "Task Geometries",  "memory");
        styleFile=self.cmss.plugin_dir+'/task_geom.qml'
        self.layer.loadNamedStyle(styleFile)

        QgsMapLayerRegistry.instance().addMapLayer(self.layer)
        self.layer.startEditing()

        feature = QgsFeature()
        wkt=arr['geom']
        gm=QgsGeometry.fromWkt(wkt);
        feature.setGeometry(gm)
        id=arr['id']
        if id>self.maxID:
            self.maxID=id
        feature.setAttributes([id,arr['label']])
        self.layer.addFeature(feature, True)

        self.fid_dict[str(id)]=feature.id()
        self.layer.commitChanges()
        self.layer.startEditing()
        ext=self.layer.extent()
        if self.layer.featureCount()>0:
            print('Zooming to:' + str(ext))
            self.cmss.iface.mapCanvas().setExtent(ext)
        else:
            print('Empty extent,trying kebele')
            res = self.cmss.invokeServer('/api/task_kebele?task_uid=' + taskid, None)
            print('kebele id:' + str(res))
            if res['error']==None:
                print('kebele id:'+str(res))
                features=self.cmss.kebele_layer.getFeatures(QgsFeatureRequest(QgsExpression("nrlais_kebeleid='"+res['res']+"'")))
                f=next(features,None)
                if f:
                    print('kebele found')
                    bbox = f.geometry().boundingBox()
                    print(str(bbox))
                    self.cmss.iface.mapCanvas().setExtent(bbox)
                else:
                    print('kebele not found')

        self.layer.featureAdded .connect(self.featureAdded)
        self.layer.featureDeleted.connect(self.featureDeletd)
        self.layer.geometryChanged.connect(self.geomChanged)
        self.layer.selectionChanged.connect(self.selectionChanged)
        self.layer.beforeCommitChanges.connect(self.beforeComit)
    def beforeComit(self):
        for f in self.layer.getFeatures():
            f.geometry().exportToWkt()

    def onLayerChanged(self,type,id):
        self.page.mainFrame().evaluateJavaScript('layerChanged('+str(type)+','+str(id)+','+str(self.layer.featureCount())+')')

    def featureAdded(self,fid):
        #self.cmss.showCriticalMessage('CMSS 2','add')
        self.maxID=self.maxID+1
        self.layer.changeAttributeValue(fid,0,self.maxID)
        self.fid_dict[str(self.maxID)]=fid
        self.onLayerChanged(1,self.maxID)

    def featureDeletd(self,fid):
        #self.cmss.showCriticalMessage('CMSS 2','deleted')
        self.onLayerChanged(2,-1)

    def geomChanged(self,fid):
        #self.cmss.showCriticalMessage('CMSS 2','geom changed')
        id=next(self.layer.getFeatures(QgsFeatureRequest(fid))).attribute('id')
        print('modified: ' +str(fid))
        self.onLayerChanged(3,id)

    def selectionChanged(self,selected):
        if not (self.selectionMode):
            return;
        if len(selected)==1:
            f=next(self.layer.getFeatures(QgsFeatureRequest(selected[0])))
            self.page.mainFrame().evaluateJavaScript('featureSelected('+str(f.attribute('id'))+')')

