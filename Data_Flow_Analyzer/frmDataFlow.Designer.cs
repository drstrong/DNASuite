namespace Data_Flow_Analyzer
{
    partial class frmDataFlow
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataFlow));
            this.folderBrowserInput = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonOutputDir = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgress = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgressBarFiles = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSelectData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRunProgram = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAdjustSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClearLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportLog = new System.Windows.Forms.ToolStripButton();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.textBoxOutDir = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOutputDir
            // 
            this.buttonOutputDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonOutputDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOutputDir.Location = new System.Drawing.Point(12, 28);
            this.buttonOutputDir.Name = "buttonOutputDir";
            this.buttonOutputDir.Size = new System.Drawing.Size(104, 27);
            this.buttonOutputDir.TabIndex = 2;
            this.buttonOutputDir.Text = "Output Directory:";
            this.buttonOutputDir.UseVisualStyleBackColor = true;
            this.buttonOutputDir.Click += new System.EventHandler(this.buttonOutputDir_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "DAT files | *.dat";
            this.openFileDialog1.Multiselect = true;
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.Location = new System.Drawing.Point(12, 61);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.ScrollAlwaysVisible = true;
            this.listBoxFiles.Size = new System.Drawing.Size(715, 199);
            this.listBoxFiles.TabIndex = 9;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgress,
            this.toolStripProgressBarFiles});
            this.toolStrip1.Location = new System.Drawing.Point(0, 423);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(737, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgress
            // 
            this.toolStripProgress.Name = "toolStripProgress";
            this.toolStripProgress.Size = new System.Drawing.Size(75, 22);
            this.toolStripProgress.Text = "Not Running";
            // 
            // toolStripProgressBarFiles
            // 
            this.toolStripProgressBarFiles.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBarFiles.Name = "toolStripProgressBarFiles";
            this.toolStripProgressBarFiles.Size = new System.Drawing.Size(600, 22);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSelectData,
            this.toolStripSeparator1,
            this.toolStripButtonRunProgram,
            this.toolStripButtonCancel,
            this.toolStripSeparator2,
            this.toolStripButtonAdjustSettings,
            this.toolStripSeparator3,
            this.toolStripButtonClearLog,
            this.toolStripButtonExportLog});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(737, 25);
            this.toolStrip2.TabIndex = 10;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonSelectData
            // 
            this.toolStripButtonSelectData.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSelectData.Image")));
            this.toolStripButtonSelectData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSelectData.Name = "toolStripButtonSelectData";
            this.toolStripButtonSelectData.Size = new System.Drawing.Size(111, 22);
            this.toolStripButtonSelectData.Text = "Select Data Files";
            this.toolStripButtonSelectData.ToolTipText = "Select the compressed data files which were received from the ship, ending in \".d" +
    "at\".";
            this.toolStripButtonSelectData.Click += new System.EventHandler(this.toolStripButtonSelectData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRunProgram
            // 
            this.toolStripButtonRunProgram.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRunProgram.Image")));
            this.toolStripButtonRunProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRunProgram.Name = "toolStripButtonRunProgram";
            this.toolStripButtonRunProgram.Size = new System.Drawing.Size(97, 22);
            this.toolStripButtonRunProgram.Text = "Run Program";
            this.toolStripButtonRunProgram.ToolTipText = "Run the program";
            this.toolStripButtonRunProgram.Click += new System.EventHandler(this.toolStripButtonRunProgram_Click);
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.Enabled = false;
            this.toolStripButtonCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCancel.Image")));
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(63, 22);
            this.toolStripButtonCancel.Text = "Cancel";
            this.toolStripButtonCancel.ToolTipText = "This button will cancel the current run. However, cancellation is only checked so" +
    " often,\r\nso it may not take effect immediately.";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAdjustSettings
            // 
            this.toolStripButtonAdjustSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdjustSettings.Image")));
            this.toolStripButtonAdjustSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdjustSettings.Name = "toolStripButtonAdjustSettings";
            this.toolStripButtonAdjustSettings.Size = new System.Drawing.Size(106, 22);
            this.toolStripButtonAdjustSettings.Text = "Adjust Settings";
            this.toolStripButtonAdjustSettings.ToolTipText = "This button will allow program settings to be adjusted.";
            this.toolStripButtonAdjustSettings.Click += new System.EventHandler(this.toolStripButtonAdjustSettings_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonClearLog
            // 
            this.toolStripButtonClearLog.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClearLog.Image")));
            this.toolStripButtonClearLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearLog.Name = "toolStripButtonClearLog";
            this.toolStripButtonClearLog.Size = new System.Drawing.Size(77, 22);
            this.toolStripButtonClearLog.Text = "Clear Log";
            this.toolStripButtonClearLog.ToolTipText = "This button will completely clear the log below.";
            this.toolStripButtonClearLog.Click += new System.EventHandler(this.toolStripButtonClearLog_Click);
            // 
            // toolStripButtonExportLog
            // 
            this.toolStripButtonExportLog.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExportLog.Image")));
            this.toolStripButtonExportLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExportLog.Name = "toolStripButtonExportLog";
            this.toolStripButtonExportLog.Size = new System.Drawing.Size(83, 22);
            this.toolStripButtonExportLog.Text = "Export Log";
            this.toolStripButtonExportLog.ToolTipText = "This button will export the current log to a file located in the selected output " +
    "directory.";
            this.toolStripButtonExportLog.Click += new System.EventHandler(this.toolStripButtonExportLog_Click);
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Location = new System.Drawing.Point(12, 266);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ReadOnly = true;
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(715, 154);
            this.textBoxLogger.TabIndex = 11;
            // 
            // textBoxOutDir
            // 
            this.textBoxOutDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxOutDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Data_Flow_Analyzer.Properties.Settings.Default, "OutDir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxOutDir.Enabled = false;
            this.textBoxOutDir.Location = new System.Drawing.Point(122, 32);
            this.textBoxOutDir.Name = "textBoxOutDir";
            this.textBoxOutDir.Size = new System.Drawing.Size(605, 20);
            this.textBoxOutDir.TabIndex = 3;
            this.textBoxOutDir.Text = global::Data_Flow_Analyzer.Properties.Settings.Default.OutDir;
            // 
            // frmDataFlow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 448);
            this.Controls.Add(this.textBoxLogger);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBoxOutDir);
            this.Controls.Add(this.buttonOutputDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDataFlow";
            this.Text = "eDNA Data Flow Analyzer";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.frmDataFlow_HelpButtonClicked);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserInput;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserOutput;
        private System.Windows.Forms.Button buttonOutputDir;
        private System.Windows.Forms.TextBox textBoxOutDir;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripProgress;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarFiles;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSelectData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunProgram;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearLog;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportLog;
        private System.Windows.Forms.TextBox textBoxLogger;
    }
}

