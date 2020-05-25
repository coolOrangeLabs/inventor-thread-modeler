namespace ThreadModeler.Commands
{
    partial class Controller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (_InteractionManager != null && _InteractionManager.SelectEvents != null)
            {
                _InteractionManager.SelectEvents.OnSelect -= SelectEvents_OnSelect;

                _InteractionManager.SelectEvents.OnUnSelect -= SelectEvents_OnUnSelect;
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Thread Feature 1",
            "Pitch1",
            "Standard",
            "Exterior"}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Controller));
            this.bOk = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.bSelect = new System.Windows.Forms.Button();
            this.gbTemplate = new System.Windows.Forms.GroupBox();
            this.tbTemplate = new System.Windows.Forms.TextBox();
            this.lvFeatures = new System.Windows.Forms.ListView();
            this.hThread = new System.Windows.Forms.ColumnHeader();
            this.hPitch = new System.Windows.Forms.ColumnHeader();
            this.hType = new System.Windows.Forms.ColumnHeader();
            this.hFace = new System.Windows.Forms.ColumnHeader();
            this.gbPitchOffset = new System.Windows.Forms.GroupBox();
            this.lbExtraPitch2 = new System.Windows.Forms.Label();
            this.tbExtraPitch = new System.Windows.Forms.TextBox();
            this.lbExtraPitch = new System.Windows.Forms.Label();
            this.scrollPitch = new System.Windows.Forms.VScrollBar();
            this.gbTemplate.SuspendLayout();
            this.gbPitchOffset.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(209, 239);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 0;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(290, 239);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bSelect
            // 
            this.bSelect.Location = new System.Drawing.Point(314, 19);
            this.bSelect.Name = "bSelect";
            this.bSelect.Size = new System.Drawing.Size(32, 20);
            this.bSelect.TabIndex = 2;
            this.bSelect.Text = "...";
            this.bSelect.UseVisualStyleBackColor = true;
            this.bSelect.Click += new System.EventHandler(this.bSelect_Click);
            // 
            // gbTemplate
            // 
            this.gbTemplate.Controls.Add(this.tbTemplate);
            this.gbTemplate.Controls.Add(this.bSelect);
            this.gbTemplate.Location = new System.Drawing.Point(12, 12);
            this.gbTemplate.Name = "gbTemplate";
            this.gbTemplate.Size = new System.Drawing.Size(353, 51);
            this.gbTemplate.TabIndex = 3;
            this.gbTemplate.TabStop = false;
            this.gbTemplate.Text = "Thread Sketch Template";
            // 
            // tbTemplate
            // 
            this.tbTemplate.Location = new System.Drawing.Point(6, 19);
            this.tbTemplate.Name = "tbTemplate";
            this.tbTemplate.ReadOnly = true;
            this.tbTemplate.Size = new System.Drawing.Size(302, 20);
            this.tbTemplate.TabIndex = 3;
            // 
            // lvFeatures
            // 
            this.lvFeatures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hThread,
            this.hPitch,
            this.hType,
            this.hFace});
            this.lvFeatures.FullRowSelect = true;
            this.lvFeatures.GridLines = true;
            this.lvFeatures.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lvFeatures.Location = new System.Drawing.Point(12, 128);
            this.lvFeatures.MultiSelect = false;
            this.lvFeatures.Name = "lvFeatures";
            this.lvFeatures.Size = new System.Drawing.Size(353, 99);
            this.lvFeatures.TabIndex = 4;
            this.lvFeatures.UseCompatibleStateImageBehavior = false;
            this.lvFeatures.View = System.Windows.Forms.View.Details;
            // 
            // hThread
            // 
            this.hThread.Text = "Selected Threads";
            this.hThread.Width = 110;
            // 
            // hPitch
            // 
            this.hPitch.Text = "Pitch";
            this.hPitch.Width = 77;
            // 
            // hType
            // 
            this.hType.Text = "Thread Type";
            this.hType.Width = 82;
            // 
            // hFace
            // 
            this.hFace.Text = "Face Type";
            this.hFace.Width = 80;
            // 
            // gbPitchOffset
            // 
            this.gbPitchOffset.Controls.Add(this.lbExtraPitch2);
            this.gbPitchOffset.Controls.Add(this.tbExtraPitch);
            this.gbPitchOffset.Controls.Add(this.lbExtraPitch);
            this.gbPitchOffset.Controls.Add(this.scrollPitch);
            this.gbPitchOffset.Location = new System.Drawing.Point(12, 70);
            this.gbPitchOffset.Name = "gbPitchOffset";
            this.gbPitchOffset.Size = new System.Drawing.Size(353, 47);
            this.gbPitchOffset.TabIndex = 5;
            this.gbPitchOffset.TabStop = false;
            this.gbPitchOffset.Text = "Pitch Offset";
            // 
            // lbExtraPitch2
            // 
            this.lbExtraPitch2.AutoSize = true;
            this.lbExtraPitch2.Location = new System.Drawing.Point(133, 23);
            this.lbExtraPitch2.Name = "lbExtraPitch2";
            this.lbExtraPitch2.Size = new System.Drawing.Size(15, 13);
            this.lbExtraPitch2.TabIndex = 3;
            this.lbExtraPitch2.Text = "%";
            // 
            // tbExtraPitch
            // 
            this.tbExtraPitch.Location = new System.Drawing.Point(87, 20);
            this.tbExtraPitch.Name = "tbExtraPitch";
            this.tbExtraPitch.Size = new System.Drawing.Size(43, 20);
            this.tbExtraPitch.TabIndex = 2;
            this.tbExtraPitch.Text = "0.1";
            // 
            // lbExtraPitch
            // 
            this.lbExtraPitch.AutoSize = true;
            this.lbExtraPitch.Location = new System.Drawing.Point(6, 22);
            this.lbExtraPitch.Name = "lbExtraPitch";
            this.lbExtraPitch.Size = new System.Drawing.Size(77, 13);
            this.lbExtraPitch.TabIndex = 1;
            this.lbExtraPitch.Text = "Thread Pitch +";
            // 
            // scrollPitch
            // 
            this.scrollPitch.LargeChange = 1;
            this.scrollPitch.Location = new System.Drawing.Point(165, 13);
            this.scrollPitch.Maximum = 990;
            this.scrollPitch.Name = "scrollPitch";
            this.scrollPitch.Size = new System.Drawing.Size(28, 28);
            this.scrollPitch.TabIndex = 0;
            this.scrollPitch.Value = 990;
            this.scrollPitch.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            // 
            // Controller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(377, 270);
            this.Controls.Add(this.gbPitchOffset);
            this.Controls.Add(this.lvFeatures);
            this.Controls.Add(this.gbTemplate);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Controller";
            this.Text = "Thread Modeler Control";
            this.gbTemplate.ResumeLayout(false);
            this.gbTemplate.PerformLayout();
            this.gbPitchOffset.ResumeLayout(false);
            this.gbPitchOffset.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bSelect;
        private System.Windows.Forms.GroupBox gbTemplate;
        private System.Windows.Forms.TextBox tbTemplate;
        private System.Windows.Forms.ListView lvFeatures;
        private System.Windows.Forms.ColumnHeader hThread;
        private System.Windows.Forms.ColumnHeader hPitch;
        private System.Windows.Forms.GroupBox gbPitchOffset;
        private System.Windows.Forms.Label lbExtraPitch;
        private System.Windows.Forms.VScrollBar scrollPitch;
        private System.Windows.Forms.ColumnHeader hType;
        private System.Windows.Forms.ColumnHeader hFace;
        private System.Windows.Forms.TextBox tbExtraPitch;
        private System.Windows.Forms.Label lbExtraPitch2;
    }
}