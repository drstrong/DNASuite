using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data_Flow_Analyzer.Properties;

namespace Data_Flow_Analyzer
{
    public partial class frmDataFlow : Form
    {    
        //This is all just initialization, buttons, controls, etc., it's really boring, and trust me, you can skip it all unless you are REALLY interested
        //The actual "meat" of the program is contained in the AnalysisClasses.cs file
        public frmDataFlow()
        {
            InitializeComponent();
            //This is necessary to declare at the start of the program because the startup path is not known at compile time
            Properties.Settings.Default.OutDir = Application.StartupPath;
            //Again, the current time is not known at compile time
            Properties.Settings.Default.OutFilename = String.Format("{0:yyyy-MM-dd}_DataFlowHeaders.csv", DateTime.Now);
        }
        internal void WriteToLogger(string log) { this.textBoxLogger.AppendText(DateTime.Now.ToString() + ": " + log + "\n"); }
        internal void frmDataFlow_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            //Boring, boring popup
            MessageBox.Show("eDNA Data Flow Analyzer Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
            ". Please see Eric Strong for any suggestions, comments, or bugs.",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        internal void toolStripButtonAdjustSettings_Click(object sender, EventArgs e)
        {
            //This will open the settings dialog. All options are bound to program properties.
            Form frmset = new frmSettings();
            frmset.ShowDialog();
        }
        internal void toolStripButtonClearLog_Click(object sender, EventArgs e)
        {
            textBoxLogger.Clear();
        }
        internal void toolStripButtonExportLog_Click(object sender, EventArgs e)
        {
            //Build the directory (create if necessary) and file path names
            string directory = textBoxOutDir.Text;
            Directory.CreateDirectory(textBoxOutDir.Text);
            string filenameFormat = String.Format("Data_Flow_Analyzer_Log_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            File.WriteAllText(fullFilename, textBoxLogger.Text);
            this.WriteToLogger(String.Format("Log file exported to {0}",fullFilename));
        }
        internal void buttonOutputDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserOutput.ShowDialog() == DialogResult.OK){ textBoxOutDir.Text = folderBrowserOutput.SelectedPath;}
        }
        internal void toolStripButtonSelectData_Click(object sender, EventArgs e)
        {
            //If we are selecting new data files, we want to make sure the old list (and listBox) is cleared to make way for the FUTURE!
            listBoxFiles.Items.Clear();
            //Allow the user to select new data files, and then add them to the listBox
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string fn in openFileDialog1.FileNames) listBoxFiles.Items.Add(fn);
            }
        }
        internal void DisableWhileRunning()
        {
            //We want to disable a lot of buttons while the program is actually running, to avoid threading problems. Obviously, the
            //"RunProgram" button is the most important one to disable.
            this.toolStripProgress.Text = "Running...";
            this.toolStripButtonAdjustSettings.Enabled = false;
            this.toolStripButtonCancel.Enabled = true;
            this.toolStripButtonRunProgram.Enabled = false;
            this.toolStripButtonSelectData.Enabled = false;
        }
        internal void EnableWhileNotRunning()
        {
            //Once we're finished with the program, the various buttons can be re-enabled.
            this.toolStripProgressBarFiles.Value = 0;
            this.toolStripProgress.Text = "Not Running";
            this.toolStripButtonAdjustSettings.Enabled = true;
            this.toolStripButtonCancel.Enabled = false;
            this.toolStripButtonRunProgram.Enabled = true;
            this.toolStripButtonSelectData.Enabled = true;
        }
        internal void toolStripButtonRunProgram_Click(object sender, EventArgs e)
        {         
            //There are some buttons that should be disabled while running
            this.DisableWhileRunning();
            //I know this type of try-catch loop is not ideal, but it's mostly a fail-safe. I'll try to catch most of the errors within the actual run method.
            try { backgroundWorker1.RunWorkerAsync(); }
            catch (Exception ex)
            {
                this.WriteToLogger(String.Format("ERROR- unspecified error ({0}). Program failed. Please ask Eric Strong for help.",ex.Message));
                //This is easy to forget, but even if the program fails, we want to make sure to re-enable the buttons. Otherwise, the buttons will be re-enabled
                //at the end of program execution
                this.EnableWhileNotRunning();
            }
        }
        internal void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            //Nice job sherlock, if we click the "Cancel" button then the background worker should cancel
            backgroundWorker1.CancelAsync();
        }
        internal void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Okay, so I use this function differently in my programs.
            int progPerc = e.ProgressPercentage;
            string progressstring = e.UserState.ToString();
            //If the first character is a $, then the progress update is for a particular point
            if (!string.IsNullOrEmpty(progressstring)) this.WriteToLogger(progressstring);
            //Update the overall progress bar
            if (progPerc > 0 && progPerc <= 100) this.toolStripProgressBarFiles.Value = progPerc;
        }
        internal void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.WriteToLogger("Data file analysis complete.");
            toolStripProgressBarFiles.Value = 0;
            //Dont forget to enable the buttons after the program is done running. This has to be done in the "RunWorkerCompleted"
            //and not in the main program loop, because the main program loop will execute to the end, only threading the backgroundWorker
            this.EnableWhileNotRunning();
        }   
        //The main flow of the program
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0, "*Starting data file analysis...");
            var dfa = new DataFlowAnalysis(Settings.Default.OutDir, Settings.Default.OutFilename, listBoxFiles.Items);
            this.PerformAnalysis(dfa);    
        }
        public void PerformAnalysis(DataFlowAnalysis dfa)
        {
            using (StreamWriter outfile = new StreamWriter(dfa.OutputPath))
            {
                //This is the header for the CSV output file
                outfile.WriteLine(String.Join(",", "Filename", "Site.Service", "Service DateTime", "Size (MB)", "Last Modified"));                              
                //Why do I use a for loop instead of foreach? I need the iterator for the progress bar.
                for (int curDFIndex = 0; curDFIndex < dfa.TotalDataFiles(); curDFIndex++)
                {
                    DataFile curDF = dfa.DataFileList[curDFIndex];                   
                    //There's only a few opportunities to check for a pending cancellation, I take it where I can get it
                    if (backgroundWorker1.CancellationPending) break;
                    //This code looks nasty. The current value of the iterator needs to be divided by the total count of files to analyze to get the decimal percentage, but that
                    //needs to be a double or else it's truncated. Then, it's multiplied by 100 to get the percentage, which now needs to be supplied as an int to the ReportProgress
                    int progPerc = (int)(((double)curDFIndex * 100.0 / (double)dfa.TotalDataFiles()));
                    //Why do I supply two parameters to the ReportProgress? You can look at the method itself for more information, but basically I want to update both the 
                    //progress bar (parameter 1) and the logger (parameter 2) at the same time. If the second parameter is empty, the logger isn't updated, and if the first 
                    //parameter isn't between 0 and 100, the progress bar will not be updated.
                    backgroundWorker1.ReportProgress(progPerc, String.Format("Analyzing {0}...", curDF.Filename));
                    //Now return a string to be written to the data file
                    outfile.WriteLine(curDF.AnalyzeArchive(Settings.Default.DataPrefix,Settings.Default.OutDir,Settings.Default.ExtractLogs));
                }
            }
        }     
    }
}
