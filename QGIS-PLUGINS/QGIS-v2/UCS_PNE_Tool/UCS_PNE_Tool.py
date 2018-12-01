# -*- coding: utf-8 -*-
"""
/***************************************************************************
 UCS_PNE_Tool
                                 A QGIS plugin
 UCS Planning and Engineering Tools for QGIS
                              -------------------
        begin                : 2017-08-05
        git sha              : $Format:%H$
        copyright            : (C) 2017 by Steven H. Brubaker
        email                : SBrubaker@ucseng.com
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
from PyQt4.QtCore import *
from PyQt4.QtGui import *
# Initialize Qt resources from file resources.py
import resources
# Import the code for the dialog
from UCS_PNE_Tool_dialog import UCS_PNE_ToolDialog
import os.path
# My imports
from qgis.core import *
from qgis.gui import *
from collections import Counter
import csv
import sys
import terminal_sheet
import mdu_sheet
import reel_sheets


class UCS_PNE_Tool:
    # Layer list for populating/refreshing (3) QComboBoxes
    layers = None
    # Path to store CSV files at
    pathToCSVFiles = ''

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
            'UCS_PNE_Tool_{}.qm'.format(locale))

        if os.path.exists(locale_path):
            self.translator = QTranslator()
            self.translator.load(locale_path)

            if qVersion() > '4.3.3':
                QCoreApplication.installTranslator(self.translator)


        # Declare instance attributes
        self.actions = []
        self.menu = self.tr(u'&UCS QGIS PnE Tools')
        # TODO: We are going to let the user set this up in a future iteration
        self.toolbar = self.iface.addToolBar(u'UCS_PNE_Tool')
        self.toolbar.setObjectName(u'UCS_PNE_Tool')

    def tr(self, message):
        """Get the translation for a string using Qt translation API.

        We implement this ourselves since we do not inherit QObject.

        :param message: String for translation.
        :type message: str, QString

        :returns: Translated version of message.
        :rtype: QString
        """
        # noinspection PyTypeChecker,PyArgumentList,PyCallByClass
        return QCoreApplication.translate('UCS_PNE_Tool', message)

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
        self.dlg = UCS_PNE_ToolDialog()

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
            self.iface.addPluginToMenu(
                self.menu,
                action)

        self.actions.append(action)

        return action

    def initGui(self):
        """Create the menu entries and toolbar icons inside the QGIS GUI."""

        icon_path = ':/plugins/UCS_PNE_Tool/icon.png'
        self.add_action(
            icon_path,
            text=self.tr(u'UCS PnE Tools'),
            callback=self.run,
            parent=self.iface.mainWindow())

    def unload(self):
        """Removes the plugin menu item and icon from QGIS GUI."""
        for action in self.actions:
            self.iface.removePluginMenu(
                self.tr(u'&UCS QGIS PnE Tools'),
                action)
            self.iface.removeToolBarIcon(action)
        # remove the toolbar
        del self.toolbar

    def load_layers(self):
        # Load up the Combo Boxes
        self.layers = self.iface.legendInterface().layers()
        layer_list = []
        for layer in self.layers:
            layer_list.append(layer.name())
        self.dlg.cBoxValidateExistingLayer.addItems(layer_list)
        self.dlg.cBoxMDUJoinLayer.addItems(layer_list)
        self.dlg.cBoxReelLayer.addItems(layer_list)

    def select_output_folder(self):
        self.pathToCSVFiles = QFileDialog.getExistingDirectory(self.dlg, "Save CSV Files to: ","N:/", QFileDialog.ShowDirsOnly)
        if self.pathToCSVFiles != '':
            self.dlg.txtPathToCSVFiles.setText(self.pathToCSVFiles)
            self.dlg.btnGetPathToCSVFiles.setStyleSheet('QPushButton {background-color: #9bef9f; color: rgb(2, 81, 6);}\n')
            self.dlg.btnCheckTermLayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(2, 22, 102);}\n')
            self.dlg.btnCheckTermLayer.setEnabled(True)
            self.dlg.cBoxValidateExistingLayer.setEnabled(True)
            self.dlg.textEditFeedback.append("CSV Files will be saved to: " + self.pathToCSVFiles)

    def CheckAndValidateTerminalLayer(self):
        # Change the ActiveLayer to the one in cBoxValidateExistingLayer
        selectedLayerIndex = self.dlg.cBoxValidateExistingLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        if terminal_sheet.ValidTerminalLayer(selectedLayer) is True:
            self.dlg.textEditFeedback.setText("All required Terminal fields found.")
            # Now to make sure there are less than 12 Addresses and/or Fibers assigned to each Terminal
            tooMany = terminal_sheet.ValidateTerminalLayer(selectedLayer)
            if tooMany == '':
                self.dlg.textEditFeedback.setText("Terminal Layer passed validation.")
                self.dlg.btnCheckTermLayer.setStyleSheet('QPushButton {background-color: #9bef9f; color: rgb(2, 81, 6);}\n')
                self.dlg.btnCheckMDULayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(2, 22, 102);}\n')
                self.dlg.btnCheckMDULayer.setEnabled(True)
                self.dlg.cBoxMDUJoinLayer.setEnabled(True)
            else:
                self.dlg.textEditFeedback.setText('<font color=red><b>Too many Addresses or Fibers assigned to Terminal(s)!</b></font>\n')
                for i, er in enumerate(tooMany):
                    self.dlg.textEditFeedback.append(er)
        else:
            self.dlg.textEditFeedback.setText("Please select another Final Join (Terminal) "
                                              "Layer or check this Layer for these missing field(s):")
            self.dlg.textEditFeedback.append(" (1) TERM_NUM\n (2) POLE_ADDRE\n (3) VATS\n (4) SIZE\n "
                                             "(5) REEL_NUM\n (6) TAP_POINT\n (7) STREET_NUM\n (8) UNIT\n "
                                             "(9) R_B\n (10) FULL_ADDRE")

    def CheckMDULayer(self):
        """Check for required Fields:
            (1) ID
            (2) COIL_LOC
            (3) MDX_ADDRES
            (4) REEL_NUM
            (5) TYPE
            (6) STREET_NUM
            (7) FULL_ADDRE
            (8) FULL_STREE
        """
        # Change the ActiveLayer to the one in cBoxMDUJoinLayer
        selectedLayerIndex = self.dlg.cBoxMDUJoinLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        if mdu_sheet.ValidMDULayer(selectedLayer) is True:
            self.dlg.textEditFeedback.setText("All required MDU fields found.")
            self.dlg.btnCheckMDULayer.setStyleSheet('QPushButton {background-color: #9bef9f; color: rgb(2, 81, 6);}\n')
            self.dlg.btnCheckReelLayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(2, 22, 102);}\n')
            self.dlg.btnCheckReelLayer.setEnabled(True)
            self.dlg.cBoxReelLayer.setEnabled(True)
        else:
            self.dlg.textEditFeedback.setText("Please select another MDU Join (MDU-MTU) "
                                              "Layer or check this Layer for these missing field(s):")
            self.dlg.textEditFeedback.append(" (1) ID\n (2) COIL_LOC\n (3) MDX_ADDRES\n (4) REEL_NUM\n "
                                             "(5) TYPE\n (6) STREET_NUM\n (7) FULL_ADDRE\n (8) FULL_STREE\n ")


    def CheckReelLayer(self):
        """Check for required Fields:
            (1) TO_POLE
            (2) FROM_POLE
            (3) REEL_NUM
            (4) TYPE
            (5) SIZE
            (6) FRC_CODE
            (7) SOURCE
            (8) FULL_STREE
        """
        # Change the ActiveLayer to the one in cBoxReelLayer
        selectedLayerIndex = self.dlg.cBoxReelLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        if reel_sheets.ValidReelLayer(selectedLayer) is True:
            self.dlg.textEditFeedback.setText("All required Reel fields found.")
            self.dlg.btnCheckReelLayer.setStyleSheet('QPushButton {background-color: #9bef9f; color: rgb(2, 81, 6);}\n')
            self.dlg.btnCreateCSVs.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(2, 22, 102);}\n')
            self.dlg.btnCreateCSVs.setEnabled(True)
        else:
            self.dlg.textEditFeedback.setText("Please select another Reel "
                                              "Layer or check this Layer for these missing field(s):")
            self.dlg.textEditFeedback.append(" (1) TO_POLE\n (2) FROM_POLE\n (3) REEL_NUM\n (4) TYPE\n "
                                             "(5) SIZE\n (6) FRC_CODE\n (7) SOURCE\n ")

    def CreateCSVFiles(self):
        # Change the ActiveLayer to the one in cBoxValidateExistingLayer
        selectedLayerIndex = self.dlg.cBoxValidateExistingLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        terminal_sheet.WriteTerminalCSV(selectedLayer, self.pathToCSVFiles)
        self.dlg.textEditFeedback.setText("Terminal CSV File saved to:\n" + self.pathToCSVFiles + '\\TERMINAL_SHEET.csv' + '\n')

        # Change the ActiveLayer to the one in cBoxMDUJoinLayer
        selectedLayerIndex = self.dlg.cBoxMDUJoinLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        mdu_sheet.WriteMDUCSV(selectedLayer, self.pathToCSVFiles)
        self.dlg.textEditFeedback.append("MDU-MTU CSV File saved to:\n" + self.pathToCSVFiles + '\\MDU_MTU_SHEET.csv' + '\n')

        # Change the ActiveLayer to the one in cBoxReelLayer
        selectedLayerIndex = self.dlg.cBoxReelLayer.currentIndex()
        selectedLayer = self.layers[selectedLayerIndex]

        reel_sheets.WriteReelCSV(selectedLayer, self.pathToCSVFiles)
        self.dlg.textEditFeedback.append("REEL LENGTH and REEL INFO CSV File saved to:\n" + self.pathToCSVFiles + '\\REEL_LEN_AND_INFO_SHEETS.csv' + '\n')

        self.dlg.btnCreateCSVs.setStyleSheet('QPushButton {background-color: #9bef9f; color: rgb(2, 81, 6);}\n')

    def run(self):
        # Clear previous status report(s)
        self.dlg.textEditFeedback.setText('')

        # Change button Styles back to normal
        self.dlg.btnGetPathToCSVFiles.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(2, 22, 102);}\n')
        self.dlg.btnCheckTermLayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(158, 158, 158);}\n')
        self.dlg.btnCheckMDULayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(158, 158, 158);}\n')
        self.dlg.btnCheckReelLayer.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(158, 158, 158);}\n')
        self.dlg.btnCreateCSVs.setStyleSheet('QPushButton {background-color: #badaf4; color: rgb(158, 158, 158);}\n')

        # Disable buttons until person picks a Save CSV Files Path
        self.dlg.btnCheckTermLayer.setEnabled(False)
        self.dlg.btnCheckMDULayer.setEnabled(False)
        self.dlg.btnCheckReelLayer.setEnabled(False)
        self.dlg.btnCreateCSVs.setEnabled(False)

        # Clear and then load up the (4) Combo Boxes with the current Project Layers
        self.layers = None
        self.dlg.cBoxValidateExistingLayer.clear()
        self.dlg.cBoxMDUJoinLayer.clear()
        self.dlg.cBoxReelLayer.clear()
        self.load_layers()

        # Lock Layer Comboboxes until person has selected a place to save CSV files
        self.dlg.cBoxValidateExistingLayer.setEnabled(False)
        self.dlg.cBoxMDUJoinLayer.setEnabled(False)
        self.dlg.cBoxReelLayer.setEnabled(False)

        # (1) Clear and select a directory to save the CSV files into
        self.dlg.txtPathToCSVFiles.setText('')
        self.dlg.btnGetPathToCSVFiles.clicked.connect(self.select_output_folder)

        # (2) Check the chosen Terminal Layer for:
        self.dlg.btnCheckTermLayer.clicked.connect(self.CheckAndValidateTerminalLayer)

        # (3) Check the chosen MDU-MTU Layer for:
        self.dlg.btnCheckMDULayer.clicked.connect(self.CheckMDULayer)

        # (4) Check the chosen Reel Layer for:
        self.dlg.btnCheckReelLayer.clicked.connect(self.CheckReelLayer)

        # (5) We have valid data so we can now create our CSV files
        self.dlg.btnCreateCSVs.clicked.connect(self.CreateCSVFiles)

        # show the dialog
        self.dlg.show()

        # Run the dialog event loop
        result = self.dlg.exec_()
