# -*- coding: utf-8 -*-
"""
/***************************************************************************
 CMSS2
                                 A QGIS plugin
 CMSS Version 2
                             -------------------
        begin                : 2018-01-05
        copyright            : (C) 2018 by INTAPS Consultancy plc
        email                : info@intaps.com
        git sha              : $Format:%H$
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/
 This script initializes the plugin, making it known to QGIS.
"""


# noinspection PyPep8Naming
def classFactory(iface):  # pylint: disable=invalid-name
    """Load CMSS2 class from file CMSS2.

    :param iface: A QGIS interface instance.
    :type iface: QgsInterface
    """
    #
    from .cmss_task_manager import CMSS2
    return CMSS2(iface)
