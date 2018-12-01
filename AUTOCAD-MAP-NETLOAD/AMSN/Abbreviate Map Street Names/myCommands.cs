/* ---- AbbrMSN v1.0 ---
 * Abbreviate all DTEXT Street Names imported from ArcGIS
 * Originally written and compiled in .NET version 3.5 for AutoCAD 3D Map 2011
 *
 * 27 May 2014 - Created by Steven H. Brubaker
 *
 * 01 Dec 2018 - Updated to .NET 4.7 for AutoCAD Map 3D 2019 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Abbreviate_Map_Street_Names
{

    public class AbbrMSN
    {
        [CommandMethod("AMSN")]
        public void AMSN()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {

                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

                foreach (ObjectId id in btr)
                {
                    Entity currentEntity = tr.GetObject(id, OpenMode.ForWrite, false) as Entity;
                    if (currentEntity == null)
                    {
                        continue;
                    }
                    if (currentEntity.GetType() == typeof(DBText))
                    {
                        // See Note 1
                        String testString = (((DBText)currentEntity).TextString).ToUpper();
                        if (testString.Contains(" ROAD"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" ROAD", " RD");
                        }
                        else if (testString.Contains(" AVENUE"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" AVENUE", " AVE");
                        }
                        else if (testString.Contains(" BOULEVARD"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" BOULEVARD", " BLVD");
                        }
                        else if (testString.Contains(" LANE"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" LANE", " LN");
                        }
                        else if (testString.Contains(" STREET"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" STREET", " ST");
                        }
                        else if (testString.Contains(" CIRCLE"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" CIRCLE", " CIR");
                        }
                        else if (testString.Contains(" HIGHWAY"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" HIGHWAY", " HWY");
                        }
                        else if (testString.Contains(" ROUTE"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" ROUTE", " RTE");
                        }
                        else if (testString.Contains(" DRIVE"))
                        {
                            ((DBText)currentEntity).TextString = testString.Replace(" DRIVE", " DR");
                        }
                        /*
                            Note 1:
                            A lot more abbrv can be found at: http://pe.usps.gov/text/pub28/28apc_002.htm
                            but I resisted the temptation to implement any of the others as a lot of New
                            England motorways seem to include them in their proper names...
                            
                            Note 2: 
                            The same can be done in Visual LISP without running into errors with locked layers.

                            Note 3: Sort of a 'TO DO': Add MTEXT Search & Replace should we have to give up
                            our ArcGIS application that extracts names... However, I would probably just
                            write a new .NET program using the AutoCAD.MAP.FDO libraries to extract the road
                            names from shapefiles.
                        */
                    }
                }
                tr.Commit();
            }
        }
    }

}
