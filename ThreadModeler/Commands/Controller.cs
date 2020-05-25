////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2011 - ADN/Developer Technical Services
//
// This software is provided as is, without any warranty that it will work. You choose to use this tool at your own risk.
// Neither Autodesk nor the author Philippe Leefsma can be taken as responsible for any damage this tool can cause to 
// your data. Please always make a back up of your data prior to use this tool, as it will modify the documents involved 
// in the feature transformation.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Inventor;
using ThreadModeler.Utilities;

namespace ThreadModeler.Commands
{
    /////////////////////////////////////////////////////////////////
    // ThreadModeler control Dialog
    //
    /////////////////////////////////////////////////////////////////
    public partial class Controller : Form
    {
        /////////////////////////////////////////////////////////////
        // Members
        //
        /////////////////////////////////////////////////////////////
        private double _extraPitch;

        private bool _selecSetPopulated;

        private bool _extraPitchValid;

        private Inventor.Application _Application;

        private PartDocument _Document;

        private ButtonDefinition _btnDef;

        private AdnInteractionManager _InteractionManager;

        private static string _ThreadTemplatePath = string.Empty;
        private bool _cleaned;


        /////////////////////////////////////////////////////////////
        // Constructor
        //
        /////////////////////////////////////////////////////////////
        public Controller(Inventor.Application Application,
            AdnInteractionManager InteractionManager)
        {
            InitializeComponent();

            bOk.Enabled = false;

            tbTemplate.Text =
                (_ThreadTemplatePath == string.Empty ?
                    FindThreadTemplate() : _ThreadTemplatePath);

            tbExtraPitch.Text = "0.10";

            tbExtraPitch.TextChanged +=
                new EventHandler(this.tbExtraPitch_TextChanged);

            lvFeatures.Items.Clear();

            _Application = Application;

            _Document = _Application.ActiveEditDocument as PartDocument;

            _selecSetPopulated = false;

            _extraPitch = 0.1;

            _extraPitchValid = true;

            _InteractionManager = InteractionManager;

            //Event Handlers Init

            this.tbTemplate.DoubleClick +=
                new EventHandler(tbTemplate_DoubleClick);

            _InteractionManager.SelectEvents.OnSelect +=
                new SelectEventsSink_OnSelectEventHandler(
                    SelectEvents_OnSelect);

            _InteractionManager.SelectEvents.OnUnSelect +=
                new SelectEventsSink_OnUnSelectEventHandler(
                    SelectEvents_OnUnSelect);
            this.FormClosing += Controller_FormClosing;
        }

