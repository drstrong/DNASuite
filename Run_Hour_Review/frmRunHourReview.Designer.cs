namespace Run_Hour_Review
{
    partial class frmRunHourReview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunHourReview));
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxPullSpreadsheet = new System.Windows.Forms.TextBox();
            this.buttonPullSpreadsheet = new System.Windows.Forms.Button();
            this.radioButtonPullData = new System.Windows.Forms.RadioButton();
            this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxPushSpreadsheet = new System.Windows.Forms.TextBox();
            this.buttonPushSpreadsheet = new System.Windows.Forms.Button();
            this.openPullSpreadsheet = new System.Windows.Forms.OpenFileDialog();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openPushSpreadsheet = new System.Windows.Forms.OpenFileDialog();
            this.toolStripProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRunProgram = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAdjustSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClearLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportLog = new System.Windows.Forms.ToolStripButton();
            this.textBoxOutDir = new System.Windows.Forms.TextBox();
            this.buttonOutputDir = new System.Windows.Forms.Button();
            this.folderBrowserOutDir = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(4, 95);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(102, 24);
            this.radioButton1.TabIndex = 8;
            this.radioButton1.Text = "Push Data";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.65296F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.34705F));
            this.tableLayoutPanel1.Controls.Add(this.radioButton1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioButtonPullData, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 61);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(792, 130);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.79641F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.20359F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel3.Controls.Add(this.textBoxPullSpreadsheet, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonPullSpreadsheet, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(120, 4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(668, 33);
            this.tableLayoutPanel3.TabIndex = 10;
            // 
            // textBoxPullSpreadsheet
            // 
            this.textBoxPullSpreadsheet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxPullSpreadsheet.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Run_Hour_Review.Properties.Settings.Default, "InputPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxPullSpreadsheet.Location = new System.Drawing.Point(182, 6);
            this.textBoxPullSpreadsheet.Name = "textBoxPullSpreadsheet";
            this.textBoxPullSpreadsheet.Size = new System.Drawing.Size(483, 20);
            this.textBoxPullSpreadsheet.TabIndex = 1;
            this.textBoxPullSpreadsheet.Text = global::Run_Hour_Review.Properties.Settings.Default.InputPath;
            // 
            // buttonPullSpreadsheet
            // 
            this.buttonPullSpreadsheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPullSpreadsheet.Location = new System.Drawing.Point(3, 3);
            this.buttonPullSpreadsheet.Name = "buttonPullSpreadsheet";
            this.buttonPullSpreadsheet.Size = new System.Drawing.Size(173, 26);
            this.buttonPullSpreadsheet.TabIndex = 0;
            this.buttonPullSpreadsheet.Text = "Pull Spreadsheet";
            this.buttonPullSpreadsheet.UseVisualStyleBackColor = true;
            this.buttonPullSpreadsheet.Click += new System.EventHandler(this.buttonPullSpreadsheet_Click);
            // 
            // radioButtonPullData
            // 
            this.radioButtonPullData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonPullData.AutoSize = true;
            this.radioButtonPullData.Checked = global::Run_Hour_Review.Properties.Settings.Default.PullData;
            this.radioButtonPullData.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Run_Hour_Review.Properties.Settings.Default, "PullData", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioButtonPullData.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonPullData.Location = new System.Drawing.Point(4, 9);
            this.radioButtonPullData.Name = "radioButtonPullData";
            this.radioButtonPullData.Size = new System.Drawing.Size(91, 24);
            this.radioButtonPullData.TabIndex = 7;
            this.radioButtonPullData.TabStop = true;
            this.radioButtonPullData.Text = "Pull Data";
            this.radioButtonPullData.UseVisualStyleBackColor = true;
            // 
            // dateTimeStart
            // 
            this.dateTimeStart.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateTimeStart.CustomFormat = "yyyy/MM/dd HH:mm";
            this.dateTimeStart.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Run_Hour_Review.Properties.Settings.Default, "StartDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.dateTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStart.Location = new System.Drawing.Point(3, 7);
            this.dateTimeStart.Name = "dateTimeStart";
            this.dateTimeStart.Size = new System.Drawing.Size(137, 20);
            this.dateTimeStart.TabIndex = 3;
            this.dateTimeStart.Value = global::Run_Hour_Review.Properties.Settings.Default.StartDate;
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.dateTimeEnd.CustomFormat = "yyyy/MM/dd HH:mm";
            this.dateTimeEnd.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Run_Hour_Review.Properties.Settings.Default, "EndDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.dateTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEnd.Location = new System.Drawing.Point(204, 7);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(135, 20);
            this.dateTimeEnd.TabIndex = 5;
            this.dateTimeEnd.Value = global::Run_Hour_Review.Properties.Settings.Default.EndDate;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.79641F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.20359F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel4.Controls.Add(this.textBoxPushSpreadsheet, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.buttonPushSpreadsheet, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(120, 88);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(668, 33);
            this.tableLayoutPanel4.TabIndex = 11;
            // 
            // textBoxPushSpreadsheet
            // 
            this.textBoxPushSpreadsheet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxPushSpreadsheet.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Run_Hour_Review.Properties.Settings.Default, "PushPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxPushSpreadsheet.Location = new System.Drawing.Point(182, 6);
            this.textBoxPushSpreadsheet.Name = "textBoxPushSpreadsheet";
            this.textBoxPushSpreadsheet.Size = new System.Drawing.Size(483, 20);
            this.textBoxPushSpreadsheet.TabIndex = 1;
            this.textBoxPushSpreadsheet.Text = global::Run_Hour_Review.Properties.Settings.Default.PushPath;
            // 
            // buttonPushSpreadsheet
            // 
            this.buttonPushSpreadsheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPushSpreadsheet.Location = new System.Drawing.Point(3, 3);
            this.buttonPushSpreadsheet.Name = "buttonPushSpreadsheet";
            this.buttonPushSpreadsheet.Size = new System.Drawing.Size(173, 26);
            this.buttonPushSpreadsheet.TabIndex = 0;
            this.buttonPushSpreadsheet.Text = "Push Spreadsheet";
            this.buttonPushSpreadsheet.UseVisualStyleBackColor = true;
            this.buttonPushSpreadsheet.Click += new System.EventHandler(this.buttonPushSpreadsheet_Click);
            // 
            // openPullSpreadsheet
            // 
            this.openPullSpreadsheet.Filter = "CSV files|*.csv";
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Location = new System.Drawing.Point(12, 197);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(785, 182);
            this.textBoxLogger.TabIndex = 12;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // openPushSpreadsheet
            // 
            this.openPushSpreadsheet.Filter = "XLSX files | *.xlsx";
            // 
            // toolStripProgress
            // 
            this.toolStripProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgress.Name = "toolStripProgress";
            this.toolStripProgress.Size = new System.Drawing.Size(500, 22);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Enabled = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressLabel,
            this.toolStripProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 386);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(807, 25);
            this.toolStrip1.TabIndex = 28;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressLabel
            // 
            this.toolStripProgressLabel.Name = "toolStripProgressLabel";
            this.toolStripProgressLabel.Size = new System.Drawing.Size(113, 22);
            this.toolStripProgressLabel.Text = "Status: Not Running";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRunProgram,
            this.toolStripButtonCancel,
            this.toolStripSeparator2,
            this.toolStripButtonAdjustSettings,
            this.toolStripSeparator3,
            this.toolStripButtonClearLog,
            this.toolStripButtonExportLog});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(807, 25);
            this.toolStrip2.TabIndex = 29;
            this.toolStrip2.Text = "toolStrip2";
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
            // textBoxOutDir
            // 
            this.textBoxOutDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxOutDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Run_Hour_Review.Properties.Settings.Default, "OutDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxOutDir.Enabled = false;
            this.textBoxOutDir.Location = new System.Drawing.Point(135, 32);
            this.textBoxOutDir.Name = "textBoxOutDir";
            this.textBoxOutDir.Size = new System.Drawing.Size(662, 20);
            this.textBoxOutDir.TabIndex = 31;
            this.textBoxOutDir.Text = global::Run_Hour_Review.Properties.Settings.Default.OutDirectory;
            // 
            // buttonOutputDir
            // 
            this.buttonOutputDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonOutputDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOutputDir.Location = new System.Drawing.Point(12, 28);
            this.buttonOutputDir.Name = "buttonOutputDir";
            this.buttonOutputDir.Size = new System.Drawing.Size(117, 27);
            this.buttonOutputDir.TabIndex = 30;
            this.buttonOutputDir.Text = "Output Directory:";
            this.buttonOutputDir.UseVisualStyleBackColor = true;
            this.buttonOutputDir.Click += new System.EventHandler(this.buttonOutputDir_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.46429F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.53572F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 466F));
            this.tableLayoutPanel2.Controls.Add(this.dateTimeStart, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dateTimeEnd, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(120, 46);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(668, 35);
            this.tableLayoutPanel2.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "-";
            // 
            // frmRunHourReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 411);
            this.Controls.Add(this.textBoxOutDir);
            this.Controls.Add(this.buttonOutputDir);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBoxLogger);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRunHourReview";
            this.Text = "eDNA Run Hour Review";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.frmRunHourReview_HelpButtonClicked);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeStart;
        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.RadioButton radioButtonPullData;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.OpenFileDialog openPullSpreadsheet;
        private System.Windows.Forms.Button buttonPullSpreadsheet;
        private System.Windows.Forms.TextBox textBoxPullSpreadsheet;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox textBoxPushSpreadsheet;
        private System.Windows.Forms.Button buttonPushSpreadsheet;
        private System.Windows.Forms.OpenFileDialog openPushSpreadsheet;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgress;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripProgressLabel;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunProgram;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearLog;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportLog;
        private System.Windows.Forms.TextBox textBoxOutDir;
        private System.Windows.Forms.Button buttonOutputDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserOutDir;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
    }
}

