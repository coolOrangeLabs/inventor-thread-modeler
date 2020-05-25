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

using System.Reflection;
using Inventor;

namespace ThreadModeler.Utilities
{
    abstract public class AdnCommand
    {
        UserInputEvents _userInputEvents;

        // Static method to add new command, needs to be called
        // in add-in Activate typically
        public static void AddCommand(AdnCommand command)
        {
            command.ResAssembly = Assembly.GetExecutingAssembly();

            command.CreateControl();
        }

        public static void AddCommand(AdnCommand command, Assembly resAssembly)
        {
            command.ResAssembly = resAssembly;

            command.CreateControl();
        }

        abstract protected void CreateControl();

        protected AdnCommand(Inventor.Application Application)
        {
            this.Application = Application;

            _userInputEvents = Application.CommandManager.UserInputEvents;

            _userInputEvents.OnLinearMarkingMenu +=
                new UserInputEventsSink_OnLinearMarkingMenuEventHandler(
                    OnLinearMarkingMenu);

            _userInputEvents.OnRadialMarkingMenu +=
                new UserInputEventsSink_OnRadialMarkingMenuEventHandler(
                    OnRadialMarkingMenu);
        }

        protected Assembly ResAssembly
        {
            get;
            set;
        }

        public ControlDefinition ControlDefinition
        {
            get;
            internal set;
        }

        public Application Application
        {
            get;
            internal set;
        }

        public abstract string DisplayName
        {
            get;
        }

        public abstract string InternalName
        {
            get;
        }

        public abstract CommandTypesEnum Classification
        {
            get;
        }

        public abstract string ClientId
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract string ToolTipText
        {
            get;
        }

        public abstract ButtonDisplayEnum ButtonDisplay
        {
            get;
        }

        public void Remove()
        {
            OnRemove();

            if (ControlDefinition != null)
            {
                ControlDefinition.Delete();
            }
        }

        protected virtual void OnRemove()
        {

        }

        protected virtual void OnLinearMarkingMenu(
            ObjectsEnumerator SelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            CommandControls LinearMenu,
            NameValueMap AdditionalInfo)
        {

        }

        protected virtual void OnRadialMarkingMenu(
            ObjectsEnumerator SelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            RadialMarkingMenu RadialMenu,
            NameValueMap AdditionalInfo)
        {

        }
    }
}
