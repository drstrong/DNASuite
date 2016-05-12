using System;
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
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }
        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void CheckForInsert()
        {
            DialogResult result = MessageBox.Show("Do you know what you're doing?",
                       "The Ultimate Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Cancel)
            {
                frmMiniGame mg = new frmMiniGame();
                mg.Show();
            }
            else if (result == DialogResult.Yes)
            {
            }
            else
            {
                radioButtonInsert.Checked = false;
                radioButtonAppendConnect.Checked = true;
            }
        }
        private void radioButtonInsert_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonInsert.Checked && checkBox1.Checked) this.CheckForInsert();
        }
        private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.OutputType = (int)WriteType.AppendConnect;
            if (radioButtonInsertConnect.Checked) { Properties.Settings.Default.OutputType = (int)WriteType.InsertConnect; }
            else if (radioButtonAppend.Checked) { Properties.Settings.Default.OutputType = (int)WriteType.AppendHistory; }
            else if (radioButtonInsert.Checked) { Properties.Settings.Default.OutputType = (int)WriteType.InsertHistory; }
        }
        //Data structures
        /// <summary> A list of potential data writing types. Refer to eDNA documentation for more information.</summary>
        public enum WriteType
        {
            /// <summary>This data writing mode will append values to history, then write the last point to the CVT.</summary>
            AppendConnect,
            /// <summary>This data writing mode will insert values to history, then write the last point to the CVT.</summary>
            InsertConnect,
            /// <summary>This data writing mode will append values to history.</summary>
            AppendHistory,
            /// <summary>This data writing mode will insert values to history.</summary>
            InsertHistory,
            /// <summary>This data writing mode will write values to a file.</summary>
            WriteToFile
        }
    }
}
