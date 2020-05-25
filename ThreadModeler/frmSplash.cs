using System;
using System.Windows.Forms;

namespace ThreadModeler
{
	public partial class FrmSplash : Form
	{


		public FrmSplash()
		{
			InitializeComponent();
		}

		public void CloseDialog()
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(Dispose));
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape && closeBtn.Visible)
			{
				Exit();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void closeBtn_Click(object sender, EventArgs e)
		{
			Exit();
		}

		private void Exit()
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.coolorange.com");
        }
	}
}
