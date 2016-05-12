using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Run_Hour_Review.Properties;
using InStep;
using InStep.eDNA.EzDNAApiNet;
using Run_Hour_Review.OpStateTools;
using Run_Hour_Review.PushTools;
using Run_Hour_Review.PullTools;
using Run_Hour_Review.DataReview;
using Excel = ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Run_Hour_Review
{
    //THIS PROGRAM NEEDS MAJOR WORK. I MADE IT WAY TOO COMPLICATED. SORRY.
    public partial class frmRunHourReview : Form
    {
        //Initialization and general methods
        public frmRunHourReview()
        {
            InitializeComponent();
            Settings.Default.StartDate = DateTime.Now.Date;
            Settings.Default.EndDate = DateTime.Now.Date;
            Settings.Default.OutDirectory = Application.StartupPath;
        }
        private void WriteToLogger(string log)
        {
            this.textBoxLogger.AppendText(DateTime.Now.ToString() + ": " + log + "\n");
        }
        private void frmRunHourReview_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("eDNA Run Hour Review Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
            ". Please see Eric Strong for any suggestions, comments, or bugs.",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        //Misc toolbar buttons
        private void toolStripButtonAdjustSettings_Click(object sender, EventArgs e)
        {
            //Open the settings dialog. All options are bound to program properties.
            Form frmset = new frmSettings();
            frmset.ShowDialog();
        }
        private void toolStripButtonClearLog_Click(object sender, EventArgs e)
        {
            textBoxLogger.Clear();
        }
        private void toolStripButtonExportLog_Click(object sender, EventArgs e)
        {
            //Build the directory (create if necessary) and file path names
            string directory = textBoxOutDir.Text;
            Directory.CreateDirectory(textBoxOutDir.Text);
            string filenameFormat = string.Format("DNAPush_Log_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            //Write all of the logger text to the file
            File.WriteAllText(fullFilename, textBoxLogger.Text);
            //Update the logger with what you did
            this.WriteToLogger("Log file exported to " + fullFilename);
        }
        private void buttonOutputDir_Click(object sender, EventArgs e)
        {
            textBoxOutDir.Text = Application.StartupPath;
            if (folderBrowserOutDir.ShowDialog() == DialogResult.OK) textBoxOutDir.Text = folderBrowserOutDir.SelectedPath;
        }
        private void buttonPullSpreadsheet_Click(object sender, EventArgs e)
        {
            if (openPullSpreadsheet.ShowDialog() == DialogResult.OK)
                textBoxPullSpreadsheet.Text = openPullSpreadsheet.FileName;
            else
                textBoxPullSpreadsheet.Text = "";
        }
        private void buttonPushSpreadsheet_Click(object sender, EventArgs e)
        {
            if (openPushSpreadsheet.ShowDialog() == DialogResult.OK)
                textBoxPushSpreadsheet.Text = openPushSpreadsheet.FileName;
            else
                textBoxPushSpreadsheet.Text = "";
        }
        //Intermediate Functions
        private void DisableWhileRunning()
        {
            this.toolStripProgressLabel.Text = "Running...";
            this.toolStripButtonAdjustSettings.Enabled = false;
            this.toolStripButtonCancel.Enabled = true;
            this.toolStripButtonRunProgram.Enabled = false;
        }
        private void EnableWhileNotRunning()
        {
            this.toolStripProgress.Value = 0;
            this.toolStripProgressLabel.Text = "Not Running";
            this.toolStripButtonAdjustSettings.Enabled = true;
            this.toolStripButtonCancel.Enabled = false;
            this.toolStripButtonRunProgram.Enabled = true;
        }
        //Run program
        private void toolStripButtonRunProgram_Click(object sender, EventArgs e)
        {
            this.DisableWhileRunning();
            backgroundWorker1.RunWorkerAsync();
        }
        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.Default.PullData) 
            {
                backgroundWorker1.ReportProgress(0, "*Starting data pull...");
                this.PullData();
            }
            else 
            {
                backgroundWorker1.ReportProgress(0, "*Starting data push...");
                this.PushData();
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Update the progress bar
            if (e.ProgressPercentage > 0 && e.ProgressPercentage <= 100)
            {
                this.toolStripProgress.Value = e.ProgressPercentage;
            }
            //Update the logger
            string progressstring = e.UserState.ToString();
            if (!string.IsNullOrEmpty(progressstring))
            {
                this.WriteToLogger(progressstring);
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.EnableWhileNotRunning();
        }
        //Pull Data
        private void PullData()
        {
            DateTime startReview = DateTime.Now;

            //Save the user-supplied values from the main Windows Form.
            DateTime startDate = Settings.Default.StartDate;
            DateTime endDate = Settings.Default.EndDate;
            TimeSpan granularPeriod = TimeSpan.FromHours((double)Settings.Default.TimePeriod);
            FileInfo inputPath = new FileInfo(Settings.Default.InputPath);

            //Check for user input errors.
            backgroundWorker1.ReportProgress(1, "*Starting data review (pull) at " + startReview.ToString() + "...\n");
            if (this.PullErrorCheck())
            {
                //Try to read the input CSV data file.
                backgroundWorker1.ReportProgress(1, "*Reading from input file...\n");
                var overallReview = new OverallDataReview(inputPath.FullName);

                //Initialization
                int totalReviews = overallReview.RunHourReviewList.Count + overallReview.StressEventReviewList.Count + overallReview.StressHourReviewList.Count;
                int currentReviewCount = 0;

                //Accomplish the run hour review.
                backgroundWorker1.ReportProgress(0, "*Calculating run hours summary...\n");
                var runHourSummaryList = new List<RunHourOverallSummary>();
                foreach (RunHourReview currentReview in overallReview.RunHourReviewList)
                {
                    DateTime startPull = DateTime.Now;
                    runHourSummaryList.Add(overallReview.RunSingleRunHourReview(currentReview,startDate,endDate,granularPeriod));
                    //Update the logger
                    TimeSpan pullSeconds = DateTime.Now - startPull;
                    backgroundWorker1.ReportProgress((int)(((double)currentReviewCount * 100.0) / (double)totalReviews),
                        currentReview.Description + "(" + currentReview.OutputTag + ") completed in " + pullSeconds.TotalSeconds.ToString() + " seconds\n");
                    currentReviewCount++;
                }

                //Accomplish the stress hour review
                backgroundWorker1.ReportProgress(0, "*Calculating stress hours summary...\n");
                var stressHourSummaryList = new List<StressHourOverallSummary>();
                foreach (StressHourReview currentReview in overallReview.StressHourReviewList)
                {
                    DateTime startPull = DateTime.Now;
                    stressHourSummaryList.Add(overallReview.RunSingleStressHourReview(currentReview, startDate, endDate, granularPeriod));
                    //Update the logger
                    TimeSpan pullSeconds = DateTime.Now - startPull;
                    backgroundWorker1.ReportProgress((int)(((double)currentReviewCount * 100.0) / (double)totalReviews),
                        currentReview.Description + "(" + currentReview.OutputTag + ") completed in " + pullSeconds.TotalSeconds.ToString() + " seconds\n");
                    currentReviewCount++;
                }

                //Accomplish the stress event review
                backgroundWorker1.ReportProgress(0, "*Calculating stress events summary...\n");
                var stressEventSummaryList = new List<StressEventOverallSummary>();
                foreach (StressEventReview currentReview in overallReview.StressEventReviewList)
                {
                    DateTime startPull = DateTime.Now;
                    stressEventSummaryList.Add(overallReview.RunSingleStressEventReview(currentReview, startDate, endDate, granularPeriod));
                    //Update the logger
                    TimeSpan pullSeconds = DateTime.Now - startPull;
                    backgroundWorker1.ReportProgress((int)(((double)currentReviewCount * 100.0) / (double)totalReviews),
                        currentReview.Description + "(" + currentReview.StressEventTag + ") completed in " + pullSeconds.TotalSeconds.ToString() + " seconds\n");
                    currentReviewCount++;
                }

                //Create a new overall summary instance.
                var overallSummary = new OverallDataReviewSummary(startDate, endDate, granularPeriod, runHourSummaryList, stressEventSummaryList, stressHourSummaryList);

                //Write the data review to a file.
                backgroundWorker1.ReportProgress(0, "*Creating directory...\n");
                string newDirectory = Settings.Default.OutDirectory;
                Directory.CreateDirectory(newDirectory);
                string fileName = Path.GetFileNameWithoutExtension(inputPath.Name) + "_" + startDate.ToString("yyyyMMdd") + "-" + endDate.ToString("yyyyMMdd") + ".xlsx";
                string workbookPath = Path.Combine(Settings.Default.OutDirectory, fileName);
                backgroundWorker1.ReportProgress(0, "*Writing values to " + workbookPath + "...\n");
                this.WriteReviewToExcel(workbookPath, overallSummary);  

                //Finish the data review summary
                TimeSpan totalSeconds = DateTime.Now - startReview;
                backgroundWorker1.ReportProgress(0, "*Data review finished in " + totalSeconds.TotalSeconds.ToString() + " seconds\n");
            }
            else
            {
                backgroundWorker1.ReportProgress(0, "-ERROR- Invalid user input.\n");
            }
        }
        private void WriteReviewToExcel(string filePath, OverallDataReviewSummary odrs)
        {
            //Construct the workbook and worksheets.
            var workbook = new Excel.XLWorkbook();
            var machineHourSheet = workbook.AddWorksheet("Machine Hours");
            var stressHourSheet = workbook.AddWorksheet("Stress Hours (SEVV)");
            var stressHourEtaReductionSheet = workbook.AddWorksheet("SEVV Eta Reduction");
            var stressEventSheet = workbook.AddWorksheet("Stress Events (SEV)");
            var stressEventEtaReductionSheet = workbook.AddWorksheet("SEV Eta Reduction");

            //Write values to the sheets
            backgroundWorker1.ReportProgress(0, "Writing run hours...\n");
            machineHourSheet = odrs.WriteMachineHourValues(machineHourSheet);
            backgroundWorker1.ReportProgress(0, "Writing stress hours...\n");
            stressHourSheet = odrs.WriteStressHourValues(stressHourSheet);
            backgroundWorker1.ReportProgress(0, "Writing stress hour eta reduction...\n");
            stressHourEtaReductionSheet = odrs.WriteStressHourEtaReductionValues(stressHourEtaReductionSheet);
            backgroundWorker1.ReportProgress(0, "Writing stress events...\n");
            stressEventSheet = odrs.WriteStressEventValues(stressEventSheet);
            backgroundWorker1.ReportProgress(0, "Writing stress event eta reduction...\n");
            stressEventEtaReductionSheet = odrs.WriteStressEventEtaReductionValues(stressEventEtaReductionSheet);

            //Save to a file
            backgroundWorker1.ReportProgress(0, "Writing to file...\n");
            File.Delete(filePath);
            workbook.SaveAs(filePath);
        }
        private bool PullErrorCheck()
        {
            Settings.Default.InputPath = Settings.Default.InputPath.Trim();
            //Check that the Start Time is Before the End Time
            if (Settings.Default.StartDate >= Settings.Default.EndDate)
            {
                MessageBox.Show("Error- the start time is earlier than the end time.",
                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            //Check that a points file was selected
            if (String.IsNullOrWhiteSpace(Settings.Default.InputPath))
            {
                MessageBox.Show("Error- run hour input file not selected.",
                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            //Check that the points file exists
            if (!File.Exists(Settings.Default.InputPath))
            {
                MessageBox.Show("Error- run hour input file does not exist.",
                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            return true;
        }
        //Push Data
        private void PushData()
        {
            DateTime startPush= DateTime.Now;
            FileInfo inputPath = new FileInfo(Settings.Default.PushPath);

            //Check for user input errors.
            backgroundWorker1.ReportProgress(1, "*Starting data review (push) at " + startPush.ToString() + "...\n");
            if (this.PushErrorCheck())
            {
                //Try to read the input CSV data file.
                backgroundWorker1.ReportProgress(0, "*Reading from input file...\n");
                var dataSummary = new OverallDataReviewSummary(inputPath.FullName);

                backgroundWorker1.ReportProgress(0, "*Pushing data to eDNA...\n");
                dataSummary.PushToEdna();

                double pushTime = (DateTime.Now - startPush).TotalSeconds;
                backgroundWorker1.ReportProgress(0, "*Data push complete in " + pushTime.ToString() + " seconds.\n");
            }
            else
            {
                backgroundWorker1.ReportProgress(0, "-ERROR- Invalid user input.\n");
            }
        }
        private bool PushErrorCheck()
        {
            Settings.Default.PushPath = Settings.Default.PushPath.Trim();
            //Check that a points file was selected
            if (String.IsNullOrWhiteSpace(Settings.Default.PushPath))
            {
                MessageBox.Show("Error- run hour push file not selected.",
                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            //Check that the points file exists
            if (!File.Exists(Settings.Default.PushPath))
            {
                MessageBox.Show("Error- run hour push file does not exist.",
                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            return true;
        }
    }
}
