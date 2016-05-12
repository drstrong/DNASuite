namespace Push_Tools
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
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonAppendConnect = new System.Windows.Forms.RadioButton();
            this.radioButtonInsertConnect = new System.Windows.Forms.RadioButton();
            this.radioButtonAppend = new System.Windows.Forms.RadioButton();
            this.radioButtonInsert = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxDoublePass = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonAppendConnect, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.checkBox1, 0, 9);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonInsertConnect, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonAppend, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.radioButtonInsert, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.checkBoxDoublePass, 0, 8);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel1, 0, 7);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 10;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(182, 336);
            this.tableLayoutPanel5.TabIndex = 10;
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
            // radioButtonAppendConnect
            // 
            this.radioButtonAppendConnect.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonAppendConnect.AutoSize = true;
            this.radioButtonAppendConnect.Checked = true;
            this.radioButtonAppendConnect.Location = new System.Drawing.Point(4, 41);
            this.radioButtonAppendConnect.Name = "radioButtonAppendConnect";
            this.radioButtonAppendConnect.Size = new System.Drawing.Size(107, 17);
            this.radioButtonAppendConnect.TabIndex = 8;
            this.radioButtonAppendConnect.TabStop = true;
            this.radioButtonAppendConnect.Text = "Append/Connect";
            this.toolTipHelp.SetToolTip(this.radioButtonAppendConnect, "This push mode will attempt to append all values to history, and then flush the \r" +
        "\nlast value to the CVT. (Recommended)");
            this.radioButtonAppendConnect.UseVisualStyleBackColor = true;
            // 
            // radioButtonInsertConnect
            // 
            this.radioButtonInsertConnect.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonInsertConnect.AutoSize = true;
            this.radioButtonInsertConnect.Location = new System.Drawing.Point(4, 74);
            this.radioButtonInsertConnect.Name = "radioButtonInsertConnect";
            this.radioButtonInsertConnect.Size = new System.Drawing.Size(96, 17);
            this.radioButtonInsertConnect.TabIndex = 9;
            this.radioButtonInsertConnect.Text = "Insert/Connect";
            this.toolTipHelp.SetToolTip(this.radioButtonInsertConnect, "This push mode will attempt to insert all values to history, and then flush the \r" +
        "\nlast value to the CVT. WARNING- Insert mode is not recommended, as it will\r\nslo" +
        "w down future eDNA data retrieval.");
            this.radioButtonInsertConnect.UseVisualStyleBackColor = true;
            // 
            // radioButtonAppend
            // 
            this.radioButtonAppend.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonAppend.AutoSize = true;
            this.radioButtonAppend.Location = new System.Drawing.Point(4, 107);
            this.radioButtonAppend.Name = "radioButtonAppend";
            this.radioButtonAppend.Size = new System.Drawing.Size(62, 17);
            this.radioButtonAppend.TabIndex = 10;
            this.radioButtonAppend.Text = "Append";
            this.toolTipHelp.SetToolTip(this.radioButtonAppend, "This push mode will attempt to append all values to history. (Recommended)");
            this.radioButtonAppend.UseVisualStyleBackColor = true;
            // 
            // radioButtonInsert
            // 
            this.radioButtonInsert.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButtonInsert.AutoSize = true;
            this.radioButtonInsert.Location = new System.Drawing.Point(4, 140);
            this.radioButtonInsert.Name = "radioButtonInsert";
            this.radioButtonInsert.Size = new System.Drawing.Size(51, 17);
            this.radioButtonInsert.TabIndex = 11;
            this.radioButtonInsert.Text = "Insert";
            this.toolTipHelp.SetToolTip(this.radioButtonInsert, "This push mode will attempt to insert all values to histor. WARNING- Insert \r\nmod" +
        "e is not recommended, as it will slow down future eDNA data retrieval.");
            this.radioButtonInsert.UseVisualStyleBackColor = true;
            this.radioButtonInsert.CheckedChanged += new System.EventHandler(this.radioButtonInsert_CheckedChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.numericUpDown1, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 202);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(171, 25);
            this.tableLayoutPanel2.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Rounding Decimals:";
            this.toolTipHelp.SetToolTip(this.label5, "This setting will adjust the number of decimal places to round each simulation va" +
        "lue.");
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Other Settings";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 308);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(60, 17);
            this.checkBox1.TabIndex = 21;
            this.checkBox1.Text = "mgame";
            this.toolTipHelp.SetToolTip(this.checkBox1, "Optional minigame");
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Location = new System.Drawing.Point(12, 354);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(83, 23);
            this.buttonSaveSettings.TabIndex = 11;
            this.buttonSaveSettings.Text = "Save Settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Push_Tools.Properties.Settings.Default, "RoundDecimals", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown1.Location = new System.Drawing.Point(117, 3);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(51, 20);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.Value = global::Push_Tools.Properties.Settings.Default.RoundDecimals;
            // 
            // checkBoxDoublePass
            // 
            this.checkBoxDoublePass.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkBoxDoublePass.AutoSize = true;
            this.checkBoxDoublePass.Checked = global::Push_Tools.Properties.Settings.Default.DoublePass;
            this.checkBoxDoublePass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDoublePass.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Push_Tools.Properties.Settings.Default, "DoublePass", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxDoublePass.Location = new System.Drawing.Point(4, 272);
            this.checkBoxDoublePass.Name = "checkBoxDoublePass";
            this.checkBoxDoublePass.Size = new System.Drawing.Size(86, 17);
            this.checkBoxDoublePass.TabIndex = 2;
            this.checkBoxDoublePass.Text = "Double Pass";
            this.toolTipHelp.SetToolTip(this.checkBoxDoublePass, "Sometimes eDNA does not accept a data push the first time around. By\r\nperforming " +
        "a double pass, values are more likely to be written correctly.");
            this.checkBoxDoublePass.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 235);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(171, 25);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Sleep Time (ms)";
            this.toolTipHelp.SetToolTip(this.label2, "The amount of milliseconds to sleep after pushing each real-time \r\npoint (helps e" +
        "nsure the point is actually written).");
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Push_Tools.Properties.Settings.Default, "SleepTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown2.Location = new System.Drawing.Point(117, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(51, 20);
            this.numericUpDown2.TabIndex = 16;
            this.numericUpDown2.Value = global::Push_Tools.Properties.Settings.Default.SleepTime;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 389);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.tableLayoutPanel5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.Text = "User Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSettings_FormClosed);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonAppendConnect;
        private System.Windows.Forms.RadioButton radioButtonInsertConnect;
        private System.Windows.Forms.RadioButton radioButtonAppend;
        private System.Windows.Forms.RadioButton radioButtonInsert;
        private System.Windows.Forms.CheckBox checkBoxDoublePass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
    }
}