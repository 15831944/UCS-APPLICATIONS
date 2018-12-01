# -*- coding: utf-8 -*-
# A QGIS plugin: Export Terminal Address information
# from a layer to an Excel "TERMINAL SHEET"
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
import resources
# Import the code for the dialog
from Terminal_Processing_dialog import TermProcDialog
import os.path
# My imports
from qgis.core import *
from qgis.gui import *
from collections import Counter
import csv
import sys

class TermProc:
    # Sentinal variables to be checked before attempting to create CSV
    wrongLayer = True
    hasRBField = False
    hasFibersField = False
    validCSV = False
    tooManyAddrs = True
    tooManyFibers = True
    Validated = False
    SpareFibersAdded = False
    
    # Dictionary to populate/update (1) QTableWidget
    termDict = {'1: Term Ports':[], '2: Assigned Fibers':[], '3: Assigned Spare Fibers':[], '4: Nr Spare Fibers Assigned':[], '5: Unassigned Spare Fibers':[], '6: Addrs per Term':[], '7: Terminal Number':[]}

    # Dictionary to fill in CSV H row for Spare Fiber assignments in self.writeOutData()
    spareDict = {}

    # Layer list for populating/refreshing (4) QComboBoxes
    layers = None

    # Reference to Final Join layer
    selectedLayer = None
    numberOfTerminals = 0
    termNRs = []

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
            'TermProc_{}.qm'.format(locale))

        if os.path.exists(locale_path):
            self.translator = QTranslator()
            self.translator.load(locale_path)

            if qVersion() > '4.3.3':
                QCoreApplication.installTranslator(self.translator)


        # Declare instance attributes
        self.actions = []
        self.menu = self.tr(u'&UCS Planning and Engineering Tools')
        # TODO: We are going to let the user set this up in a future iteration
        self.toolbar = self.iface.addToolBar(u'TermProc')
        self.toolbar.setObjectName(u'TermProc')

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
        return QCoreApplication.translate('TermProc', message)

    def add_action(
        self,
        icon_path,
        text,
        callback,
        enabled_flag=True,
        add_to_menu=True,
        add_to_toolbar=True,
        status_tip=None,
        whats_this=None,
        parent=None):
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
        self.dlg = TermProcDialog()

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

        icon_path = ':/plugins/TermProc/icon.png'
        self.add_action(
            icon_path,
            text=self.tr(u'Terminal Sheet to CSV Tool'),
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

    def ValidateData(self):
        self.dlg.lineEditValidationFeedback.clear()
        
        # Change the ActiveLayer to the one in cBoxValidateExistingLayer
        selectedLayerIndex = self.dlg.cBoxValidateExistingLayer.currentIndex()
        self.selectedLayer = self.layers[selectedLayerIndex]

        # Check to see if it is a viable layer to validate
        for field in self.selectedLayer.pendingFields():
            if field.name() == "TERM_NUM":
                self.wrongLayer = False
                self.dlg.lineEditValidationFeedback.append('TERM_NUM attribute found. Proceeding...')
                self.dlg.textEditFeedback.append('TERM_NUM attribute found. Proceeding...')
            if field.name() == "R_B":
                self.hasRBField = True
            if field.name() == "Fibers":
                self.hasFibersField = True

        # If we have a null or wrong layer
        if self.wrongLayer == True:
            self.dlg.lineEditValidationFeedback.setText("Please choose another Layer as this one lacks any Terminals or Addresses.")
            self.dlg.textEditFeedback.setText("Please choose another Layer as this one lacks any Terminals or Addresses.")
        # else we have a proper layer but...
        else:
            # Populate our LIST of Terminal Numbers
            iter = self.selectedLayer.getFeatures()
            for feature in iter:
                self.termNRs.append(int(feature['TERM_NUM']))

            # Find the highest TERM_NUM
            self.numberOfTerminals = max(self.termNRs)
            self.dlg.lineEditValidationFeedback.append('Checking for R_B and Fibers Fields...')
            self.dlg.textEditFeedback.append('Checking for R_B and Fibers Fields...')
            # If needed, add "R_B" and "Fibers" fields
            if self.hasFibersField == False or self.hasRBField == False:
                caps = self.selectedLayer.dataProvider().capabilities()
                if caps & QgsVectorDataProvider.AddAttributes:
                    if self.hasFibersField == False:
                        res = self.selectedLayer.dataProvider().addAttributes([QgsField("Fibers", QVariant.Int)])
                        self.selectedLayer.updateFields()
                    if self.hasRBField == False:
                        res = self.selectedLayer.dataProvider().addAttributes([QgsField("R_B", QVariant.String)])
                        self.selectedLayer.updateFields()
            self.dlg.lineEditValidationFeedback.append('Checking for Spare Fiber Placeholders...')
            self.dlg.textEditFeedback.append('Checking for Spare Fiber Placeholders...')
            self.HasSpareFiberPlaceHolders()
            self.dlg.lineEditValidationFeedback.append('Updating R_B and Fibers Fields...')
            self.dlg.textEditFeedback.append('Updating R_B and Fibers Fields...')
            self.UpdateRBAndFibers()
            self.CheckAddressCount()
            self.CheckFiberCount()
            self.FillValidationTable()
            if self.tooManyAddrs == False and self.tooManyFibers == False and self.wrongLayer == False:
                self.dlg.lineEditValidationFeedback.append('SUCCESS! Data validated and ready for Step 3: Create CSV File')
                self.dlg.textEditFeedback.append('SUCCESS! Data validated and ready for Step 3: Create CSV File')
            else:
                self.dlg.lineEditValidationFeedback.append('Please scroll up to find out what needs fixed.')
                self.dlg.textEditFeedback.append('Please check the Validation page and Summary Report for problems with validation.')

    def HasSpareFiberPlaceHolders(self):
        iter = self.selectedLayer.getFeatures()
        for feature in iter:
            if feature['FULL_ADDRE'] == "SPARE FIBER":
                self.SpareFibersAdded = True

    def FillValidationTable(self):
        self.dlg.tableWidgetValidation.clear()
        qTable = self.dlg.tableWidgetValidation
        qTable.setRowCount(self.numberOfTerminals)
        qTable.setColumnCount(7)
        horHeaders = []
        for n, key in enumerate(sorted(self.termDict.keys())):
            horHeaders.append(key)
            for m, item in enumerate(self.termDict[key]):
                newitem = QTableWidgetItem(str(item))
                qTable.setItem(m, n, newitem)
        qTable.setHorizontalHeaderLabels(horHeaders)
        qTable.resizeColumnsToContents()
        self.Validated = True

    def UpdateRBAndFibers(self):
        # After creating R_B Field(Attribute) and Fibers Field, they will be None (Null)
        # so we need to update null to appropriate BUS/RES/"V/L" and Fiber Count
        iter = self.selectedLayer.getFeatures()
        R_B = self.selectedLayer.fieldNameIndex('R_B')
        fibers = self.selectedLayer.fieldNameIndex('Fibers')
        self.selectedLayer.startEditing()
        for feature in iter:
            if str(feature['PART_USE']) == "AH" or str(feature['PART_USE']) == "C" or str(feature['PART_USE']) == "CC" or str(feature['PART_USE']) == "I" or str(feature['PART_USE']) == "RC":
                self.selectedLayer.changeAttributeValue(feature.id(), R_B, "BUS")
                self.selectedLayer.changeAttributeValue(feature.id(), fibers, 2)
            else:
                self.selectedLayer.changeAttributeValue(feature.id(), fibers, 1)
                if str(feature['PART_USE']) == "CL" or str(feature['PART_USE']) == "RL":
                    self.selectedLayer.changeAttributeValue(feature.id(), R_B, "V/L")
                else:
                    self.selectedLayer.changeAttributeValue(feature.id(), R_B, "RES")
        self.selectedLayer.commitChanges()

    def CheckFiberCount(self):
        self.dlg.lineEditValidationFeedback.append('Checking Fibers per Terminal count...')
        self.dlg.textEditFeedback.append('Checking Fibers per Terminal count...')

        fibers = []
        ports = []
        spareFibers = []
        for i in range(1, self.numberOfTerminals + 1):
            expr = QgsExpression("\"TERM_NUM\" = " + str( i))
            it = self.selectedLayer.getFeatures(QgsFeatureRequest(expr))
            for feature in it:
                fibers.append([int(feature['TERM_NUM']), feature['Fibers']])
                ports.append([int(feature['TERM_NUM']), int(feature['SIZE'])])
                spareFibers.append([int(feature['TERM_NUM']), feature['SPARE_FIBE']])
                self.spareDict[i] = [feature['POLE_ADDRE'],feature['VATS'],feature['SIZE'],feature['REEL_NUM'],feature['TAP_POINT'], feature['SPARE_FIBE']]

        fiberDistro = {}
        portDistro = {}
        spareFiberDistro = {}
        fiberCount = 0
        for i in range(1, self.numberOfTerminals + 1):
            for f in range(0, len(fibers)):
                if fibers[f][0] == i:
                    fiberCount += fibers[f][1]
                    fiberDistro[i] = fiberCount
                    portDistro[i] = ports[f][1]
                    spareFiberDistro[i]= spareFibers[f][1]
                else:
                    fiberCount = 0        
                
        for i in range(1, self.numberOfTerminals + 1):
            self.termDict['2: Assigned Fibers'].append(fiberDistro[i])
            self.termDict['1: Term Ports'].append(portDistro[i])
            self.termDict['3: Assigned Spare Fibers'].append(str(spareFiberDistro[i]))
            
            if spareFiberDistro[i] == u'Y':
                self.termDict['4: Nr Spare Fibers Assigned'].append(int(portDistro[i]) - int(fiberDistro[i]))
                self.termDict['5: Unassigned Spare Fibers'].append(0)
            else:
                self.termDict['4: Nr Spare Fibers Assigned'].append(0)
                self.termDict['5: Unassigned Spare Fibers'].append(int(portDistro[i]) - int(fiberDistro[i]))

        # Iterate over the DICT(fiberDistro) to see if there are
        tooManyFibersAssigned = []
        for i in range(1, self.numberOfTerminals + 1):
            if fiberDistro[i] > portDistro[i]:
                tooManyFibersAssigned.append(i)

        # and inform client of what Terminals need check
        if len(tooManyFibersAssigned) > 0:
            self.dlg.lineEditValidationFeedback.append("FAILED: Too many Fibers have been assigned! Check Terminal Number(s):")
            self.dlg.textEditFeedback.append("FAILED: Too many Fibers have been assigned! Check Terminal Number(s):")
            for i in range(len(tooManyFibersAssigned)):
                self.dlg.lineEditValidationFeedback.append(str(tooManyFibersAssigned[i]) + ' - ')
                self.dlg.textEditFeedback.append(str(tooManyFibersAssigned[i]) + ' - ')
        # Else we have not over-assigned fibers to any 4/8/12 port terminals
        else:
            self.tooManyFibers = False
            self.dlg.lineEditValidationFeedback.append("PASSED: Assigned Fibers are less than Terminal Size.")
            self.dlg.textEditFeedback.append("PASSED: Assigned Fibers are less than Terminal Size.")

    def CheckAddressCount(self):
        # ...we need to check the Terminal-to-Address counts
        self.dlg.lineEditValidationFeedback.append('Checking Address per Terminal count...')
        self.dlg.textEditFeedback.append('Checking Address per Terminal count...')

        # Create a DICTionary using the imported Counter() function
        termDistribution = Counter(self.termNRs)

        for i in range(1, self.numberOfTerminals + 1):
            self.termDict['7: Terminal Number'].append(i)
            self.termDict['6: Addrs per Term'].append(termDistribution[i])

        # Iterate over the DICT(Counter(termDistribution)) to see if there are
        tooManyAddressesAssigned = []
        for i in range(1, self.numberOfTerminals + 1):
            if termDistribution[i] > 12:
                tooManyAddressesAssigned.append(i)

        # and inform client of what Terminals need check
        if len(tooManyAddressesAssigned) > 0:
            self.dlg.lineEditValidationFeedback.append("FAILED: Too many Addresses have been assigned! Check Terminal Number(s):")
            self.dlg.textEditFeedback.append("FAILED: Too many Addresses have been assigned! Check Terminal Number(s):")
            for i in range(len(tooManyAddressesAssigned)):
                self.dlg.lineEditValidationFeedback.append(str(tooManyAddressesAssigned[i]) + ' - ')
                self.dlg.textEditFeedback.append(str(tooManyAddressesAssigned[i]) + ' - ')
        # Else we have both a valid Layer and less than 12 Addresses per Terminal
        else:
            self.tooManyAddrs = False
            self.dlg.lineEditValidationFeedback.append("PASSED: Less than 12 Addresses per Terminal.")
            self.dlg.textEditFeedback.append("PASSED: Less than 12 Addresses per Terminal.")

    def select_output_file(self):
        filename = QFileDialog.getSaveFileName(self.dlg, "Save CSV to: ","n:/", 'CSV file (*.csv);;All Files (*)')
        self.dlg.lineEditFileName.setText(filename)
        self.validCSV = True

    def writeOutData(self):
        if self.validCSV == False:
            self.dlg.lineEditValidationFeedback.append("Please name and find a place to save the CSV first.")
            self.dlg.textEditFeedback.append("Please name and find a place to save the CSV first.")

        if self.tooManyAddrs == False and self.tooManyFibers == False and self.wrongLayer == False and self.validCSV == True:
            if self.SpareFibersAdded == False:
                self.dlg.lineEditValidationFeedback.append("Adding Spare Fiber Placeholders...")
                self.dlg.textEditFeedback.append("Adding Spare Fiber Placeholders...")
                caps = self.selectedLayer.dataProvider().capabilities()
                currentTerminal = 0
                for f in range(self.numberOfTerminals):
                    currentTerminal += 1
                    cnt = 1
                    if self.termDict['3: Assigned Spare Fibers'][f] == 'Y':
                        while int(self.termDict['4: Nr Spare Fibers Assigned'][f]) >= cnt:
                            if caps & QgsVectorDataProvider.AddFeatures:
                                feat = QgsFeature(self.selectedLayer.pendingFields())
                                feat.setAttribute('TERM_NUM', str(currentTerminal))
                                feat.setAttribute('POLE_ADDRE', self.spareDict[currentTerminal][0])
                                feat.setAttribute('VATS', self.spareDict[currentTerminal][1])
                                if self.spareDict[currentTerminal][2] == None:
                                    feat.setAttribute('SIZE', 12)
                                else:
                                    feat.setAttribute('SIZE', int(self.spareDict[currentTerminal][2]))
                                if self.spareDict[currentTerminal][3] == None:
                                    feat.setAttribute('REEL_NUM', 0)
                                else:
                                    feat.setAttribute('REEL_NUM', int(self.spareDict[currentTerminal][3]))
                                feat.setAttribute('TAP_POINT', self.spareDict[currentTerminal][4])
                                feat.setAttribute('SPARE_FIBE', self.spareDict[currentTerminal][5])
                                feat.setAttribute('STREET_NUM', 'NA')
                                feat.setAttribute('R_B', 'RES')
                                feat.setAttribute('Fibers', 1)
                                feat.setAttribute('FULL_ADDRE', 'SPARE FIBER')
                                (res, outFeats) = self.selectedLayer.dataProvider().addFeatures([feat])
                            cnt += 1

            terms = []
            iter = self.selectedLayer.getFeatures()
            for feature in iter:
                terms.append([
                    int(feature['TERM_NUM']),
                    feature['POLE_ADDRE'],
                    feature['VATS'],
                    feature['SIZE'],
                    feature['REEL_NUM'],
                    feature['TAP_POINT'],
                    feature['STREET_NUM'],
                    feature['UNIT'],
                    feature['R_B'],
                    feature['FULL_ADDRE']
                ])
            terms.sort()
            with open(self.dlg.lineEditFileName.text(), 'wb') as csvfile:
                csvw = csv.writer(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
                currentTermNr = 1
                addrCount = 1
                for rw in terms:
                    if rw[0] == currentTermNr:
                        if addrCount <= 1:
                            csvw.writerow(['H', rw[0], rw[1], rw[2], rw[3], rw[4], rw[5] ])
                            csvw.writerow(['A', rw[6], rw[7], rw[8], rw[9] ])
                            addrCount += 1
                        else:
                            csvw.writerow(['A', rw[6], rw[7], rw[8], rw[9] ])
                            addrCount += 1
                    else:
                        currentTermNr = rw[0]
                        addrCount = 1
                        if addrCount <= 1:
                            csvw.writerow(['H', rw[0], rw[1], rw[2], rw[3], rw[4], rw[5] ])
                            csvw.writerow(['A', rw[6], rw[7], rw[8], rw[9] ])
                            addrCount += 1
                        else:
                            csvw.writerow(['A', rw[6], rw[7], rw[8], rw[9] ])
                            addrCount += 1
            csvfile.close()
            self.dlg.lineEditValidationFeedback.append("CSV written to: " + self.dlg.lineEditFileName.text())
            self.dlg.textEditFeedback.append("CSV written to: " + self.dlg.lineEditFileName.text())

    def SaveAndOpenReport(self):
        self.dlg.lineEditValidationFeedback.append("Creating the Report now...")
        self.dlg.textEditFeedback.append("Creating the Report now...")
        if self.Validated == True:
            qTable = self.dlg.tableWidgetValidation
            # Identify the location of the current project so we can save the report as a CSV file
            path_absolute = QgsProject.instance().readPath("./")
            rpt = unicode(path_absolute) + '/Final_Join_Report.csv'
            filePath = rpt.replace(r'/','\\')
            with open(rpt, 'wb') as csvfile:
                csvw = csv.writer(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
                hdr = []
                hdr.append(unicode("Term Ports").encode('utf8'))
                hdr.append(unicode("Assigned Fibers").encode('utf8'))
                hdr.append(unicode("Assigned Spare Fibers").encode('utf8'))
                hdr.append(unicode("Nr Spare Fibers Assigned").encode('utf8'))
                hdr.append(unicode("Unassigned Spare Fibers").encode('utf8'))
                hdr.append(unicode("Addrs per Term").encode('utf8'))
                hdr.append(unicode("Terminal Number").encode('utf8'))
                csvw.writerow(hdr)
                for row in range(qTable.rowCount()):
                    rowdata = []
                    for column in range(qTable.columnCount()):
                        item = qTable.item(row, column)
                        if item is not None:
                            rowdata.append(unicode(item.text()).encode('utf8'))
                        else:
                            rowdata.append('')
                    csvw.writerow(rowdata)
                hdr = []
                hdr.append(unicode("***Note that this CSV is saved in:").encode('utf8'))
                csvw.writerow(hdr)
                hdr = []
                hdr.append(unicode(filePath).encode('utf8'))
                csvw.writerow(hdr)
            csvfile.close()
            excelOpen = 'start excel.exe ' + '\"' + filePath + '\"'
            os.system(excelOpen)

    def load_layers(self):
        # Load up the (4) Combo Boxes
        self.layers = self.iface.legendInterface().layers()
        layer_list = []
        for layer in self.layers:
            layer_list.append(layer.name())
        self.dlg.cBoxValidateExistingLayer.addItems(layer_list)

    def run(self):
        # Event handling (Slot & Signal)
        self.dlg.lineEditFileName.clear()
        self.dlg.textEditFeedback.clear()
        self.dlg.lineEditValidationFeedback.clear()
        self.dlg.tableWidgetValidation.clear()

        # Load up the (4) Combo Boxes
        self.load_layers()

        # Create Validation Signal & Slot connection
        self.dlg.btnValidateExistingLayer.clicked.connect(self.ValidateData)

        # Create CSV Signal & Slot connection
        self.dlg.btnCSVFile.clicked.connect(self.select_output_file)

        # Create CSV Signal & Slot connection
        self.dlg.btnSaveReport.clicked.connect(self.SaveAndOpenReport)

        # Create CSV Signal & Slot connection
        self.dlg.btnWriteOutCSV.clicked.connect(self.writeOutData)

        # show the dialog
        self.dlg.show()
        # Run the dialog event loop
        result = self.dlg.exec_()