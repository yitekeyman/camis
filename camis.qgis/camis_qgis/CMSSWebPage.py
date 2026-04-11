# -*- coding: utf-8 -*-
"""
CMSSWebPage - Web page handler for CMSS plugin.
"""

#  PyQt5 imports with WebEngine/WebKit fallback
try:
    from PyQt5.QtWebEngineWidgets import QWebEngineView, QWebEnginePage, QWebEngineSettings
    USING_WEBENGINE = True
except ImportError:
    from PyQt5.QtWebKitWidgets import QWebView, QWebPage
    from PyQt5.QtWebKit import QWebSettings
    USING_WEBENGINE = False

from PyQt5.QtCore import QUrl
from PyQt5.QtWidgets import QWidget, QMessageBox

# Import the main plugin class type hint if needed
# from cmss_task_manager import CMSS2


class CMSSWebPage(QWebEnginePage if USING_WEBENGINE else QWebPage):
    """Custom web page for CMSS plugin with navigation handling."""

    def __init__(self, cmss, parent=None):
        """Constructor.

        :param cmss: Reference to the main CMSS2 plugin instance.
        :param parent: Parent widget.
        """
        super(CMSSWebPage, self).__init__(parent)
        self.cmss = cmss
        
        if not USING_WEBENGINE:
            self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
        
        # Create the view and set this page
        if USING_WEBENGINE:
            self.view = QWebEngineView()
            self.view.setPage(self)
            u = QUrl(self.cmss.http_server + '/cmss/home.jsp')
            self.view.setUrl(u)
        else:
            self.view = QWebView()
            self.view.setPage(self)
            u = QUrl(self.cmss.http_server + '/cmss/home.jsp')
            self.currentFrame().setUrl(u)

    def decodeQuery(self, url):
        """Decode URL query parameters into a dictionary.

        :param url: QUrl object or string.
        :return: Dictionary of query parameters.
        """
        # Get query string
        if isinstance(url, str):
            query_str = url.split('?')[1] if '?' in url else ''
        else:
            query_str = url.query()
        
        result = {}
        if query_str:
            from urllib.parse import unquote
            for pair in query_str.split('&'):
                if '=' in pair:
                    key, value = pair.split('=', 1)
                    result[key] = unquote(value)
        return result

    def acceptNavigationRequest(self, request_or_url, type_or_isMainFrame, isMainFrame_or_None=None):
        """Handle navigation requests.

        For WebEngine: acceptNavigationRequest(self, url, type, isMainFrame)
        For WebKit: acceptNavigationRequest(self, frame, request, type)
        """
        if USING_WEBENGINE:
            url = request_or_url
            # type and isMainFrame are the other parameters
        else:
            frame = request_or_url
            request = type_or_isMainFrame
            url = request.url()
        
        #  Added parentheses to print
        print('Navigation Request:', url.path())
        
        if url.path() == '/cmss/qcmd/showMessage':
            query = self.decodeQuery(url)
            msg = query.get('msg', 'No message provided')
            #  Proper QMessageBox usage without unnecessary QWidget
            QMessageBox.information(None, "CMSS 2", msg)
            return False
        
        return True

    def view(self):
        """Return the associated QWebView/QWebEngineView."""
        return self.view