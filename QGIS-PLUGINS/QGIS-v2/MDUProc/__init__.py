# -*- coding: utf-8 -*-
"""
/***************************************************************************
 MDUProc
                                 A QGIS plugin
 UCS Tools for Planning and Engineering GIS Solutions
                             -------------------
        begin                : 2017-07-24
        copyright            : (C) 2017 by Steven H. Brubaker
        email                : sbrubaker@UCSEng.com
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
    """Load MDUProc class from file MDUProc.

    :param iface: A QGIS interface instance.
    :type iface: QgsInterface
    """
    #
    from .MDU_Pproc import MDUProc
    return MDUProc(iface)
