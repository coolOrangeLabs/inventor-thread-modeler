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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Inventor;

namespace ThreadModeler.Utilities
{
    abstract public class AdnButtonCommandBase : AdnCommand
    {
        protected Icon _standardIcon = null;
        protected Icon _largeIcon = null;

        protected ButtonDefinition _controlDef;

        private bool _bIsRunning;
        private bool _bNotifyExecWhenRunning;

        public Form CommandForm
        {
            get;
            internal set;
        }

        public bool DisplayFormModal
        {
            get;
            internal set;
        }

        public AdnInteractionManager InteractionManager
        {
            get;
            internal set;
        }

        protected AdnButtonCommandBase(Inventor.Application Application) :
            base(Application)
        {
            _bIsRunning = false;

            _bNotifyExecWhenRunning = false;

            CommandForm = null;

            InteractionManager = new AdnInteractionManager(Application);

            InteractionManager.OnTerminateEvent +=
                new AdnInteractionManager.OnTerminateHandler(Handle_OnTerminateEvent);
        }

        protected override void CreateControl()
        {
            CommandManager commandMgr = Application.CommandManager;

            //Create IPictureDisp from Icon resources
            stdole.IPictureDisp standardIconPict =
                (StandardIcon != null ? PictureDispConverter.ToIPictureDisp(StandardIcon) : null);

            stdole.IPictureDisp largeIconPict =
                (LargeIcon != null ? PictureDispConverter.ToIPictureDisp(LargeIcon) : null);

            _controlDef = commandMgr.ControlDefinitions.AddButtonDefinition(
                DisplayName,
                InternalName,
                Classification,
                ClientId,
                Description,
                ToolTipText,
                standardIconPict, 
                largeIconPict,
                ButtonDisplay);

            ControlDefinition = _controlDef as ControlDefinition;

            _controlDef.OnExecute +=
                new ButtonDefinitionSink_OnExecuteEventHandler(Handle_ButtonDefinition_OnExecute);

            _controlDef.OnHelp +=
                new ButtonDefinitionSink_OnHelpEventHandler(Handle_ButtonDefinition_OnHelp);
        }

        #region IconMembers

        public virtual string StandardIconName
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual string LargeIconName
        {
            get
            {
                return string.Empty;
            }
        }

        protected Icon StandardIcon
        {
            get
            {
                try
                {
                    if ((_standardIcon == null) && (!string.IsNullOrEmpty(StandardIconName)))
                    {
                        _standardIcon = GetIcon(StandardIconName, new Size(16, 16));
                    }

                    return _standardIcon;
                }
                catch
                {
                    return null;
                }
            }
        }

        protected Icon LargeIcon
        {
            get
            {
                try
                {
                    if ((_largeIcon == null) && (!string.IsNullOrEmpty(LargeIconName)))
                    {
                        _largeIcon = GetIcon(LargeIconName, new Size(32, 32));
                    }

                    return _largeIcon;
                }
                catch
                {
                    return null;
                }
            }
        }

        private Icon GetIcon(string name, Size size)
        {
            Stream resourceStream = ResAssembly.GetManifestResourceStream(name);
            return new Icon(resourceStream, size);
        }

        #endregion

        #region FormMembers

        protected void RegisterCommandForm(Form form, bool modal)
        {
            CommandForm = form;

            DisplayFormModal = modal;

            CommandForm.KeyPreview = true;

            CommandForm.FormClosed += new FormClosedEventHandler(CommandMainForm_FormClosed);

            CommandForm.KeyDown += new KeyEventHandler(CommandMainForm_KeyDown);
        }

        private void DisplayForm()
        {
            if (CommandForm != null)
            {
                if (CommandForm.Visible)
                {
                    CommandForm.Focus();
                }
                else
                {
                    if (DisplayFormModal)
                    {
                        CommandForm.ShowDialog(new WindowWrapper((IntPtr)Application.MainFrameHWND));
                    }
                    else
                    {
                        CommandForm.Show(new WindowWrapper((IntPtr)Application.MainFrameHWND));
                    }
                }
            }
        }

        private void CommandMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_bIsRunning)
                Terminate();
        }

        private void CommandMainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                CommandForm.Close();
            }
        }

        #endregion

        protected virtual void Terminate()
        {
            _bIsRunning = false;
            _controlDef.Pressed = false;

            if (CommandForm != null)
                CommandForm.Close();

            CommandForm = null;

            InteractionManager.Terminate();
        }

        private void Handle_OnTerminateEvent(object o, AdnInteractionManager.OnTerminateEventArgs e)
        {
            Terminate();
        }

        abstract protected void OnExecute(NameValueMap context);
        abstract protected void OnHelp(NameValueMap context);

        private void Handle_ButtonDefinition_OnExecute(NameValueMap context)
        {
            try
            {
                if (!_bIsRunning)
                {
                    _bIsRunning = true;
                    _controlDef.Pressed = true;
                    InteractionManager.Initialize();

                    OnExecute(context);
                }
                else if (_bNotifyExecWhenRunning)
                    OnExecute(context);

                DisplayForm();
            }
            catch
            {
                Terminate();
            }
        }

        private void Handle_ButtonDefinition_OnHelp(
            NameValueMap context,
            out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventHandled;

            try
            {
                OnHelp(context);
            }
            catch
            {
            }
        }
    }
}
