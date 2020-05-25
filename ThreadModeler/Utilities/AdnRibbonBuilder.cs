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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Inventor;

namespace ThreadModeler.Utilities
{
    public class AdnRibbonBuilder
    {
        private Inventor.Application _Application;
        private string _addinGuid;

        // Create Ribbon items based on a Stream generated from an XML description file 
        public static void CreateRibbon(Inventor.Application app, Type addinType, Stream stream)
        {
            new AdnRibbonBuilder(app, addinType.GUID.ToString("B")).DoCreateRibbon(stream);
        }

        // Create Ribbon items based on a Stream name, typically an embedded resource xml file
        public static void CreateRibbon(Inventor.Application app, Type addinType, string resourcename)
        {
            System.IO.Stream xmlres =
                addinType.Assembly.GetManifestResourceStream(resourcename);

            new AdnRibbonBuilder(app, addinType.GUID.ToString("B")).DoCreateRibbon(xmlres);
        }

        // Create Ribbon items based on an existing xml file on the disk
        public static void CreateRibbonFromFile(Inventor.Application app, Type addinType, string xmlfile)
        {
            new AdnRibbonBuilder(app, addinType.GUID.ToString("B")).DoCreateRibbon(xmlfile);
        }

        private AdnRibbonBuilder(Inventor.Application Application, string guid)
        {
            _Application = Application;
            _addinGuid = guid;
        }

        private void DoCreateRibbon(Stream stream)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(stream);
            ProcessDocument(xdoc);
        }

