﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Push_Tools
{
    public partial class frmDates : Form
    {
        //Properties
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        //Initialization
        public frmDates()
        {
            InitializeComponent();
        }
        //On closing
        private void buttonAdjustAllDatesFromForm_Click(object sender, EventArgs e)
        {
            this.startDate = dateTimeStartDate.Value;
            this.endDate = dateTimeEndDate.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
