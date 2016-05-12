using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InStep.eDNA.EzDNAApiNet;
using Pull_Tools.Properties;
using MathNet.Numerics.LinearRegression;

namespace Pull_Tools
{
    /// <summary>Miscellaneous methods which are useful across all namespaces.</summary>
    public static class MiscMethods
    {
        /// <summary> This method checks if an eDNA point has been configured within a selected service.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static bool CheckPointExists(string fullTag)
        {
            //Find service and point ID
            string service = MiscMethods.FindService(fullTag);
            string pointID = MiscMethods.FindPointID(fullTag);
            //Find point list from the service
            string[] idList;
            ushort numTags;
            Configuration.DNAGetRTIds(service, out idList, out numTags);
            return idList.Contains(pointID);
        }
        /// <summary>Returns only the point ID from a fully-qualified eDNA tag.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static string FindPointID(string fullTag)
        {
            return fullTag.Split('.')[2];
        }
        /// <summary>Returns only the Site.Service from a fully-qualified eDNA tag.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static string FindService(string fullTag)
        {
            string[] splitPoint = fullTag.Split('.');
            return String.Join(".", splitPoint[0], splitPoint[1]);
        }
        /// <summary>Returns a description of an eDNA tag.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static string GetPointDescription(string fullTag)
        {
            //eDNA Initialization
            double value;
            string valueString;
            DateTime time;
            ushort status;
            string statusString;
            string description;
            string units;

            //Pull the real-time data point
            RealTime.DNAGetRTFull(fullTag, out value, out valueString, out time, out status, out statusString, out description, out units);

            return description;
        }
        /// <summary>Uses regex to find substrings.</summary>
        public static IEnumerable<string> GetSubStrings(string input, string start, string end)
        {
            Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match.Groups[1].Value;
        }
        /// <summary>Pulls the current real-time value of the output tag.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static Tuple<DateTime, double> PullRealTimeValue(string fullTag)
        {
            //eDNA Initialization
            double value;
            string valueString;
            DateTime time;
            ushort status;
            string statusString;
            string description;
            string units;

            //Pull the real-time data point
            RealTime.DNAGetRTFull(fullTag, out value, out valueString, out time, out status, out statusString, out description, out units);

            return new Tuple<DateTime, double>(time, value);
        }
    }
    /// <summary>This struct is used to hold a data point for an eDNA tag with the following properties: date, value, and status.</summary>
    public class PointData
    {
        /// <summary>The timestamp (as a DateTime) for the data point.</summary>
        public DateTime Timestamp { get; private set; }
        /// <summary>The value (as a double) associated with the timestamp.</summary>
        public double? Value { get; private set; }
        /// <summary>A boolean value that can be TRUE (reliable) or FALSE (unreliable).</summary>
        public bool? Status { get; private set; }
        /// <summary>This class is used to hold a data point for an eDNA tag with the following properties: date, value, and status.</summary>
        /// <param name="date">The timestamp (as a DateTime) for the data point.</param>
        /// <param name="value">The value (as a double) associated with the timestamp.</param>
        /// <param name="status">A boolean value that can be TRUE (reliable) or FALSE (unreliable).</param>
        public PointData(DateTime date, double? value, bool? status)
        {
            this.Timestamp = date;
            this.Value = value;
            this.Status = status;
        }
    }
    /// <summary>This class defines a data pull for a single eDNA point, to use with multithreading.</summary>
    public class TagPullThread
    {
        /// <summary>An index used internally.</summary>
        public int Index { get; private set; }
        /// <summary>The fully-qualified eDNA tag (Site.Service.Point) used to perform the data pull.</summary>
        public string Tag { get; private set; }
        /// <summary>A description of the eDNA tag used to perform the data pull.</summary>
        public string Description { get; private set; }
        /// <summary>Useful for grouping similar points, such as those that all refer to the same equipment.</summary>
        public string Group { get; private set; }
        /// <summary>The filename to which data should be written.</summary>
        public string OutputFilename { get; private set; }
        /// <summary>The DateTime that defines the starting point of the data pull.</summary>
        public DateTime StartDate { get; private set; }
        /// <summary>The DateTime that defines the ending point of the data pull.</summary>
        public DateTime EndDate { get; private set; }
        /// <summary>Is state filtering being used?</summary>
        public bool StateFiltering { get; private set; }
        /// <summary>The fully-qualified eDNA tag (Site.Service.Point) that defines equipment steady-state.</summary>
        public string StateFilterTag { get; private set; }
        /// <summary>The low value that defines the state for the equipment.</summary>
        public double StateFilter_Low { get; private set; }
        /// <summary>The high value that defines the state for the equipment.</summary>
        public double StateFilter_High { get; private set; }
        /// <summary>Is value filtering being used?</summary>
        public bool ValueFiltering { get; private set; }
        /// <summary>This optional parameter can be used to ignore all values less than the low value filter.</summary>
        public double ValueFilter_Low { get; private set; }
        /// <summary>This optional parameter can be used to ignore all values greater than the high value filter.</summary>
        public double ValueFilter_High { get; private set; }
        /// <summary>This class defines a data pull for a single eDNA point, to use with multithreading.</summary>
        /// <param name="tag">The fully-qualified eDNA tag (Site.Service.Point) used to perform the data pull.</param>
        /// <param name="description">A description of the eDNA tag used to perform the data pull.</param>
        /// <param name="startdate">The DateTime that defines the starting point of the data pull.</param>
        /// <param name="enddate">The DateTime that defines the ending point of the data pull.</param>
        /// <param name="group">Useful for grouping similar points, such as those that all refer to the same equipment.</param>
        /// <param name="sFilter">Is state filtering being used?</param>
        /// <param name="sFilterTag">The fully-qualified eDNA tag (Site.Service.Point) that defines equipment steady-state.</param>
        /// <param name="sFilterLow">The low value that defines the state for the equipment.</param>
        /// <param name="sFilterHigh">The high value that defines the state for the equipment.</param>
        /// <param name="vFilter">Is value filtering being used?</param>
        /// <param name="vFilterLow">This optional parameter can be used to ignore all values less than the low value filter.</param>
        /// <param name="vFilterHigh">This optional parameter can be used to ignore all values greater than the high value filter.</param>
        public TagPullThread(int index, string tag, string description, string directory, DateTime startdate, DateTime enddate, string group = "", bool sFilter = false, string sFilterTag = "",
            double sFilterLow = -9999.0, double sFilterHigh = 9999.0, bool vFilter = false, double vFilterLow = -9999.0, double vFilterHigh = 9999.0)
        {
            this.Index = index;
            this.Tag = tag.Trim();
            this.Description = description.Trim();
            this.StartDate = startdate;
            this.EndDate = enddate;
            this.Group = group.Trim();
            this.StateFiltering = sFilter;
            this.StateFilterTag = sFilterTag.Trim();
            this.StateFilter_Low = sFilterLow;
            this.StateFilter_High = sFilterHigh;
            this.ValueFiltering = vFilter;
            this.ValueFilter_Low = vFilterLow;
            this.ValueFilter_High = vFilterHigh;
            //Construct the output filename
            string newDirectory = (String.IsNullOrWhiteSpace(group)) ? directory : Path.Combine(directory, group);
            string filename = tag + "_" + description + ".csv";
            this.OutputFilename = Path.Combine(newDirectory, filename);     
        }
        /// <summary>This function will pull data over a time period specified by "startDate" and "endDate".</summary>
        /// <param name="startDate">The DateTime that defines the starting point of the data pull.</param>
        /// <param name="endDate">The DateTime that defines the ending point of the data pull.</param>
        /// <param name="filtUnrel">Is unreliable filtering being used?</param>
        /// <param name="pullType">The type of eDNA data pull being accomplished (raw, snap, min, max, etc.)</param>
        /// <param name="timePeriod">The interval time period for the eDNA data pull (if applicable).</param>
        /// <param name="retDesc">A description of any errors that were returned.</param>
        /// <param name="pointDataList">A list of PointData that was returned from the data pull.</param>
        public void PullData(DateTime startDate, DateTime endDate, bool filtUnrel, PullMode pullType, TimeSpan timePeriod, out string retDesc, out List<PointData> pointDataList)
        {
            TimeSpan timeSpan = (timePeriod == null) ? new TimeSpan(0, 0, 1) : timePeriod;
            pointDataList = new List<PointData>();
            //eDNA Initialization               
            int nRet;
            uint uiKey = 0;
            double dValue = 0.0;
            DateTime dtTime = DateTime.MinValue;
            string strStatus = "";
            //Select the correct eDNA history mode                            
            switch (pullType)
            {
                case (PullMode.Totals): nRet = History.DnaGetHistTotal(this.Tag, startDate, endDate, timeSpan, out uiKey); break;
                case (PullMode.Snap): nRet = History.DnaGetHistSnap(this.Tag, startDate, endDate, timeSpan, out uiKey); break;
                case (PullMode.Avg): nRet = History.DnaGetHistAvg(this.Tag, startDate, endDate, timeSpan, out uiKey); break;
                case (PullMode.Min): nRet = History.DnaGetHistMin(this.Tag, startDate, endDate, timeSpan, out uiKey); break;
                case (PullMode.Max): nRet = History.DnaGetHistMax(this.Tag, startDate, endDate, timeSpan, out uiKey); break;
                default: nRet = History.DnaGetHistRaw(this.Tag, startDate, endDate, out uiKey); break;
            }
            //Run the data pull           
            while (nRet == 0)
            {
                nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                //Check that the point is within bounds- sometimes eDNA pulls too early or too far
                if (dtTime > this.StartDate && dtTime < this.EndDate) { pointDataList.Add(new PointData(dtTime, dValue, strStatus != "UNRELIABLE")); }
            }
            //Filtering, if selected
            {
                if (filtUnrel) { pointDataList = this.FilterUnreliable(pointDataList); }
                if (this.ValueFiltering) { pointDataList = this.FilterByValue(pointDataList); }
                if (this.StateFiltering) { pointDataList = this.FilterByState(pointDataList, this.StateFilterTag,this.StateFilter_Low,this.StateFilter_High); }
            }
            //Cleanup
            History.GetHistoryError(nRet, out retDesc);
            History.DNACancelHistRequest(uiKey);
        }
        /// <summary> This method will filter out all the unreliable values. </summary>
        /// <param name="pointDataList">A list containing PointData, likely from this.PullData().</param>
        /// <returns>A list of PointData with unreliable data points filtered out.</returns>
        public List<PointData> FilterUnreliable(List<PointData> pointDataList)
        {
            var newValues = new List<PointData>();
            if (pointDataList.Count > 0)
            {
                foreach (PointData pd in pointDataList)
                {
                    if (pd.Status == false) { newValues.Add(new PointData(pd.Timestamp, null, null)); }
                    else { newValues.Add(pd); }
                }
            }
            return newValues;
        }
        /// <summary>This method will filter out all the values less than the Low Filter and all the values greater than the High Filter.</summary>
        /// <param name="pointDataList">A list containing PointData, likely from this.PullData().</param>
        /// <returns>A list of PointData with data points filtered out if they are lower than this.ValueFilter_Low or greater than this.ValueFilter_High.</returns>
        public List<PointData> FilterByValue(List<PointData> pointDataList)
        {
            var newValues = new List<PointData>();
            if (pointDataList.Count > 0)
            {
                foreach (PointData pd in pointDataList)
                {
                    if (pd.Value < this.ValueFilter_Low || pd.Value > this.ValueFilter_High) { newValues.Add(new PointData(pd.Timestamp, null, null)); }
                    else { newValues.Add(pd); }
                }
            }
            return newValues;
        }
        /// <summary>This method is used to filter this.Values by whether the equipment is in steady-state or not.</summary>
        /// <param name="steadyStateTag">This is the fully-qualified eDNA tag (Site.Service.Tag) associated with the steady-state point.</param>
        /// <param name="steadyStateDefLow">The value that defines the low limit of whether the equipment is in steady-state or not. Typically this value will equal "1".</param>
        /// <param name="steadyStateDefHigh">The value that defines the high limit of whether the equipment is in steady-state or not. Typically this value will equal "1".</param>
        public List<PointData> FilterByState(List<PointData> pointDataList, string steadyStateTag, double steadyStateDefLow = 1, double steadyStateDefHigh = 1,
            bool snapData = false, int snapSeconds = 1)
        {
            if (pointDataList.Count > 0)
            {
                //First, the raw steady-state values need to be pulled
                var ssValues = new List<Tuple<DateTime, bool>>();
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime = DateTime.MinValue;
                string strStatus;
                //Pull the first data using Snap
                nRet = History.DnaGetHistSnap(steadyStateTag, this.StartDate, this.StartDate, new TimeSpan(0, 0, 1), out uiKey);
                nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                ssValues.Add(new Tuple<DateTime, bool>(this.StartDate, dValue >= steadyStateDefLow && dValue <= steadyStateDefHigh));
                //Pull data
                if (snapData == false)
                {
                    nRet = History.DnaGetHistRaw(steadyStateTag, this.StartDate, this.EndDate, out uiKey);
                }
                else
                {
                    nRet = History.DnaGetHistSnap(steadyStateTag, this.StartDate, this.EndDate, TimeSpan.FromSeconds(snapSeconds), out uiKey);
                }
                while (nRet == 0)
                {
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                    if (dtTime >= this.StartDate || dtTime <= this.EndDate) ssValues.Add(new Tuple<DateTime, bool>(dtTime, dValue >= steadyStateDefLow && dValue <= steadyStateDefHigh));
                }
                //Pull the last data using Snap
                nRet = History.DnaGetHistSnap(steadyStateTag, this.EndDate, this.EndDate, new TimeSpan(0, 0, 1), out uiKey);
                nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                ssValues.Add(new Tuple<DateTime, bool>(this.EndDate, dValue >= steadyStateDefLow && dValue <= steadyStateDefHigh));

                //Now, iterate over all values and filter those that aren't in steady-state
                var newValues = new List<PointData>();
                for (int ii = 0; ii < pointDataList.Count; ii++)
                {
                    PointData currentValue = pointDataList[ii];
                    int indexSS = ssValues.FindIndex(f => f.Item1 > currentValue.Timestamp) - 1;
                    if (indexSS >= 0 && ssValues[indexSS].Item2 == false)
                    {
                        newValues.Add(new PointData(currentValue.Timestamp, null, null));
                    }
                    else
                    {
                        newValues.Add(currentValue);
                    }
                }
                return newValues;
            }
            return null;
        }
        /// <summary>A method to calculate the mean of the points.</summary>
        /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
        public double? CalculateMean(List<PointData> pointDataList, int analysisWindow = 0)
        {
            //Check if the values are empty and return null
            if (pointDataList.Count == 0) return null;

            //Check if an Analysis Window was selected and skip the appropriate number of points
            List<PointData> calcValues = (analysisWindow > 0) ?
                pointDataList.Skip(Math.Max(0, pointDataList.Count() - analysisWindow)).ToList() :
                pointDataList;

            return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Average();
        }
        /// <summary>A method to calculate the minimum of the points.</summary>
        /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
        public double? CalculateMin(List<PointData> pointDataList, int analysisWindow = 0)
        {
            //Check if the values are empty and return null
            if (pointDataList.Count == 0) return null;

            //Check if an Analysis Window was selected and skip the appropriate number of points
            List<PointData> calcValues = (analysisWindow > 0) ?
                pointDataList.Skip(Math.Max(0, pointDataList.Count() - analysisWindow)).ToList() :
                pointDataList;

            return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Min();
        }
        /// <summary>A method to calculate the maximum of the points.</summary>
        /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
        public double? CalculateMax(List<PointData> pointDataList, int analysisWindow = 0)
        {
            //Check if the values are empty and return null
            if (pointDataList.Count == 0) return null;

            //Check if an Analysis Window was selected and skip the appropriate number of points
            List<PointData> calcValues = (analysisWindow > 0) ?
                pointDataList.Skip(Math.Max(0, pointDataList.Count() - analysisWindow)).ToList() :
                pointDataList;

            return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Max();
        }
        /// <summary>A method to linearly regress the points and return the intercept and slope. The first value returned is the intercept, and the second is the slope.</summary>
        /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
        public Tuple<double, double> CalculateLinearRegression(List<PointData> pointDataList, int analysisWindow = 0)
        {
            //Check if the values are empty and return null
            if (pointDataList.Count < 2) return null;

            //Check if an Analysis Window was selected and skip the appropriate number of points
            List<PointData> calcValues = (analysisWindow > 0) ?
                pointDataList.Skip(Math.Max(0, pointDataList.Count() - analysisWindow)).ToList() :
                pointDataList;

            //Create a Tuple (the input data type for the static regression method)
            var regvalues = new List<Tuple<double, double>>();
            for (int ii = 0; ii < calcValues.Count; ii++)
            {
                regvalues.Add(new Tuple<double, double>((double)ii, calcValues[ii].Value.Value));
            }

            //Uses the MathNet library to regress the points to a line
            return SimpleRegression.Fit(regvalues);
        }
        /// <summary>A method to calculate the number of points pulled.</summary>
        public int CalculateNumPointsWithValues(List<PointData> pointDataList)
        {
            return pointDataList.FindAll(f => f.Value.HasValue).Count;
        }
        /// <summary></summary>
        /// <param name="pointDataList"></param>
        /// <param name="zipResults"></param>
        public void WriteDataToCSV(List<PointData> pointDataList)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(this.OutputFilename));
            File.Delete(this.OutputFilename);
            var sb = new StringBuilder();
            string filename = this.OutputFilename;
            if (pointDataList != null)
            {
                //Yeah, I agree this looks REALLY clunky. But it's faster not to have to check each setting for each iteration.
                //If writing the status
                if (Properties.Settings.Default.Out_WriteStatus)
                {
                    //If not using UTC time
                    if (!Properties.Settings.Default.Out_UTCTime)
                    {
                        foreach (PointData pd in pointDataList) { sb.AppendLine(String.Join(",",pd.Timestamp.ToString(), pd.Value.ToString(), pd.Status.ToString())); }
                        File.AppendAllText(filename, sb.ToString());
                    }
                    //If using UTC time
                    else
                    {
                        foreach (PointData pd in pointDataList) { sb.AppendLine(String.Join(",",pd.Timestamp.ToUniversalTime().ToString(), pd.Value.ToString(), pd.Status.ToString())); }
                        File.AppendAllText(filename, sb.ToString());
                    }
                }
                //If not writing the status
                else
                {
                    //If not using UTC time
                    if (!Properties.Settings.Default.Out_UTCTime)
                    {
                        foreach (PointData pd in pointDataList) { sb.AppendLine(String.Join(",",pd.Timestamp.ToString(), pd.Value.ToString())); }
                        File.AppendAllText(filename, sb.ToString());
                    }
                    //If using UTC time
                    else
                    {
                        foreach (PointData pd in pointDataList) { sb.AppendLine(String.Join(",",pd.Timestamp.ToUniversalTime().ToString(), pd.Value.ToString())); }
                        File.AppendAllText(filename, sb.ToString());
                    }
                }
            }
        }
        /// <summary>Returns the point header information to be used in the header row of an output csv file. This method is used
        /// when data is being written to one consistent time column.</summary>
        /// <example>If the description supplied during construction was "Machine Hours", Status = TRUE, and StatusColumnName =
        /// "Reliable", the output will be: "Machine Hours,Reliable"</example>
        /// <param name="status">A value of TRUE will add a status column to the string, while a value of "false" will only return the 
        /// point description. Default is FALSE.</param>
        /// <param name="statusColumnName">If Status = TRUE, this parameter will define the name of the status column. Default is "Reliable".</param>
        public string WritePointHeader(bool status = true, string statusColumnName = "Reliable")
        {
            return (status) ?
                this.Description + "," + statusColumnName + "," :
                this.Description + ",";
        }
        /// <summary>Returns the point header information to be used in the header row of an output csv file. This method is used
        /// when data is being written to each file based on individual points, instead of multiple points using one consistent time column.</summary>
        /// <example>If the description supplied during construction was "Machine Hours", Status = TRUE, StatusColumnName =
        /// "Reliable", and DateTimeName = "DateTime", the output will be: "DateTime,Machine Hours,Reliable".</example>
        /// <param name="status">A value of TRUE will add a status column to the string, while a value of FALSE will only return the 
        /// point description. Default is TRUE.</param>
        /// <param name="statusColumnName">If Status = TRUE, this parameter will define the name of the status column. Default is "Reliable".</param>
        /// <param name="dateTimeName">The description for the DateTime column. Default is "DateTime".</param>
        public string WriteFullHeader(bool status = true, string statusColumnName = "Reliable", string dateTimeName = "DateTime")
        {
            return (status) ?
                dateTimeName + "," + this.Description + "," + statusColumnName + ",\n" :
                dateTimeName + "," + this.Description + ",\n";
        }
        /// <summary>This method will write the data to a file that can be read by StrategyStudio PointDefs.</summary>
        /// <param name="directory">The directory to write the file.</param>
        public void WriteDataToPointDefs(string directory, List<PointData> tempPointData)
        {
            //Open a Stream Writer
            string dirPath = directory + @"\POINT DEFINITIONS\";
            if (!System.IO.Directory.Exists(dirPath))
                System.IO.Directory.CreateDirectory(dirPath);

            //Format dates
            string SD = String.Format("{0:yyyy-MM-dd}", this.StartDate);
            string ED = String.Format("{0:yyyy-MM-dd}", this.EndDate);
            string pathString = dirPath + MiscMethods.FindPointID(this.Tag) + "~" + SD + "~" + ED + ".pt.csv";
            File.Delete(pathString);

            //Write to File
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathString, true))
            {
                file.WriteLine("#offset=false; units=mm; description=demo of the pt.csv for this Variable	");
                file.WriteLine("# first revision headings are NOT read and must be in {Time[, Value[, Status]]}");
                file.WriteLine("#Value is defaulted to NaN and the last value is retained for missing values	");
                file.WriteLine("#Status defaulted to 3 (OK) and the last value is retained for missing values	");

                foreach (PointData currentValue in tempPointData)
                {
                    if (currentValue.Value == null) continue;
                    file.WriteLine(String.Join(",", currentValue.Timestamp.ToString(), currentValue.Value.ToString()));
                }
            }
        }
        /// <summary>Easily write selected metrics to a string.</summary>
        /// <param name="mean">Should the mean be calculated?</param>
        /// <param name="min">Should the minimum be calculated?</param>
        /// <param name="max">Should the maximum be calculated?</param>
        /// <param name="linearRegression">Should linear regression be calculated?</param>
        /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
        public StringBuilder WriteMetricsToString(List<PointData> tempPointData, int analysisWindow = 0, bool mean = true, bool min = true, bool max = true, bool linearRegression = true)
        {
            //Initialization
            var retsb = new StringBuilder();
            retsb.Append(this.Tag + "," + this.Description + "," + this.CalculateNumPointsWithValues(tempPointData));
            //If selected, add each metric to the stringbuilder
            if (mean) { retsb.Append("," + this.CalculateMean(tempPointData,analysisWindow)); }
            if (min) { retsb.Append("," + this.CalculateMin(tempPointData,analysisWindow)); }
            if (max) { retsb.Append("," + this.CalculateMax(tempPointData,analysisWindow)); }
            if (linearRegression)
            {
                Tuple<double, double> retparams = this.CalculateLinearRegression(tempPointData,analysisWindow);
                if (retparams != null) retsb.Append("," + retparams.Item1 + "," + retparams.Item2);
                else retsb.Append(",,");
            }
            retsb.Append("\n");
            return retsb;
        }
    }
}
