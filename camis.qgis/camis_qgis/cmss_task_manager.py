# -*- coding: utf-8 -*-
"""
/***************************************************************************
 CAMIS Qgis
                                 A QGIS plugin
 CAMIS Qgis Version
                              -------------------
        begin                : 2018-01-05
        git sha              : $Format:%H$
        copyright            : (C) 2018 by INTAPS Consultancy plc
        email                : info@intaps.com
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/
"""

from PyQt4.QtCore import QSettings, QTranslator, qVersion, QCoreApplication, Qt
from PyQt4.QtGui import QAction, QIcon
from PyQt4.QtGui import QApplication
from PyQt4.QtCore import QUrl
from PyQt4.QtCore import QFile
from PyQt4.QtCore import QIODevice
from PyQt4.QtWebKit import QWebView
from PyQt4.QtWebKit import QWebPage
from PyQt4.QtWebKit import QWebSettings
from PyQt4.QtGui import *

from qgis.core import *

import configparser
import json
import os.path

import resources
from cmss_task_manager_dockwidget import CMSS2DockWidget
import requests

from CMSSURLProcessor import CMSSURLProcessor
from CMSSLoginForm import CMSSLoginForm

class CMSSWebPage(QWebPage):
    def __init__(self, cmss,parent=None):
        super(CMSSWebPage, self).__init__(parent)
        self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
        self.cmss=cmss;
        self.urlProcessor=CMSSURLProcessor(self.cmss,self)

    def unload(self):
        self.urlProcessor.unload()
        return

    def acceptNavigationRequest(self, frame, request, type):
        if self.urlProcessor.processURL(request.url().path(), self.cmss.decodeQuery(request.url())):
            return False
        return True

