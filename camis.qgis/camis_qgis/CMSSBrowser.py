# -*- coding: utf-8 -*-
import os
import traceback

#  PyQt5 imports with WebEngine/WebKit fallback
try:
    from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEnginePage, QWebEngineSettings
    USING_WEBENGINE = True
except ImportError:
    from PyQt5.QtWebKitWidgets import QWebView, QWebPage
    from PyQt5.QtWebKit import QWebSettings
    USING_WEBENGINE = False

from PyQt5.QtCore import QUrl
from PyQt5 import uic
from PyQt5.QtWidgets import QDockWidget, QToolBar, QAction
from PyQt5.QtGui import QIcon

# ✅ Load UI file (compatible with PyQt5)
FORM_CLASS, _ = uic.loadUiType(os.path.join(
    os.path.dirname(__file__), 'browser_widget.ui'))


#  WebPage class adapted for WebEngine/WebKit
if USING_WEBENGINE:
    class BrowserPage(QWebEnginePage):
        def __init__(self, browser, parent=None):
            super(BrowserPage, self).__init__(parent)
            self.browser = browser

        def acceptNavigationRequest(self, url, type, isMainFrame):
            return True
else:
    class BrowserPage(QWebPage):
        def __init__(self, browser, parent=None):
            super(BrowserPage, self).__init__(parent)
            self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
            self.browser = browser

        def acceptNavigationRequest(self, frame, request, type):
            return True


class CMSSBrowser(QDockWidget, FORM_CLASS):
    def __init__(self, cmss, url, parent=None):
        """Constructor."""
        super(CMSSBrowser, self).__init__(parent)
        self.setupUi(self)
        self.cmss = cmss
        self.url = url
        # tool bar
        self.initToolBar()
        self.initBrowser()

    def initToolBar(self):
        self.toolBar = QToolBar()
        p = os.path.join(os.path.dirname(__file__), 'browser-back.png')
        # ✅ Check if icon exists; fallback to text-only if missing
        if os.path.exists(p):
            icon = QIcon(p)
            action = QAction(icon, "Back", None)
        else:
            action = QAction("Back", None)
        print(p)  # ✅ Print statement already has parentheses
        self.toolBar.addAction(action)
        action.triggered.connect(self.back)

        # ✅ Add toolbar to the widget's layout
        self.widget().layout().addWidget(self.toolBar)

    def back(self):
        # ✅ Handle both WebEngine and WebKit back navigation
        if USING_WEBENGINE:
            self.view.back()
        else:
            self.view.back()

    def setUrl(self, url):
        self.url = url
        # ✅ Update the browser if already loaded
        if hasattr(self, 'view'):
            full_url = QUrl(self.cmss.http_server + self.url + "&session_id=" + self.cmss.sessionid)
            if USING_WEBENGINE:
                self.view.setUrl(full_url)
            else:
                self.view.setUrl(full_url)

    def onClose(self):
        self.close()

    #  Updated for Qt5 WebEngine/WebKit
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

    #  Updated for Qt5 WebEngine/WebKit
    def initBrowser(self):
        if USING_WEBENGINE:
            self.view = QWebEngineView()
            self.page = BrowserPage(self)
            self.fixBrowserSetting()
            self.view.setPage(self.page)
            full_url = QUrl(self.cmss.http_server + self.url + "&session_id=" + self.cmss.sessionid)
            self.view.setUrl(full_url)
        else:
            self.view = QWebView()
            self.page = BrowserPage(self)
            self.fixBrowserSetting()
            u = QUrl(self.cmss.http_server + self.url + "&session_id=" + self.cmss.sessionid)
            self.page.currentFrame().setUrl(u)
            self.view.setPage(self.page)
        
        self.widget().layout().addWidget(self.view)