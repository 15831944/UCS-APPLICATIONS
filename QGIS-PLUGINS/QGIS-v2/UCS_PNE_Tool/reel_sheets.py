# My imports
from PyQt4.QtCore import *
from PyQt4.QtGui import *
from qgis.core import *
from qgis.gui import *
from collections import Counter
import csv
import sys


def ValidReelLayer(selectedLayer):
    """Check for required Fields:
        (1) TO_POLE
        (2) FROM_POLE
        (3) REEL_NUM
        (4) TYPE
        (5) SIZE
        (6) FRC_CODE
        (7) SOURCE
    """
    fieldDict = {'TO_POLE': False, 'FROM_POLE': False, 'REEL_NUM': False,
                 'TYPE': False, 'SIZE': False,'FRC_CODE': False,
                 'SOURCE': False}
    for field in selectedLayer.pendingFields():
        if fieldDict.has_key(field.name()):
            fieldDict[field.name()] = True
    for v in fieldDict.itervalues():
        if v is False:
            return False
    return True

def WriteReelCSV(selectedLayer, pathToCSVFiles):
    Reels = []
    iter = selectedLayer.getFeatures()
    for feature in iter:
        Reels.append([
            feature['REEL_NUM'],
            feature['TYPE'],
            feature['SIZE'],
            feature['FRC_CODE'],
            feature['SOURCE'],
            feature['TO_POLE'],
            feature['FROM_POLE']
        ])
    Reels.sort()
    with open(pathToCSVFiles + '\\REEL_LEN_AND_INFO_SHEETS.csv', 'wb') as csvfile:
        csvw = csv.writer(csvfile, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)
        for rw in Reels:
            csvw.writerow([rw[0], rw[1], rw[2], rw[3], rw[4], rw[5], rw[6]])

    csvfile.close()
