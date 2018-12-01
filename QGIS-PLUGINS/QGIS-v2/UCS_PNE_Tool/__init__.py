# -*- coding: utf-8 -*-
"""
/***************************************************************************
 UCS_PNE_Tool
                                 A QGIS plugin
 UCS Planning and Engineering Tools for QGIS
                             -------------------
        begin                : 2017-08-05
        copyright            : (C) 2017 by Steven H. Brubaker
        email                : SBrubaker@ucseng.com
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
    """Load UCS_PNE_Tool class from file UCS_PNE_Tool.

    :param iface: A QGIS interface instance.
    :type iface: QgsInterface
    """
    #
    from .UCS_PNE_Tool import UCS_PNE_Tool
    return UCS_PNE_Tool(iface)
