# My imports
from PyQt4.QtCore import *
from PyQt4.QtGui import *
from qgis.core import *
from qgis.gui import *
from collections import Counter
import csv
import sys


def ValidMDULayer(selectedLayer):
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
    fieldDict = {'ID': False, 'COIL_LOC': False, 'MDX_ADDRES': False,
                 'REEL_NUM': False, 'TYPE': False,'STREET_NUM': False,
                 'FULL_ADDRE': False, 'FULL_STREE': False}
    for field in selectedLayer.pendingFields():
        if fieldDict.has_key(field.name()):
            fieldDict[field.name()] = True
    for v in fieldDict.itervalues():
        if v is False:
            return False
    return True

def WriteMDUCSV(selectedLayer, pathToCSVFiles):
    MDUs = []
    iter = selectedLayer.getFeatures()
    for feature in iter:
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

    with open(pathToCSVFiles + '\\MDU_MTU_SHEET.csv', 'wb') as csvfile:
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
