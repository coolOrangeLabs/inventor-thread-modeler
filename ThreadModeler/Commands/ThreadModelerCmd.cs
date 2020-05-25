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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ThreadModelerCmd Inventor Add-in Command
//  
// Author: Philippe Leefsma
// Creation date: 11/7/2011 9:20:55 AM
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Inventor;
using ThreadModeler.Addin;
using ThreadModeler.Utilities;

namespace ThreadModeler.Commands
{
    class ThreadModelerCmd : AdnButtonCommandBase
    {
        public ThreadModelerCmd(Inventor.Application Application) :
            base(Application)
        {

        }

        public override string DisplayName
        {
            get
            {
                return "threadModeler";
            }
        }

        public override string InternalName
        {
            get
            {
                return "coolOrange.ThreadModeler.ThreadModelerCmd";
            }
        }

        public override CommandTypesEnum Classification
        {
            get
            {
                return CommandTypesEnum.kEditMaskCmdType;
            }
        }

        public override string ClientId
        {
            get
            {
                Type t = typeof(StandardAddInServer);
                return t.GUID.ToString("B");
            }
        }

        public override string Description
        {
            get
            {
                return "Displays ThreadModeler main control";
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Displays ThreadModeler main control";
            }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get
            {
                return ButtonDisplayEnum.kDisplayTextInLearningMode;
            }
        }

        public override string StandardIconName
        {
            get
            {
                return "ThreadModeler.resources.threadModeler.ico";
            }
        }

        public override string LargeIconName
        {
            get
            {
                return "ThreadModeler.resources.threadModeler.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
            RegisterCommandForm(new Controller(Application, InteractionManager) , false);

            InteractionManager.AddPreSelectionFilter(
                ObjectTypeEnum.kThreadFeatureObject);

            InteractionManager.AddPreSelectionFilter(
                ObjectTypeEnum.kThreadFeatureProxyObject);
            
            // Start Interaction
            InteractionManager.Start("Select ThreadFeatures to modelize from the browser");
        }

        protected override void OnHelp(NameValueMap context)
        {

        }

        protected override void OnLinearMarkingMenu(
           ObjectsEnumerator SelectedEntities,
           SelectionDeviceEnum SelectionDevice,
           CommandControls LinearMenu,
           NameValueMap AdditionalInfo)
        {

        }

        protected override void OnRadialMarkingMenu(
            ObjectsEnumerator SelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            RadialMarkingMenu RadialMenu,
            NameValueMap AdditionalInfo)
        {

        }
    }
}
