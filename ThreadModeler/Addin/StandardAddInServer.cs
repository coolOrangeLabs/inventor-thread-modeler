////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Jan Liska & Philippe Leefsma 2011 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ThreadModeler Inventor Add-in
//  
// Author: Philippe Leefsma
// Creation date: 11/7/2011 9:15:24 AM
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using ThreadModeler.Commands;
using ThreadModeler.Utilities;

namespace ThreadModeler.Addin
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [Guid("90f6009a-a2ed-400f-9cbd-cee55ad30d23"),ComVisible(true)]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        // Create a logger for use in this class
        private static log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        public StandardAddInServer()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            FileInfo fi = new FileInfo(thisAssembly.Location + ".log4net");
            log4net.GlobalContext.Properties["LogFileName"] = fi.DirectoryName + "\\Log\\threadModeler";
            log4net.Config.XmlConfigurator.Configure(fi);
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            log.Debug("Activating ThreadModeler Addin");
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            Type addinType = this.GetType();

            AdnInventorUtilities.Initialize(m_inventorApplication, addinType);

            Toolkit.Initialize(m_inventorApplication);
            ThreadWorker.Initialize(m_inventorApplication);
            
            AdnCommand.AddCommand(new ThreadModelerCmd(m_inventorApplication));

            AdnCommand.AddCommand(new AboutCtrlCmd(m_inventorApplication));

            AdnCommand.AddCommand(new HelpCtrlCmd(m_inventorApplication));
           
            // Only after all commands have been added,
            // load Ribbon UI from customized xml file.
            // Make sure "InternalName" of above commands is matching 
            // "internalName" tag described in xml of corresponding command.
            AdnRibbonBuilder.CreateRibbon(
                m_inventorApplication,
               addinType,
               "ThreadModeler.resources.ribbons.xml");
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            Marshal.ReleaseComObject(m_inventorApplication);
            m_inventorApplication = null;
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }
        #endregion
    }
}
