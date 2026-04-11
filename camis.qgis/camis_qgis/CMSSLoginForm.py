import os
import traceback


try:
    from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEnginePage, QWebEngineSettings
    USING_WEBENGINE = True
except ImportError:
    from PyQt5.QtWebKitWidgets import QWebView, QWebPage
    from PyQt5.QtWebKit import QWebSettings
    USING_WEBENGINE = False

from PyQt5.QtCore import QUrl
from PyQt5 import uic, QtCore
from PyQt5.QtWidgets import QDialog, QApplication

FORM_CLASS, _ = uic.loadUiType(os.path.join(
    os.path.dirname(__file__), 'cmss_login.ui'))

if USING_WEBENGINE:
    class LoginPage(QWebEnginePage):
        def __init__(self, parent=None):
            super(LoginPage, self).__init__(parent)
            self.cmss = parent.cmss
            self.parent_dialog = parent  # Store reference to dialog

        def unload(self):
            pass

        def acceptNavigationRequest(self, url, type, isMainFrame):
            q = self.cmss.decodeQuery(url)
            if url.path() == "/cmss/login_cmd":
                self.parent_dialog.onOk(q.get('un', ''), q.get('pw', ''))
                return False
            if url.path() == "/cmss/cancel_cmd":
                self.parent_dialog.onCancel()
                return False
            return True
else:
    class LoginPage(QWebPage):
        def __init__(self, parent=None):
            super(LoginPage, self).__init__(parent)
            self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
            self.cmss = parent.cmss
            self.parent_dialog = parent

        def unload(self):
            return

        def acceptNavigationRequest(self, frame, request, type):
            q = self.cmss.decodeQuery(request.url())
            if request.url().path() == "/cmss/login_cmd":
                self.parent_dialog.onOk(q.get('un', ''), q.get('pw', ''))
                return False
            if request.url().path() == "/cmss/cancel_cmd":
                self.parent_dialog.onCancel()
                return False
            return True




class CMSSLoginForm(QDialog, FORM_CLASS):
    def __init__(self, cmss, cb_method, parent=None):
        """Constructor."""
        super(CMSSLoginForm, self).__init__(parent)
        self.setupUi(self)
        self.cmss = cmss
        self.cb_method = cb_method
        self.initBrowser()

    # Updated for Qt5 WebEngine/WebKit
    def fixBrowserSetting(self):
        if USING_WEBENGINE:
            settings = self.page.settings()
            settings.setAttribute(QWebEngineSettings.JavascriptEnabled, True)
            settings.setAttribute(QWebEngineSettings.JavascriptCanOpenWindows, True)
            settings.setAttribute(QWebEngineSettings.LocalStorageEnabled, True)
            settings.setAttribute(QWebEngineSettings.LocalContentCanAccessRemoteUrls, True)
            settings.setAttribute(QWebEngineSettings.LocalContentCanAccessFileUrls, True)
        else:
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

    # Updated for Qt5 WebEngine/WebKit
    def initBrowser(self):
        if USING_WEBENGINE:
            self.view = QWebEngineView()
            self.page = LoginPage(self)
            self.fixBrowserSetting()
            self.view.setPage(self.page)
            self.view.setUrl(QUrl(self.cmss.http_server + "/api/cmss/login"))
        else:
            self.view = QWebView()
            self.page = LoginPage(self)
            self.fixBrowserSetting()
            u = QUrl(self.cmss.http_server + "/api/cmss/login")
            self.page.currentFrame().setUrl(u)
            self.view.setPage(self.page)
        
        self.layout().addWidget(self.view)

    def onCancel(self):
        self.hide()

    def onOk(self, un, pw):
        try:
            res = self.cmss.invokeServer("/api/admin/login", {'UserName': un, 'Password': pw})
            if res['error'] is None:
                # Added parentheses for print
                print(self.cmss.sessionid)
                self.cmss.sid = res['sid']
                self.hide()
                self.cb_method()
            else:
                self.cmss.showCriticalMessage('CAMIS Qgis', 'Error trying to connect to the server: ' + str(res['error']))
        except Exception as ex:
            self.cmss.showCriticalMessage('Login failed', traceback.format_exc())



