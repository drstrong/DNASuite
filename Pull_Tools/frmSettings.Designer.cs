namespace Pull_Tools
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxUTCTime = new System.Windows.Forms.CheckBox();
            this.checkBoxSingleFile = new System.Windows.Forms.CheckBox();
            this.checkBoxWriteStatus = new System.Windows.Forms.CheckBox();
            this.checkBoxZipResults = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.numericTimeout = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.radioButtonRaw = new System.Windows.Forms.RadioButton();
            this.radioButtonSnap = new System.Windows.Forms.RadioButton();
            this.radioButtonMin = new System.Windows.Forms.RadioButton();
            this.radioButtonMax = new System.Windows.Forms.RadioButton();
            this.radioButtonAvg = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxFilterUnreliable = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxCalculateMetrics = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxCalcMin = new System.Windows.Forms.CheckBox();
            this.checkBoxCalcMax = new System.Windows.Forms.CheckBox();
            this.checkBoxCalcMean = new System.Windows.Forms.CheckBox();
            this.checkBoxCalcLinear = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 232);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Preprocessing";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output Settings";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Pull Mode";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxUTCTime, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSingleFile, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxWriteStatus, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxZipResults, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 0, 8);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(218, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.882353F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(220, 292);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // checkBoxUTCTime
            // 
            this.checkBoxUTCTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxUTCTime.AutoSize = true;
            this.checkBoxUTCTime.Checked = global::Pull_Tools.Properties.Settings.Default.Out_UTCTime;
            this.checkBoxUTCTime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Out_UTCTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxUTCTime.Location = new System.Drawing.Point(4, 40);
            this.checkBoxUTCTime.Name = "checkBoxUTCTime";
            this.checkBoxUTCTime.Size = new System.Drawing.Size(96, 17);
            this.checkBoxUTCTime.TabIndex = 7;
            this.checkBoxUTCTime.Text = "Use UTC Time";
            this.toolTipHelp.SetToolTip(this.checkBoxUTCTime, "If checked, the Date/Time output will use UTC time instead.");
            this.checkBoxUTCTime.UseVisualStyleBackColor = true;
            // 
            // checkBoxSingleFile
            // 
            this.checkBoxSingleFile.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxSingleFile.AutoSize = true;
            this.checkBoxSingleFile.Checked = global::Pull_Tools.Properties.Settings.Default.Out_SingleFile;
            this.checkBoxSingleFile.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Out_SingleFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxSingleFile.Location = new System.Drawing.Point(4, 72);
            this.checkBoxSingleFile.Name = "checkBoxSingleFile";
            this.checkBoxSingleFile.Size = new System.Drawing.Size(114, 17);
            this.checkBoxSingleFile.TabIndex = 0;
            this.checkBoxSingleFile.Text = "Write to Single File";
            this.checkBoxSingleFile.UseVisualStyleBackColor = true;
            this.checkBoxSingleFile.CheckedChanged += new System.EventHandler(this.checkBoxSingleFile_CheckedChanged);
            // 
            // checkBoxWriteStatus
            // 
            this.checkBoxWriteStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxWriteStatus.AutoSize = true;
            this.checkBoxWriteStatus.Checked = global::Pull_Tools.Properties.Settings.Default.Out_WriteStatus;
            this.checkBoxWriteStatus.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Out_WriteStatus", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxWriteStatus.Location = new System.Drawing.Point(4, 104);
            this.checkBoxWriteStatus.Name = "checkBoxWriteStatus";
            this.checkBoxWriteStatus.Size = new System.Drawing.Size(122, 17);
            this.checkBoxWriteStatus.TabIndex = 1;
            this.checkBoxWriteStatus.Text = "Write Status Column";
            this.toolTipHelp.SetToolTip(this.checkBoxWriteStatus, "If checked, the eDNA status column will be added.");
            this.checkBoxWriteStatus.UseVisualStyleBackColor = true;
            // 
            // checkBoxZipResults
            // 
            this.checkBoxZipResults.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxZipResults.AutoSize = true;
            this.checkBoxZipResults.Checked = global::Pull_Tools.Properties.Settings.Default.Out_ZipResults;
            this.checkBoxZipResults.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Out_ZipResults", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxZipResults.Location = new System.Drawing.Point(4, 136);
            this.checkBoxZipResults.Name = "checkBoxZipResults";
            this.checkBoxZipResults.Size = new System.Drawing.Size(79, 17);
            this.checkBoxZipResults.TabIndex = 6;
            this.checkBoxZipResults.Text = "Zip Results";
            this.toolTipHelp.SetToolTip(this.checkBoxZipResults, "If checked, each output CSV file will be automatically compressed using gzip.");
            this.checkBoxZipResults.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Pull Settings";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.70833F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.29167F));
            this.tableLayoutPanel3.Controls.Add(this.numericTimeout, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 196);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(192, 25);
            this.tableLayoutPanel3.TabIndex = 20;
            // 
            // numericTimeout
            // 
            this.numericTimeout.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericTimeout.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Pull_Tools.Properties.Settings.Default, "Pull_TimeoutPeriod", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericTimeout.Location = new System.Drawing.Point(84, 3);
            this.numericTimeout.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericTimeout.Name = "numericTimeout";
            this.numericTimeout.Size = new System.Drawing.Size(92, 20);
            this.numericTimeout.TabIndex = 21;
            this.numericTimeout.Value = global::Pull_Tools.Properties.Settings.Default.Pull_TimeoutPeriod;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Timeout (s):";
            this.toolTipHelp.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.1875F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.8125F));
            this.tableLayoutPanel4.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.numericUpDown1, 1, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 228);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(192, 25);
            this.tableLayoutPanel4.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Max Threads:";
            this.toolTipHelp.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Pull_Tools.Properties.Settings.Default, "Pull_MaxThreads", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown1.Location = new System.Drawing.Point(84, 3);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(92, 20);
            this.numericUpDown1.TabIndex = 19;
            this.numericUpDown1.Value = global::Pull_Tools.Properties.Settings.Default.Pull_MaxThreads;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.70833F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.29167F));
            this.tableLayoutPanel6.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.numericUpDown2, 1, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(4, 260);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(192, 28);
            this.tableLayoutPanel6.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Batching (d):";
            this.toolTipHelp.SetToolTip(this.label8, "This program pulls data in batches, by default per day, due to eDNA errors which " +
        "may be \r\nencountered during long data pulls.");
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Pull_Tools.Properties.Settings.Default, "Pull_BatchingInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown2.Location = new System.Drawing.Point(84, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(92, 20);
            this.numericUpDown2.TabIndex = 20;
            this.numericUpDown2.Value = global::Pull_Tools.Properties.Settings.Default.Pull_BatchingInterval;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.95833F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.04167F));
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxInterval, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 196);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(192, 25);
            this.tableLayoutPanel2.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Interval:";
            this.toolTipHelp.SetToolTip(this.label5, "This setting specifies the interval used to pull the data and must be provided fo" +
        "r \'Snap\', \'Min\',\r\n\'Max\', or \'Average\' modes above.");
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxInterval.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Pull_Tools.Properties.Settings.Default, "Pull_ModeInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxInterval.Location = new System.Drawing.Point(96, 3);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(91, 20);
            this.textBoxInterval.TabIndex = 13;
            this.textBoxInterval.Text = global::Pull_Tools.Properties.Settings.Default.Pull_ModeInterval;
            // 
            // radioButtonRaw
            // 
            this.radioButtonRaw.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonRaw.AutoSize = true;
            this.radioButtonRaw.Checked = true;
            this.radioButtonRaw.Location = new System.Drawing.Point(4, 40);
            this.radioButtonRaw.Name = "radioButtonRaw";
            this.radioButtonRaw.Size = new System.Drawing.Size(47, 17);
            this.radioButtonRaw.TabIndex = 8;
            this.radioButtonRaw.TabStop = true;
            this.radioButtonRaw.Text = "Raw";
            this.toolTipHelp.SetToolTip(this.radioButtonRaw, resources.GetString("radioButtonRaw.ToolTip"));
            this.radioButtonRaw.UseVisualStyleBackColor = true;
            // 
            // radioButtonSnap
            // 
            this.radioButtonSnap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonSnap.AutoSize = true;
            this.radioButtonSnap.Location = new System.Drawing.Point(4, 72);
            this.radioButtonSnap.Name = "radioButtonSnap";
            this.radioButtonSnap.Size = new System.Drawing.Size(50, 17);
            this.radioButtonSnap.TabIndex = 9;
            this.radioButtonSnap.Text = "Snap";
            this.toolTipHelp.SetToolTip(this.radioButtonSnap, resources.GetString("radioButtonSnap.ToolTip"));
            this.radioButtonSnap.UseVisualStyleBackColor = true;
            // 
            // radioButtonMin
            // 
            this.radioButtonMin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonMin.AutoSize = true;
            this.radioButtonMin.Location = new System.Drawing.Point(4, 104);
            this.radioButtonMin.Name = "radioButtonMin";
            this.radioButtonMin.Size = new System.Drawing.Size(42, 17);
            this.radioButtonMin.TabIndex = 10;
            this.radioButtonMin.Text = "Min";
            this.toolTipHelp.SetToolTip(this.radioButtonMin, resources.GetString("radioButtonMin.ToolTip"));
            this.radioButtonMin.UseVisualStyleBackColor = true;
            // 
            // radioButtonMax
            // 
            this.radioButtonMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonMax.AutoSize = true;
            this.radioButtonMax.Location = new System.Drawing.Point(4, 136);
            this.radioButtonMax.Name = "radioButtonMax";
            this.radioButtonMax.Size = new System.Drawing.Size(45, 17);
            this.radioButtonMax.TabIndex = 11;
            this.radioButtonMax.Text = "Max";
            this.toolTipHelp.SetToolTip(this.radioButtonMax, resources.GetString("radioButtonMax.ToolTip"));
            this.radioButtonMax.UseVisualStyleBackColor = true;
            // 
            // radioButtonAvg
            // 
            this.radioButtonAvg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonAvg.AutoSize = true;
            this.radioButtonAvg.Location = new System.Drawing.Point(4, 168);
            this.radioButtonAvg.Name = "radioButtonAvg";
            this.radioButtonAvg.Size = new System.Drawing.Size(65, 17);
            this.radioButtonAvg.TabIndex = 12;
            this.radioButtonAvg.Text = "Average";
            this.toolTipHelp.SetToolTip(this.radioButtonAvg, resources.GetString("radioButtonAvg.ToolTip"));
            this.radioButtonAvg.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonRaw, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonSnap, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonMin, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonMax, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonAvg, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 7);
            this.tableLayoutPanel5.Controls.Add(this.checkBoxFilterUnreliable, 0, 8);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 9;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(200, 292);
            this.tableLayoutPanel5.TabIndex = 9;
            // 
            // checkBoxFilterUnreliable
            // 
            this.checkBoxFilterUnreliable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxFilterUnreliable.AutoSize = true;
            this.checkBoxFilterUnreliable.Checked = global::Pull_Tools.Properties.Settings.Default.Pre_FilterUnreliable;
            this.checkBoxFilterUnreliable.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Pre_FilterUnreliable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxFilterUnreliable.Location = new System.Drawing.Point(4, 265);
            this.checkBoxFilterUnreliable.Name = "checkBoxFilterUnreliable";
            this.checkBoxFilterUnreliable.Size = new System.Drawing.Size(150, 17);
            this.checkBoxFilterUnreliable.TabIndex = 2;
            this.checkBoxFilterUnreliable.Text = "Filter Out Unreliable Points";
            this.toolTipHelp.SetToolTip(this.checkBoxFilterUnreliable, "If checked, this setting will remove all points marked \"unreliable\" by eDNA\r\nfrom" +
        " the data pull.");
            this.checkBoxFilterUnreliable.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(4, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 17);
            this.label9.TabIndex = 21;
            this.label9.Text = "Metrics";
            // 
            // checkBoxCalculateMetrics
            // 
            this.checkBoxCalculateMetrics.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxCalculateMetrics.AutoSize = true;
            this.checkBoxCalculateMetrics.Checked = global::Pull_Tools.Properties.Settings.Default.Metrics_Perform;
            this.checkBoxCalculateMetrics.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Metrics_Perform", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCalculateMetrics.Location = new System.Drawing.Point(4, 40);
            this.checkBoxCalculateMetrics.Name = "checkBoxCalculateMetrics";
            this.checkBoxCalculateMetrics.Size = new System.Drawing.Size(107, 17);
            this.checkBoxCalculateMetrics.TabIndex = 11;
            this.checkBoxCalculateMetrics.Text = "Calculate Metrics";
            this.checkBoxCalculateMetrics.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.52083F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.47917F));
            this.tableLayoutPanel7.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.numericUpDown3, 1, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(4, 68);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(192, 25);
            this.tableLayoutPanel7.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Analysis Window";
            this.toolTipHelp.SetToolTip(this.label10, "This setting specifies the interval used to pull the data and must be provided fo" +
        "r \'Snap\', \'Min\',\r\n\'Max\', or \'Average\' modes above.");
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Pull_Tools.Properties.Settings.Default, "Metrics_AnalysisWindow", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown3.Location = new System.Drawing.Point(99, 3);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(90, 20);
            this.numericUpDown3.TabIndex = 16;
            this.numericUpDown3.Value = global::Pull_Tools.Properties.Settings.Default.Metrics_AnalysisWindow;
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Location = new System.Drawing.Point(562, 272);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(83, 23);
            this.buttonSaveSettings.TabIndex = 10;
            this.buttonSaveSettings.Text = "Save Settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.checkBoxCalculateMetrics, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel7, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.checkBoxCalcMin, 0, 3);
            this.tableLayoutPanel8.Controls.Add(this.checkBoxCalcMax, 0, 4);
            this.tableLayoutPanel8.Controls.Add(this.checkBoxCalcMean, 0, 5);
            this.tableLayoutPanel8.Controls.Add(this.checkBoxCalcLinear, 0, 6);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(445, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 7;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(200, 225);
            this.tableLayoutPanel8.TabIndex = 11;
            // 
            // checkBoxCalcMin
            // 
            this.checkBoxCalcMin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxCalcMin.AutoSize = true;
            this.checkBoxCalcMin.Checked = global::Pull_Tools.Properties.Settings.Default.Metrics_CalcMin;
            this.checkBoxCalcMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCalcMin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Metrics_CalcMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCalcMin.Location = new System.Drawing.Point(4, 104);
            this.checkBoxCalcMin.Name = "checkBoxCalcMin";
            this.checkBoxCalcMin.Size = new System.Drawing.Size(114, 17);
            this.checkBoxCalcMin.TabIndex = 23;
            this.checkBoxCalcMin.Text = "Calculate Minimum";
            this.checkBoxCalcMin.UseVisualStyleBackColor = true;
            // 
            // checkBoxCalcMax
            // 
            this.checkBoxCalcMax.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxCalcMax.AutoSize = true;
            this.checkBoxCalcMax.Checked = global::Pull_Tools.Properties.Settings.Default.Metrics_CalcMax;
            this.checkBoxCalcMax.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCalcMax.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Metrics_CalcMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCalcMax.Location = new System.Drawing.Point(4, 136);
            this.checkBoxCalcMax.Name = "checkBoxCalcMax";
            this.checkBoxCalcMax.Size = new System.Drawing.Size(117, 17);
            this.checkBoxCalcMax.TabIndex = 24;
            this.checkBoxCalcMax.Text = "Calculate Maximum";
            this.checkBoxCalcMax.UseVisualStyleBackColor = true;
            // 
            // checkBoxCalcMean
            // 
            this.checkBoxCalcMean.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxCalcMean.AutoSize = true;
            this.checkBoxCalcMean.Checked = global::Pull_Tools.Properties.Settings.Default.Metrics_CalcMean;
            this.checkBoxCalcMean.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCalcMean.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Metrics_CalcMean", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCalcMean.Location = new System.Drawing.Point(4, 168);
            this.checkBoxCalcMean.Name = "checkBoxCalcMean";
            this.checkBoxCalcMean.Size = new System.Drawing.Size(100, 17);
            this.checkBoxCalcMean.TabIndex = 25;
            this.checkBoxCalcMean.Text = "Calculate Mean";
            this.checkBoxCalcMean.UseVisualStyleBackColor = true;
            // 
            // checkBoxCalcLinear
            // 
            this.checkBoxCalcLinear.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxCalcLinear.AutoSize = true;
            this.checkBoxCalcLinear.Checked = global::Pull_Tools.Properties.Settings.Default.Metrics_CalcLinear;
            this.checkBoxCalcLinear.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCalcLinear.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Pull_Tools.Properties.Settings.Default, "Metrics_CalcLinear", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCalcLinear.Location = new System.Drawing.Point(4, 200);
            this.checkBoxCalcLinear.Name = "checkBoxCalcLinear";
            this.checkBoxCalcLinear.Size = new System.Drawing.Size(158, 17);
            this.checkBoxCalcLinear.TabIndex = 26;
            this.checkBoxCalcLinear.Text = "Calculate Linear Regression";
            this.checkBoxCalcLinear.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 303);
            this.Controls.Add(this.tableLayoutPanel8);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.Text = "User Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSettings_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxSingleFile;
        private System.Windows.Forms.CheckBox checkBoxWriteStatus;
        private System.Windows.Forms.CheckBox checkBoxFilterUnreliable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxZipResults;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioButtonRaw;
        private System.Windows.Forms.RadioButton radioButtonSnap;
        private System.Windows.Forms.RadioButton radioButtonMin;
        private System.Windows.Forms.RadioButton radioButtonMax;
        private System.Windows.Forms.RadioButton radioButtonAvg;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.NumericUpDown numericTimeout;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox checkBoxUTCTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.CheckBox checkBoxCalculateMetrics;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.CheckBox checkBoxCalcMin;
        private System.Windows.Forms.CheckBox checkBoxCalcMax;
        private System.Windows.Forms.CheckBox checkBoxCalcMean;
        private System.Windows.Forms.CheckBox checkBoxCalcLinear;

    }
}