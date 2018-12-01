# My imports
from PyQt4.QtCore import *
from PyQt4.QtGui import *
from qgis.core import *
from qgis.gui import *
from collections import *
import csv
import sys


def ValidTerminalLayer(selectedLayer):
    """Check for required Fields:
        (1) TERM_NUM
        (2) POLE_ADDRE
        (3) VATS
        (4) SIZE
        (5) REEL_NUM
        (6) TAP_POINT
        (7) STREET_NUM
        (8) UNIT
        (9) R_B
        (10) FULL_ADDRE
    """
    fieldDict = {'TERM_NUM': False, 'POLE_ADDRE': False, 'VATS': False,
                 'SIZE': False, 'TAP_POINT': False,'STREET_NUM': False,
                 'UNIT': False, 'FULL_ADDRE': False, 'PART_USE': False}
    for field in selectedLayer.pendingFields():
        if fieldDict.has_key(field.name()):
            fieldDict[field.name()] = True
    for v in fieldDict.itervalues():
        if v is False:
            return False
    return True

def ValidateTerminalLayer(selectedLayer):
    # Sentinel variables to be checked before attempting to create CSV
    hasRBField = False
    hasFibersField = False
    termNRs = []

    # Check to see if it is a viable layer to validate
    for field in selectedLayer.pendingFields():
        if field.name() == "R_B":
            hasRBField = True
        if field.name() == "Fibers":
            hasFibersField = True

    # Populate our LIST of Terminal Numbers
    feats = selectedLayer.getFeatures()
    for feature in feats:
        termNRs.append(int(feature['TERM_NUM']))
    numberOfTerminals = max(termNRs)

    # If needed, add "R_B" and "Fibers" fields
    if hasFibersField == False or hasRBField == False:
        caps = selectedLayer.dataProvider().capabilities()
        if caps & QgsVectorDataProvider.AddAttributes:
            if hasFibersField == False:
                res = selectedLayer.dataProvider().addAttributes([QgsField("Fibers", QVariant.Int)])
                selectedLayer.updateFields()
            if hasRBField == False:
                res = selectedLayer.dataProvider().addAttributes([QgsField("R_B", QVariant.String)])
                selectedLayer.updateFields()

    # No values returned - just admin used to check we do not have too
    # many Addresses and/or Fibers assigned to a Terminal (12 ports/fibers max)
    UpdateRBAndFibers(selectedLayer)
    tooManyAddressesAssigned = CheckAddressCount(numberOfTerminals, termNRs)
    if tooManyAddressesAssigned != '':
        return tooManyAddressesAssigned
    tooManyFibersAssigned = CheckFiberCount(selectedLayer, numberOfTerminals)
    if tooManyFibersAssigned != '':
        return tooManyFibersAssigned
    return ''

def UpdateRBAndFibers(selectedLayer):
    # After creating R_B Field(Attribute) and Fibers Field, they will be None (Null)
    # so we need to update null to appropriate BUS/RES/"V/L" and Fiber Count
    feats = selectedLayer.getFeatures()
    R_B = selectedLayer.fieldNameIndex('R_B')
    fibers = selectedLayer.fieldNameIndex('Fibers')
    selectedLayer.startEditing()
    for feature in feats:
        if str(feature['PART_USE']) == "AH" or str(feature['PART_USE']) == "C" or \
           str(feature['PART_USE']) == "CC" or str(feature['PART_USE']) == "I" or \
           str(feature['PART_USE']) == "RC":
            selectedLayer.changeAttributeValue(feature.id(), R_B, "BUS")
            selectedLayer.changeAttributeValue(feature.id(), fibers, 2)
        else:
            selectedLayer.changeAttributeValue(feature.id(), fibers, 1)
            if str(feature['PART_USE']) == "CL" or str(feature['PART_USE']) == "RL":
                selectedLayer.changeAttributeValue(feature.id(), R_B, "V/L")
            else:
                selectedLayer.changeAttributeValue(feature.id(), R_B, "RES")
    selectedLayer.commitChanges()

def CheckAddressCount(numberOfTerminals, termNRs):
    # Create a DICTionary using the imported Counter() function
    termDistribution = Counter(termNRs)
    tooMany = []
    # Iterate over the DICT(Counter(termDistribution)) to see if there are
    tooManyAddressesAssigned = []
    for i in range(1, numberOfTerminals + 1):
        if termDistribution[i] > 12:
            tooManyAddressesAssigned.append(i)

    # and inform client of what Terminals need check
    if len(tooManyAddressesAssigned) > 0:
        for i in range(len(tooManyAddressesAssigned)):
            tooMany.append('Too many addresses assigned to Terminal: ' + str(tooManyAddressesAssigned[i]))
        return tooMany
    # Else we have both a valid Layer and less than 12 Addresses per Terminal
    else:
        return ''

def CheckFiberCount(selectedLayer, numberOfTerminals):
    # Lists of all the Terminals with there associated Fiber(s) and available Ports (aka SIZE)
    fibers = []
    ports = []
    for i in range(1, numberOfTerminals + 1):
        expr = QgsExpression("\"TERM_NUM\" = " + str( i))
        it = selectedLayer.getFeatures(QgsFeatureRequest(expr))
        for feature in it:
            fibers.append([int(feature['TERM_NUM']), feature['Fibers']])
            ports.append([int(feature['TERM_NUM']), int(feature['SIZE'])])

    # Now to create a Distribution Dictionary of Terminal-to-Fibers-to-Ports so we can...
    fiberDistro = {}
    portDistro = {}
    fiberCount = 0
    for i in range(1, numberOfTerminals + 1):
        for f in range(0, len(fibers)):
            if fibers[f][0] == i:
                fiberCount += fibers[f][1]
                fiberDistro[i] = fiberCount
                portDistro[i] = ports[f][1]
            else:
                fiberCount = 0

    # ...Iterate over the DICT(fiberDistro) to see if there are
    tooManyFibersAssigned = []
    for i in range(1, numberOfTerminals + 1):
        if fiberDistro[i] > portDistro[i]:
            tooManyFibersAssigned.append(i)

    # and inform client of what Terminals need check
    if len(tooManyFibersAssigned) > 0:
        tooMany = []
        for i in range(len(tooManyFibersAssigned)):
            tooMany.append('Too many fibers assigned to Terminal: ' + str(tooManyFibersAssigned[i]))
        return tooMany
    # Else we have not over-assigned fibers to any 4/8/12 port terminals
    else:
        return ''

def WriteTerminalCSV(selectedLayer, pathToCSVFiles):
    terms = []
    iter = selectedLayer.getFeatures()
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
    with open(pathToCSVFiles + '\\TERMINAL_SHEET.csv', 'wb') as csvfile:
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
