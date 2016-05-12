using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InStep.eDNA.EzDNAApiNet;
using Pull_Tools.Properties;

namespace Pull_Tools
{
    public partial class frmPullTools : Form
    {
        //Properties
        int numTagsFinished;
        int totalTags;
        List<TagPullThread> totalTagPull;
        //Initialization and general methods
        public frmPullTools()
        {
            InitializeComponent();
            Properties.Settings.Default.Out_Directory = Application.StartupPath + @"\Data";
            numTagsFinished = 0;
            totalTags = 0;
            this.totalTagPull = new List<TagPullThread>();
        }
        private void WriteToLogger(string log)
        {
            this.textBoxLogger.AppendText(DateTime.Now.ToString() + ": " + log + "\n");
        }       
        private void frmPull_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("eDNA Pull Tools Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
            ". Please see Eric Strong for any suggestions, comments, or bugs.",
            "Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        //Data loading
        private void toolStripButtonLoadFromCSV_Click(object sender, EventArgs e)
        {
            //If the user selects "ok" in the dialog, try to load the points from the file
            if (openFileCSVInput.ShowDialog() == DialogResult.OK)
            {
                this.DisableButtonsWhileRunning();
                backgroundWorkerLoadFromCSV.RunWorkerAsync();
            }
        }
        private void backgroundWorkerLoadFromCSV_DoWork(object sender, DoWorkEventArgs e)
        {           
            this.LoadFromCSV(openFileCSVInput.FileName);
        }
        private void backgroundWorkerLoadFromCSV_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.EnableButtonsWhileNotRunning();
            dataGridView1.AutoResizeColumns();
        }
        private void backgroundWorkerLoadFromCSV_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string progressstring = e.UserState.ToString();
            //If the first character is a $, then the progress update is for a particular point
            if (!string.IsNullOrEmpty(progressstring)) this.WriteToLogger(progressstring); 
        }  
        private void LoadFromCSV(string filename)
        {
            string extension = new FileInfo(filename).Extension;
            if (extension == ".csv")
            {
                backgroundWorkerLoadFromCSV.ReportProgress(0,"*Loading data from specified CSV input file...");
                //Open a Stream Reader and catch all the possible errors
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(filename)))
                    {
                        //Read each line of the csv file
                        int lineNum = 0;
                        while (!reader.EndOfStream)
                        {
                            lineNum++;
                            //Read the line and check that something exists
                            var line = reader.ReadLine();
                            if (String.IsNullOrWhiteSpace(line))
                            {
                                backgroundWorkerLoadFromCSV.ReportProgress(0, "Line " + lineNum.ToString() + " is empty");
                                continue;
                            }
                            //Add the line to the dataSet
                            this.CSVLineToDataSet(line, lineNum);
                        }
                    }
                    //Success!
                    backgroundWorkerLoadFromCSV.ReportProgress(0, "Tags successfully loaded from CSV input file.");
                }
                catch (FileNotFoundException) { backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR- CSV input file does not exist."); }
                catch (UnauthorizedAccessException) { backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR- access to CSV input file is denied. Check that it isn't currently open."); }
                catch (IOException) { backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR- access to CSV input file is denied. Check that it isn't currently open."); }
                catch { backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR- unspecified error. Please ask Eric Strong for help."); }
            }
            else { backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR- Input file is not in CSV format."); }
        }
        private void CSVLineToDataSet(string line, int lineNum)
        {
            var values = line.Split(',');
            //Load the specified Group (column 7)
            string group = String.Empty;
            if (values.Length > 6) { group = values[6]; }
            //Load the specified Value Filter (column 6)
            string valFilt = String.Empty;
            if (values.Length > 5 && !String.IsNullOrWhiteSpace(values[5]))
            {
                valFilt = values[5];
                if (valFilt.Split('|').Length < 2)
                {
                    backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR on line " + lineNum.ToString() + "- " + values[4] + " does not contain a lower and upper value for " +
                        "the definition of the value filter. Use '|' as a separator. No value filter loaded.");
                }
            }
            //Load the specified State Filter (column 5)
            string steadyFilt = String.Empty;
            if (values.Length > 4 && !String.IsNullOrWhiteSpace(values[4]))
            {
                steadyFilt = values[4];
                if (steadyFilt.Split('|').Length < 3)
                {
                    backgroundWorkerLoadFromCSV.ReportProgress(0, "WARNING on line " + lineNum.ToString() + "- " + values[4] + " does not contain a lower and upper value for " +
                        "the definition of the state filter. Use '|' as a separator. This program will assume that a value of 1 means the state is 'on'.");
                    steadyFilt = values[4] + "|1|1";
                }
            }
            //Load the specified End Date (column 4)
            DateTime enddate = DateTime.Now;
            if (values.Length > 3 && !String.IsNullOrWhiteSpace(values[3]))
            {
                try { enddate = Convert.ToDateTime(values[3]); }
                catch (FormatException)
                {
                    values[3] = String.IsNullOrWhiteSpace(values[3]) ? "String.Empty" : values[3];
                    backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR on line " + lineNum.ToString() + "- " + values[3] + " is not a recognized DateTime.");
                }
            }
            //Load the specified Start Date (column 3)
            DateTime startdate = DateTime.Parse("2001/01/01 00:01");
            if (values.Length > 2 && !String.IsNullOrWhiteSpace(values[2]))
            {
                try { startdate = Convert.ToDateTime(values[2]); }
                catch (FormatException)
                {
                    values[2] = String.IsNullOrWhiteSpace(values[2]) ? "String.Empty" : values[2];
                    backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR on line " + lineNum.ToString() + "- " + values[2] + " is not a recognized DateTime.");
                }
            }
            //Load the specified Description (column 1)
            string description = values[0].Trim();
            if (values.Length > 1 && !String.IsNullOrWhiteSpace(values[1])) description = values[1].Trim();
            //Load the specified eDNA Tag (column 1)
            string tag = values[0].Trim();
            //Check that the eDNA tag exists
            if (Configuration.DoesIdExist(tag) == 0)
            {
                backgroundWorkerLoadFromCSV.ReportProgress(0, "ERROR on line " + lineNum.ToString() + "- " + values[0] + " wasn't found in eDNA. Check that it is fully " +
                    "specified (site.service.pointID) and that an eDNA connection is available.");
            }
            //All the error-checking above should not throw an error under any situation. If a tag doesn't exist, the error will be handled again during data pull. 
            //If the startDate or endDate won't convert, or weren't specified, then the data pull reverts from DateTime.Min to DateTime.Now, which is a perfectly 
            //valid data pull.
            dataSetTagPull.Tables[0].Rows.Add(tag, description, startdate, enddate, steadyFilt, valFilt, group);
        }
        private void toolStripButtonLoadFromDNA_Click(object sender, EventArgs e)
        {
            this.LoadFromDNA();
        }     
        private void LoadFromDNA()
        {
            //Pop up an eDNA dialog to select multiple points
            this.WriteToLogger("*Loading points from eDNA...");
            string[] selectedPoints;
            int err = Configuration.DnaSelectPoints(out selectedPoints);
            //If err is 0, then points were actually selected
            if (err == 0)
            {
                foreach (string point in selectedPoints)
                {
                    //Iterate over each selected point to find the description
                    string outDesc = "";
                    int err2 = RealTime.DNAGetRTDesc(point, out outDesc);
                    //Add the row to the dataSet
                    dataSetTagPull.Tables[0].Rows.Add(point, outDesc, DateTime.Parse("2001/01/01 00:01"), DateTime.Now, String.Empty, String.Empty);
                }
                this.WriteToLogger("Points successfully loaded from eDNA.");
            }
            else { this.WriteToLogger("No points selected."); }
        }
        //Misc toolbar buttons
        private void toolStripButtonAdjustSettings_Click(object sender, EventArgs e)
        {
            //Open the settings dialog. All options are bound to program properties.
            Form frmset = new frmSettings();
            frmset.ShowDialog();
        }
        private void toolStripButtonAdjustDates_Click(object sender, EventArgs e)
        {
            //Open a new dialog to adjust all dates
            var frmdate = new frmDates();
            if (frmdate.ShowDialog() == DialogResult.OK)
            {
                //Iterate through each data row and replace the start and end dates with the ones selected in the form
                foreach (DataRow row in dataSetTagPull.Tables[0].Rows)
                {
                    row[2] = frmdate.startDate;
                    row[3] = frmdate.endDate;
                }
            }
            //Report what you just did in the logger
            dataGridView1.AutoResizeColumns();
            this.WriteToLogger("All dates adjusted from " + frmdate.startDate.ToString() + " to " + frmdate.endDate);
        }
        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            textBoxOutDir.Text = Application.StartupPath + @"\Data";
            if (folderBrowserOutDir.ShowDialog() == DialogResult.OK) textBoxOutDir.Text = folderBrowserOutDir.SelectedPath;
        }
        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            dataSetTagPull.Clear();
            this.WriteToLogger("Configuration cleared.");
        }
        private void toolStripButtonExportCSV_Click(object sender, EventArgs e)
        {
            //Build the directory (create if necessary) and file path names
            string directory = textBoxOutDir.Text;
            Directory.CreateDirectory(textBoxOutDir.Text);
            string filenameFormat = string.Format("Pull_Tools_Config_{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            //Use a stringbuilder to create CSV rows for every row in the dataSet
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dataSetTagPull.Tables[0].Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            //Write all of the configuration to text
            File.WriteAllText(fullFilename, sb.ToString());
            //Update the logger with what you did
            this.WriteToLogger("Config file exported to " + fullFilename);
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
            string filenameFormat = string.Format("Pull_Tools_Log_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            //Write all of the logger text to the file
            File.WriteAllText(fullFilename, textBoxLogger.Text);
            //Update the logger with what you did
            this.WriteToLogger("Log file exported to " + fullFilename);
        }
        //Intermediate functions
        public void DisableButtonsWhileRunning()
        {
            this.toolStripProgress.Value = 0;
            this.toolStripProgressLabel.Text = "Running...";
            this.toolStripButtonRunProgram.Enabled = false;
            this.toolStripButtonCancel.Enabled = true;
            this.toolStripButtonAdjustDates.Enabled = false;
            this.toolStripButtonLoadFromCSV.Enabled = false;
            this.toolStripButtonAdjustSettings.Enabled = false;
            this.toolStripButtonClear.Enabled = false;
            this.toolStripButtonExportCSV.Enabled = false;
            this.toolStripButtonLoadFromDNA.Enabled = false;
            this.dataGridView1.Enabled = false;
            this.textBoxOutDir.Enabled = false;
            this.buttonOutDir.Enabled = false;
        }
        public void EnableButtonsWhileNotRunning()
        {
            this.toolStripProgress.Value = 0;
            this.toolStripProgressLabel.Text = "Not Running";
            this.toolStripButtonRunProgram.Enabled = true;
            this.toolStripButtonCancel.Enabled = false;
            this.toolStripButtonAdjustDates.Enabled = true;
            this.toolStripButtonLoadFromCSV.Enabled = true;
            this.toolStripButtonAdjustSettings.Enabled = true;
            this.toolStripButtonClear.Enabled = true;
            this.toolStripButtonExportCSV.Enabled = true;
            this.toolStripButtonLoadFromDNA.Enabled = true;
            this.dataGridView1.Enabled = true;
            this.textBoxOutDir.Enabled = true;
            this.buttonOutDir.Enabled = true;
        }       
        //Run program  
        public StringBuilder WriteMetricsHeader(bool mean = true, bool min = true, bool max = true, bool linearRegression = true)
        {
            //Initialization
            var retSB = new StringBuilder();
            retSB.Append("PointTag,Description,# Points");
            //If selected, add each metric to the stringbuilder
            if (mean) { retSB.Append(",Mean"); }
            if (min) { retSB.Append(",Min"); }
            if (max) { retSB.Append(",Max"); }
            if (linearRegression) { retSB.Append(",Intercept,Slope"); }
            return retSB;
        }
        private List<TagPullThread> ConvertDataset()
        {
            var tempList = new List<TagPullThread>();
            int dataRowNum = 0;
            foreach (DataRow row in dataSetTagPull.Tables[0].Rows)
            {
                dataRowNum++;
                //Convert the specified Group (column 7)
                string group = string.Empty;
                if (!String.IsNullOrWhiteSpace(row[6].ToString())) { group = row[6].ToString(); }
                //Convert the specified State Filter (column 6)
                bool vFilter = false;
                double vLow = -99999.0;
                double vHigh = 99999.0;
                if (!String.IsNullOrWhiteSpace(row[5].ToString()))
                {
                    string[] vSplit = row[5].ToString().Split('|');
                    if (vSplit.Length < 1)
                    {
                        backgroundWorkerRunPull.ReportProgress(0, "WARNING on row " + dataRowNum.ToString() + "- (Value Filtering) Low value not supplied. Use '|' to separate the low and high filter values. " +
                            "Defaulting to value of -99999.");
                    }
                    else
                    {
                        try { vLow = Convert.ToDouble(vSplit[0]); }
                        catch (FormatException)
                        {
                            backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- (Value Filtering) Low value conversion error. Check that a number was supplied " +
                                "before the '|'. Defaulting to value of -99999.");
                        }
                    }
                    if (vSplit.Length < 2)
                    {
                        backgroundWorkerRunPull.ReportProgress(0, "WARNING on row " + dataRowNum.ToString() + "- (Value Filtering) High value not supplied. Use '|' to separate the low and high filter values. " +
                            "Defaulting to value of 99999.");
                    }
                    else
                    {
                        try { vLow = Convert.ToDouble(vSplit[1]); }
                        catch (FormatException)
                        {
                            backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- (Value Filtering) High value conversion error. Check that a number was supplied " +
                                "after the '|'. Defaulting to value of 99999.");
                        }
                    }
                }
                //Convert the specified State Filter (column 5)
                bool sFilter = false;
                string sTag = String.Empty;
                double sLow = 1;
                double sHigh = 1;
                if (!String.IsNullOrWhiteSpace(row[4].ToString()))
                {
                    string[] stateSplit = row[4].ToString().Split('|');
                    //Verify that high and low values were supplied
                    if (stateSplit.Length < 2)
                    {
                        backgroundWorkerRunPull.ReportProgress(0, "WARNING on row " + dataRowNum.ToString() + "- (State Filtering) Low value not supplied. Use '|' to separate the tag, low, and high filter. " +
                            "Defaulting to value of 1.");
                    }
                    else
                    {
                        try { sLow = Convert.ToDouble(stateSplit[1]); }
                        catch (FormatException)
                        {
                            backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- (State Filtering) Low value conversion error. Check that a number was supplied " +
                                "after the first '|'. Defaulting to value of 1.");
                        }
                    }
                    if (stateSplit.Length < 3)
                    {
                        backgroundWorkerRunPull.ReportProgress(0, "WARNING on row " + dataRowNum.ToString() + "- (State Filtering) High value not supplied. Use '|' to separate the tag, low, and high filter. " +
                            "Defaulting to value of 1.");
                    }
                    else
                    {
                        try { sHigh = Convert.ToDouble(stateSplit[2]); }
                        catch (FormatException)
                        {
                            backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- (State Filtering) High value conversion error. Check that a number was supplied " +
                                "after the second '|'. Defaulting to value of 1.");
                        }
                    }
                    //Check that the state definition tag exists
                    sTag = stateSplit[0];
                    if (Configuration.DoesIdExist(sTag) == 0)
                    {
                        backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- (State Filtering) " + sTag + " wasn't found in eDNA. Check that it is fully " +
                            "specified (site.service.pointID) and that an eDNA connection is available. State filter is disabled.");
                    }
                    else { sFilter = true; }
                }
                //Convert the specified End Date (column 4)
                DateTime enddate = DateTime.Now;
                try { enddate = Convert.ToDateTime(row[3].ToString()); }
                catch (FormatException) { backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- " + row[3] + " is not a recognized DateTime. Point skipped."); continue; }
                //Load the specified Start Date (column 3)
                DateTime startdate = DateTime.MinValue;
                try { startdate = Convert.ToDateTime(row[2].ToString()); }
                catch (FormatException) { backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- " + row[2] + " is not a recognized DateTime. Point skipped."); continue; }
                //Load the specified Description (column 1)
                string description = row[1].ToString();
                //Load the specified eDNA Tag (column 1)
                string tag = row[0].ToString();
                if (Configuration.DoesIdExist(tag) == 0)
                {
                    backgroundWorkerRunPull.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- " + tag + " wasn't found in eDNA. Check that it is fully " +
                        "specified (site.service.pointID) and that an eDNA connection is available. No data file will be written.");
                }
                //Add to the list of TagPullThread objects
                string outDir = Properties.Settings.Default.Out_Directory;
                tempList.Add(new TagPullThread(dataRowNum - 1, tag, description, outDir, startdate, enddate, group, sFilter, sTag, sLow, sHigh, vFilter, vLow, vHigh));
            }
            return tempList;
        }
        private void toolStripButtonRunProgram_Click(object sender, EventArgs e)
        {
            this.RunPull(); 
        }
        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerRunPull.IsBusy) backgroundWorkerRunPull.CancelAsync();
            if (backgroundWorkerLoadFromCSV.IsBusy) backgroundWorkerLoadFromCSV.CancelAsync();
        }     
        private void RunPull()
        {
            this.DisableButtonsWhileRunning();
            this.numTagsFinished = 0;
            //Converting the dataset from the GUI dataGridView to a list of TagPullThreads
            this.WriteToLogger("*Loading data grid configuration and checking for errors...");
            this.totalTagPull = this.ConvertDataset();
            this.totalTags = this.totalTagPull.Count;
            this.WriteToLogger("Configuration loaded successfully.");
            //Set the history timeout to the value specified by the user within the GUI. This should be roughly 1 hour to cover time periods where the services
            //restart, either automatically or as a result of an import script.
            this.WriteToLogger("*Attempting to set eDNA service history timeout...");
            try { History.SetHistoryTimeout(Convert.ToUInt32(Properties.Settings.Default.Pull_TimeoutPeriod)); }
            catch { this.WriteToLogger("ERROR- Setting eDNA service history timeout failed."); }
            //Set up the thread pool
            int maxThread = (int)Properties.Settings.Default.Pull_MaxThreads;            
            string maxThreadString = (maxThread > 0) ? maxThread.ToString() : "(automatic)";
            this.WriteToLogger("*Setting up the thread pool using a maximum of " + maxThreadString + " threads...");
            try { if (maxThread > 0) { ThreadPool.SetMaxThreads(maxThread, maxThread); } }
            catch { this.WriteToLogger("ERROR- Adjusting max number of threads failed."); }
            //Run the thread pool
            this.WriteToLogger( "*Running data pulls in parallel...");
            try { backgroundWorkerRunPull.RunWorkerAsync(); }
            catch
            {
                this.WriteToLogger("ERROR- unspecified error. Program failed. Please ask Eric Strong for help.");
                this.EnableButtonsWhileNotRunning();
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progPerc = e.ProgressPercentage;
            string progressstring = e.UserState.ToString();
            //If the first character is a $, then the progress update is for a particular point
            if (!string.IsNullOrEmpty(progressstring))
            {
                if (progressstring.StartsWith("$"))
                {
                    int pointIndex = Convert.ToInt32(progressstring.Substring(1));
                    //TO DO FUTURE
                }
                else { this.WriteToLogger(progressstring); }
            }
            //Update the overall progress bar
            if (progPerc == -9999)
            {
                numTagsFinished++;
                this.toolStripProgress.Value = (int)((double)this.numTagsFinished * 100.0 / (double)this.totalTags);
            }
        }        
        private void backgroundWorkerRunPull_DoWork(object sender, DoWorkEventArgs e)
        {
            //Validate the user settings
            int batchInterval = (int)Properties.Settings.Default.Pull_BatchingInterval;
            bool unrelFilter = Properties.Settings.Default.Pre_FilterUnreliable;
            PullMode pullMode = (PullMode)Properties.Settings.Default.Pull_Mode;
            TimeSpan interval = new TimeSpan(0,0,1);
            bool writeZip = Properties.Settings.Default.Out_ZipResults;
            try { interval = TimeSpan.Parse(Properties.Settings.Default.Pull_ModeInterval); }
            catch (FormatException) {backgroundWorkerRunPull.ReportProgress(0,"ERROR- Could not parse eDNA interval time span from user settings. Reverting to " +
                "default value of 1 second.");}
            //Select either metrics or a normal data pull
            if (Settings.Default.Metrics_Perform)
            {
                //Write the metrics header
                string filenameFormat = String.Format("Pull_ToolsMetrics_{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now);
                string filePath = Path.Combine(Settings.Default.Out_Directory, filenameFormat);
                File.AppendAllText(filePath, this.WriteMetricsHeader().ToString() + "\n");
                //Don't use paralleling... we will be writing to the same file, not thread safe?
                foreach (TagPullThread tpt in this.totalTagPull)
                {
                    backgroundWorkerRunPull.ReportProgress(0, "Beginning metrics calculation for " + tpt.Tag + "...");
                    var startPullTime = DateTime.Now;                   
                    this.PullMetrics(tpt, batchInterval, unrelFilter, pullMode, interval, writeZip, filePath);
                    //Report that the data pull is finished
                    double minutesElapsed = (DateTime.Now - startPullTime).TotalMinutes;
                    backgroundWorkerRunPull.ReportProgress(-9999, "Metrics calculation for " + tpt.Tag + " completed in " + Math.Round(minutesElapsed, 2).ToString() + " minutes.");              
                }
            }
            else
            {
                //Start the data pull in parallel
                Parallel.ForEach(this.totalTagPull, (tpt, loopState) =>
                {
                    if (backgroundWorkerRunPull.CancellationPending) loopState.Stop();
                    backgroundWorkerRunPull.ReportProgress(0, "Beginning data pull for " + tpt.Tag + "...");
                    var startPullTime = DateTime.Now;
                    this.PullData(tpt, batchInterval, unrelFilter, pullMode, interval, writeZip); 
                    //Report that the data pull is finished
                    double minutesElapsed = (DateTime.Now - startPullTime).TotalMinutes;
                    backgroundWorkerRunPull.ReportProgress(-9999, "Data pull for " + tpt.Tag + " completed in " + Math.Round(minutesElapsed, 2).ToString() + " minutes.");
                });
            }
        }
        private void PullData(TagPullThread tpt, int batchInterval, bool unrelFilter, PullMode pullMode, TimeSpan interval, bool zipResults)
        {          
            //Set up the progress reporting
            double completedDays = 0.0;
            double totalDays = (tpt.EndDate - tpt.StartDate).TotalDays;
            backgroundWorkerRunPull.ReportProgress(0, "$" + tpt.Index.ToString());
            //Write the point header
            tpt.WritePointHeader(Settings.Default.Out_WriteStatus);
            //Iterate over each day          
            for (DateTime currentStartDate = tpt.StartDate; currentStartDate < tpt.EndDate; currentStartDate = currentStartDate.AddDays(batchInterval))
            {
                //Initialization (save the starting time, find the ending date for the pull, check for cancellation)
                DateTime startDayPull = DateTime.Now;
                DateTime currentEndDate = (currentStartDate.AddDays(batchInterval) > tpt.EndDate) ? tpt.EndDate : currentStartDate.AddDays(batchInterval);
                if (backgroundWorkerRunPull.CancellationPending) { break; }
                //Do the incremental data pull
                string retDesc = String.Empty;
                List<PointData> retList = new List<PointData>();
                tpt.PullData(currentStartDate, currentEndDate, unrelFilter, pullMode, interval, out retDesc, out retList);
                tpt.WriteDataToCSV(retList);
                //Update the progress for each point
                completedDays += batchInterval;
                int curProgress = (int)(completedDays / totalDays);
                backgroundWorkerRunPull.ReportProgress(curProgress, "$" + tpt.Index.ToString());
            }
            //Compress the file, if selected
            if (zipResults && File.Exists(tpt.OutputFilename))
            {
                var fileToCompress = new FileInfo(tpt.OutputFilename);
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {                
                    if ((File.GetAttributes(fileToCompress.FullName) &
                       FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);
                            }
                        }
                        string dirName = Path.GetDirectoryName(tpt.OutputFilename);
                        FileInfo info = new FileInfo(dirName + "\\" + fileToCompress.Name + ".gz");
                        backgroundWorkerRunPull.ReportProgress(0, String.Format("Compressed {0} from {1} to {2} bytes.",
                             fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString()));
                    }
                }
                File.Delete(tpt.OutputFilename);
            }
        }
        private void PullMetrics(TagPullThread tpt, int batchInterval, bool unrelFilter, PullMode pullMode, TimeSpan interval, bool zipResults, string filename)
        {
            //Set up the progress reporting
            double completedDays = 0.0;
            double totalDays = (tpt.EndDate - tpt.StartDate).TotalDays;
            backgroundWorkerRunPull.ReportProgress(0, "$" + tpt.Index.ToString());
            //Do the incremental data pull
            string retDesc = String.Empty;
            List<PointData> retList = new List<PointData>();
            tpt.PullData(tpt.StartDate,tpt.EndDate, unrelFilter, pullMode, interval, out retDesc, out retList);
            StringBuilder sb = tpt.WriteMetricsToString(retList, (int) Settings.Default.Metrics_AnalysisWindow, Settings.Default.Metrics_CalcMean,
                Settings.Default.Metrics_CalcMin, Settings.Default.Metrics_CalcMax,Settings.Default.Metrics_CalcLinear);
            File.AppendAllText(filename, sb.ToString());
            //Update the progress for each point
            completedDays += batchInterval;
            int curProgress = (int)(completedDays / totalDays);
            backgroundWorkerRunPull.ReportProgress(curProgress, "$" + tpt.Index.ToString());
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.WriteToLogger("All data pulls finished.");
            this.EnableButtonsWhileNotRunning();
        }             
    }
}
