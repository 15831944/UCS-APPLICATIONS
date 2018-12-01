// (C) Copyright 2016 by Steven H. Brubaker
// Purpose: Define and create/apply AutoCAD formats needed to emulate painted lines on roads

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace UCS_Road_Lines
{
    public static class road_styles
    {
        /*
         Name                               Color   Global Width    LineType
         MUTCD-LINE-DASHED-WHITE            9       0.41            ACAD_ISO03W100
         MUTCD-LINE-DASHED-YELLOW           9       0.33            ACAD_ISO03W100
         MUTCD-LINE-SOLID-DOUBLE-YELLOW     52      0.33            CONTINUOUS
         MUTCD-LINE-SOLID-WHITE             9       0.41            CONTINUOUS
         T-EOP                              8       0.0             CONTINUOUS
         T-ROAD-CNTR                        2       0.0             CENTER                    
        */

        // One Lane Roads
        public static void OneLaneRoads(Polyline poly)
        {
            /* One Lane Road Info
            Button Name         roadStyle
            OneL_No_Markings    1L-NM 
            OneL_W_W            1L-W-W
            OneL_Y_W            1L-Y-W
            OneL_W_Y            1L-W-Y
            OneL_Y_Y            1l-Y-Y
            */

            switch(utils.roadStyle)
            {
                case ("1L-NM"):
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth / 2.0, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth / 2.0), "T-EOP");
                        }
                        break;
                    }
                case ("1L-W-W"):
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.leftShoulderWidth + (utils.laneWidth / 2.0), "T-EOP");
                            utils.OffsetRoadLine(poly, utils.rightShoulderWidth + -(utils.laneWidth / 2.0), "T-EOP");
                        }
                        // Painted Lines
                        utils.OffsetRoadLine(poly, utils.laneWidth / 2.0, "MUTCD-LINE-SOLID-WHITE");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2.0), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case ("1L-Y-W"):
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.leftShoulderWidth + (utils.laneWidth / 2.0), "T-EOP");
                            utils.OffsetRoadLine(poly, utils.rightShoulderWidth + -(utils.laneWidth / 2.0), "T-EOP");
                        }
                        // Painted Lines
                        utils.OffsetRoadLine(poly, utils.laneWidth / 2.0, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2.0), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case ("1L-W-Y"):
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.leftShoulderWidth + (utils.laneWidth / 2.0), "T-EOP");
                            utils.OffsetRoadLine(poly, utils.rightShoulderWidth + -(utils.laneWidth / 2.0), "T-EOP");
                        }
                        // Painted Lines
                        utils.OffsetRoadLine(poly, utils.laneWidth / 2.0, "MUTCD-LINE-SOLID-WHITE");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2.0), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        break;
                    }
                case ("1L-Y-Y"):
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.leftShoulderWidth + (utils.laneWidth / 2.0), "T-EOP");
                            utils.OffsetRoadLine(poly, utils.rightShoulderWidth + -(utils.laneWidth / 2.0), "T-EOP");
                        }
                        // Painted Lines
                        utils.OffsetRoadLine(poly, utils.laneWidth / 2.0, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2.0), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        break;
                    }
            }
        } // End Function OneLaneRoads

        // Two Lane Roads
        public static void TwoLaneRoads(Polyline poly)
        {
            /* Two Lane Road Info
            Button Name         roadStyle
            TwoL_No_Markings    2L-NM 
            TwoL_CL             2L-CL
            TwoL_W_CL_W         2L-W-CL-W
            TwoL_DY             2L-DY
            TwoL_W_DY_W         2L-W-DY-W
            TwoL_W_YCL_Y_W      2L-W-YCL-Y-W
            TwoL_W_Y_YCL_W      2L-W-Y-YCL-W
            */
            switch (utils.roadStyle)
            {
                case "2L-NM":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth), "T-EOP");
                        }
                        break;
                    }
                case "2L-CL":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }   
                        // Centerline = new Polyline placed over selected object (LINE, 2DPOLY or LWPOLY)
                        utils.OffsetRoadLine(poly, 0.0, "MUTCD-LINE-DASHED-WHITE");
                        break;
                    }
                case "2L-W-CL-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");
                        // Centerline = new Polyline placed over selected object (LINE, 2DPOLY or LWPOLY)
                        utils.OffsetRoadLine(poly, 0.0, "MUTCD-LINE-DASHED-WHITE");
                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "2L-DY":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth + 
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) + 
                                -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        break;
                    }
                case "2L-W-DY-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth +
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) +
                                -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-SOLID-WHITE");

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.33, "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "2L-W-YCL-Y-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth +
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) +
                                -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-SOLID-WHITE");

                        // Centerline = Dashed Yellow - Solid Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-DASHED-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right EOP & MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.33, "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "2L-W-Y-YCL-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, utils.laneWidth +
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(utils.laneWidth) +
                                -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-SOLID-WHITE");

                        // Centerline = Solid Yellow - Dashed Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-DASHED-YELLOW");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.33, "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
            }
        } // End Function TwoLaneRoads

        // Three Lane Roads
        public static void ThreeLaneRoads(Polyline poly)
        {
            /* Three Lane Styles
             Button Name          roadStyle
             ThreeL_W_CLT_W       3L-W-CLT-W          Three Lane - Center Lane for Turns
             ThreeL_W_DAW_DBY_W   3L-W-DAW-DBY-W      Three Lane - Two Lanes Left, No Passing Right
             ThreeL_W_DBY_DAW_W   3L-W-DBY-DAW-W      Three Lane - No Passing Left, Two Lanes Right
             ThreeL_W_DAW_Y_DAY_W 3L-W-DAW-Y-DAY-W    Three Lane - Two Lanes Left, Restricted Passing Right
             ThreeL_W_DAY_Y_DAW_W 3L-W-DAY-Y-DAW-W    Three Lane - Restricted Passing Left, Two Lanes Right
             */

            switch (utils.roadStyle)
            {
                case "3L-W-CLT-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) - 0.66, "MUTCD-LINE-DASHED-YELLOW");
                        
                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) - 0.66), "MUTCD-LINE-DASHED-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "3L-W-DAW-DBY-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-DASHED-WHITE");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) - 0.66), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "3L-W-DBY-DAW-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) - 0.66, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "3L-W-DAW-Y-DAY-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-DASHED-WHITE");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) - 0.66), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-DASHED-YELLOW");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "3L-W-DAY-Y-DAW-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth) + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) + utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-DASHED-YELLOW");
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) - 0.66, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) + utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
            }
        } // End Function ThreeLaneRoads

        // Four Lane Roads
        public static void FourLaneRoads(Polyline poly)
        {
            /* Four Lane Styles
             * Button Name              roadStyle
             * FourL_W_DW_DBY_DW_W      4L-W-DW-DBY-DW-W    Four Lanes - Double Yellow Centerline and White Side Lines
             * FourL_DW_DBY_DW          4L-DW-DBY-DW        Four Lanes - Double Yellow Centerline and No White Side Lines
             * FourLDH_W_DW_DBY_DW_W    4LDH-W-DW-DBY-DW-W  Four Lanes - Divided Highway with White Side Lines
             * FourLDH_DW_DBY_DW        4LDH-DW-DBY-DW      Four Lanes - Divided Highway without White Side Lines
             */

            switch(utils.roadStyle)
            {
                case "4L-W-DW-DBY-DW-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2 * utils.laneWidth) 
                                + -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, 2 * utils.laneWidth + 0.33, "MUTCD-LINE-SOLID-WHITE");

                        // Lane Divider Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-DASHED-WHITE");

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Divider Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth + 0.33), "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((2 * utils.laneWidth) + 0.33), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "4L-DW-DBY-DW":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2 * utils.laneWidth) + 
                                -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }

                        // Lane Divider Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-DASHED-WHITE");

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Divider Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth + 0.33), "MUTCD-LINE-DASHED-WHITE");
                        break;
                    }
                
                // Divided Highway or Street with Median
                case "4LDH-W-DW-DBY-DW-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + utils.leftShoulderWidth
                                + 0.5 + utils.leftMedianWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2 * utils.laneWidth) + utils.rightShoulderWidth 
                                + -0.5 + utils.rightMedianWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Lane Divider Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.5 + 
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Left Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, 0.5 + utils.leftMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Median EOP's
                        utils.OffsetRoadLine(poly, utils.leftMedianWidth, "T-EOP");
                        utils.OffsetRoadLine(poly, utils.rightMedianWidth, "T-EOP");

                        // Right Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, -0.5 + utils.rightMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Divider Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(2 * utils.laneWidth) + -0.5 + 
                            utils.rightMedianWidth, "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }

                case "4LDH-DW-DBY-DW":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + utils.leftShoulderWidth
                                + 0.5 + utils.leftMedianWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2 * utils.laneWidth) + utils.rightShoulderWidth
                                + -0.5 + utils.rightMedianWidth, "T-EOP");
                        }

                        // Lane Divider Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Left Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, 0.5 + utils.leftMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Median EOP's
                        utils.OffsetRoadLine(poly, utils.leftMedianWidth, "T-EOP");
                        utils.OffsetRoadLine(poly, utils.rightMedianWidth, "T-EOP");

                        // Right Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, -0.5 + utils.rightMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Divider Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth + 0.5) + 
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        break;
                }
            }
        } // End Function FourLaneRoads

        // Five Lane Roads
        public static void FiveLaneRoads(Polyline poly)
        {
            /* Five Lane Styles
             * Button Name                  roadStyle
             * FiveL_W_DW_Y_DY_DY_Y_DW      5L-W-DW-Y-DY-DY-Y-DW-W  Five Lanes - (Four Lane Road) with Center Turnout
             * FiveL_DW_Y_DY_DY_Y_DW        5L-DW-Y-DY-DY-Y-DW      Five Lanes - (Four Lane Road) with Center Turnout but without White Edge Lines
             */

            switch (utils.roadStyle)
            {
                case "5L-W-DW-Y-DY-DY-Y-DW-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, 2.5 * utils.laneWidth
                                + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2.5 * utils.laneWidth) 
                                + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, 2.5 * utils.laneWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Left MUTCD-LINE-DASHED-WHITE
                        utils.OffsetRoadLine(poly, 1.5 * utils.laneWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) - 0.66, "MUTCD-LINE-DASHED-YELLOW");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) - 0.66), "MUTCD-LINE-DASHED-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right MUTCD-LINE-DASHED-WHITE
                        utils.OffsetRoadLine(poly, -(1.5 * utils.laneWidth), "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(2.5 * utils.laneWidth), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "5L-DW-Y-DY-DY-Y-DW ":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, 2.5 * utils.laneWidth
                                + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(2.5 * utils.laneWidth)
                                + utils.rightShoulderWidth, "T-EOP");
                        }

                        // Left MUTCD-LINE-DASHED-WHITE
                        utils.OffsetRoadLine(poly, 1.5 * utils.laneWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Turnout Centerlines Left
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, (utils.laneWidth / 2) - 0.66, "MUTCD-LINE-DASHED-YELLOW");

                        // Turnout Centerlines Right
                        utils.OffsetRoadLine(poly, -((utils.laneWidth / 2) - 0.66), "MUTCD-LINE-DASHED-YELLOW");
                        utils.OffsetRoadLine(poly, -(utils.laneWidth / 2), "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Right MUTCD-LINE-DASHED-WHITE
                        utils.OffsetRoadLine(poly, -(1.5 * utils.laneWidth), "MUTCD-LINE-DASHED-WHITE");
                        break;
                    }
            }
        } // End Function FiveLaneRoads

        // Six Lane Roads
        public static void SixLaneRoads(Polyline poly)
        {
            /* Six Lane Styles
             * Button Name          roadStyle
             * SixL_W_NO_MED_W      6L-W-NO-MED-W   Six Lanes - No Median, 3 Lanes on each side
             * SixL_NO_MED_NO_W     6L-NO-MED-NO-W  Six Lanes - No Median, 3 Lanes on each side, No White Edge Lines
             * SixL_W_MED_W         6L-W-MED-W      Six Lanes - Divided Highway, 3 Lanes on each side
             * SixL_MED_NO_W        6L-MED-NO-W     Six Lanes - Divided Highway, 3 Lanes each side, No White Edge Lines
             */

            switch (utils.roadStyle)
            {
                case "6L-W-NO-MED-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (3 * utils.laneWidth) +
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(3 * utils.laneWidth)
                                + -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, 3 * utils.laneWidth + 0.33, "MUTCD-LINE-SOLID-WHITE");

                        // Lane Dividers, Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 0.33, "MUTCD-LINE-DASHED-WHITE");

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Dividers, Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth + 0.33), "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, -((2 * utils.laneWidth) + 0.33), "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -((3 * utils.laneWidth) + 0.33), "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }
                case "6L-NO-MED-NO-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (3 * utils.laneWidth) +
                                0.33 + utils.leftShoulderWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(3 * utils.laneWidth)
                                + -0.33 + utils.rightShoulderWidth, "T-EOP");
                        }

                        // Lane Dividers, Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.33, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 0.33, "MUTCD-LINE-DASHED-WHITE");

                        // Centerline = Double Yellow Polylines
                        utils.OffsetRoadLine(poly, 0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");
                        utils.OffsetRoadLine(poly, -0.33, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Dividers, Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth + 0.33), "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, -((2 * utils.laneWidth) + 0.33), "MUTCD-LINE-DASHED-WHITE");
                        break;
                    }

                // Divided Highway or Street with Median
                case "6L-W-MED-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (3 * utils.laneWidth) + utils.leftShoulderWidth
                                + 0.5 + utils.leftMedianWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(3 * utils.laneWidth) + utils.rightShoulderWidth
                                + -0.5 + utils.rightMedianWidth, "T-EOP");
                        }
                        // Left MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, (3 * utils.laneWidth) + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-SOLID-WHITE");

                        // Lane Dividers, Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Left Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, 0.5 + utils.leftMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Median EOP's
                        utils.OffsetRoadLine(poly, utils.leftMedianWidth, "T-EOP");
                        utils.OffsetRoadLine(poly, utils.rightMedianWidth, "T-EOP");

                        // Right Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, -0.5 + utils.rightMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Dividers, Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, -((2 * utils.laneWidth)) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Right MUTCD-LINE-SOLID-WHITE
                        utils.OffsetRoadLine(poly, -(3 * utils.laneWidth) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-SOLID-WHITE");
                        break;
                    }

                case "6L-MED-NO-W":
                    {
                        // EOP
                        if (utils.drawEOP)
                        {
                            utils.OffsetRoadLine(poly, (3 * utils.laneWidth) + utils.leftShoulderWidth
                                + 0.5 + utils.leftMedianWidth, "T-EOP");
                            utils.OffsetRoadLine(poly, -(3 * utils.laneWidth) + utils.rightShoulderWidth
                                + -0.5 + utils.rightMedianWidth, "T-EOP");
                        }

                        // Lane Dividers, Left
                        utils.OffsetRoadLine(poly, utils.laneWidth + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, (2 * utils.laneWidth) + 0.5 +
                            utils.leftMedianWidth, "MUTCD-LINE-DASHED-WHITE");

                        // Left Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, 0.5 + utils.leftMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Median EOP's
                        utils.OffsetRoadLine(poly, utils.leftMedianWidth, "T-EOP");
                        utils.OffsetRoadLine(poly, utils.rightMedianWidth, "T-EOP");

                        // Right Median Yellow Painted Line
                        utils.OffsetRoadLine(poly, -0.5 + utils.rightMedianWidth, "MUTCD-LINE-SOLID-DOUBLE-YELLOW");

                        // Lane Dividers, Right
                        utils.OffsetRoadLine(poly, -(utils.laneWidth) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        utils.OffsetRoadLine(poly, -((2 * utils.laneWidth)) + -0.5 +
                            utils.rightMedianWidth, "MUTCD-LINE-DASHED-WHITE");
                        break;
                    }
            }
        } // End Function SixLaneRoads

    } // End class
} // End namespace
