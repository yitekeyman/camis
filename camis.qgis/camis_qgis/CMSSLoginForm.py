import os

from PyQt4.QtWebKit import QWebView
from PyQt4.QtWebKit import QWebPage
from PyQt4.QtWebKit import QWebSettings
from PyQt4.QtCore import QUrl


from PyQt4 import QtGui, uic,QtCore
from PyQt4.QtGui import *
import traceback

FORM_CLASS, _ = uic.loadUiType(os.path.join(
    os.path.dirname(__file__), 'cmss_login.ui'))

class LoginPage(QWebPage):
    def __init__(self,parent=None):
        super(LoginPage, self).__init__(parent)
        self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
        self.cmss=parent.cmss;

    def unload(self):
        self.urlProcessor.unload()
        return

    def acceptNavigationRequest(self, frame, request, type):
        q=self.cmss.decodeQuery(request.url())
        if request.url().path()=="/cmss/login_cmd":
            self.parent().onOk(q['un'],q['pw'])
            return False
        if request.url().path()=="/cmss/cancel_cmd":
            self.parent().onCancel()
            return False
        return True

class CMSSLoginForm(QtGui.QDialog, FORM_CLASS):
    def __init__(self, cmss,cb_method, parent=None):
        """Constructor."""
        super(CMSSLoginForm, self).__init__(parent)
        self.setupUi(self)
        self.cmss=cmss
        self.cb_method=cb_method
        self.initBrowser()

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
        self.page=LoginPage(self)
        self.fixBrowserSetting()
        u=QUrl(self.cmss.http_server+"/api/cmss/login")
        self.page.currentFrame().setUrl(u)
        self.view.setPage(self.page)
        self.layout().addWidget(self.view)

    def onCancel(self):
        self.hide()

    def onOk(self,un,pw):
        try:
            res=self.cmss.invokeServer("/api/admin/login",{'UserName':un,'Password':pw})
            if res['error'] is None:
                print self.cmss.sessionid
                self.cmss.sid=res['sid']
                self.hide()
                self.cb_method()
            else:
                self.cmss.showCriticalMessage('CAMIS Qgis','Error trying to connect to the server'+res['error'])
        except Exception as ex:
            self.cmss.showCriticalMessage('Login failed',traceback.format_exc())




