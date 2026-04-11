# -*- coding: utf-8 -*-
"""
/***************************************************************************
 CMSS2DockWidget
                                 A QGIS plugin
 CMSS Version 2
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

import os

#  Changed from PyQt4 to PyQt5
from PyQt5 import uic
from PyQt5.QtWidgets import QDockWidget
from PyQt5.QtCore import pyqtSignal

# ✅ Load UI file (compatible with PyQt5)
FORM_CLASS, _ = uic.loadUiType(os.path.join(
    os.path.dirname(__file__), 'cmss_task_manager_dockwidget_base.ui'))


class CMSS2DockWidget(QDockWidget, FORM_CLASS):

    closingPlugin = pyqtSignal()

    def __init__(self, parent=None):
        """Constructor."""
        super(CMSS2DockWidget, self).__init__(parent)
        # Set up the user interface from Designer.
        # After setupUI you can access any designer object by doing
        # self.<objectname>, and you can use autoconnect slots - see
        # http://qt-project.org/doc/qt-4.8/designer-using-a-ui-file.html
        # #widgets-and-dialogs-with-auto-connect
        self.setupUi(self)

    def closeEvent(self, event):
        self.closingPlugin.emit()
        event.accept()