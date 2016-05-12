namespace Push_Tools
{
    partial class frmPushTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPushTools));
            this.openFileCSVInput = new System.Windows.Forms.OpenFileDialog();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.backgroundWorkerRunPush = new System.ComponentModel.BackgroundWorker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updateRatesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pushTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parametersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.buttonOutDir = new System.Windows.Forms.Button();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLoadFromCSV = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLoadFromDNA = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRunProgram = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAdjustSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdjustDates = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportCSV = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonClearLog = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExportLog = new System.Windows.Forms.ToolStripButton();
            this.textBoxOutDir = new System.Windows.Forms.TextBox();
            this.toolStripProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressLabel = new System.Windows.Forms.ToolStripLabel();
            this.folderBrowserOutDir = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileCSVInput
            // 
            this.openFileCSVInput.Filter = "CSV Files | *.csv";
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Location = new System.Drawing.Point(15, 386);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ReadOnly = true;
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(991, 161);
            this.textBoxLogger.TabIndex = 5;
            // 
            // backgroundWorkerRunPush
            // 
            this.backgroundWorkerRunPush.WorkerReportsProgress = true;
            this.backgroundWorkerRunPush.WorkerSupportsCancellation = true;
            this.backgroundWorkerRunPush.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorkerRunPush.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorkerRunPush.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tagDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.startDateDataGridViewTextBoxColumn,
            this.endDateDataGridViewTextBoxColumn,
            this.updateRatesDataGridViewTextBoxColumn,
            this.pushTypeDataGridViewTextBoxColumn,
            this.parametersDataGridViewTextBoxColumn});
            this.dataGridView1.DataMember = "TagCollection";
            this.dataGridView1.DataSource = this.dataSet1;
            this.dataGridView1.Location = new System.Drawing.Point(12, 62);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(994, 318);
            this.dataGridView1.TabIndex = 10;
            // 
            // tagDataGridViewTextBoxColumn
            // 
            this.tagDataGridViewTextBoxColumn.DataPropertyName = "Tag";
            this.tagDataGridViewTextBoxColumn.HeaderText = "Tag";
            this.tagDataGridViewTextBoxColumn.Name = "tagDataGridViewTextBoxColumn";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            // 
            // startDateDataGridViewTextBoxColumn
            // 
            this.startDateDataGridViewTextBoxColumn.DataPropertyName = "StartDate";
            this.startDateDataGridViewTextBoxColumn.HeaderText = "StartDate";
            this.startDateDataGridViewTextBoxColumn.Name = "startDateDataGridViewTextBoxColumn";
            // 
            // endDateDataGridViewTextBoxColumn
            // 
            this.endDateDataGridViewTextBoxColumn.DataPropertyName = "EndDate";
            this.endDateDataGridViewTextBoxColumn.HeaderText = "EndDate";
            this.endDateDataGridViewTextBoxColumn.Name = "endDateDataGridViewTextBoxColumn";
            // 
            // updateRatesDataGridViewTextBoxColumn
            // 
            this.updateRatesDataGridViewTextBoxColumn.DataPropertyName = "UpdateRate (s)";
            this.updateRatesDataGridViewTextBoxColumn.HeaderText = "UpdateRate (s)";
            this.updateRatesDataGridViewTextBoxColumn.Name = "updateRatesDataGridViewTextBoxColumn";
            // 
            // pushTypeDataGridViewTextBoxColumn
            // 
            this.pushTypeDataGridViewTextBoxColumn.DataPropertyName = "Push Type";
            this.pushTypeDataGridViewTextBoxColumn.HeaderText = "Push Type";
            this.pushTypeDataGridViewTextBoxColumn.Name = "pushTypeDataGridViewTextBoxColumn";
            // 
            // parametersDataGridViewTextBoxColumn
            // 
            this.parametersDataGridViewTextBoxColumn.DataPropertyName = "Parameters";
            this.parametersDataGridViewTextBoxColumn.HeaderText = "Parameters";
            this.parametersDataGridViewTextBoxColumn.Name = "parametersDataGridViewTextBoxColumn";
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7});
            this.dataTable1.TableName = "TagCollection";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Tag";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Description";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "StartDate";
            this.dataColumn3.DataType = typeof(System.DateTime);
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "EndDate";
            this.dataColumn4.DataType = typeof(System.DateTime);
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "UpdateRate (s)";
            this.dataColumn5.DataType = typeof(int);
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "Push Type";
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Parameters";
            // 
            // buttonOutDir
            // 
            this.buttonOutDir.Location = new System.Drawing.Point(12, 28);
            this.buttonOutDir.Name = "buttonOutDir";
            this.buttonOutDir.Size = new System.Drawing.Size(104, 28);
            this.buttonOutDir.TabIndex = 26;
            this.buttonOutDir.Text = "Output Directory:";
            this.buttonOutDir.UseVisualStyleBackColor = true;
            this.buttonOutDir.Click += new System.EventHandler(this.buttonOutDir_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLoadFromCSV,
            this.toolStripButtonLoadFromDNA,
            this.toolStripSeparator4,
            this.toolStripButtonRunProgram,
            this.toolStripButtonCancel,
            this.toolStripSeparator1,
            this.toolStripButtonAdjustSettings,
            this.toolStripButtonAdjustDates,
            this.toolStripSeparator2,
            this.toolStripButtonClear,
            this.toolStripButtonExportCSV,
            this.toolStripSeparator3,
            this.toolStripButtonClearLog,
            this.toolStripButtonExportLog});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1018, 25);
            this.toolStrip2.TabIndex = 25;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonLoadFromCSV
            // 
            this.toolStripButtonLoadFromCSV.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadFromCSV.Image")));
            this.toolStripButtonLoadFromCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoadFromCSV.Name = "toolStripButtonLoadFromCSV";
            this.toolStripButtonLoadFromCSV.Size = new System.Drawing.Size(106, 22);
            this.toolStripButtonLoadFromCSV.Text = "Load from CSV";
            this.toolStripButtonLoadFromCSV.ToolTipText = resources.GetString("toolStripButtonLoadFromCSV.ToolTipText");
            this.toolStripButtonLoadFromCSV.Click += new System.EventHandler(this.toolStripButtonLoadFromCSV_Click);
            // 
            // toolStripButtonLoadFromDNA
            // 
            this.toolStripButtonLoadFromDNA.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadFromDNA.Image")));
            this.toolStripButtonLoadFromDNA.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoadFromDNA.Name = "toolStripButtonLoadFromDNA";
            this.toolStripButtonLoadFromDNA.Size = new System.Drawing.Size(116, 22);
            this.toolStripButtonLoadFromDNA.Text = "Load from eDNA";
            this.toolStripButtonLoadFromDNA.ToolTipText = "This button will open a point picker from eDNA. Multiple points can be selected.";
            this.toolStripButtonLoadFromDNA.Click += new System.EventHandler(this.toolStripButtonLoadFromDNA_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRunProgram
            // 
            this.toolStripButtonRunProgram.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRunProgram.Image")));
            this.toolStripButtonRunProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRunProgram.Name = "toolStripButtonRunProgram";
            this.toolStripButtonRunProgram.Size = new System.Drawing.Size(97, 22);
            this.toolStripButtonRunProgram.Text = "Run Program";
            this.toolStripButtonRunProgram.ToolTipText = "This button will run the program, disabling most control buttons.";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // toolStripButtonAdjustDates
            // 
            this.toolStripButtonAdjustDates.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdjustDates.Image")));
            this.toolStripButtonAdjustDates.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdjustDates.Name = "toolStripButtonAdjustDates";
            this.toolStripButtonAdjustDates.Size = new System.Drawing.Size(110, 22);
            this.toolStripButtonAdjustDates.Text = "Adjust All Dates";
            this.toolStripButtonAdjustDates.ToolTipText = "This button will allow a mass edit of all \"Start Date\" and \"End Date\" values.";
            this.toolStripButtonAdjustDates.Click += new System.EventHandler(this.toolStripButtonAdjustDates_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonClear
            // 
            this.toolStripButtonClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonClear.Image")));
            this.toolStripButtonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClear.Name = "toolStripButtonClear";
            this.toolStripButtonClear.Size = new System.Drawing.Size(93, 22);
            this.toolStripButtonClear.Text = "Clear Config";
            this.toolStripButtonClear.ToolTipText = "This button will completely clear the configuration in the Data Grid below.";
            this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
            // 
            // toolStripButtonExportCSV
            // 
            this.toolStripButtonExportCSV.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExportCSV.Image")));
            this.toolStripButtonExportCSV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExportCSV.Name = "toolStripButtonExportCSV";
            this.toolStripButtonExportCSV.Size = new System.Drawing.Size(99, 22);
            this.toolStripButtonExportCSV.Text = "Export Config";
            this.toolStripButtonExportCSV.ToolTipText = "This button will export the current Configuration in the Data Grid below\r\nto a CS" +
    "V file in the selected output directory.";
            this.toolStripButtonExportCSV.Click += new System.EventHandler(this.toolStripButtonExportCSV_Click);
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
            this.textBoxOutDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Push_Tools.Properties.Settings.Default, "OutDirectory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxOutDir.Location = new System.Drawing.Point(123, 33);
            this.textBoxOutDir.Name = "textBoxOutDir";
            this.textBoxOutDir.Size = new System.Drawing.Size(883, 20);
            this.textBoxOutDir.TabIndex = 24;
            this.textBoxOutDir.Text = global::Push_Tools.Properties.Settings.Default.OutDirectory;
            // 
            // toolStripProgress
            // 
            this.toolStripProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgress.Name = "toolStripProgress";
            this.toolStripProgress.Size = new System.Drawing.Size(800, 22);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Enabled = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressLabel,
            this.toolStripProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 550);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1018, 25);
            this.toolStrip1.TabIndex = 27;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressLabel
            // 
            this.toolStripProgressLabel.Name = "toolStripProgressLabel";
            this.toolStripProgressLabel.Size = new System.Drawing.Size(113, 22);
            this.toolStripProgressLabel.Text = "Status: Not Running";
            // 
            // frmPushTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 575);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.buttonOutDir);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.textBoxOutDir);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBoxLogger);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPushTools";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "eDNA Push Tools";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.frmPushTools_HelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileCSVInput;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRunPush;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonOutDir;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromCSV;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromDNA;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunProgram;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustSettings;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustDates;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportCSV;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearLog;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportLog;
        private System.Windows.Forms.TextBox textBoxOutDir;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgress;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripProgressLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserOutDir;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn tagDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updateRatesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pushTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parametersDataGridViewTextBoxColumn;
    }
}

