# -*- coding: utf-8 -*-
from PyQt4.QtWebKit import QWebView
from PyQt4.QtWebKit import QWebPage
from PyQt4.QtGui import *
from PyQt4.QtCore import QUrl
import cmss_task_manager

class CMSSWebPage(QWebPage):
    def view(self):
        return self.view
    def __init__(self,cmss, parent=None):
        super(CMSSWebPage, self).__init__(parent)
        self.cmss=cmss
        self.setLinkDelegationPolicy(QWebPage.DelegateExternalLinks)
        self.view=QWebView()
        self.view.setPage(self)
        u=QUrl(self.cmss.http_server+'/cmss/home.jsp')
        self.currentFrame().setUrl(u)

    def decodeQuery(self,url):
        q=QUrl.fromPercentEncoding(url().encodedQuery()).split("&")
        dict={}
        for qq in q:
            s=qq.split("=")
            dict[s[0]]=s[1]
        return dict
    def acceptNavigationRequest(self, frame, request, type):
        print('Navigation Request:', request.url().path())
        if request.url().path()=='/cmss/qcmd/showMessage':
            w = QWidget()
            QMessageBox.information(w, "CMSS 2",self.decodeQuery(request.url)['msg'])
            w.show
            return False
        return True
