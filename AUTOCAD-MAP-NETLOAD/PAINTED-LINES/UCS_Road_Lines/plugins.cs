// (C) Copyright 2016 by Steven H. Brubaker

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

[assembly: ExtensionApplication(typeof(UCS_Road_Lines.ACAD_Plugin))]

namespace UCS_Road_Lines
{ 
    public class ACAD_Plugin : IExtensionApplication
    {
        // Intialize AutoCAD drawing OnLoad of .NET DLL
        void IExtensionApplication.Initialize()
        {

        }

        void IExtensionApplication.Terminate()
        {
            
        }

    }

}
