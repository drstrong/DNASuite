namespace Pull_Tools
{
    partial class frmPullTools
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPullTools));
            this.folderBrowserOutDir = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileCSVInput = new System.Windows.Forms.OpenFileDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stateFilterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueFilterDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataSetTagPull = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.textBoxLogger = new System.Windows.Forms.TextBox();
            this.backgroundWorkerRunPull = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripProgressLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgress = new System.Windows.Forms.ToolStripProgressBar();
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
            this.buttonOutDir = new System.Windows.Forms.Button();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.textBoxOutDir = new System.Windows.Forms.TextBox();
            this.backgroundWorkerLoadFromCSV = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetTagPull)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileCSVInput
            // 
            this.openFileCSVInput.FileName = "openFileDialog1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tagDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.startDateDataGridViewTextBoxColumn,
            this.endDateDataGridViewTextBoxColumn,
            this.stateFilterDataGridViewTextBoxColumn,
            this.valueFilterDataGridViewTextBoxColumn,
            this.groupDataGridViewTextBoxColumn});
            this.dataGridView1.DataMember = "TagCollection";
            this.dataGridView1.DataSource = this.dataSetTagPull;
            this.dataGridView1.Location = new System.Drawing.Point(12, 54);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(987, 325);
            this.dataGridView1.TabIndex = 2;
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
            // stateFilterDataGridViewTextBoxColumn
            // 
            this.stateFilterDataGridViewTextBoxColumn.DataPropertyName = "StateFilter";
            this.stateFilterDataGridViewTextBoxColumn.HeaderText = "StateFilter";
            this.stateFilterDataGridViewTextBoxColumn.Name = "stateFilterDataGridViewTextBoxColumn";
            // 
            // valueFilterDataGridViewTextBoxColumn
            // 
            this.valueFilterDataGridViewTextBoxColumn.DataPropertyName = "ValueFilter";
            this.valueFilterDataGridViewTextBoxColumn.HeaderText = "ValueFilter";
            this.valueFilterDataGridViewTextBoxColumn.Name = "valueFilterDataGridViewTextBoxColumn";
            // 
            // groupDataGridViewTextBoxColumn
            // 
            this.groupDataGridViewTextBoxColumn.DataPropertyName = "Group";
            this.groupDataGridViewTextBoxColumn.HeaderText = "Group";
            this.groupDataGridViewTextBoxColumn.Name = "groupDataGridViewTextBoxColumn";
            // 
            // dataSetTagPull
            // 
            this.dataSetTagPull.DataSetName = "NewDataSet";
            this.dataSetTagPull.Tables.AddRange(new System.Data.DataTable[] {
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
            this.dataColumn5.ColumnName = "StateFilter";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "ValueFilter";
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "Group";
            // 
            // textBoxLogger
            // 
            this.textBoxLogger.Location = new System.Drawing.Point(13, 385);
            this.textBoxLogger.Multiline = true;
            this.textBoxLogger.Name = "textBoxLogger";
            this.textBoxLogger.ReadOnly = true;
            this.textBoxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLogger.Size = new System.Drawing.Size(987, 179);
            this.textBoxLogger.TabIndex = 3;
            // 
            // backgroundWorkerRunPull
            // 
            this.backgroundWorkerRunPull.WorkerReportsProgress = true;
            this.backgroundWorkerRunPull.WorkerSupportsCancellation = true;
            this.backgroundWorkerRunPull.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRunPull_DoWork);
            this.backgroundWorkerRunPull.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorkerRunPull.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Enabled = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressLabel,
            this.toolStripProgress});
            this.toolStrip1.Location = new System.Drawing.Point(0, 567);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1012, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressLabel
            // 
            this.toolStripProgressLabel.Name = "toolStripProgressLabel";
            this.toolStripProgressLabel.Size = new System.Drawing.Size(113, 22);
            this.toolStripProgressLabel.Text = "Status: Not Running";
            // 
            // toolStripProgress
            // 
            this.toolStripProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgress.Name = "toolStripProgress";
            this.toolStripProgress.Size = new System.Drawing.Size(800, 22);
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
            this.toolStrip2.Size = new System.Drawing.Size(1012, 25);
            this.toolStrip2.TabIndex = 8;
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
            // buttonOutDir
            // 
            this.buttonOutDir.Location = new System.Drawing.Point(12, 23);
            this.buttonOutDir.Name = "buttonOutDir";
            this.buttonOutDir.Size = new System.Drawing.Size(104, 28);
            this.buttonOutDir.TabIndex = 9;
            this.buttonOutDir.Text = "Output Directory:";
            this.buttonOutDir.UseVisualStyleBackColor = true;
            this.buttonOutDir.Click += new System.EventHandler(this.buttonOutDir_Click);
            // 
            // textBoxOutDir
            // 
            this.textBoxOutDir.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Pull_Tools.Properties.Settings.Default, "Out_Directory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxOutDir.Location = new System.Drawing.Point(123, 28);
            this.textBoxOutDir.Name = "textBoxOutDir";
            this.textBoxOutDir.Size = new System.Drawing.Size(876, 20);
            this.textBoxOutDir.TabIndex = 1;
            this.textBoxOutDir.Text = global::Pull_Tools.Properties.Settings.Default.Out_Directory;
            // 
            // backgroundWorkerLoadFromCSV
            // 
            this.backgroundWorkerLoadFromCSV.WorkerReportsProgress = true;
            this.backgroundWorkerLoadFromCSV.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoadFromCSV_DoWork);
            this.backgroundWorkerLoadFromCSV.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerLoadFromCSV_ProgressChanged);
            this.backgroundWorkerLoadFromCSV.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoadFromCSV_RunWorkerCompleted);
            // 
            // frmPullTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 592);
            this.Controls.Add(this.buttonOutDir);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBoxLogger);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBoxOutDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPullTools";
            this.Text = "eDNA Pull Tools";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.frmPull_HelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetTagPull)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOutDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserOutDir;
        private System.Windows.Forms.OpenFileDialog openFileCSVInput;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxLogger;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRunPull;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripProgressLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgress;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromCSV;
        private System.Windows.Forms.ToolStripButton toolStripButtonRunProgram;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustSettings;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdjustDates;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportCSV;
        private System.Windows.Forms.ToolStripButton toolStripButtonExportLog;
        private System.Windows.Forms.ToolStripButton toolStripButtonClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearLog;
        private System.Windows.Forms.Button buttonOutDir;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Data.DataSet dataSetTagPull;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadFromDNA;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn tagDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn stateFilterDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueFilterDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn groupDataGridViewTextBoxColumn;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoadFromCSV;
    }
}

