// (C) Copyright 2014-2018 by Steven H. Brubaker

using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

// This line is not mandatory, but improves loading performances
[assembly: ExtensionApplication(typeof(Abbreviate_Map_Street_Names.MyPlugin))]

namespace Abbreviate_Map_Street_Names
{
    public class MyPlugin : IExtensionApplication
    {

        void IExtensionApplication.Initialize()
        {
            // No input, yet...
        }

        void IExtensionApplication.Terminate()
        {
            // No input, yet...
        }

    }

}
