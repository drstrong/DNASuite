namespace Push_Tools
{
    partial class frmDates
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
            this.dateTimeStartDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimeEndDate = new System.Windows.Forms.DateTimePicker();
            this.buttonAdjustAllDatesFromForm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimeStartDate
            // 
            this.dateTimeStartDate.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dateTimeStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStartDate.Location = new System.Drawing.Point(13, 13);
            this.dateTimeStartDate.Name = "dateTimeStartDate";
            this.dateTimeStartDate.Size = new System.Drawing.Size(151, 20);
            this.dateTimeStartDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "to";
            // 
            // dateTimeEndDate
            // 
            this.dateTimeEndDate.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dateTimeEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEndDate.Location = new System.Drawing.Point(196, 13);
            this.dateTimeEndDate.Name = "dateTimeEndDate";
            this.dateTimeEndDate.Size = new System.Drawing.Size(150, 20);
            this.dateTimeEndDate.TabIndex = 2;
            // 
            // buttonAdjustAllDatesFromForm
            // 
            this.buttonAdjustAllDatesFromForm.Location = new System.Drawing.Point(352, 12);
            this.buttonAdjustAllDatesFromForm.Name = "buttonAdjustAllDatesFromForm";
            this.buttonAdjustAllDatesFromForm.Size = new System.Drawing.Size(92, 23);
            this.buttonAdjustAllDatesFromForm.TabIndex = 3;
            this.buttonAdjustAllDatesFromForm.Text = "Adjust All Dates";
            this.buttonAdjustAllDatesFromForm.UseVisualStyleBackColor = true;
            this.buttonAdjustAllDatesFromForm.Click += new System.EventHandler(this.buttonAdjustAllDatesFromForm_Click);
            // 
            // frmDates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 47);
            this.Controls.Add(this.buttonAdjustAllDatesFromForm);
            this.Controls.Add(this.dateTimeEndDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimeStartDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDates";
            this.Text = "Adjust All Dates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeStartDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimeEndDate;
        private System.Windows.Forms.Button buttonAdjustAllDatesFromForm;
    }
}