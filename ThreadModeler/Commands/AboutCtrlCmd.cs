using System;
using System.Reflection;
using Inventor;
using ThreadModeler.Addin;
using ThreadModeler.Utilities;

namespace ThreadModeler.Commands
{
    class AboutCtrlCmd : AdnButtonCommandBase
    {

        public AboutCtrlCmd(Application application) : base(application)
        {
        }

        public override string DisplayName
        {
            get { return "About"; }
        }

        public override string InternalName
        {
            get { return "coolOrange.ThreadModeler.AboutCtrlCmd"; }
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
            get { return "Displays threadModeler Info"; }
        }

        public override string ToolTipText
        {
            get { return "Displays threadModeler Info"; }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get { return ButtonDisplayEnum.kDisplayTextInLearningMode; }
        }

        public override string StandardIconName
        {
            get
            {
                return "ThreadModeler.resources.about.ico";
            }
        }

        public override string LargeIconName
        {
            get
            {
                return "ThreadModeler.resources.about.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
			var frmSplashAbout = new FrmSplash()
	        {
		        lblInfo = { Text = "Free License" },
		        versionlbl = { Text = "2019" },
		        buildlbl = { Text = Assembly.GetExecutingAssembly().GetName().Version.ToString() },
		        BackgroundImage = resources.threadModeler
	        };
	        RegisterCommandForm(frmSplashAbout, true);
        }

        protected override void OnHelp(NameValueMap context)
        {
        }
    }
}
