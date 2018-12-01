# -*- coding: utf-8 -*-
# A QGIS plugin:    This plug-in is for automating the process of filling
#                   MDU information on the "MDU MTU CALC" of an Excel Workbook.
# Written by: Steven H. Brubaker (sbrubaker@ucseng.com)
# /***************************************************************************
#  *                                                                         *
#  *   This program is free software; you can redistribute it and/or modify  *
#  *   it under the terms of the GNU General Public License as published by  *
#  *   the Free Software Foundation; either version 2 of the License, or     *
#  *   (at your option) any later version.                                   *
#  *                                                                         *
#  ***************************************************************************/
# Qt Designer & QGIS generated imports
from PyQt4.QtCore import *
from PyQt4.QtGui import *
# Initialize Qt resources from file resources.py
import resources
# Import the code for the dialog
from MDU_Pproc_dialog import MDUProcDialog
import os.path
# My imports
from qgis.core import *
from qgis.gui import *
from collections import Counter
import csv
import sys

class MDUProc:

    """Notes about MDU MTU CALC sheet:
    25 MDU's possible, horizontal on worksheet "MDU MTU CALC"
    Header Fields:
    [0] H   [1] ID  [2] COIL_LOC    [3] MDX_ADDRES  [4] TYPE    [5] REEL_NUM
    Address Fields:
    [6] STREET_NUM  [7] FULL_ADDRE  [8] FULL_STREE
    """
    # References to MDU Join layer
    validCSV = False
    selectedLayer = None
    wrongLayer = True

    # Layer list for populating/refreshing (4) QComboBoxes
    layers = None

    def __init__(self, iface):
        """Constructor.
        :param iface: An interface instance that will be passed to this class
            which provides the hook by which you can manipulate the QGIS
            application at run time.
        :type iface: QgsInterface
        """
        # Save reference to the QGIS interface
        self.iface = iface
        # initialize plugin directory
        self.plugin_dir = os.path.dirname(__file__)
        # initialize locale
        locale = QSettings().value('locale/userLocale')[0:2]
        locale_path = os.path.join(
            self.plugin_dir,
            'i18n',
            'MDUProc_{}.qm'.format(locale))

        if os.path.exists(locale_path):
            self.translator = QTranslator()
            self.translator.load(locale_path)

            if qVersion() > '4.3.3':
                QCoreApplication.installTranslator(self.translator)


        # Declare instance attributes
        self.actions = []
        self.menu = self.tr(u'&UCS Planning and Engineering Tools')
        # TODO: We are going to let the user set this up in a future iteration
        self.toolbar = self.iface.addToolBar(u'MDUProc')
        self.toolbar.setObjectName(u'MDUProc')

    # noinspection PyMethodMayBeStatic
    def tr(self, message):
        """Get the translation for a string using Qt translation API.

        We implement this ourselves since we do not inherit QObject.

        :param message: String for translation.
        :type message: str, QString

        :returns: Translated version of message.
        :rtype: QString
        """
        # noinspection PyTypeChecker,PyArgumentList,PyCallByClass
        return QCoreApplication.translate('MDUProc', message)

    def add_action(self, icon_path, text, callback, enabled_flag=True, add_to_menu=True,
        add_to_toolbar=True, status_tip=None, whats_this=None, parent=None):
        """Add a toolbar icon to the toolbar.

        :param icon_path: Path to the icon for this action. Can be a resource
            path (e.g. ':/plugins/foo/bar.png') or a normal file system path.
        :type icon_path: str

        :param text: Text that should be shown in menu items for this action.
        :type text: str

        :param callback: Function to be called when the action is triggered.
        :type callback: function

        :param enabled_flag: A flag indicating if the action should be enabled
            by default. Defaults to True.
        :type enabled_flag: bool

        :param add_to_menu: Flag indicating whether the action should also
            be added to the menu. Defaults to True.
        :type add_to_menu: bool

        :param add_to_toolbar: Flag indicating whether the action should also
            be added to the toolbar. Defaults to True.
        :type add_to_toolbar: bool

        :param status_tip: Optional text to show in a popup when mouse pointer
            hovers over the action.
        :type status_tip: str

        :param parent: Parent widget for the new action. Defaults None.
        :type parent: QWidget

        :param whats_this: Optional text to show in the status bar when the
            mouse pointer hovers over the action.

        :returns: The action that was created. Note that the action is also
            added to self.actions list.
        :rtype: QAction
        """

        # Create the dialog (after translation) and keep reference
        self.dlg = MDUProcDialog()

        icon = QIcon(icon_path)
        action = QAction(icon, text, parent)
        action.triggered.connect(callback)
        action.setEnabled(enabled_flag)

        if status_tip is not None:
            action.setStatusTip(status_tip)

        if whats_this is not None:
            action.setWhatsThis(whats_this)

        if add_to_toolbar:
            self.toolbar.addAction(action)

        if add_to_menu:
            self.iface.addPluginToVectorMenu(
                self.menu,
                action)

        self.actions.append(action)

        return action

    def initGui(self):
        """Create the menu entries and toolbar icons inside the QGIS GUI."""
        icon_path = ':/plugins/MDUProc/icon.png'
        self.add_action(
            icon_path,
            text=self.tr(u'MDU-MTU Sheet to CSV'),
            callback=self.run,
            parent=self.iface.mainWindow())

    def unload(self):
        """Removes the plugin menu item and icon from QGIS GUI."""
        for action in self.actions:
            self.iface.removePluginVectorMenu(
                self.tr(u'&UCS Planning and Engineering Tools'),
                action)
            self.iface.removeToolBarIcon(action)
        # remove the toolbar
        del self.toolbar

    def select_output_file(self):
        filename = QFileDialog.getSaveFileName(self.dlg, "Save CSV to: ","n:/", 'CSV file (*.csv);;All Files (*)')
        self.dlg.lineEditFileName.setText(filename)
        self.dlg.textEditFeedback.append("CSV to be saved to: " + filename)
        self.validCSV = True

    def WriteOutMDUs(self):
        if self.validCSV is False:
            self.dlg.textEditFeedback.append("Please name and find a place to save the CSV first.")

        # Change the ActiveLayer to the one in cBoxValidateExistingLayer
        selectedLayerIndex = self.dlg.cBoxMDUJoinLayer.currentIndex()
        self.selectedLayer = self.layers[selectedLayerIndex]

        # Check to see if it is a viable layer to validate
        for field in self.selectedLayer.pendingFields():
            if field.name() == "MDX_ADDRES":
                self.wrongLayer = False
                self.dlg.textEditFeedback.append('MDX_ADDRES attribute found. Proceeding...')

        if self.wrongLayer is True:
            self.dlg.textEditFeedback.setText("Please choose another Layer as this one lacks any MDU's with Addresses.")
        else:
            MDUs = []
            iter = self.selectedLayer.getFeatures()
            for feature in iter:
                if (feature['ID'] == NULL):
                    self.dlg.textEditFeedback.setText("Please check your MDU-MTU ID number(s) as it appears some are not assigned. Stopping now for you to check/fix...")
                    return
                else:
                    MDUs.append([
                        feature['ID'],
                        feature['COIL_LOC'],
                        feature['MDX_ADDRES'],
                        feature['REEL_NUM'],
                        feature['TYPE'],
                        feature['STREET_NUM'],
                        feature['FULL_ADDRE'],
                        feature['FULL_STREE']
                    ])
                    MDUs.sort()
                
            with open(self.dlg.lineEditFileName.text(), 'wb') as csvfile:
                csvw = csv.writer(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
                currentMDUNr = 1
                addrCount = 1
                for rw in MDUs:
                    if rw[0] == currentMDUNr:
                        if addrCount <= 1:
                            csvw.writerow(['H', rw[0], rw[1], rw[2], rw[3]])
                            csvw.writerow(['A', rw[4], rw[5], rw[6], rw[7] ])
                            addrCount += 1
                        else:
                            csvw.writerow(['A', rw[4], rw[5], rw[6], rw[7] ])
                            addrCount += 1
                    else:
                        currentMDUNr = rw[0]
                        addrCount = 1
                        if addrCount <= 1:
                            csvw.writerow(['H', rw[0], rw[1], rw[2], rw[3] ])
                            csvw.writerow(['A',rw[4], rw[5], rw[6], rw[7] ])
                            addrCount += 1
                        else:
                            csvw.writerow(['A', rw[4], rw[5], rw[6], rw[7] ])
                            addrCount += 1
            csvfile.close()
            self.dlg.textEditFeedback.append("CSV written to: " + self.dlg.lineEditFileName.text())

    def load_layers(self):
        # Load up the (4) Combo Boxes
        self.layers = self.iface.legendInterface().layers()
        layer_list = []
        for layer in self.layers:
            layer_list.append(layer.name())
        self.dlg.cBoxMDUJoinLayer.addItems(layer_list)

    def run(self):
        # Event handling (Slot & Signal)
        self.dlg.textEditFeedback.clear()

        # Load up the QGIS Project Layer(s) Combo Box
        self.load_layers()

        # Create CSV Signal & Slot connection
        self.dlg.btnCSVFile.clicked.connect(self.select_output_file)

        # Create CSV Signal & Slot connection
        self.dlg.btnWriteOutMDUs.clicked.connect(self.WriteOutMDUs)

        # show the dialog
        self.dlg.show()

        # Run the dialog event loop
        self.dlg.exec_()
