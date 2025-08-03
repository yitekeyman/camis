import os

from PyQt4 import QtGui, uic,QtCore
from PyQt4.QtGui import *
from PyQt4.QtWebKit import QWebView
from PyQt4.QtWebKit import QWebPage
from PyQt4.QtWebKit import QWebSettings
from PyQt4.QtCore import QUrl

import traceback

FORM_CLASS, _ = uic.loadUiType(os.path.join(
    os.path.dirname(__file__), 'browser_widget.ui'))



class BrowserPage(QWebPage):
    def __init__(self,brower,parent=None):
        super(BrowserPage, self).__init__(parent)
        self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)

    def acceptNavigationRequest(self, frame, request, type):
        return True



class CMSSBrowser(QtGui.QDockWidget, FORM_CLASS):
    def __init__(self, cmss,url,parent=None):
        """Constructor."""
        super(CMSSBrowser, self).__init__(parent)
        self.setupUi(self)
        self.cmss=cmss
        self.url=url
        #tool bar
        self.initToolBar()
        self.initBrowser()

    def initToolBar(self):
        self.toolBar=QToolBar()
        p=os.path.join(os.path.dirname(__file__), 'browser-back.png')
        icon=QIcon(p)
        print(p)
        #action = QAction(icon, "Back",None)
        action = QAction("Back",None)
        self.toolBar.addAction("Back")
        action.triggered.connect(self.back)

        self.widget().layout().addWidget(self.toolBar)

    def back(self):
        self.view.back()

    def setUrl(self,url):
        self.url=url

    def onClose(self):
        self.close()

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
        self.page=BrowserPage(self)
        self.fixBrowserSetting()
        u=QUrl(self.cmss.http_server+self.url+"&session_id="+self.cmss.sessionid)
        self.page.currentFrame().setUrl(u)
        self.view.setPage(self.page)
        self.widget().layout().addWidget(self.view)