class CMSS2:
    """QGIS Plugin Implementation."""

    def __init__(self, iface):
        """Constructor.

        :param iface: An interface instance that will be passed to this class
            which provides the hook by which you can manipulate the QGIS
            application at run time.
        :type iface: QgsInterface
        """
        # Save reference to the QGIS interface
        self.iface = iface

        # initialize plugin directory
        self.plugin_dir = os.path.dirname(__file__)

        #load configuration
        self.readConfig()

        # initialize locale
        locale = QSettings().value('locale/userLocale')[0:2]
        locale_path = os.path.join(
            self.plugin_dir,
            'i18n',
            'CMSS2_{}.qm'.format(locale))

        if os.path.exists(locale_path):
            self.translator = QTranslator()
            self.translator.load(locale_path)

            if qVersion() > '4.3.3':
                QCoreApplication.installTranslator(self.translator)

        # Declare instance attributes
        self.actions = []
        self.menu = self.tr(u'&CAMIS Qgis')
        # TODO: We are going to let the user set this up in a future iteration
        self.toolbar = self.iface.addToolBar(u'CAMIS_Qgis')
        self.toolbar.setObjectName(u'CAMIS_Qgis')

        #print "** INITIALIZING CAMIS Qgis"

        self.pluginIsActive = False
        self.dockwidget = None
        self.sessionid =None
        self.page=None
        self.logedIn=False

        self.map_layers=[]

    def decodeQuery(self,url):
        #print(str(url.encodedQuery()))
        q=str(url.encodedQuery()).split("&")
        print(str(q))
        dict={}
        for qq in q:
            if len(qq)==0:
                continue
            s=qq.split("=")
            if len(s)==2:
                dict[s[0]]=QUrl.fromPercentEncoding(s[1])
        return dict

    def showCriticalMessage(self,title,msg):
        w = QWidget()
        QMessageBox.critical(w, title,msg)
        w.show

    def showWarningMessage(self,msg):
        w = QWidget()
        QMessageBox.warning(w, 'MASSREG',msg)
        w.show

    def showUserConfirmation(self,msg):
        w = QWidget()
        reply=QMessageBox.question(w,'CMSS 2',msg,QMessageBox.Yes,QMessageBox.No)
        return reply==QMessageBox.Yes

    def showInformationMessage(self,title,msg):
        w = QWidget()
        QMessageBox.information(w, title,msg)
        w.show

    def readConfig(self):
        jstr=open(os.path.join(self.plugin_dir,'camis_qgis.json')).read()
        config=json.loads(jstr)
        print str(jstr)
        self.db_host=config['db-host']
        self.db_port=config['db-port']
        self.http_server=config['http']

    # noinspection PyMethodMayBeStatic
    def tr(self, message):
        """Get the translation for a string using Qt translation API.

        We implement this ourselves since we do not inherit QObject.

        :param message: String for translation.
        :type message: str, QString

        :returns: Translated version of message.
        :rtype: QString
        """
        # noinspection PyTypeChecker,PyArgumentList,PyCallByClass
        return QCoreApplication.translate('CAMIS Qgis', message)

    def add_action(
        self,
        icon_path,
        text,
        callback,
        enabled_flag=True,
        add_to_menu=True,
        add_to_toolbar=True,
        status_tip=None,
        whats_this=None,
        parent=None):
        """Add a toolbar icon to the toolbar.

        :param icon_path: Path to the icon for this action. Can be a resource
            path (e.g. ':/plugins/foo/bar.png') or a normal file system path.
        :type icon_path: str

        :param text: Text that should be shown in menu items for this action.
        :type text: str

        :param callback: Function to be called when the action is triggered.
        :type callback: function

        :param enabled_flag: A flag indicating if the action should be enabled
            by default. Defaults to True.
        :type enabled_flag: bool

        :param add_to_menu: Flag indicating whether the action should also
            be added to the menu. Defaults to True.
        :type add_to_menu: bool

        :param add_to_toolbar: Flag indicating whether the action should also
            be added to the toolbar. Defaults to True.
        :type add_to_toolbar: bool

        :param status_tip: Optional text to show in a popup when mouse pointer
            hovers over the action.
        :type status_tip: str

        :param parent: Parent widget for the new action. Defaults None.
        :type parent: QWidget

        :param whats_this: Optional text to show in the status bar when the
            mouse pointer hovers over the action.

        :returns: The action that was created. Note that the action is also
            added to self.actions list.
        :rtype: QAction
        """

        icon = QIcon(icon_path)
        action = QAction(icon, text, parent)
        action.triggered.connect(callback)
        action.setEnabled(enabled_flag)

        if status_tip is not None:
            action.setStatusTip(status_tip)

        if whats_this is not None:
            action.setWhatsThis(whats_this)

        if add_to_toolbar:
            self.toolbar.addAction(action)

        if add_to_menu:
            self.iface.addPluginToMenu(
                self.menu,
                action)

        self.actions.append(action)

        return action


    def initGui(self):
        """Create the menu entries and toolbar icons inside the QGIS GUI."""

        icon_path =self.plugin_dir+ '/cmss-login-sm.png'
        self.add_action(
            icon_path,
            text=self.tr(u'Login'),
            callback=self.loginAndRun,
            parent=self.iface.mainWindow())
        icon_path =self.plugin_dir+ '/cmss-logout-sm.png'	
        self.add_action(
            icon_path,
            text='Logout',
            callback=self.logout,
            parent=self.iface.mainWindow())
        self.add_geometryActions()
    #--------------------------------------------------------------------------
    def logout(self):
        if not(self.logedIn):
            self.showCriticalMessage('CAMIS Qgis', 'You are not loged in')
            return
        if not(self.showUserConfirmation('Are you sure you want to logout?')):
            return

        try:
            self.invokeServer('/api/admin/logout?sid='+self.sid,{})
        except Exception as ex:
            self.showWarningMessage('Error trying to close the server session.\nThe server may be down')

        self.deactivatePlugin()
        self.logedIn=False

    def add_geometryActions(self):
        self.toolbar.addSeparator()
        self.toolbar.addAction(self.iface.actionAddFeature())
        self.toolbar.addAction(self.iface.actionNodeTool())
        self.toolbar.addAction(self.iface.actionDeleteSelected())
        self.toolbar.addAction(self.iface.actionSplitFeatures())



    def onClosePlugin(self):
        """Cleanup necessary items here when plugin dockwidget is closed"""

        #print "** CLOSING CAMIS Qgis"

        # disconnects
        self.dockwidget.closingPlugin.disconnect(self.onClosePlugin)

        # remove this statement if dockwidget is to remain
        # for reuse if plugin is reopened
        # Commented next statement since it causes QGIS crashe
        # when closing the docked window:
        # self.dockwidget = None
        self.pluginIsActive = False

    def invokeServer(self,cmd,data):
        url=self.http_server+cmd
        print str(url)
        r=None
        if data is None:
            if self.sessionid:
                r=requests.get(url,cookies={'.ASPNetCoreSession':self.sessionid})
            else:
                r=requests.get(url)
        else:
            print str(data)
            headers = {'Content-type': 'application/json'}
            if self.sessionid:
                r=requests.post(url,data=json.dumps(data),cookies={'.ASPNetCoreSession':self.sessionid},headers=headers)
            else:
                r=requests.post(url,data=json.dumps(data),headers=headers)
        if '.ASPNetCoreSession' in r.cookies.keys():
            self.sessionid=r.cookies['.ASPNetCoreSession']
        res=r.json()
        if r.status_code!=200:
            res['error']=res['message']
        else:
            res['error']=None
        print str(res)

        return res

    def deactivatePlugin(self):
        if self.page:
            self.page.unload()
            self.page=None
        if self.dockwidget:
            self.iface.removeDockWidget(self.dockwidget)
            self.dockwidget=None
        self.unloadCMSSLayers()

    def unload(self):
        """Removes the plugin menu item and icon from QGIS GUI."""

        #print "** UNLOAD CAMIS Qgis"
        self.deactivatePlugin()

        for action in self.actions:
            self.iface.removePluginMenu(
                self.tr(u'&CAMIS Qgis'),
                action)
            self.iface.removeToolBarIcon(action)
        # remove the toolbar
        if self.toolbar:
            del self.toolbar


    #--------------------------------------------------------------------------

    def fixBrowserSetting(self):
        self.page.settings().setAttribute(QWebSettings.JavascriptEnabled, True)
        self.page.settings().setAttribute(QWebSettings.JavascriptCanOpenWindows, True)
        self.page.settings().setAttribute(QWebSettings.JavascriptCanCloseWindows, True)
        self.page.settings().setAttribute(QWebSettings.JavascriptCanAccessClipboard, True)
        self.page.settings().setAttribute(QWebSettings.DeveloperExtrasEnabled, True)
        self.page.settings().setAttribute(QWebSettings.OfflineStorageDatabaseEnabled, True)
        self.page.settings().setAttribute(QWebSettings.OfflineWebApplicationCacheEnabled, True)
        self.page.settings().setAttribute(QWebSettings.LocalStorageEnabled, True)
        self.page.settings().setAttribute(QWebSettings.LocalStorageDatabaseEnabled, True)
        self.page.settings().setAttribute(QWebSettings.LocalContentCanAccessRemoteUrls, True)
        self.page.settings().setAttribute(QWebSettings.LocalContentCanAccessFileUrls, True)


    def initBrowser(self):
        self.view = QWebView()
        self.page=CMSSWebPage(self)
        self.fixBrowserSetting()
        u=QUrl(self.http_server+'/api/cmss/home?sid='+self.sid)
        self.page.currentFrame().setUrl(u)
        self.view.setPage(self.page)
        self.dockwidget.widget().layout().addWidget(self.view)

    def afterLogin(self):
        self.logedIn=True
        self.run()

    def loginAndRun(self):
        if self.logedIn:
            self.showCriticalMessage('CAMIS Qgis','You have already loged in, please logout first')
            return
        self.loginForm=CMSSLoginForm(self,self.afterLogin)
        self.loginForm.show();

    def loadLayer(self,name,style,uri,schema,table,field,key):
        uri.setDataSource(schema,table,field,"",key)
        layer=QgsVectorLayer(uri.uri(False),name,"postgres")
        if  style:
            style_path = os.path.dirname(__file__)+'/'+style
            layer.loadNamedStyle(style_path)
        QgsMapLayerRegistry.instance().addMapLayer(layer)
        self.map_layers.append(layer)
        return layer;
		
    def loadCMSSLayers(self):

        uri=QgsDataSourceURI()
        uri.setConnection(self.db_host,self.db_port,"nrlais","user_cmss","cmssUserPW")
        camis_uri=QgsDataSourceURI()
        camis_uri.setConnection(self.db_host,self.db_port,"camis","user_cmss","cmssUserPW")


        uri.setDatabase("nrlais")
        uri.setUsername("user_cmss")
        uri.setPassword("cmssUserPW")

        self.loadLayer("Region Boundary",'region.qml',uri,"nrlais_sys","t_regions","geometry","id")
        self.loadLayer("Woreda Boundary",'zone.qml',uri,"nrlais_sys","t_zones","geometry","id")
        self.loadLayer("Zone Boundary",'woreda.qml',uri,"nrlais_sys","t_woredas","geometry","id")
        self.kebele_layer=self.loadLayer("Kebele Boundary",'kebele.qml',uri,"nrlais_sys","t_kebeles","geometry","id")
        self.loadLayer("NRLAIS Parcels","parcel.qml",uri,"nrlais_inventory","t_parcels","geometry","uid")
        self.loadLayer("Land Bank","land_bank.qml",camis_uri,"lb","v_gs_land","geometry","id")
  


    def unloadCMSSLayers(self):
        for l in self.map_layers:
            QgsMapLayerRegistry.instance().removeMapLayer(l.id())
        self.map_layers=[]

    def initSnapping(self):
        project = QgsProject.instance()
        project.writeEntry('Digitizing', 'DefaultSnapType', 'to vertex and segment')
        project.writeEntry('Digitizing', 'DefaultSnapTolerance', 3)
        project.writeEntry('Digitizing', 'SnappingMode', 'All visible layers')
        project.writeEntry('Digitizing', 'TopologicalEditing', 1)
        project.writeEntry('Digitizing', 'IntersectionSnapping', True)

    def run(self):
        """Run method that loads and starts the plugin"""


        #print "** STARTING CAMIS Qgis"

        # dockwidget may not exist if:
        #    first run of plugin
        #    removed on close (see self.onClosePlugin method)
        if self.dockwidget == None:
            # Create the dockwidget (after translation) and keep reference
            self.dockwidget = CMSS2DockWidget()
            self.initBrowser()
        else:
            self.dockwidget.show()

        # connect to provide cleanup on closing of dockwidget
        self.dockwidget.closingPlugin.connect(self.onClosePlugin)

        # show the dockwidget
        # TODO: fix to allow choice of dock location
        self.iface.addDockWidget(Qt.RightDockWidgetArea, self.dockwidget)
        self.dockwidget.show()

        #load layers
        self.loadCMSSLayers()
        self.initSnapping()
