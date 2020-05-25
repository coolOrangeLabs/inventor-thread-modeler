using System;
using Inventor;
using ThreadModeler.Addin;
using ThreadModeler.Utilities;

namespace ThreadModeler.Commands
{
    class HelpCtrlCmd : AdnButtonCommandBase
    {
        public HelpCtrlCmd(Application application) : base(application)
        {
        }

        public override string DisplayName
        {
            get { return "Help"; }
        }

        public override string InternalName
        {
            get { return "coolOrange.ThreadModeler.HelpCtrlCmd"; }
        }

        public override CommandTypesEnum Classification
        {
            get { return CommandTypesEnum.kEditMaskCmdType; }
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
            get { return "Displays threadModeler help"; }
        }

        public override string ToolTipText
        {
            get { return "Displays threadModeler help"; }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get { return ButtonDisplayEnum.kDisplayTextInLearningMode; }
        }

        public override string StandardIconName
        {
            get
            {
                return "ThreadModeler.resources.Help.ico";
            }
        }

        public override string LargeIconName
        {
            get
            {
                return "ThreadModeler.resources.Help.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
            System.Diagnostics.Process.Start("http://wiki.coolorange.com/display/threadModeler");
            Terminate();
        }

        protected override void OnHelp(NameValueMap context)
        {
        }
    }
}