        private void DoCreateRibbon(string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fileName);
            ProcessDocument(xdoc);
        }

        private void ProcessDocument(XmlDocument xdoc)
        {
            XmlNodeList tabNodes = xdoc.SelectNodes("//RibbonTab");

            foreach (XmlNode tabNode in tabNodes)
            {
                ProcessTabNode(tabNode);
            }
        }

        private void ProcessTabNode(XmlNode node)
        {
            UserInterfaceManager uiManager = _Application.UserInterfaceManager;

            string[] ribbonNames = node.Attributes["ribbons"].Value.Split('|');
            string displayName = XmlUtilities.GetAttributeValue<string>(node, "displayName");
            string internalName = node.Attributes["internalName"].Value;
            string targetTab = XmlUtilities.GetAttributeValue<string>(node, "targetTab");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? contextual = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "contextual");

            foreach (Ribbon ribbon in uiManager.Ribbons)
            {
                if (ribbonNames.Contains(ribbon.InternalName) == false)
                {
                    continue;
                }

                RibbonTab tab = ribbon.RibbonTabs.OfType<RibbonTab>().SingleOrDefault(t => t.InternalName == internalName);

                if (tab == null)
                {
                    tab = ribbon.RibbonTabs.Add(
                        displayName,
                        internalName,
                        _addinGuid,
                        string.IsNullOrEmpty(targetTab) ? string.Empty : targetTab,
                        insertBefore.HasValue ? insertBefore.Value : false,
                        contextual.HasValue ? contextual.Value : false);
                }

                XmlNodeList panelNodes = node.SelectNodes("RibbonPanel");

                foreach (XmlNode panelNode in panelNodes)
                {
                    ProcessPanelNode(tab, panelNode);
                }
            }
        }

        private void ProcessPanelNode(RibbonTab tab, XmlNode node)
        {
            string displayName = XmlUtilities.GetAttributeValue<string>(node, "displayName");
            string internalName = node.Attributes["internalName"].Value;
            string targetPanel = XmlUtilities.GetAttributeValue<string>(node, "targetPanel");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");

            RibbonPanel panel = tab.RibbonPanels.OfType<RibbonPanel>().SingleOrDefault(p => p.InternalName == internalName);

            if (panel == null)
            {
                panel = tab.RibbonPanels.Add(
                    displayName,
                    internalName,
                    _addinGuid,
                    string.IsNullOrEmpty(targetPanel) ? string.Empty : targetPanel,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                Action<RibbonPanel, XmlNode> processMethod = null;

                switch (childNode.Name)
                {
                    case "Button":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessButtonNode);
                        break;

                    case "Separator":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessSeparatorNode);
                        break;

                    case "ButtonPopup":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessButtonPopupNode);
                        break;

                    case "Popup":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessPopupNode);
                        break;

                    case "ComboBox":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessComboBoxNode);
                        break;

                    case "Gallery":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessGalleryNode);
                        break;

                    case "Macro":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessMacroNode);
                        break;

                    case "SplitButton":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessSplitButtonNode);
                        break;

                    case "SplitButtonMRU":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessSplitButtonMRUNode);
                        break;

                    case "TogglePopup":
                        processMethod = new Action<RibbonPanel, XmlNode>(ProcessTogglePopupNode);
                        break;

                    default:
                        continue;
                }

                if (processMethod != null)
                {
                    try
                    {
                        processMethod.Invoke(panel, childNode);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void ProcessButtonNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ButtonDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            controls.AddButton(
                controlDef,
                useLargeIcon.HasValue ? useLargeIcon.Value : false,
                showText.HasValue ? showText.Value : true,
                string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                insertBefore.HasValue ? insertBefore.Value : false);
        }

        private void ProcessSeparatorNode(RibbonPanel panel, XmlNode node)
        {
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            controls.AddSeparator(
                string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                insertBefore.HasValue ? insertBefore.Value : false);
        }

        private void ProcessButtonPopupNode(RibbonPanel panel, XmlNode node)
        {
            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddButtonPopup(
                    buttons,
                    useLargeIcon.HasValue ? useLargeIcon.Value : false,
                    showText.HasValue ? showText.Value : true,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private void ProcessPopupNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ButtonDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddPopup(
                    controlDef,
                    buttons,
                    useLargeIcon.HasValue ? useLargeIcon.Value : false,
                    showText.HasValue ? showText.Value : true,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private void ProcessComboBoxNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ComboBoxDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ComboBoxDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            controls.AddComboBox(
                controlDef,
                string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                insertBefore.HasValue ? insertBefore.Value : false);

            XmlNodeList nodes = node.SelectNodes("ComboItem");

            foreach (XmlNode comboItemNode in nodes)
            {
                string comboItem = comboItemNode.Attributes["item"].Value;

                int? index = XmlUtilities.GetAttributeValueAsNullable<int>(comboItemNode, "index");

                controlDef.AddItem(
                    comboItem,
                    index.HasValue ? index.Value : 0);
            }
        }

        private void ProcessGalleryNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ButtonDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

            bool? displayAsCombo = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "displayAsCombo");
            int? width = XmlUtilities.GetAttributeValueAsNullable<int>(node, "width");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddGallery(
                    buttons,
                    displayAsCombo.HasValue ? displayAsCombo.Value : false,
                    width.HasValue ? width.Value : 15,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private void ProcessMacroNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            MacroControlDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as MacroControlDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            controls.AddMacro(
                controlDef,
                useLargeIcon.HasValue ? useLargeIcon.Value : false,
                showText.HasValue ? showText.Value : true,
                string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                insertBefore.HasValue ? insertBefore.Value : false);
        }

        private void ProcessSplitButtonNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ButtonDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddSplitButton(
                    controlDef,
                    buttons,
                    useLargeIcon.HasValue ? useLargeIcon.Value : false,
                    showText.HasValue ? showText.Value : true,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private void ProcessSplitButtonMRUNode(RibbonPanel panel, XmlNode node)
        {
            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddSplitButtonMRU(
                    buttons,
                    useLargeIcon.HasValue ? useLargeIcon.Value : false,
                    showText.HasValue ? showText.Value : true,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private void ProcessTogglePopupNode(RibbonPanel panel, XmlNode node)
        {
            string commandName = node.Attributes["internalName"].Value;

            ButtonDefinition controlDef =
                _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

            bool? useLargeIcon = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "useLargeIcon");
            bool? showText = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "showText");
            string targetControl = XmlUtilities.GetAttributeValue<string>(node, "targetControl");
            bool? insertBefore = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "insertBefore");
            bool? isInSlideout = XmlUtilities.GetAttributeValueAsNullable<bool>(node, "isInSlideout");

            bool slideout = isInSlideout.HasValue ? isInSlideout.Value : false;

            CommandControls controls = (slideout ? panel.SlideoutControls : panel.CommandControls);

            ObjectCollection buttons = GetButtons(node);

            if (buttons.Count > 0)
            {
                controls.AddTogglePopup(
                    controlDef,
                    buttons,
                    useLargeIcon.HasValue ? useLargeIcon.Value : false,
                    showText.HasValue ? showText.Value : true,
                    string.IsNullOrEmpty(targetControl) ? string.Empty : targetControl,
                    insertBefore.HasValue ? insertBefore.Value : false);
            }
        }

        private ObjectCollection GetButtons(XmlNode node)
        {
            ObjectCollection buttons = _Application.TransientObjects.CreateObjectCollection(Type.Missing);

            XmlNodeList nodes = node.SelectNodes("Button");

            foreach (XmlNode buttonNode in nodes)
            {
                string commandName = buttonNode.Attributes["internalName"].Value;

                ButtonDefinition buttonDef = _Application.CommandManager.ControlDefinitions[commandName] as ButtonDefinition;

                if (buttonDef != null)
                {
                    buttons.Add(buttonDef);
                }
            }

            return buttons;
        }
    }

    class XmlUtilities
    {
        public static T? GetAttributeValueAsNullable<T>(XmlNode node, string attributeName) where T : struct
        {
            XmlAttribute attr = node.Attributes.GetNamedItem(attributeName) as XmlAttribute;

            if (attr != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFrom(attr.Value);
            }

            return null;
        }

        public static T GetAttributeValue<T>(XmlNode node, string attributeName) where T : class
        {
            XmlAttribute attr = node.Attributes.GetNamedItem(attributeName) as XmlAttribute;

            if (attr != null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFrom(attr.Value);
            }

            return null;
        }
    }
}
