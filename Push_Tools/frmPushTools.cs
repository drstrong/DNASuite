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
using InStep;
using InStep.eDNA.EzDNAApiNet;
using Push_Tools.Properties;
using MathNet.Filtering;
using MathNet.Numerics;

namespace Push_Tools
{
    public partial class frmPushTools : Form
    {
        //This is all just initialization, buttons, controls, etc., it's really boring, and trust me, you can skip it all unless you are REALLY interested
        public frmPushTools()
        {
            InitializeComponent();
            Properties.Settings.Default.OutDirectory = Application.StartupPath;
        }
        internal void WriteToLogger(string log) { this.textBoxLogger.AppendText(DateTime.Now.ToString() + ": " + log + "\n");}
        internal void frmPushTools_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("eDNA Push Tools Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                ". Please see Eric Strong for any suggestions, comments, or bugs.",
                "Help", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        internal void buttonOutDir_Click(object sender, EventArgs e)
        {
            textBoxOutDir.Text = Application.StartupPath;
            if (folderBrowserOutDir.ShowDialog() == DialogResult.OK) textBoxOutDir.Text = folderBrowserOutDir.SelectedPath;
        }
        internal void toolStripButtonLoadFromCSV_Click(object sender, EventArgs e)
        {
            //If the user selects "ok" in the dialog, try to load the points from the file
            if (openFileCSVInput.ShowDialog() == DialogResult.OK) this.LoadFromCSV(openFileCSVInput.FileName);
            dataGridView1.AutoResizeColumns();
        }     
        internal void toolStripButtonLoadFromDNA_Click(object sender, EventArgs e)
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
                    dataSet1.Tables[0].Rows.Add(point, outDesc, DateTime.Now, DateTime.Now, 60, String.Empty, String.Empty);
                }
                this.WriteToLogger("Points successfully loaded from eDNA.");
            }
            else { this.WriteToLogger("No points selected."); }
        }
        internal void toolStripButtonAdjustSettings_Click(object sender, EventArgs e)
        {
            //Open the settings dialog. All options are bound to program properties.
            Form frmset = new frmSettings();
            frmset.ShowDialog();
        }
        internal void toolStripButtonAdjustDates_Click(object sender, EventArgs e)
        {
            //Open a new dialog to adjust all dates
            var frmdate = new frmDates();
            if (frmdate.ShowDialog() == DialogResult.OK)
            {
                //Iterate through each data row and replace the start and end dates with the ones selected in the form
                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    row[2] = frmdate.startDate;
                    row[3] = frmdate.endDate;
                }
            }
            //Report what you just did in the logger
            dataGridView1.AutoResizeColumns();
            this.WriteToLogger("All dates adjusted from " + frmdate.startDate.ToString() + " to " + frmdate.endDate);
        }
        internal void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            dataSet1.Clear();
            this.WriteToLogger("Configuration cleared.");
        }
        internal void toolStripButtonExportCSV_Click(object sender, EventArgs e)
        {
            //Build the directory (create if necessary) and file path names
            string directory = textBoxOutDir.Text;
            Directory.CreateDirectory(textBoxOutDir.Text);
            string filenameFormat = string.Format("Push_Tools_Config_{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            //Use a stringbuilder to create CSV rows for every row in the dataSet
            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            //Write all of the configuration to text
            File.WriteAllText(fullFilename, sb.ToString());
            //Update the logger with what you did
            this.WriteToLogger("Config file exported to " + fullFilename);
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
            string filenameFormat = string.Format("Push_Tools_Log_{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            string fullFilename = Path.Combine(directory, filenameFormat);
            //Write all of the logger text to the file
            File.WriteAllText(fullFilename, textBoxLogger.Text);
            //Update the logger with what you did
            this.WriteToLogger("Log file exported to " + fullFilename);
        }
        internal void toolStripButtonRunProgram_Click(object sender, EventArgs e)
        {
            this.DisableButtonsWhileRunning();
            try { backgroundWorkerRunPush.RunWorkerAsync(); }
            catch
            {
                this.WriteToLogger("ERROR- unspecified error. Program failed. Please ask Eric Strong for help.");
                this.EnableButtonsWhileNotRunning();
            }
        }
        internal void toolStripButtonCancel_Click(object sender, EventArgs e){backgroundWorkerRunPush.CancelAsync();}   
        internal void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.WriteToLogger("Data push complete.");
            this.EnableButtonsWhileNotRunning();
        }
        internal void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Okay, so I use this function differently in my programs.
            int progPerc = e.ProgressPercentage;
            string progressstring = e.UserState.ToString();
            //If the first character is a $, then the progress update is for a particular point
            if (!string.IsNullOrEmpty(progressstring)) this.WriteToLogger(progressstring);
            //Update the overall progress bar
            if (progPerc > 0 && progPerc <= 100) this.toolStripProgress.Value = progPerc;
        }
        internal void DisableButtonsWhileRunning()
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
        internal void EnableButtonsWhileNotRunning()
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
        //This is the first important part of the program- validation of loading a configuration from CSV, which can involve complicated checks
        internal void LoadFromCSV(string filename)
        {
            string extension = new FileInfo(filename).Extension;
            if (extension == ".csv")
            {
                this.WriteToLogger("*Loading data from specified CSV input file...");
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(filename)))
                    {
                        int linenum = 0;
                        while (!reader.EndOfStream)
                        {
                            linenum++;
                            var line = reader.ReadLine();
                            if (String.IsNullOrWhiteSpace(line))
                            {
                                this.WriteToLogger(String.Format("Line {0} is empty", linenum.ToString()));
                                continue;
                            }
                            this.ValidateCSVLine(line, linenum);
                        }
                    }
                    this.WriteToLogger("Tags successfully loaded from CSV input file. WARNING- No automatic validation performed on column 7 (parameters).");
                }
                //Why do I catch all these exceptions instead of letting C# do it? For the purposes of this company, it's more user-friendly to give
                //an explanation about what to check/fix.
                catch (FileNotFoundException) { this.WriteToLogger("ERROR- CSV input file does not exist."); }
                catch (UnauthorizedAccessException) { this.WriteToLogger("ERROR- access to CSV input file is denied. Check that it isn't currently open."); }
                catch (IOException) { this.WriteToLogger("ERROR- access to CSV input file is denied. Check that it isn't currently open."); }}
            else { this.WriteToLogger("ERROR- Input file is not in CSV format."); }
        }        
        internal void ValidateCSVLine(string line, int linenum)
        {
            var values = line.Split(',');
            string parameters = String.Empty;
            if (values.Length > 6) { parameters = values[6]; }
            string pushType = "raw";
            if (values.Length > 5) { pushType = ValidatePushType(values[5], linenum); }
            int updateRate = 0;
            if (values.Length > 4 && pushType != "raw") updateRate = ValidateUpdateRate(values[4],linenum);
            DateTime endDate = DateTime.MinValue;
            if (values.Length > 3 && pushType != "raw") endDate = ValidateDateTime(values[3], linenum);
            DateTime startDate = DateTime.Parse("2001/01/01 00:01");
            if (values.Length > 2) startDate = ValidateDateTime(values[2], linenum);
            string description = values[0];
            if (values.Length > 1 && !String.IsNullOrWhiteSpace(values[1])) description = values[1];
            string tag = values[0];
            //Check that the eDNA tag exists
            if (Configuration.DoesIdExist(tag) == 0)
            {
                this.WriteToLogger(String.Format("ERROR on line {0}- {1} wasn't found in eDNA. Check that it is fully " +
                    "specified (site.service.pointID) and that an eDNA connection is available.",linenum.ToString(),tag);
            }
            //All the error-checking above should not throw an error under any situation. If a tag doesn't exist, the error will be handled again during data pull. 
            //If the startDate or endDate won't convert, or weren't specified, then the data pull reverts from DateTime.Min to DateTime.Now, which is a perfectly 
            //valid data pull.
            dataSet1.Tables[0].Rows.Add(tag, description, startDate, endDate, updateRate, pushType, parameters);
        }
        internal string ValidatePushType(string value, int linenum)
        {
            string[] possibleTypes = { "raw", "ramp", "sine", "impulse", "step", "periodic", "periodicimpulse", "rand", "randn", "rande" };
            if (possibleTypes.Contains(value.ToLower())) return value.ToLower();
            else
            {
                this.WriteToLogger(String.Format("ERROR on line {0}- {1} is an unrecognized push type. Reverting to default value of 'raw'", linenum.ToString(), value));
                return "raw";
            }
        }
        internal int ValidateUpdateRate(string value, int linenum)
        {
            int updateRate = 60;
            try
            {
                int newUpdateRate = Convert.ToInt32(value);
                if (newUpdateRate < 1)
                {
                    newUpdateRate = 60;
                    this.WriteToLogger(String.Format("ERROR on line {0}- {1} is less than 1. Reverting to default value of 60 s", linenum.ToString(), value));
                }
                else { updateRate = newUpdateRate; }
            }
            catch (FormatException)
            {
                string displayString = String.IsNullOrWhiteSpace(value) ? "Integer.Empty" : value.ToString();
                this.WriteToLogger(String.Format("ERROR on line {0}- {1} is not a recognized integer. Reverting to default value of 60 s.", linenum.ToString(), value));
            }
            return updateRate;
        }
        internal DateTime ValidateDateTime(string value, int linenum)
        {
            DateTime retDate = DateTime.MinValue;
            try { retDate = Convert.ToDateTime(value); }
            catch (FormatException)
            {
                string displayString = String.IsNullOrWhiteSpace(value) ? "String.Empty" : value.ToString();
                this.WriteToLogger(String.Format("ERROR on line {0}- {1} is not a recognized DateTime.", linenum.ToString(), value));
            }
            return retDate;
        }      
        //Next, we validate 
        internal bool ValidateNumParameters(string pushType, int dataRowNum, List<double> parameterList)
        {
            //Validate number of parameters supplied
            if (pushType == "raw" || pushType == "rande")
            {
                if (Double.IsNaN(parameterList[0]))
                {
                    backgroundWorkerRunPush.ReportProgress(0, String.Format("ERROR on row {0}- Not enough parameters supplied. 1 required. Skipping tag.", dataRowNum.ToString()));
                    return false;
                }
            }
            else if (pushType == "impulse" || pushType == "step" || pushType == "rand" || pushType == "randn" | pushType == "ramp")
            {
                if (parameterList.Count < 2)
                {
                    backgroundWorkerRunPush.ReportProgress(0, String.Format("ERROR on row {0}- Not enough parameters supplied. 2 required. Skipping tag.", dataRowNum.ToString()));
                    return false;
                }
            }
            else if (pushType == "periodicimpulse")
            {
                if (parameterList.Count < 3)
                {
                    backgroundWorkerRunPush.ReportProgress(0, String.Format("ERROR on row {0}- Not enough parameters supplied. 3 required. Skipping tag.", dataRowNum.ToString()));
                    return false;
                }
            }
            else if (pushType == "sine" || pushType == "periodic")
            {
                if (parameterList.Count < 4)
                {
                    backgroundWorkerRunPush.ReportProgress(0, String.Format("ERROR on row {0}- Not enough parameters supplied. 4 required. Skipping tag.", dataRowNum.ToString()));
                    return false;
                }
            }
            else
            {
                backgroundWorkerRunPush.ReportProgress(0, String.Format("ERROR on row {0}- Push Type not recognized. Skipping tag.", dataRowNum.ToString()));
                return false;
            }
            return true;
        }
        //Constructing the push     
        private void ConstructHistoryPush(DataTable pushDataTable)
        {
            //Initialize values
            var historyDataPush = new List<PushService>();
            //Iterate over each line in the PointGrid
            int dataRowNum = 0;
            foreach (DataRow tempDataRow in pushDataTable.Rows)
            {
                dataRowNum++;
                string pushType = tempDataRow.Field<string>(5).ToLower();
                string pointTag = tempDataRow.Field<string>(0);
                //Verify the StartingDate
                DateTime startTime = DateTime.MinValue;
                try { startTime = Convert.ToDateTime(tempDataRow.ItemArray[2]); }
                catch 
                { 
                    backgroundWorkerRunPush.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- Column 3 could not be converted to a DateTime. Skipping tag.");
                    continue;
                }
                //Verify the EndingDate
                DateTime endTime = DateTime.MinValue;
                if (pushType.ToLower() != "raw")
                {
                    try { endTime = Convert.ToDateTime(tempDataRow.ItemArray[3]); }
                    catch
                    {
                        backgroundWorkerRunPush.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- Column 4 could not be converted to a DateTime. Skipping tag.");
                        continue;
                    }
                }
                int updateRate = tempDataRow.Field<int>(4);
                
                try 
                { 
                    List<double> parameterList = tempDataRow.Field<string>(6).Split('|').ToList().ConvertAll(x => Convert.ToDouble(x));
                    //Validate that the number of supplied parameters are correct, and the push type is recognized
                    if (!this.ValidateNumParameters(pushType, dataRowNum, parameterList)) continue;
                    //Select the type of data pull, and construct the list of values to push
                    var tempPushValueList = this.ReturnPushValues(pushType, updateRate, startTime, endTime, parameterList);
                    //Determine the history service to write to
                    string service = "";
                    History.DnaHistResolveHistoryName(pointTag, out service);
                    //Check if the HistoryWrite has already been defined
                    int index = historyDataPush.FindIndex(f => f.Service == service);
                    //If the service hasn't been defined yet, create it and add it to the service list
                    if (index < 0)
                    {
                        historyDataPush.Add(new PushService(service));
                        index = historyDataPush.Count - 1;
                    }
                    //Now add the PushTag to the correct HistoryDataPush index.
                    //Note- in the future, I should also check if the same point already exists. If so,
                    //values should be added to the end of the already-existing PushTag, and then sorted
                    var tempPushTag = new PushTag(pointTag, service);
                    tempPushTag.AddTagValues(tempPushValueList);
                    historyDataPush[index].AddPushTag(tempPushTag);
                }
                catch (FormatException)
                { 
                    backgroundWorkerRunPush.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- parameters could not be converted to doubles. Skipping tag."); 
                    continue; 
                }
                catch
                {
                    backgroundWorkerRunPush.ReportProgress(0, "ERROR on row " + dataRowNum.ToString() + "- Unhandled exception. Skipping tag."); 
                    continue; 
                }              
            }
            //Save the values
            this.TotalPoints = dataRowNum;
            this.HistoryDataPush = historyDataPush;
        }        
        private List<PushValue> ReturnPushValues(string pushType, int updateRate, DateTime startTime, DateTime endTime, List<double> parameterList)
        {
            //Select which type of data pull              
            switch (pushType)
            {
                case ("raw"): return SimulationMethods.SimulateRaw(startTime,
                    parameterList[0]);
                case ("ramp"): return SimulationMethods.SimulateRamp(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], parameterList[1]);
                case ("sine"): return SimulationMethods.SimulateSine(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], parameterList[1], parameterList[2], parameterList[3]);
                case ("impulse"): return SimulationMethods.SimulateImpulse(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], Convert.ToInt32(parameterList[1]));
                case ("step"): return SimulationMethods.SimulateStep(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], Convert.ToInt32(parameterList[1]));
                case ("periodic"): return SimulationMethods.SimulatePeriodic(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], parameterList[1], parameterList[2], Convert.ToInt32(parameterList[3]));
                case ("periodicimpulse"): return SimulationMethods.SimulatePeriodicImpulse(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    Convert.ToInt32(parameterList[0]), parameterList[1], Convert.ToInt32(parameterList[2]));
                case ("rand"): return SimulationMethods.SimulateRand(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], parameterList[1]); 
                case ("randn"): return SimulationMethods.SimulateRandn(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0], parameterList[1]);
                case ("rande"): return SimulationMethods.SimulateRande(new TimeSpan(0, 0, updateRate), startTime, endTime,
                    parameterList[0]); 
            }
            return null;
        }
        private void ConstructRealTimePush()
        {
            //Iterate over all the values necessary
            var realTimeDataPush = new List<PushService>();
            foreach (PushService ps in this.HistoryDataPush)
            {
                foreach (PushTag pt in ps.PushTags)
                {
                    //Find the real-time service associated with the point
                    string service = MiscMethods.FindService(pt.Tag);
                    //Check if the PushService has already been defined
                    int index = realTimeDataPush.FindIndex(f => f.Service == service);
                    //If the service hasn't been defined yet, create it and add it to the service list
                    if (index < 0)
                    {
                        realTimeDataPush.Add(new PushService(service));
                        index = realTimeDataPush.Count - 1;
                    }
                    //Now add the PushTag to the correct RealTimeDataPush index.
                    //Note- in the future, I should also check if the same point already exists. If so,
                    //values should be added to the end of the already-existing PushTag, and then sorted
                    var tempPush = new PushTag(pt.Tag, service);
                    tempPush.AddTagValues(pt.TagValues[pt.GetTotalValues() - 1]);
                    realTimeDataPush[index].AddPushTag(tempPush);
                }
            }
            //Return the data push
            this.RealTimeDataPush = realTimeDataPush;
        }
        //Running the program   
        private void PushToHistory(WriteType outType)
        {
            foreach (PushService currentServ in this.HistoryDataPush)
            {
                if (backgroundWorkerRunPush.CancellationPending) { break; }
                if (outType == WriteType.AppendConnect || outType == WriteType.AppendHistory)
                {
                    int currentPoint = 0;
                    foreach (PushTag currentTag in currentServ.PushTags)
                    {
                        if (backgroundWorkerRunPush.CancellationPending) { break; }
                        StringBuilder progressString = currentTag.PushAppendAllValues((int) Settings.Default.RoundDecimals);
                        backgroundWorkerRunPush.ReportProgress((int)(((double)currentPoint * 100.0) / (double) this.TotalPoints),
                                    progressString.ToString());
                        currentPoint++;
                    }
                }
                else
                {
                    int currentPoint = 0;
                    foreach (PushTag currentTag in currentServ.PushTags)
                    {
                        if (backgroundWorkerRunPush.CancellationPending) { break; }
                        StringBuilder progressString = currentTag.PushInsertAllValues((int)Settings.Default.RoundDecimals);
                        backgroundWorkerRunPush.ReportProgress((int)(((double)currentPoint * 100.0) / (double) this.TotalPoints),
                                    progressString.ToString());
                        currentPoint++;
                    }
                }
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {          
            //Save user-selected properties
            backgroundWorkerRunPush.ReportProgress(1, "*Saving user properties...\n");
            int roundDecimals = (int) Settings.Default.RoundDecimals;
            bool doublePass = Settings.Default.DoublePass;
            WriteType outType = (WriteType)Settings.Default.OutputType;
            //Construct the data push
            backgroundWorkerRunPush.ReportProgress(0, "*Constructing the data push...\n");
            this.ConstructHistoryPush(dataSet1.Tables[0]);
            this.ConstructRealTimePush();
            //Start the data push
            DateTime startPush = DateTime.Now;
            backgroundWorkerRunPush.ReportProgress(0, "*Data push initialized at " + startPush.ToString() + ".\n");
            //Start the history push if necessary
            if (outType == WriteType.AppendConnect || outType == WriteType.AppendHistory ||
                outType == WriteType.InsertConnect || outType == WriteType.InsertHistory)
            {
                backgroundWorkerRunPush.ReportProgress(0, "*Pushing values to history...\n");
                this.PushToHistory(outType);
                if (doublePass)
                {
                    backgroundWorkerRunPush.ReportProgress(0, "*Performing second pass...\n");
                    this.PushToHistory(outType);
                }
            }
            //Start the real-time push if necessary
            if (outType == WriteType.AppendConnect || outType == WriteType.InsertConnect)
            {
                backgroundWorkerRunPush.ReportProgress(0, "*Pushing values to CVT...\n");
                foreach (PushService currentServ in this.RealTimeDataPush)
                {
                    if (backgroundWorkerRunPush.CancellationPending) { break; }
                    backgroundWorkerRunPush.ReportProgress(0, currentServ.PushRealTimeAllTags(currentServ.Service,(int)Settings.Default.RoundDecimals,
                        (int)Settings.Default.SleepTime).ToString());
                }
                if (doublePass)
                {
                    backgroundWorkerRunPush.ReportProgress(0, "*Performing second pass...\n");
                    foreach (PushService currentServ in this.RealTimeDataPush)
                    {
                        if (backgroundWorkerRunPush.CancellationPending) { break; }
                        backgroundWorkerRunPush.ReportProgress(0, currentServ.PushRealTimeAllTags(currentServ.Service, (int)Settings.Default.RoundDecimals, 
                            (int)Settings.Default.SleepTime).ToString());
                    }
                }
            }     
            //Update the logger
            string elapsedtime = (DateTime.Now - startPush).TotalSeconds.ToString();
            backgroundWorkerRunPush.ReportProgress(0, "*Data push finished in " + elapsedtime + " seconds.\n");
        }       
    }  
}