        void Controller_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_cleaned)
                CleanUp();
        }

        /////////////////////////////////////////////////////////////
        // Use: OnSelect ThreadFeature Handler.
        //
        /////////////////////////////////////////////////////////////
        void SelectEvents_OnSelect(
            ObjectsEnumerator JustSelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            Point ModelPosition,
            Point2d ViewPosition,
            Inventor.View View)
        {
            foreach (System.Object obj in JustSelectedEntities)
            {
                PartFeature feature = obj as PartFeature;

                ThreadFeature thread = obj as ThreadFeature;

                ThreadInfo threadInfo = thread.ThreadInfo;

                Face threadedFace = thread.ThreadedFace[1];


                if (feature.Suppressed)
                    continue;

                if (thread.ThreadInfoType == ThreadTypeEnum.kTaperedThread &&
                    threadedFace.SurfaceType != SurfaceTypeEnum.kConeSurface)
                {
                    DialogResult res = MessageBox.Show(
                        "Threaded face surface type is not cone surface but it is applied" +
                        System.Environment.NewLine +
                        "with tapered thread. ThreadModeler cannot modelize this thread.",
                        "Invalid Surface Type",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    continue;
                }

                string iMateName = string.Empty;

                if (Toolkit.HasiMate(threadedFace,
                    out iMateName))
                {
                    DialogResult res = MessageBox.Show(
                        "Threaded face or one of its edge has" +
                        " iMate associated to it." +
                        System.Environment.NewLine +
                        "Please delete iMate " + iMateName  +
                        " before modelizing this thread.",
                        "Invalid iMate",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    continue;
                }

                double pitch = ThreadWorker.GetThreadPitch(threadInfo);

                string pitchStr =
                    ThreadWorker.GetThreadPitchStr(threadInfo,
                        (Document)_Document);

                string minStr =
                    _Document.UnitsOfMeasure.GetStringFromValue(
                        ThreadWorker.ThresholdPitchCm,
                        UnitsTypeEnum.kDefaultDisplayLengthUnits);

                if (pitch < ThreadWorker.ThresholdPitchCm)
                {
                    DialogResult res = MessageBox.Show(
                       "Selected thread pitch " +
                       "is too small (" + pitchStr + ")." +
                       System.Environment.NewLine +
                       "The minimum thread pitch that can " +
                       "be modelized is " + minStr + " .",
                       "Invalid Thread Pitch",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);

                    continue;
                }

                ListViewItem item =
                    lvFeatures.Items.Add(feature.Name);

                item.Tag = feature;

                item.SubItems.Add(pitchStr);

                item.SubItems.Add(ThreadWorker.GetThreadTypeStr(
                    feature));

                item.SubItems.Add(
                    ThreadWorker.GetThreadedFaceTypeStr(
                        threadedFace));
            }

            _selecSetPopulated = (lvFeatures.Items.Count != 0);

            bOk.Enabled = ValidateOkButton();
        }

        /////////////////////////////////////////////////////////////
        // use: OnUnselect ThreadFeature Handler.
        //
        /////////////////////////////////////////////////////////////
        void SelectEvents_OnUnSelect(
            ObjectsEnumerator UnSelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            Point ModelPosition,
            Point2d ViewPosition,
            Inventor.View View)
        {
            foreach (System.Object obj in UnSelectedEntities)
            {
                foreach (ListViewItem item in lvFeatures.Items)
                {
                    if (item.Tag == obj)
                    {
                        item.Remove();
                        break;
                    }
                }
            }

            if (_InteractionManager.SelectedEntities.Count == 0)
                bOk.Enabled = false;
        }

        /////////////////////////////////////////////////////////////
        // Use: Ok Button clicked Handler
        //
        /////////////////////////////////////////////////////////////
        private void bOk_Click(object sender, EventArgs e)
        {
            bool silentOp = _Application.SilentOperation;

            _Application.SilentOperation = true;

            PartDocument template = _Application.Documents.Open(
                _ThreadTemplatePath, false) as PartDocument;

            _Application.SilentOperation = silentOp;

            if (!ValidateTemplateParameters(template))
            {
                DialogResult res = MessageBox.Show(
                    "Missing sketch parameter in template file!",
                    "Invalid Template",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                bOk.Enabled = false;

                _ThreadTemplatePath = string.Empty;

                tbTemplate.Text = string.Empty;

                if(template != _Application.ActiveDocument)
                    template.Close(true);

                return;
            }

            List<ThreadFeature> threads = new List<ThreadFeature>();

            foreach (System.Object selectedObj in
                _InteractionManager.SelectedEntities)
            {
                ThreadFeature thread = selectedObj as ThreadFeature;

                if (thread.Suppressed)
                    continue;

                threads.Add(thread);
            }

            PlanarSketch templateSketch =
                template.ComponentDefinition.Sketches[1];



            if (!ThreadWorker.ModelizeThreads(
                _Document,
                templateSketch,
                threads,
                _extraPitch))
            {
                DialogResult res = MessageBox.Show(
                    "Failed to create CoilFeature... " +
                    "Try with a bigger Pitch Offset value",
                    "Modelization Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);

                switch (res)
                {
                    case DialogResult.OK:
                        //template.Close(true);
                        return;
                    default:
                        break;
                }
            }

            // Problem: closing the template doc here 
            // will empty the undo stack (as designed)...

            //template.Close(true);
            if (!_cleaned)
                CleanUp();
            _InteractionManager.Terminate();
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns True if template document contains required
        //      parameters to modelize thread.
        /////////////////////////////////////////////////////////////
        private bool ValidateTemplateParameters(PartDocument doc)
        {
            if(!Toolkit.FindParameter(doc, "Pitch"))
                return false;

            if (!Toolkit.FindParameter(doc, "ThreadOffset"))
                return false;

            if (!Toolkit.FindParameter(doc, "MajorRadius"))
                return false;

            if (!Toolkit.FindParameter(doc, "MinorRadius"))
                return false;

            return true;
        }

        /////////////////////////////////////////////////////////////
        // Use: Cancel Button clicked Handler
        //
        /////////////////////////////////////////////////////////////
        private void bCancel_Click(object sender, EventArgs e)
        {
            if (!_cleaned)
                CleanUp();
            _InteractionManager.Terminate();
        }

        /////////////////////////////////////////////////////////////
        // Use: Select template handler.
        //
        /////////////////////////////////////////////////////////////
        private void OnSelectTemplate()
        {
            string path = GetThreadTemplatePath();

            if (path != string.Empty)
            {
                _ThreadTemplatePath = path;

                tbTemplate.Text = path;
            }

            bOk.Enabled = ValidateOkButton();

        }

        private void bSelect_Click(object sender, EventArgs e)
        {
            OnSelectTemplate();
        }

        void tbTemplate_DoubleClick(object sender, EventArgs e)
        {
            OnSelectTemplate();
        }

        /////////////////////////////////////////////////////////////
        // Use: Prompts user to locate thread sketch template, if
        //      not found automatically.
        /////////////////////////////////////////////////////////////
        private string GetThreadTemplatePath()
        {
            try
            {
                string pathInit =
                    Assembly.GetExecutingAssembly().Location;

                FileInfo fi = new FileInfo(pathInit);

                Inventor.FileDialog fileDlg = null;
                _Application.CreateFileDialog(out fileDlg);

                fileDlg.Filter = "Inventor Parts (*.ipt)|*.ipt";
                fileDlg.FilterIndex = 1;
                fileDlg.DialogTitle = "Select Thread Template";
                fileDlg.InitialDirectory =
                    fi.DirectoryName + "\\Thread Templates";
                fileDlg.FileName = "BSW Template.ipt";
                fileDlg.MultiSelectEnabled = false;
                fileDlg.OptionsEnabled = false;
                fileDlg.CancelError = true;
                fileDlg.SuppressResolutionWarnings = true;

                fileDlg.ShowOpen();

                return fileDlg.FileName;
            }
            catch
            {
                return string.Empty;
            }
        }

        /////////////////////////////////////////////////////////////
        // Use: Tries to locate thread sketch template in addin
        //      install directory.
        /////////////////////////////////////////////////////////////
        private string FindThreadTemplate()
        {
            string pathInit =
                Assembly.GetExecutingAssembly().Location;

            FileInfo fi = new FileInfo(pathInit);

            string path =
                fi.DirectoryName +
                "\\Thread Templates\\BSW Template.ipt";

            if (System.IO.File.Exists(path))
            {
                _ThreadTemplatePath = path;
                return path;
            }

            return string.Empty;
        }

        /////////////////////////////////////////////////////////////
        // Use: Scroll control handler.
        //
        /////////////////////////////////////////////////////////////
        private void OnScroll(object sender,
            ScrollEventArgs e)
        {
            _extraPitch = (1000 - scrollPitch.Value) * 0.01;

            tbExtraPitch.Text = _extraPitch.ToString("0.00");
        }

        /////////////////////////////////////////////////////////////
        // Use: Pitch Offset text changed Handler.
        //
        /////////////////////////////////////////////////////////////
        private void tbExtraPitch_TextChanged(object sender,
            EventArgs e)
        {
            try
            {
                double extraPitch = double.Parse(tbExtraPitch.Text);

                if (extraPitch < 0.1 || extraPitch > 10.0)
                {
                    tbExtraPitch.ForeColor = System.Drawing.Color.Red;
                    _extraPitchValid = false;
                    bOk.Enabled = ValidateOkButton();
                    return;
                }

                _extraPitch = extraPitch;

                scrollPitch.Value = (int)(1000.0 - _extraPitch * 100.0);

                tbExtraPitch.ForeColor = System.Drawing.Color.Black;
                _extraPitchValid = true;

                bOk.Enabled = ValidateOkButton();
            }
            catch
            {
                tbExtraPitch.ForeColor = System.Drawing.Color.Red;
                _extraPitchValid = false;

                bOk.Enabled = ValidateOkButton();
            }
        }

        /////////////////////////////////////////////////////////////
        // Use: Returns True if Ok Button can be enabled.
        //
        /////////////////////////////////////////////////////////////
        private bool ValidateOkButton()
        {
            return (_selecSetPopulated &&
                (_ThreadTemplatePath != string.Empty) &&
                _extraPitchValid);
        }

        private void CleanUp()
        {
            _InteractionManager.SelectEvents.OnSelect -= new SelectEventsSink_OnSelectEventHandler(
                    SelectEvents_OnSelect);

            _InteractionManager.SelectEvents.OnUnSelect -=
                new SelectEventsSink_OnUnSelectEventHandler(
                    SelectEvents_OnUnSelect);
            _cleaned = true;
        }
    }
}
