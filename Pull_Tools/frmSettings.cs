using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pull_Tools.Properties;

namespace Pull_Tools
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
        private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Pull_Mode = (int)PullMode.Raw;
            if (radioButtonSnap.Checked) { Properties.Settings.Default.Pull_Mode = (int) PullMode.Snap; }
            else if (radioButtonAvg.Checked) { Properties.Settings.Default.Pull_Mode = (int) PullMode.Avg; }
            else if (radioButtonMax.Checked) { Properties.Settings.Default.Pull_Mode = (int) PullMode.Max; }
            else if (radioButtonMin.Checked) { Properties.Settings.Default.Pull_Mode = (int) PullMode.Min; }
        }
        private void checkBoxSingleFile_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>An enum of each eDNA option to pull data. For more information about each mode,please refer to eDNA documentation.</summary>
    public enum PullMode
    {
        ///<summary>Pulls raw values from eDNA.</summary>
        Raw,
        ///<summary>Pulls data in "Snap" mode- takes the last written value from eDNA before the current time.</summary>
        Snap,
        ///<summary>Accumulates values into a total.</summary>
        Totals,
        ///<summary>Finds the minimum value over the time period selected.</summary>
        Min,
        ///<summary>Finds the maximum value over the time period selected.</summary>
        Max,
        ///<summary>Finds the average value over the time period selected.</summary>
        Avg   
    };
}
