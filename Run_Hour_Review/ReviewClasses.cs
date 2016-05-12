using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Excel = ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using InStep;
using InStep.eDNA.EzDNAApiNet;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using MathNet.Filtering;

namespace Run_Hour_Review
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
            int nRet = RealTime.DNAGetRTFull(fullTag, out value, out valueString, out time, out status, out statusString, out description, out units);
            //If an error occurs, pull from history
            if (nRet != 0)
            {
                int nRet2;
                uint uiKey = 0;
                double dValue = 0.0;
                DateTime dtTime = DateTime.MinValue;
                string strStatus = "";
                nRet2 = History.DnaGetHistRaw(fullTag, DateTime.Now, DateTime.Now, out uiKey);
                nRet2 = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                return new Tuple<DateTime, double>(dtTime, dValue);
            }
            return new Tuple<DateTime, double>(time, value);
        }
    }
    namespace CalcTools
    {
        /// <summary>A class used to contain the calc tags for an entire service.</summary>
        public class CalcService
        {
            /// <summary>The XML document used to load the calc tags.</summary>
            public XmlDocument XML { get; private set; }
            /// <summary>The calc tags extracted from the XML document.</summary>
            public List<CalcTag> CalcTags { get; private set; }
            /// <summary>A class used to contain the calc tags for an entire service.</summary>
            /// <param name="xDoc">The XML document used to load the calc tags.</param>
            public CalcService(XmlDocument xDoc)
            {
                this.XML = xDoc;
                this.CalcTags = new List<CalcTag>();

                //Iterate over all the points
                for (int ii = 0; ii < xDoc["POINTS"].ChildNodes.Count; ii++)
                {
                    string currentTag = xDoc["POINTS"].ChildNodes[ii]["PointId"].InnerText;
                    string currentEquation = xDoc["POINTS"].ChildNodes[ii]["Equation"].InnerXml;
                    this.CalcTags.Add(new CalcTag(currentTag, currentEquation));
                }
            }
            /// <summary>Writes all the associated tags of all this.CalcTags to a CSV file.</summary>
            /// <param name="filePath">The file path to write the CSV file to.</param>
            public void WriteAllAssociatedTagsToFile(string filePath)
            {
                File.Delete(filePath);
                using (var file = new StreamWriter(filePath, true))
                {
                    foreach (CalcTag currentTag in this.CalcTags)
                    {
                        file.Write(currentTag.WriteAssociatedTags());
                    }
                }
            }
        }
        /// <summary>Defines an eDNA calculation, with tag, equation, and associated points.</summary>
        public class CalcTag
        {
            /// <summary>The eDNA tag for the calculation.</summary>
            public string Tag { get; private set; }
            /// <summary>The equation used in the calculation.</summary>
            public string Equation { get; private set; }
            /// <summary>The list of eDNA tags used in the calculation</summary>
            public List<string> AssociatedTags { get; private set; }
            /// <summary>Defines an eDNA calculation, with tag, equation, and associated points.</summary>
            /// <param name="tag">The eDNA tag for the calculation.</param>
            /// <param name="equation">The equation used in the calculation.</param>
            public CalcTag(string tag, string equation)
            {
                //Save the user-supplied properties
                this.Tag = tag;
                this.Equation = equation;
                this.AssociatedTags = new List<string>();
                this.FindAssociatedTags();
            }
            private void FindAssociatedTags()
            {
                //Find the associated tags
                this.AssociatedTags.AddRange(this.GetSubStrings(this.Equation.ToLower(), "dnagetrtvalue(\"", "\")"));
                //Clear the duplicates
                this.AssociatedTags = this.AssociatedTags.Distinct().ToList();
                //Make sure the tags go back to upper case
                this.AssociatedTags = this.AssociatedTags.ConvertAll(f => f.ToUpper());
            }
            private IEnumerable<string> GetSubStrings(string input, string start, string end)
            {
                Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
                MatchCollection matches = r.Matches(input);
                foreach (Match match in matches)
                    yield return match.Groups[1].Value;
            }
            /// <summary>This method detects if the last line in the equation calls a constant value, which usually indicates user error.</summary>
            public bool DetectBadConstantValue(out string retString)
            {
                retString = String.Empty;
                //Find the last line
                string[] splitLines = this.Equation.Split('\n');
                if (splitLines.Length > 0)
                {
                    string lastLine = splitLines[splitLines.Length - 1];
                    lastLine = Regex.Replace(lastLine, @"\s", "");
                    Regex r = new Regex(@"^Value=\d+[^\w]", RegexOptions.IgnoreCase);
                    Match match = r.Match(lastLine);
                    if (match.Success)
                    {
                        retString = this.Tag + "," + lastLine;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            /// <summary>Writes all the associated tags to a StringBuilder.</summary>
            public StringBuilder WriteAssociatedTags()
            {
                var retSB = new StringBuilder();

                foreach (string currentTag in this.AssociatedTags)
                {
                    retSB.AppendLine(String.Join(",", this.Tag, currentTag));
                }

                return retSB;
            }
        }
    }
    namespace Comms
    {
        /// <summary>This class is useful for holding the entire list of ServiceComms and mass-pulling uptime.</summary>
        public class ServicesCommsInfo
        {
            /// <summary>A list of the ServiceComms instances to be </summary>
            public List<ServiceComms> ServicesComms { get; private set; }
            /// <summary>This class is useful for holding the entire list of ServiceComms and mass-pulling uptime.</summary>
            /// <param name="filePath">The file path to the CSV input file.</param>
            public ServicesCommsInfo(string filePath)
            {
                this.ServicesComms = new List<ServiceComms>();
                //Read From File
                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    //Iterate over each line
                    while (!reader.EndOfStream)
                    {
                        string[] splitLine = reader.ReadLine().Split(',');
                        this.ServicesComms.Add(new ServiceComms(splitLine[1], splitLine[0]));
                    }
                }
            }
            /// <summary>This method will write the uptime for all services in this.ServicesComms to a StringBuilder.</summary>
            /// <param name="startDate">The start date to find the uptime.</param>
            /// <param name="endDate">The end date to find the uptime.</param>
            public StringBuilder WriteUptime(DateTime startDate, DateTime endDate)
            {
                var retSB = new StringBuilder();
                foreach (ServiceComms currentComms in this.ServicesComms)
                {
                    double upTime = currentComms.FindUptime(startDate, endDate);
                    double percUpTime = upTime / ((endDate - startDate).TotalSeconds);
                    retSB.Append(String.Join(",", currentComms.Name, upTime.ToString(), percUpTime.ToString()) + "\n");
                }

                return retSB;
            }
        }
        /// <summary>This class is used to summarize service status at particular time periods.</summary>
        public class ServiceComms
        {
            /// <summary>The name of the service.</summary>
            public string Name { get; private set; }
            /// <summary>The tag in the Calc service used to check service status.</summary>
            public string CheckTag { get; private set; }
            /// <summary>This class is used to summarize service status at particular time periods.</summary>
            /// <param name="checkTag">The tag in the Calc service used to check service status.</param>
            /// <param name="name">The name of the service.</param>
            public ServiceComms(string checkTag, string name)
            {
                this.Name = name;
                this.CheckTag = checkTag;
            }
            /// <summary>This method is used to find the uptime of the service. Any missing data points are assumed
            /// to be downtime, and any event rates less than 1 are assumed to be downtime.</summary>
            /// <param name="startDate">The start date to find the uptime.</param>
            /// <param name="endDate">The end date to find the uptime.</param>
            public double FindUptime(DateTime startDate, DateTime endDate)
            {
                //eDNA Initialization
                uint uiKey = 0;
                int nRet = History.DnaGetHistRaw(this.CheckTag, startDate, endDate, out uiKey);
                double dValue;
                string strStatus;
                DateTime prevTime = DateTime.MinValue;
                DateTime curTime = DateTime.MinValue;

                double uptimeSecs = 0;
                //Run the data pull
                while (nRet == 0)
                {
                    //Pull the next point
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out curTime, out strStatus);
                    //Check that the point is within bounds- sometimes eDNA pulls too early or too far
                    if (curTime < startDate || curTime > endDate) { continue; }
                    //Check that the current time is not too much further in the future than the previous time
                    double totSeconds = (curTime - prevTime).TotalSeconds;
                    if (totSeconds < 100) uptimeSecs += totSeconds;
                    //Update the previous time
                    prevTime = curTime;
                }

                return uptimeSecs;
            }
        }
    }
    namespace DataReview
    {
        //Data Review Initialization Classes
        /// <summary>This class defines the overall data review, including run hours, stress events, and stress hours.</summary>
        public class OverallDataReview
        {
            /// <summary>A list of instances of RunHourReview.</summary>
            public List<RunHourReview> RunHourReviewList { get; private set; }
            /// <summary>A list of instances of StressEventReview.</summary>
            public List<StressEventReview> StressEventReviewList { get; private set; }
            /// <summary>A list of instances of StressEventReview.</summary>
            public List<StressHourReview> StressHourReviewList { get; private set; }
            /// <summary>This constructor will initialize the data review. Warning- the lists of run hours and stress
            /// events to review must be populated manually.</summary>
            public OverallDataReview()
            {
                this.RunHourReviewList = new List<RunHourReview>();
                this.StressEventReviewList = new List<StressEventReview>();
                this.StressHourReviewList = new List<StressHourReview>();
            }
            /// <summary>This constructor will initialize an overall data review based on an input CSV file.</summary>
            /// <param name="filePath">The fully-qualified file path to the input CSV file.</param>
            public OverallDataReview(string filePath)
            {
                //Initialization
                var runHoursReview = new List<RunHourReview>();
                var stressEventsReview = new List<StressEventReview>();
                var stressHoursReview = new List<StressHourReview>();

                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    //Iterate over each line
                    while (!reader.EndOfStream)
                    {
                        //Read from the input file
                        string[] splitLine = reader.ReadLine().Split(',');

                        //The first four values are necessary- the others are optional.
                        string outputTag = splitLine[0];
                        string runTag = splitLine[1];
                        string calcTag = splitLine[2];
                        string description = (splitLine.Length < 5) ? splitLine[0].Trim() : splitLine[4].Trim();
                        string assignedEngineer = (splitLine.Length < 6) ? "Unassigned" : splitLine[5];
                        decimal multiplier = (splitLine.Length < 7 || String.IsNullOrWhiteSpace(splitLine[6])) ?
                            Decimal.Zero : Convert.ToDecimal(splitLine[6]);

                        //Detect which type of data summary to perform
                        switch (splitLine[3])
                        {
                            case ("1"):
                                stressHoursReview.Add(new StressHourReview(outputTag, runTag, calcTag, multiplier, description, assignedEngineer));
                                break;
                            case ("0"):
                                stressEventsReview.Add(new StressEventReview(outputTag, calcTag, multiplier, description, assignedEngineer));
                                break;
                            case ("2"):
                                runHoursReview.Add(new RunHourReview(outputTag, runTag, calcTag, description, assignedEngineer));
                                break;
                        }
                    }
                }

                this.RunHourReviewList = runHoursReview;
                this.StressEventReviewList = stressEventsReview;
                this.StressHourReviewList = stressHoursReview;
            }
            /// <summary>Runs the data review for a single RunHourReview. (Usually "RunAllReview" should be used. This method is useful for updating
            /// the logger when using this class in a backgroundWorker.)</summary>
            /// <param name="runHourReview">The instance of RunHourReview to calculate.</param>
            /// <param name="startDate">The start date of the review.</param>
            /// <param name="endDate">The end date of the review.</param>
            /// <param name="granularPeriod">The time period granularity to use.</param>
            public RunHourOverallSummary RunSingleRunHourReview(RunHourReview runHourReview, DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                return runHourReview.CalculateSummary(startDate, endDate, granularPeriod);
            }
            /// <summary>Runs the data review for a single StressEventReview. (Usually "RunAllReview" should be used. This method is useful for updating
            /// the logger when using this class in a backgroundWorker.)</summary>
            /// <param name="stressEventReview">The instance of StressEventReview to calculate.</param>
            /// <param name="startDate">The start date of the review.</param>
            /// <param name="endDate">The end date of the review.</param>
            /// <param name="granularPeriod">The time period granularity to use.</param>
            public StressEventOverallSummary RunSingleStressEventReview(StressEventReview stressEventReview, DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                return stressEventReview.CalculateSummary(startDate, endDate, granularPeriod);
            }
            /// <summary>Runs the data review for a single StressHourReview. (Usually "RunAllReview" should be used. This method is useful for updating
            /// the logger when using this class in a backgroundWorker.)</summary>
            /// <param name="stressHourReview">The instance of StressHourReview to calculate.</param>
            /// <param name="startDate">The start date of the review.</param>
            /// <param name="endDate">The end date of the review.</param>
            /// <param name="granularPeriod">The time period granularity to use.</param>
            public StressHourOverallSummary RunSingleStressHourReview(StressHourReview stressHourReview, DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                return stressHourReview.CalculateSummary(startDate, endDate, granularPeriod);
            }
            /// <summary>This method will run the data review initialized in this object and return an OverallReviewSummary.</summary>
            /// <param name="startDate">The start date of the review.</param>
            /// <param name="endDate">The end date of the review.</param>
            /// <param name="granularPeriod">The time period granularity to use.</param>
            public OverallDataReviewSummary RunAllReview(DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                var runHourSummary = new List<RunHourOverallSummary>();
                var stressEventSummary = new List<StressEventOverallSummary>();
                var stressHourSummary = new List<StressHourOverallSummary>();

                //Iterate over each run hour review in the overall list.
                foreach (RunHourReview runHourReview in this.RunHourReviewList)
                {
                    runHourSummary.Add(runHourReview.CalculateSummary(startDate, endDate, granularPeriod));
                }

                //Iterate over each stress hour review in the overall list.
                foreach (StressHourReview stressHourReview in this.StressHourReviewList)
                {
                    stressHourSummary.Add(stressHourReview.CalculateSummary(startDate, endDate, granularPeriod));
                }

                //Iterate over each stress event review in the overall list.
                foreach (StressEventReview stressEventReview in this.StressEventReviewList)
                {
                    stressEventSummary.Add(stressEventReview.CalculateSummary(startDate, endDate, granularPeriod));
                }

                return new OverallDataReviewSummary(startDate, endDate, granularPeriod, runHourSummary, stressEventSummary, stressHourSummary);
            }
        }
        /// <summary>This class defines a machine hour tag and can be used to find an overall summary of the values over a granular time period.</summary>
        public class RunHourReview
        {
            /// <summary>The output tag to which run hours will be written.</summary>
            public string OutputTag { get; private set; }
            /// <summary>The run hour tag associated with the run hour review.</summary>
            public string RunHourTag { get; private set; }
            /// <summary>The calc tag associated with the run hour tag.</summary>
            public string CalcTag { get; private set; }
            /// <summary>A description of the run hour tag.</summary>
            public string Description { get; private set; }
            /// <summary>The steady-state operating limit associated with the run hour review.</summary>
            public OpStateTools.OpLimit SteadyOp { get; private set; }
            /// <summary>The engineer assigned to review the run hours.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>This class defines a machine hour tag and can be used to find an overall summary of the values over a granular time period.</summary>
            /// <param name="outputTag">The output tag to which run hours will be written.</param>
            /// <param name="runHourTag">The fully qualified eDNA point tag (Site.Service.Point) of the run hour point. This point will be used to pull 
            /// the starting values of the run hour tag, and it will also be used to push values back into eDNA.</param>
            /// <param name="calcTag">The fully qualified eDNA point tag (Site.Service.Point) of the calc point associated with the run hour point.</param>
            /// <param name="description">A brief description of the eDNA run hour point. This will be used for output/display purposes. By default, it is 
            /// equal to the eDNA run hour tag.</param>
            /// <param name="assignedEngineer">The engineer assigned to review the run hours. By default, this is "Unassigned".</param>
            /// <param name="steadyStateDef">The value that defines whether the equipment is in steady-state (typically "1").</param>
            public RunHourReview(string outputTag, string runHourTag, string calcTag, string description = "", string assignedEngineer = "None", double steadyStateDef = 1)
            {
                this.OutputTag = outputTag.Trim();
                this.RunHourTag = runHourTag.Trim();
                this.CalcTag = calcTag.Trim();
                this.Description = (String.IsNullOrWhiteSpace(description.Trim())) ? this.RunHourTag : description.Trim();
                this.AssignedEngineer = assignedEngineer.Trim();
                //Construct an OpLimit class which will be used to calculate the run hour summary directly, if needed.
                this.SteadyOp = new OpStateTools.OpLimit(steadyStateDef, steadyStateDef, this.CalcTag);
            }
            /// <summary> This method is used to calculate the summary of accumulated run hours in between the StartDate and
            /// EndDate, with granularity of TimePeriod.</summary>
            /// <param name="startDate">The beginning date to pull data for the run hour state.</param>
            /// <param name="endDate">The last date to pull data for the run hour state.</param>
            /// <param name="granularPeriod">The time period over which to obtain the summary. For example, if the supplied time period is 
            /// "24:00:00", the output will be a daily summary. If the difference between the Start Date and End
            /// Date is 2 days in this example, 3 elements will be present in the output list.</param>
            public RunHourOverallSummary CalculateSummary(DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                Tuple<DateTime, double> rtValue = MiscMethods.PullRealTimeValue(this.OutputTag);
                var tempSummaryList = new List<RunHourGranularSummary>();

                double lastValue = this.PullSnapValue(startDate);
                //Pull the operating state over each time period
                for (DateTime granularDT = startDate; granularDT < endDate; granularDT = granularDT.Add(granularPeriod))
                {
                    //Check that the granular end time is not greater than the absolute end time
                    DateTime granularDTEnd = granularDT.Add(granularPeriod);
                    if (granularDTEnd > endDate) { granularDTEnd = endDate; }

                    //Pull the current value at granularDTEnd using "Snap" mode.
                    double curValue = this.PullSnapValue(granularDTEnd);
                    tempSummaryList.Add(new RunHourGranularSummary(curValue - lastValue, granularDT, granularDTEnd));

                    lastValue = curValue;
                }

                return new RunHourOverallSummary(tempSummaryList, this.OutputTag, rtValue, this.AssignedEngineer, this.Description);
            }
            /// <summary>Pulls the current real-time value of the output tag.</summary>
            private double PullSnapValue(DateTime currentDate)
            {
                //eDNA Initialization
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;
                nRet = History.DnaGetHistSnap(this.RunHourTag, currentDate, currentDate, new TimeSpan(0, 0, 0, 1), out uiKey);

                //Ideally, eDNA will just return one point here, but that's not always the case (sometimes it pulls too early
                //or too late). The while loop is just to check to make sure that the time of the data point pulled is at
                //least greater than the user-supplied CurrentDate.
                while (nRet == 0)
                {
                    //Pull the next point iteratively. Warning- sometimes eDNA pulls a value before the start time?
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                    //Check that the date of the point is correct- sometimes eDNA pulls too early or too far
                    if (dtTime < currentDate) { continue; }

                    //Return the result.
                    return dValue;
                }

                //If no result is found, return -1.
                return -1.0;
            }
            // <summary> This method is used to calculate the summary of accumulated run hours in between the StartDate and
            // EndDate, with granularity of TimePeriod.</summary>
            // <param name="startDate">The beginning date to pull data for the run hour state.</param>
            // <param name="endDate">The last date to pull data for the run hour state.</param>
            // <param name="granularPeriod">The time period over which to obtain the summary. For example, if the supplied time period is 
            // "24:00:00", the output will be a daily summary. If the difference between the Start Date and End
            // Date is 2 days in this example, 3 elements will be present in the output list.</param>
            //public RunHourOverallSummary CalculateSummary(DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            //{
            //    var tempSummaryList = new List<RunHourGranularSummary>();
            //    Tuple<DateTime, TimeSpan> curValue = this.PullCurrentValue();

            //    //Pull the operating state over each time period
            //    for (DateTime granularDT = startDate; granularDT < endDate; granularDT = granularDT.Add(granularPeriod))
            //    {
            //        //Check that the granular end time is not greater than the absolute end time
            //        DateTime granularDTEnd = granularDT.Add(granularPeriod);
            //        if (granularDTEnd > endDate) { granularDTEnd = endDate; }

            //        //Actually pull the steady-state summary and add it to the list
            //        OpStateTools.OpSummary opSummary = this.SteadyOp.PullLimitStatusSummary(granularDT, granularDTEnd);
            //        tempSummaryList.Add(new RunHourGranularCalcSummary(opSummary.Inside, opSummary.Outside, opSummary.Unknown,
            //            granularDT, granularDTEnd));
            //    }

            //    return new RunHourOverallSummary(tempSummaryList, this.RunHourTag, curValue, this.AssignedEngineer, this.Description);
            //}
        }
        /// <summary>This class defines a stress event tag and can be used to find a summary of the values over a granular time period.</summary>
        public class StressEventReview
        {
            /// <summary>The fully qualified (Site.Service.Point) eDNA tag (in the run hour service) associated with the stress event.</summary>
            public string StressEventTag { get; private set; }
            /// <summary>The fully qualified (Site.Service.Point) eDNA tag (in the calc service) associated with the stress event.</summary>
            public string CalcTag { get; private set; }
            /// <summary>The multiplier for the stress event.</summary>
            public decimal Multiplier { get; private set; }
            /// <summary>A description of the stress event.</summary>
            public string Description { get; private set; }
            /// <summary>The engineer assigned to review the stress events.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>This class defines a stress event tag and can be used to find a summary of the values over a granular time period.</summary>
            /// <param name="stressEventTag">The fully qualified eDNA point tag (Site.Service.Point) of the stress event point from the run hour service. This 
            /// point will be used to pull the starting values of the stress event tag, and it will also be used to push values back 
            /// into eDNA.</param>
            /// <param name="multiplier">The multiplier associated with the stress event.</param>
            /// <param name="calcTag">The calc tag associated with the stress event. This point is used to accumulate the stress events, which will be
            /// pushed into the run service.</param>
            /// <param name="description">A brief description of the eDNA stress event point. This will be used for output/display purposes. By default, it is 
            /// equal to the eDNA point tag.</param>
            /// <param name="assignedEngineer">The engineer assigned to review the stress event. By default, this is "Unassigned".</param>
            public StressEventReview(string stressEventTag, string calcTag, decimal multiplier = Decimal.Zero, string description = "", string assignedEngineer = "None")
            {
                this.StressEventTag = stressEventTag.Trim();
                this.Multiplier = multiplier;
                this.Description = (String.IsNullOrWhiteSpace(description.Trim())) ? this.StressEventTag : description.Trim();
                this.CalcTag = calcTag;
                this.AssignedEngineer = assignedEngineer;
            }
            /// <summary>This method is used to calculate the summary of accumulated stress events in between the StartDate and
            /// EndDate, with granularity of TimePeriod.</summary>
            /// <param name="startDate">The beginning date to pull data for the event state.</param>
            /// <param name="endDate">The last date to pull data for the event state.</param>
            /// <param name="granularPeriod">The time period over which to obtain the summary. For example, if the supplied time period is 
            /// "24:00:00", the output will be a daily summary. If the difference between the Start Date and End
            /// Date is 2 days in this example, 3 elements will be present in the output list.</param>
            /// <param name="eventDefValue">The value that defines when an event occurs (typically "1").</param>
            public StressEventOverallSummary CalculateSummary(DateTime startDate, DateTime endDate, TimeSpan granularPeriod, double eventDefValue = 1)
            {
                var tempSummaryList = new List<StressEventGranularSummary>();
                Tuple<DateTime, double> curValue = MiscMethods.PullRealTimeValue(this.StressEventTag);

                //eDNA initialization
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;

                //Use the CalcOpState to iterate over each day
                for (DateTime granularDT = startDate; granularDT < endDate; granularDT = granularDT.Add(granularPeriod))
                {
                    int counter = 0;

                    //Check that the granular end time is not greater than the absolute end time
                    DateTime granularDTEnd = granularDT.Add(granularPeriod);
                    if (granularDTEnd > endDate) { granularDTEnd = endDate; }

                    nRet = History.DnaGetHistRaw(this.CalcTag, granularDT, granularDTEnd, out uiKey);

                    //Iterate over all the points
                    while (nRet == 0)
                    {
                        nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                        //Check that the point is within bounds- sometimes eDNA pulls too early or too far
                        if (dtTime < granularDT || dtTime > granularDTEnd) { continue; }
                        //Increment the stress event counter
                        counter = (dValue == eventDefValue) ? counter + 1 : counter;
                    }

                    //Add a new stress event summary to the list
                    tempSummaryList.Add(new StressEventGranularSummary(counter, granularDT, granularDTEnd));
                }

                return new StressEventOverallSummary(tempSummaryList, this.StressEventTag, curValue, this.Multiplier, this.AssignedEngineer, this.Description);
            }
        }
        /// <summary>This class defines a stress hour tag and can be used to find a summary of the values over a granular time period.</summary>
        public class StressHourReview
        {
            /// <summary>The output tag to which stress hours will be written.</summary>
            public string OutputTag { get; private set; }
            /// <summary>The run hour tag associated with the stress hour review.</summary>
            public string RunHourTag { get; private set; }
            /// <summary>The calc tag associated with the stress hour tag.</summary>
            public string CalcTag { get; private set; }
            /// <summary>The multiplier for the stress hour.</summary>
            public decimal Multiplier { get; private set; }
            /// <summary>A description of the stress hour tag.</summary>
            public string Description { get; private set; }
            /// <summary>The steady-state operating limit associated with the stress hour review.</summary>
            public OpStateTools.OpLimit SteadyOp { get; private set; }
            /// <summary>The engineer assigned to review the stress hours.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>This class defines a stress hour tag and can be used to find an overall summary of the values over a granular time period.</summary>
            /// <param name="outputTag">The output tag to which stress hours will be written.</param>
            /// <param name="runHourTag">The fully qualified eDNA point tag (Site.Service.Point) of the stress hour point. This point will be used to pull 
            /// the starting values of the stress hour tag, and it will also be used to push values back into eDNA.</param>
            /// <param name="calcTag">The fully qualified eDNA point tag (Site.Service.Point) of the calc point associated with the stress hour point.</param>
            /// <param name="description">A brief description of the eDNA stress hour point. This will be used for output/display purposes. By default, it is 
            /// equal to the eDNA stress hour tag.</param>
            /// <param name="assignedEngineer">The engineer assigned to review the stress hours. By default, this is "Unassigned".</param>
            /// <param name="steadyStateDef">The value that defines whether the equipment is in steady-state (typically "1").</param>
            /// /// <param name="multiplier">The multiplier associated with the stress hour.</param>
            public StressHourReview(string outputTag, string runHourTag, string calcTag, decimal multiplier = Decimal.Zero, string description = "",
                string assignedEngineer = "Unassigned", double steadyStateDef = 1)
            {
                this.OutputTag = outputTag.Trim();
                this.RunHourTag = runHourTag.Trim();
                this.CalcTag = calcTag.Trim();
                this.Multiplier = multiplier;
                this.Description = (String.IsNullOrWhiteSpace(description.Trim())) ? this.RunHourTag : description.Trim();
                this.AssignedEngineer = assignedEngineer.Trim();
                //Construct an OpLimit class which will be used to calculate the run hour summary directly, if needed.
                this.SteadyOp = new OpStateTools.OpLimit(steadyStateDef, steadyStateDef, this.CalcTag);
            }
            /// <summary> This method is used to calculate the summary of accumulated run hours in between the StartDate and
            /// EndDate, with granularity of TimePeriod.</summary>
            /// <param name="startDate">The beginning date to pull data for the run hour state.</param>
            /// <param name="endDate">The last date to pull data for the run hour state.</param>
            /// <param name="granularPeriod">The time period over which to obtain the summary. For example, if the supplied time period is 
            /// "24:00:00", the output will be a daily summary. If the difference between the Start Date and End
            /// Date is 2 days in this example, 3 elements will be present in the output list.</param>
            public StressHourOverallSummary CalculateSummary(DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                var tempSummaryList = new List<StressHourGranularSummary>();
                Tuple<DateTime, double> rtValue = MiscMethods.PullRealTimeValue(this.OutputTag);

                double lastValue = this.PullSnapValue(startDate);
                //Pull the operating state over each time period
                for (DateTime granularDT = startDate; granularDT < endDate; granularDT = granularDT.Add(granularPeriod))
                {
                    //Check that the granular end time is not greater than the absolute end time
                    DateTime granularDTEnd = granularDT.Add(granularPeriod);
                    if (granularDTEnd > endDate) { granularDTEnd = endDate; }

                    //Pull the current value at granularDTEnd using "Snap" mode.
                    double curValue = this.PullSnapValue(granularDTEnd);
                    tempSummaryList.Add(new StressHourGranularSummary(curValue - lastValue, granularDT, granularDTEnd));

                    lastValue = curValue;
                }

                return new StressHourOverallSummary(tempSummaryList, this.OutputTag, rtValue, this.Multiplier, this.AssignedEngineer, this.Description);
            }
            /// <summary>Pulls the current real-time value of the output tag.</summary>
            private double PullSnapValue(DateTime currentDate)
            {
                //eDNA Initialization
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;
                nRet = History.DnaGetHistSnap(this.RunHourTag, currentDate, currentDate, new TimeSpan(0, 0, 0, 1), out uiKey);

                //Ideally, eDNA will just return one point here, but that's not always the case (sometimes it pulls too early
                //or too late). The while loop is just to check to make sure that the time of the data point pulled is at
                //least greater than the user-supplied CurrentDate.
                while (nRet == 0)
                {
                    //Pull the next point iteratively. Warning- sometimes eDNA pulls a value before the start time?
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                    //Check that the date of the point is correct- sometimes eDNA pulls too early or too far
                    if (dtTime < currentDate) { continue; }

                    //Return the result.
                    return dValue;
                }

                //If no result is found, return -1.
                return -1.0;
            }
        }

        //Data structures
        /// <summary>This data structure stores the overall data review summary, using a list of RunHourOverallSummary and StressEventOverallSummary instances.</summary>
        public class OverallDataReviewSummary
        {
            /// <summary>A list of RunHourOverallSummary instances, obtained from RunHourReview initialization instances.</summary>
            public List<RunHourOverallSummary> RunHourSummary { get; private set; }
            /// <summary>A list of StressEventOverallSummary instances, obtained from StressEventReview initialization instances.</summary>
            public List<StressEventOverallSummary> StressEventSummary { get; private set; }
            /// <summary>A list of StressHourOverallSummary instances, obtained from StressHourReview initialization instances.</summary>
            public List<StressHourOverallSummary> StressHourSummary { get; private set; }
            /// <summary>A list of the time ranges used for the data summary. Constructed automatically based on user input.</summary>
            public List<Tuple<DateTime, DateTime>> ReviewTimeRanges { get; private set; }
            /// <summary>This data structure stores the overall data review summary, using a list of RunHourOverallSummary and StressEventOverallSummary instances.</summary>
            /// <param name="startDate">The start date of the review.</param>
            /// <param name="endDate">The end date of the review.</param>
            /// <param name="granularPeriod">The time period granularity to use.</param>
            /// <param name="runHourSummary">A list of RunHourOverallSummary instances, obtained from RunHourReview initialization instances.</param>
            /// <param name="stressEventSummary">A list of StressEventOverallSummary instances, obtained from StressEventReview initialization instances.</param>
            /// <param name="stressHourSummary">A list of StressHourOverallSummary instances, obtained from StressHourReview initialization instances.</param>
            public OverallDataReviewSummary(DateTime startDate, DateTime endDate, TimeSpan granularPeriod,
                List<RunHourOverallSummary> runHourSummary, List<StressEventOverallSummary> stressEventSummary, List<StressHourOverallSummary> stressHourSummary)
            {
                this.RunHourSummary = runHourSummary;
                this.StressEventSummary = stressEventSummary;
                this.StressHourSummary = stressHourSummary;
                //Calculate the review time ranges
                this.ConstructReviewTimeRanges(startDate, endDate, granularPeriod);
            }
            /// <summary>This constructor will import a data review summary from an Excel file (typically one written by the "WriteReviewToExcel" method,
            /// after being reviewed/corrected by the engineers).</summary>
            /// <param name="filePath">The fully-qualified path to the Excel import file.</param>
            public OverallDataReviewSummary(string filePath)
            {
                //The idea is that we need to do the "Construct to X Worksheet" methods, but in reverse.
                var workbook = new Excel.XLWorkbook(filePath);

                //Initialize values
                Excel.IXLWorksheet machineHourSheet;
                Excel.IXLWorksheet stressHourSheet;
                Excel.IXLWorksheet stressEventSheet;
                this.RunHourSummary = new List<RunHourOverallSummary>();
                this.StressEventSummary = new List<StressEventOverallSummary>();
                this.StressHourSummary = new List<StressHourOverallSummary>();

                if (workbook.Worksheets.TryGetWorksheet("Machine Hours", out machineHourSheet) &&
                    workbook.Worksheets.TryGetWorksheet("Stress Hours (SEVV)", out stressHourSheet) &&
                    workbook.Worksheets.TryGetWorksheet("Stress Events (SEV)", out stressEventSheet))
                {
                    this.ReadReviewTimeRanges(machineHourSheet);
                    this.ReadMachineHourValues(machineHourSheet);
                    this.ReadStressEventValues(stressEventSheet);
                    this.ReadStressHourValues(stressHourSheet);
                }
            }
            private void ConstructReviewTimeRanges(DateTime startDate, DateTime endDate, TimeSpan granularPeriod)
            {
                //Construct a list of time periods
                var timePeriodList = new List<Tuple<DateTime, DateTime>>();
                for (DateTime granularDate = startDate; granularDate < endDate; granularDate = granularDate.Add(granularPeriod))
                {
                    DateTime granularDateEnd = (granularDate.Add(granularPeriod) > endDate) ? endDate : granularDate.Add(granularPeriod);
                    timePeriodList.Add(new Tuple<DateTime, DateTime>(granularDate, granularDateEnd));
                }

                this.ReviewTimeRanges = timePeriodList;
            }
            private void ReadReviewTimeRanges(Excel.IXLWorksheet xlSheet)
            {
                this.ReviewTimeRanges = new List<Tuple<DateTime, DateTime>>();

                //Find the total number of columns
                int columnCount = xlSheet.LastCellUsed().Address.ColumnNumber;

                //Iterate over all the columns from 8 (the start of the review ranges) to the total column count found above
                for (int index = 8; index < columnCount + 1; index++)
                {
                    DateTime startDate = xlSheet.Cell(2, index).GetDateTime();
                    DateTime endDate = xlSheet.Cell(3, index).GetDateTime();
                    this.ReviewTimeRanges.Add(new Tuple<DateTime, DateTime>(startDate, endDate));
                }
            }
            private void ReadStressHourValues(Excel.IXLWorksheet xlSheet)
            {
                var tempSumList = new List<StressHourOverallSummary>();

                //Find the max number of rows and columns that were written
                int columnCount = xlSheet.LastCellUsed().Address.ColumnNumber;
                int rowCount = xlSheet.LastCellUsed().Address.RowNumber;

                //Iterate through each row. Each row is a "RunHourOverallSummary" instance.
                for (int rowIndex = 4; rowIndex < rowCount + 1; rowIndex++)
                {
                    //Find the granular summaries
                    var tempGranList = new List<StressHourGranularSummary>();
                    for (int columnIndex = 8; columnIndex < columnCount + 1; columnIndex++)
                    {
                        double cellValue = xlSheet.Cell(rowIndex, columnIndex).GetDouble();
                        DateTime startDate = this.ReviewTimeRanges[columnIndex - 8].Item1;
                        DateTime endDate = this.ReviewTimeRanges[columnIndex - 8].Item2;

                        //Add the above values to the list
                        tempGranList.Add(new StressHourGranularSummary(cellValue, startDate, endDate));
                    }

                    //Grab the required values.
                    string assignedEngineer = xlSheet.Cell(rowIndex, 1).GetString();
                    string eDNATag = xlSheet.Cell(rowIndex, 2).GetString();
                    string description = xlSheet.Cell(rowIndex, 3).GetString();
                    DateTime startValueDate = xlSheet.Cell(rowIndex, 4).GetDateTime();
                    double startValue = xlSheet.Cell(rowIndex, 5).GetDouble();
                    double endValue = xlSheet.Cell(rowIndex, 6).GetDouble();
                    double changeValue = xlSheet.Cell(rowIndex, 7).GetDouble();

                    //Add to the overall list.
                    tempSumList.Add(new StressHourOverallSummary(tempGranList, eDNATag, new Tuple<DateTime, double>(startValueDate, startValue), Decimal.Zero, assignedEngineer, description));
                }

                //Save the results into the object properties.
                this.StressHourSummary = tempSumList;
            }
            private void ReadStressEventValues(Excel.IXLWorksheet xlSheet)
            {
                var tempSumList = new List<StressEventOverallSummary>();

                //Find the max number of rows and columns that were written
                int columnCount = xlSheet.LastCellUsed().Address.ColumnNumber;
                int rowCount = xlSheet.LastCellUsed().Address.RowNumber;

                //Iterate through each row. Each row is a "RunHourOverallSummary" instance.
                for (int rowIndex = 4; rowIndex < rowCount + 1; rowIndex++)
                {
                    //Find the granular summaries
                    var tempGranList = new List<StressEventGranularSummary>();
                    for (int columnIndex = 8; columnIndex < columnCount + 1; columnIndex++)
                    {
                        double cellValue = xlSheet.Cell(rowIndex, columnIndex).GetDouble();
                        DateTime startDate = this.ReviewTimeRanges[columnIndex - 8].Item1;
                        DateTime endDate = this.ReviewTimeRanges[columnIndex - 8].Item2;

                        //Add the above values to the list
                        tempGranList.Add(new StressEventGranularSummary(cellValue, startDate, endDate));
                    }

                    //Grab the required values.
                    string assignedEngineer = xlSheet.Cell(rowIndex, 1).GetString();
                    string eDNATag = xlSheet.Cell(rowIndex, 2).GetString();
                    string description = xlSheet.Cell(rowIndex, 3).GetString();
                    DateTime startValueDate = xlSheet.Cell(rowIndex, 4).GetDateTime();
                    double startValue = xlSheet.Cell(rowIndex, 5).GetDouble();
                    double endValue = xlSheet.Cell(rowIndex, 6).GetDouble();
                    double changeValue = xlSheet.Cell(rowIndex, 7).GetDouble();

                    //Add to the overall list.
                    tempSumList.Add(new StressEventOverallSummary(tempGranList, eDNATag, new Tuple<DateTime, double>(startValueDate, startValue), Decimal.Zero, assignedEngineer, description));
                }

                //Save the results into the object properties.
                this.StressEventSummary = tempSumList;
            }
            private void ReadMachineHourValues(Excel.IXLWorksheet xlSheet)
            {
                var tempSumList = new List<RunHourOverallSummary>();

                //Find the max number of rows and columns that were written
                int columnCount = xlSheet.LastCellUsed().Address.ColumnNumber;
                int rowCount = xlSheet.LastCellUsed().Address.RowNumber;

                //Iterate through each row. Each row is a "RunHourOverallSummary" instance.
                for (int rowIndex = 4; rowIndex < rowCount + 1; rowIndex++)
                {
                    //Find the granular summaries
                    var tempGranList = new List<RunHourGranularSummary>();
                    for (int columnIndex = 8; columnIndex < columnCount + 1; columnIndex++)
                    {
                        double cellValue = xlSheet.Cell(rowIndex, columnIndex).GetDouble();
                        DateTime startDate = this.ReviewTimeRanges[columnIndex - 8].Item1;
                        DateTime endDate = this.ReviewTimeRanges[columnIndex - 8].Item2;

                        //Add the above values to the list
                        tempGranList.Add(new RunHourGranularSummary(cellValue, startDate, endDate));
                    }

                    //Grab the required values.
                    string assignedEngineer = xlSheet.Cell(rowIndex, 1).GetString();
                    string eDNATag = xlSheet.Cell(rowIndex, 2).GetString();
                    string description = xlSheet.Cell(rowIndex, 3).GetString();
                    DateTime startValueDate = xlSheet.Cell(rowIndex, 4).GetDateTime();
                    double startValue = xlSheet.Cell(rowIndex, 5).GetDouble();
                    double endValue = xlSheet.Cell(rowIndex, 6).GetDouble();
                    double changeValue = xlSheet.Cell(rowIndex, 7).GetDouble();

                    //Add to the overall list.
                    tempSumList.Add(new RunHourOverallSummary(tempGranList, eDNATag, new Tuple<DateTime, double>(startValueDate, startValue), assignedEngineer, description));
                }

                //Save the results into the object properties.
                this.RunHourSummary = tempSumList;
            }
            /// <summary>Push the accumulated summary values to eDNA.</summary>
            public void PushToEdna()
            {
                var pushSummary = new PushTools.PushAllData(this);

                //Push values to history
                foreach (var historyService in pushSummary.HistoryDataPush)
                {
                    historyService.PushAppendAllTags();
                }

                //Push values to real-time (quadruple pass, just in case)
                foreach (var rtService in pushSummary.RealTimeDataPush)
                {
                    rtService.PushRealTimeAllTags(rtService.Service);
                }
                Thread.Sleep(100);
                foreach (var rtService in pushSummary.RealTimeDataPush)
                {
                    rtService.PushRealTimeAllTags(rtService.Service);
                }
                Thread.Sleep(100);
                foreach (var rtService in pushSummary.RealTimeDataPush)
                {
                    rtService.PushRealTimeAllTags(rtService.Service);
                }
                Thread.Sleep(100);
                foreach (var rtService in pushSummary.RealTimeDataPush)
                {
                    rtService.PushRealTimeAllTags(rtService.Service);
                }
            }
            public Excel.IXLWorksheet WriteReviewTimeRanges(Excel.IXLWorksheet xlSheet)
            {
                //Construct the time ranges of the review. These appear on the second and third rows, and they are useful when nonstandard granular time
                //periods are selected by the user.
                for (int index = 0; index < this.ReviewTimeRanges.Count; index++)
                {
                    //Since the header constructed above goes to column index 7, these need to start at column index 8.
                    int columnIndex = 8 + index;
                    //The start time is on the second row, and the end time is on the third row.
                    xlSheet.Cell(2, columnIndex).Value = this.ReviewTimeRanges[index].Item1.ToString();
                    xlSheet.Cell(3, columnIndex).Value = this.ReviewTimeRanges[index].Item2.ToString();
                }

                return xlSheet;
            }
            public Excel.IXLWorksheet WriteSheetHeader(Excel.IXLWorksheet xlSheet, string assignedEngineer = "Eng", string eDNATag = "Tag", string description = "Description",
                string startDate = "Start Date", string startValue = "Start Value", string endValue = "End Value", string lastParam = "Change")
            {
                //Construct the main header for the sheet, which starts on the third row down. The first row must be left empty so that the operating 
                //state of the equipment ("In Port" or "Underway") is filled in manually by the person preparing the review. 
                xlSheet.Cell(3, 1).Value = assignedEngineer;
                xlSheet.Cell(3, 2).Value = eDNATag;
                xlSheet.Cell(3, 3).Value = description;
                xlSheet.Cell(3, 4).Value = startDate;
                xlSheet.Cell(3, 5).Value = startValue;
                xlSheet.Cell(3, 6).Value = endValue;
                xlSheet.Cell(3, 7).Value = lastParam;

                return xlSheet;
            }
            public Excel.IXLWorksheet WriteRowInfoColumns(Excel.IXLWorksheet xlSheet, int rowIndex, string col1 = "None", string col2 = "None",
                string col3 = "None", string col4 = "0001/01/01", double col5 = 0.0, double col6 = 0.0, double col7 = 0.0)
            {
                //Write each info column based on user-supplied parameters
                xlSheet.Cell(rowIndex, 1).Value = col1;
                xlSheet.Cell(rowIndex, 2).Value = col2;
                xlSheet.Cell(rowIndex, 3).Value = col3;
                xlSheet.Cell(rowIndex, 4).Value = col4;
                xlSheet.Cell(rowIndex, 5).Value = col5;
                xlSheet.Cell(rowIndex, 6).Value = col6;
                xlSheet.Cell(rowIndex, 7).Value = col7;

                return xlSheet;
            }
            public Excel.IXLWorksheet WriteSheetFormatting(Excel.IXLWorksheet xlSheet, Excel.XLColor color, double lowVal, double highVal)
            {
                //Find the max number of rows and columns that were written
                int columnCount = xlSheet.LastCellUsed().Address.ColumnNumber;
                int rowCount = xlSheet.LastCellUsed().Address.RowNumber;

                //Formatting for readability.
                xlSheet.SheetView.Freeze(3, 3);
                xlSheet.Columns().AdjustToContents();
                xlSheet.SetTabColor(color);
                xlSheet.Range(4, 8, rowCount, columnCount).AddConditionalFormat().WhenEquals(0).Fill.BackgroundColor = Excel.XLColor.Blue;
                xlSheet.Range(4, 8, rowCount, columnCount).AddConditionalFormat().WhenBetween(lowVal, highVal).Fill.BackgroundColor = color;
                xlSheet.Range(4, 8, rowCount, columnCount).AddConditionalFormat().WhenEqualOrGreaterThan(highVal).Fill.BackgroundColor = Excel.XLColor.Red;

                return xlSheet;
            }
            public Excel.IXLWorksheet WriteMachineHourValues(Excel.IXLWorksheet xlSheet)
            {
                //Run the main methods
                this.WriteReviewTimeRanges(xlSheet);
                this.WriteSheetHeader(xlSheet, "Eng", "Tag", "Description", "Start Date", "Start Value", "End Value", "Change");

                //Construct the summary values for each RunHourOverallSummary in the overall list.
                for (int summaryIndex = 0; summaryIndex < this.RunHourSummary.Count; summaryIndex++)
                {
                    RunHourOverallSummary curSum = this.RunHourSummary[summaryIndex];

                    //Since the header constructed above goes to row index 3, these need to start a row index 4.
                    int rowIndex = 4 + summaryIndex;
                    xlSheet = this.WriteRowInfoColumns(xlSheet, rowIndex, curSum.AssignedEngineer, curSum.RunHourTag, curSum.Description,
                        curSum.CurrentValue.Item1.ToString(), curSum.CurrentValue.Item2, curSum.EndingValue, curSum.ValueChange);

                    //The rest of the columns contain summary information about run hours per granular time period.
                    for (int granularIndex = 0; granularIndex < curSum.GranularSummaries.Count; granularIndex++)
                    {
                        //Since the basic information constructed above goes to column index 7, these need to start at column index 8.
                        int columnIndex = 8 + granularIndex;

                        //Add the hours running to the correct cell.
                        xlSheet.Cell(rowIndex, columnIndex).Value = curSum.GranularSummaries[granularIndex].HoursRunning;
                    }
                }

                this.WriteSheetFormatting(xlSheet, Excel.XLColor.Green, 0.0001, 24.2);
                return xlSheet;
            }
            public Excel.IXLWorksheet WriteStressHourEtaReductionValues(Excel.IXLWorksheet xlSheet)
            {
                //Run the main methods
                this.WriteReviewTimeRanges(xlSheet);
                this.WriteSheetHeader(xlSheet, "Eng", "Tag", "Description", "Start Date", "Start Value", "End Value", "Multiplier");

                //Construct the summary values for each StressHourOverallSummary in the overall list.
                for (int summaryIndex = 0; summaryIndex < this.StressHourSummary.Count; summaryIndex++)
                {
                    StressHourOverallSummary curSum = this.StressHourSummary[summaryIndex];

                    //Since the header constructed above goes to row index 3, these need to start a row index 4.
                    int rowIndex = 4 + summaryIndex;
                    xlSheet = this.WriteRowInfoColumns(xlSheet, rowIndex, curSum.AssignedEngineer, curSum.StressHourTag, curSum.Description, curSum.CurrentValue.Item1.ToString(),
                        Convert.ToDouble(curSum.EtaReduction.StartingEtaReduction), Convert.ToDouble(curSum.EtaReduction.EndingEtaReduction), Convert.ToDouble(curSum.Multiplier));

                    //The rest of the columns contain summary information about the stress events and eta reduction per granular time period.
                    for (int granularIndex = 0; granularIndex < curSum.GranularSummaries.Count; granularIndex++)
                    {
                        //Since the basic information constructed above goes to column index 7, these need to start at column index 8.
                        int columnIndex = 8 + granularIndex;
                        xlSheet.Cell(rowIndex, columnIndex).Value = curSum.EtaReduction.GranularEtaRedution[granularIndex];
                    }
                }

                this.WriteSheetFormatting(xlSheet, Excel.XLColor.Yellow, 0.0001, 0.20);
                return xlSheet;
            }
            public Excel.IXLWorksheet WriteStressHourValues(Excel.IXLWorksheet xlSheet)
            {
                //Run the main methods
                this.WriteReviewTimeRanges(xlSheet);
                this.WriteSheetHeader(xlSheet, "Eng", "Tag", "Description", "Start Date", "Start Value", "End Value", "Change");

                //Construct the summary values for each StressHourOverallSummary in the overall list.
                for (int summaryIndex = 0; summaryIndex < this.StressHourSummary.Count; summaryIndex++)
                {
                    StressHourOverallSummary curSum = this.StressHourSummary[summaryIndex];

                    //Since the header constructed above goes to row index 3, these need to start a row index 4.
                    int rowIndex = 4 + summaryIndex;
                    xlSheet = this.WriteRowInfoColumns(xlSheet, rowIndex, curSum.AssignedEngineer, curSum.StressHourTag, curSum.Description,
                        curSum.CurrentValue.Item1.ToString(), curSum.CurrentValue.Item2, curSum.EndingValue, curSum.ValueChange);

                    //The rest of the columns contain summary information about the stress events and eta reduction per granular time period.
                    for (int granularIndex = 0; granularIndex < curSum.GranularSummaries.Count; granularIndex++)
                    {
                        //Since the basic information constructed above goes to column index 7, these need to start at column index 8.
                        int columnIndex = 8 + granularIndex;

                        xlSheet.Cell(rowIndex, columnIndex).Value = curSum.GranularSummaries[granularIndex].NumberHours;
                    }
                }

                this.WriteSheetFormatting(xlSheet, Excel.XLColor.Purple, 0.0001, 24.2);
                return xlSheet;
            }
            public Excel.IXLWorksheet WriteStressEventValues(Excel.IXLWorksheet xlSheet)
            {
                //Run the main methods
                this.WriteReviewTimeRanges(xlSheet);
                this.WriteSheetHeader(xlSheet, "Eng", "Tag", "Description", "Start Date", "Start Value", "End Value", "Change");

                //Construct the summary values for each StressEventOverallSummary in the overall list.
                for (int summaryIndex = 0; summaryIndex < this.StressEventSummary.Count; summaryIndex++)
                {
                    StressEventOverallSummary curSum = this.StressEventSummary[summaryIndex];

                    //Since the header constructed above goes to row index 3, these need to start a row index 4.
                    int rowIndex = 4 + summaryIndex;
                    xlSheet = this.WriteRowInfoColumns(xlSheet, rowIndex, curSum.AssignedEngineer, curSum.StressEventTag, curSum.Description,
                        curSum.CurrentValue.Item1.ToString(), curSum.CurrentValue.Item2, curSum.EndingValue, curSum.ValueChange);

                    //The rest of the columns contain summary information about the stress events and eta reduction per granular time period.
                    for (int granularIndex = 0; granularIndex < curSum.GranularSummaries.Count; granularIndex++)
                    {
                        //Since the basic information constructed above goes to column index 7, these need to start at column index 8.
                        int columnIndex = 8 + granularIndex;

                        xlSheet.Cell(rowIndex, columnIndex).Value = curSum.GranularSummaries[granularIndex].NumberEvents;
                    }
                }

                this.WriteSheetFormatting(xlSheet, Excel.XLColor.Purple, 0.0001, 0.20);
                return xlSheet;
            }
            public Excel.IXLWorksheet WriteStressEventEtaReductionValues(Excel.IXLWorksheet xlSheet)
            {
                //Run the main methods
                this.WriteReviewTimeRanges(xlSheet);
                this.WriteSheetHeader(xlSheet, "Eng", "Tag", "Description", "Start Date", "Start Value", "End Value", "Multiplier");

                //Construct the summary values for each StressEventOverallSummary in the overall list.
                for (int summaryIndex = 0; summaryIndex < this.StressEventSummary.Count; summaryIndex++)
                {
                    StressEventOverallSummary curSum = this.StressEventSummary[summaryIndex];

                    //Since the header constructed above goes to row index 3, these need to start a row index 4.
                    int rowIndex = 4 + summaryIndex;
                    xlSheet = this.WriteRowInfoColumns(xlSheet, rowIndex, curSum.AssignedEngineer, curSum.StressEventTag, curSum.Description, curSum.CurrentValue.Item1.ToString(),
                        Convert.ToDouble(curSum.EtaReduction.StartingEtaReduction), Convert.ToDouble(curSum.EtaReduction.EndingEtaReduction), Convert.ToDouble(curSum.Multiplier));

                    //The rest of the columns contain summary information about the stress events and eta reduction per granular time period.
                    for (int granularIndex = 0; granularIndex < curSum.GranularSummaries.Count; granularIndex++)
                    {
                        //Since the basic information constructed above goes to column index 7, these need to start at column index 8.
                        int columnIndex = 8 + granularIndex;
                        xlSheet.Cell(rowIndex, columnIndex).Value = curSum.EtaReduction.GranularEtaRedution[granularIndex];
                    }
                }

                this.WriteSheetFormatting(xlSheet, Excel.XLColor.Yellow, 0.0001, 24.2);
                return xlSheet;
            }
        }
        /// <summary>This data structure stores the overall summary for a particular run hour tag, and includes a list of 
        /// RunHourGranularSummary instances for each selected summary time period.</summary>
        public struct RunHourOverallSummary
        {
            /// <summary>The engineer assigned to review the run hours.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>The fully-qualified (Site.Service.Point) eDNA tag in the run hour service associated with the run hours.</summary>
            public string RunHourTag { get; private set; }
            /// <summary>A description of the run hours (Example- "SSDG Running").</summary>
            public string Description { get; private set; }
            /// <summary>A list of run hour summaries for each time period in the data review.</summary>
            public List<RunHourGranularSummary> GranularSummaries { get; private set; }
            /// <summary>The current value of the run hour tag, pulled from the run hour service.</summary>
            public Tuple<DateTime, double> CurrentValue { get; private set; }
            /// <summary>The ending value of the run hours, after each granular summary is added to the current value.</summary>
            public double EndingValue { get; private set; }
            /// <summary>The amount the run hour value changed from the current value to the ending value.</summary>
            public double ValueChange { get; private set; }
            /// <summary>This class stores the overall summary for a particular run hour tag, and includes a list of 
            /// RunHourGranularSummary instances for each selected summary time period.</summary>
            /// <param name="runHourTag">The eDNA tag in the run hour service associated with the machine hour.</param>
            /// <param name="assignedEngineer">An optional engineer assigned to review the run hours.</param>
            /// <param name="description">An optional description of the run hours.</param>
            /// <param name="granularSummaries">A list of run hour summaries for each time period in the data review.</param>
            /// <param name="currentValue">The eDNA tag in the run hour service associated with the machine hour.</param>
            public RunHourOverallSummary(List<RunHourGranularSummary> granularSummaries, string runHourTag, Tuple<DateTime, double> currentValue,
                string assignedEngineer = "None", string description = "None")
                : this()
            {
                this.RunHourTag = runHourTag;
                this.AssignedEngineer = assignedEngineer;
                this.Description = description;
                this.GranularSummaries = granularSummaries;
                this.CurrentValue = currentValue;

                this.CalculateEndingValue();
                this.ValueChange = Math.Round(this.EndingValue - this.CurrentValue.Item2, 3);
            }
            /// <summary>This method will calculate the ending value of the stress event, after each granular summary is added to the current value.</summary>
            private void CalculateEndingValue()
            {
                var retValue = this.CurrentValue.Item2;

                //Iterate over every value in this.GranularSummaries, adding each granular number of events.
                foreach (RunHourGranularSummary granularSummary in this.GranularSummaries)
                {
                    retValue += granularSummary.HoursRunning;
                }

                this.EndingValue = Math.Round(retValue, 3);
            }
        }
        /// <summary>This data structure stores the overall summary for a particular stress hour tag, and includes a list of 
        /// StressHourGranularSummary instances for each selected summary time period.</summary>
        public struct StressHourOverallSummary
        {
            /// <summary>The multiplier associated with the stress hour.</summary>
            public decimal Multiplier { get; private set; }
            /// <summary>The engineer assigned to review the stress hours.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>The fully qualified (Site.Service.ID) eDNA tag (in the run hour service) of the stress hour.</summary>
            public string StressHourTag { get; private set; }
            /// <summary>A description of the stress hour.</summary>
            public string Description { get; private set; }
            /// <summary>A list of stress hour summaries for each time period in the data review.</summary>
            public List<StressHourGranularSummary> GranularSummaries { get; private set; }
            /// <summary>The total eta reduction over each granular time period. Calculated automatically.</summary>
            public EtaReductionOverallSummary EtaReduction { get; private set; }
            /// <summary>The current value of the stress hour tag, pulled from the run hour service.</summary>
            public Tuple<DateTime, double> CurrentValue { get; private set; }
            /// <summary>The ending value of the stress hour, after each granular summary is added to the current value.</summary>
            public double EndingValue { get; private set; }
            /// <summary>The amount the stress hour value changed from the current value to the ending value.</summary>
            public double ValueChange { get; private set; }
            /// <summary>This data structure stores the overall summary for a particular stress hour tag, and includes a list of 
            /// StressHourGranularSummary instances for each selected summary time period.</summary>
            /// <param name="granularSummaries">A list of stress hour summaries for each time period in the data review.</param>
            /// <param name="stressHourTag">The fully qualified (Site.Service.ID) eDNA tag (in the run hour service) of the stress hour.</param>
            /// <param name="currentValue">The current value of the stress hour tag, pulled from the run hour service.</param>
            /// <param name="assignedEngineer">The engineer assigned to review the stress hours.</param>
            /// <param name="description">A description of the stress hour.</param>
            /// <param name="multiplier">The multiplier associated with the stress hour.</param>
            public StressHourOverallSummary(List<StressHourGranularSummary> granularSummaries, string stressHourTag, Tuple<DateTime, double> currentValue,
                decimal multiplier = Decimal.Zero, string assignedEngineer = "None", string description = "None")
                : this()
            {
                this.GranularSummaries = granularSummaries;
                this.StressHourTag = stressHourTag;
                this.CurrentValue = currentValue;
                this.Multiplier = multiplier;
                this.AssignedEngineer = assignedEngineer;
                this.Description = description;
                //Run a few data summary methods automatically for easy output.
                this.CalculateEtaReduction();
                this.CalculateEndingValue();
                this.ValueChange = Math.Round(this.EndingValue - this.CurrentValue.Item2, 3);
            }
            /// <summary>This method will calculate the total eta reduction over each granular time period.</summary>
            private void CalculateEtaReduction()
            {
                var etaReductionList = new List<decimal>();

                //Calculate the eta reduction at the current time.
                decimal currentStressHourValue = Convert.ToDecimal(this.CurrentValue.Item2);
                decimal currentEtaReduction = Math.Round(currentStressHourValue * this.Multiplier, 3);

                //Iterate over every value in this.GranularSummaries, multiplying the multiplier times the total events
                //(current value + summary events).
                foreach (StressHourGranularSummary granularSummary in this.GranularSummaries)
                {
                    currentStressHourValue += Convert.ToDecimal(granularSummary.NumberHours);
                    etaReductionList.Add(Math.Round(currentStressHourValue * this.Multiplier, 3));
                }

                //Calculate the final eta reduction
                decimal finalEtaReduction = Math.Round(currentStressHourValue * this.Multiplier, 3);

                this.EtaReduction = new EtaReductionOverallSummary(currentEtaReduction, finalEtaReduction, etaReductionList);
            }
            /// <summary>This method will calculate the ending value of the stress event, after each granular summary is added to the current value.</summary>
            private void CalculateEndingValue()
            {
                var retValue = this.CurrentValue.Item2;

                //Iterate over every value in this.GranularSummaries, adding each granular number of events.
                foreach (StressHourGranularSummary granularSummary in this.GranularSummaries)
                {
                    retValue += granularSummary.NumberHours;
                }

                this.EndingValue = Math.Round(retValue, 3);
            }
        }
        /// <summary>This data structure stores the overall summary for a particular stress event tag, and includes a list of 
        /// StressEventGranularSummary instances for each selected summary time period.</summary>
        public struct StressEventOverallSummary
        {
            /// <summary>The multiplier associated with the stress event.</summary>
            public decimal Multiplier { get; private set; }
            /// <summary>The engineer assigned to review the stress events.</summary>
            public string AssignedEngineer { get; private set; }
            /// <summary>The fully qualified (Site.Service.ID) eDNA tag (in the run hour service) of the stress event.</summary>
            public string StressEventTag { get; private set; }
            /// <summary>A description of the stress event.</summary>
            public string Description { get; private set; }
            /// <summary>A list of stress event summaries for each time period in the data review.</summary>
            public List<StressEventGranularSummary> GranularSummaries { get; private set; }
            /// <summary>The total eta reduction over each granular time period. Calculated automatically.</summary>
            public EtaReductionOverallSummary EtaReduction { get; private set; }
            /// <summary>The current value of the stress event tag, pulled from the run hour service.</summary>
            public Tuple<DateTime, double> CurrentValue { get; private set; }
            /// <summary>The ending value of the stress event, after each granular summary is added to the current value.</summary>
            public double EndingValue { get; private set; }
            /// <summary>The amount the stress event value changed from the current value to the ending value.</summary>
            public double ValueChange { get; private set; }
            /// <summary>This class stores the overall summary for a particular stress event tag, and includes a list of 
            /// StressEventGranularSummary instances for each selected summary time period.</summary>
            /// <param name="granularSummaries">A list of stress event summaries for each time period in the data review.</param>
            /// <param name="stressEventTag">The fully qualified (Site.Service.ID) eDNA tag (in the run hour service) of the stress event.</param>
            /// <param name="currentValue">The current value of the stress event tag, pulled from the run hour service.</param>
            /// <param name="assignedEngineer">The engineer assigned to review the stress events.</param>
            /// <param name="description">A description of the stress event.</param>
            /// <param name="multiplier">The multiplier associated with the stress event.</param>
            public StressEventOverallSummary(List<StressEventGranularSummary> granularSummaries, string stressEventTag,
                Tuple<DateTime, double> currentValue, decimal multiplier = Decimal.Zero, string assignedEngineer = "None", string description = "None")
                : this()
            {
                this.GranularSummaries = granularSummaries;
                this.StressEventTag = stressEventTag;
                this.CurrentValue = currentValue;
                this.Multiplier = multiplier;
                this.AssignedEngineer = assignedEngineer;
                this.Description = description;
                //Run a few data summary methods automatically for easy output.
                this.CalculateEtaReduction();
                this.CalculateEndingValue();
                this.ValueChange = this.EndingValue - this.CurrentValue.Item2;
            }
            /// <summary>This method will calculate the total eta reduction over each granular time period.</summary>
            private void CalculateEtaReduction()
            {
                var etaReductionList = new List<decimal>();

                //Calculate the eta reduction at the current time.
                decimal currentStressEventValue = Convert.ToDecimal(this.CurrentValue.Item2);
                decimal currentEtaReduction = Math.Round(currentStressEventValue * this.Multiplier, 3);

                //Iterate over every value in this.GranularSummaries, multiplying the multiplier times the total events
                //(current value + summary events).
                foreach (StressEventGranularSummary granularSummary in this.GranularSummaries)
                {
                    currentStressEventValue += Convert.ToDecimal(granularSummary.NumberEvents);
                    etaReductionList.Add(Math.Round(currentStressEventValue * this.Multiplier, 3));
                }

                //Calculate the final eta reduction
                decimal finalEtaReduction = Math.Round(currentStressEventValue * this.Multiplier, 3);

                this.EtaReduction = new EtaReductionOverallSummary(currentEtaReduction, finalEtaReduction, etaReductionList);
            }
            /// <summary>This method will calculate the ending value of the stress event, after each granular summary is added to the current value.</summary>
            private void CalculateEndingValue()
            {
                var retValue = this.CurrentValue.Item2;

                //Iterate over every value in this.GranularSummaries, adding each granular number of events.
                foreach (StressEventGranularSummary granularSummary in this.GranularSummaries)
                {
                    retValue += granularSummary.NumberEvents;
                }

                this.EndingValue = retValue;
            }
        }
        /// <summary>This data structure stores a summary of eta reduction for a particular stress event or stress hour tag, and includes a list of 
        /// decimal values for the eta reduction at each instance for each granular stress event summary.</summary>
        public struct EtaReductionOverallSummary
        {
            /// <summary>The eta reduction at the starting date.</summary>
            public decimal StartingEtaReduction { get; private set; }
            /// <summary>The eta reduction at the ending date.</summary>
            public decimal EndingEtaReduction { get; private set; }
            /// <summary>The change in eta reduction over the total time period (i.e. the EndingEtaReduction - StartingEtaReduction).</summary>
            public decimal EtaReductionChange { get; private set; }
            /// <summary>The eta reduction over each granular time period.</summary>
            public List<decimal> GranularEtaRedution { get; private set; }
            /// <summary>This data structure stores a summary of eta reduction for a particular stress event or stress hour tag, and includes a list of 
            /// decimal values for the eta reduction at each instance for each granular stress event summary.</summary>
            /// <param name="startingEtaReduction">The eta reduction at the starting date.</param>
            /// <param name="endingEtaReduction">The eta reduction at the ending date.</param>
            /// <param name="granularEtaReduction">The eta reduction over each granular time period.</param>
            public EtaReductionOverallSummary(decimal startingEtaReduction, decimal endingEtaReduction, List<decimal> granularEtaReduction)
                : this()
            {
                this.StartingEtaReduction = Math.Round(startingEtaReduction, 3);
                this.EndingEtaReduction = Math.Round(endingEtaReduction, 3);
                this.EtaReductionChange = endingEtaReduction - startingEtaReduction;
                this.GranularEtaRedution = granularEtaReduction;
            }
        }
        /// <summary>This data structure is used to store a summary of run hours over a granular period of time.</summary>
        public struct RunHourGranularSummary
        {
            /// <summary>The start date of the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The end date of the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The number of hours the equipment was running during the time period.</summary>
            public double HoursRunning { get; private set; }
            /// <summary>This data structure is used to store a summary of run hours over a granular period of time.</summary>
            /// <param name="hoursRunning">The number of hours the equipment was running during the time period.</param>
            /// <param name="startDate">The start date of the summary.</param>
            /// <param name="endDate">The end date of the summary.</param>           
            public RunHourGranularSummary(double hoursRunning, DateTime startDate, DateTime endDate)
                : this()
            {
                this.HoursRunning = Math.Round(hoursRunning, 3);
                this.StartDate = startDate;
                this.EndDate = endDate;
            }
        }
        /// <summary>This data structure is used to store a summary of stress events over a granular period of time.</summary>
        public struct StressEventGranularSummary
        {
            /// <summary>The start date of the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The end date of the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The number of events, as a double, associated with the summary time period.</summary>
            public double NumberEvents { get; private set; }
            /// <summary>This data structure is used to store a summary of stress events over a granular period of time.</summary>
            /// <param name="numberEvents">The number of events, as a double, associated with the summary time period.</param>
            /// <param name="startDate">The start date of the summary.</param>
            /// <param name="endDate">The end date of the summary.</param>
            public StressEventGranularSummary(double numberEvents, DateTime startDate, DateTime endDate)
                : this()
            {
                this.NumberEvents = numberEvents;
                this.StartDate = startDate;
                this.EndDate = endDate;
            }
        }
        /// <summary>This data structure is used to store a summary of stress hours over a granular period of time.</summary>
        public struct StressHourGranularSummary
        {
            /// <summary>The start date of the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The end date of the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The number of events, as a double, associated with the summary time period.</summary>
            public double NumberHours { get; private set; }
            /// <summary>This data structure is used to store a summary of stress hours over a granular period of time.</summary>
            /// <param name="numberHours">The number of hours, as a TimeSpan, associated with the summary time period.</param>
            /// <param name="startDate">The start date of the summary.</param>
            /// <param name="endDate">The end date of the summary.</param>
            public StressHourGranularSummary(double numberHours, DateTime startDate, DateTime endDate)
                : this()
            {
                this.NumberHours = Math.Round(numberHours, 3);
                this.StartDate = startDate;
                this.EndDate = endDate;
            }
        }

        //Old classes    
        /// <summary>This data structure is used to store a summary of run hours over a granular period of time.</summary>
        [Obsolete]
        public struct RunHourGranularCalcSummary
        {
            /// <summary>The start date of the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The end date of the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The number of hours the equipment was running during the time period.</summary>
            public TimeSpan HoursRunning { get; private set; }
            /// <summary>The number of hours the equipment was not running during the time period.</summary>
            public TimeSpan HoursNotRunning { get; private set; }
            /// <summary>The number of hours the equipment run-state was unknown during the time period.</summary>
            public TimeSpan HoursUnknown { get; private set; }
            /// <summary>This data structure is used to store a summary of run hours over a granular period of time.</summary>
            /// <param name="hoursRunning">The number of hours the equipment was running during the time period.</param>
            /// <param name="hoursNotRunning">The number of hours the equipment was not running during the time period.</param>
            /// <param name="hoursUnknown">The number of hours the equipment run-state was unknown during the time period.</param>
            /// <param name="startDate">The start date of the summary.</param>
            /// <param name="endDate">The end date of the summary.</param>           
            public RunHourGranularCalcSummary(TimeSpan hoursRunning, TimeSpan hoursNotRunning, TimeSpan hoursUnknown,
                DateTime startDate, DateTime endDate)
                : this()
            {
                this.HoursRunning = hoursRunning;
                this.HoursNotRunning = hoursNotRunning;
                this.HoursUnknown = hoursUnknown;
                this.StartDate = startDate;
                this.EndDate = endDate;
            }
        }
    }
    namespace OpStateTools
    {
        //OpState Initialization Classes
        /// <summary>This class defines limits for a eDNA tag. It can be used to determine whether the signal is within 
        /// the limit at a particular time, and provide a summary of the amount of time spent within the limit.</summary>
        public class OpLimit
        {
            /// <summary>The fully-qualified (Site.Service.Point) eDNA tag associated with the state limits.</summary>
            public string PointTag { get; private set; }
            /// <summary>The lower limit for the operating state. It is considered to be inclusive (if the value is equal to
            /// the limit, it will also trigger).</summary>
            public double LowerLimit { get; private set; }
            /// <summary>The higher limit for the operating state. It is considered to be inclusive (if the value is equal to
            /// the limit, it will also trigger).</summary>
            public double HigherLimit { get; private set; }
            /// <summary>This class defines limits for a eDNA tag. It can be used to determine whether the signal is within 
            /// the limit at a particular time, and provide a summary of the amount of time spent within the limit.</summary>
            /// <param name="lowerLimit">The lower limit for the operating state. It is considered to be inclusive (if the value is equal to
            /// the limit, it will also trigger).</param>
            /// <param name="higherLimit">The higher limit for the operating state. It is considered to be inclusive (if the value is equal to
            /// the limit, it will also trigger).</param>
            /// <param name="pointTag">The fully-qualified (Site.Service.Point) eDNA tag associated with the state limits.</param>
            public OpLimit(double lowerLimit, double higherLimit, string pointTag)
            {
                this.LowerLimit = lowerLimit;
                this.HigherLimit = higherLimit;
                this.PointTag = pointTag;
            }
            /// <summary>This method is useful for obtaining the last point of a data pull. It needs to be "raw" because the method will need to check
            /// for a future unreliable point, which means that all the time in between is unreliable as well.</summary>
            /// <param name="currentDate">The current date to find the status at.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpStatus PullLimitStatusLastRaw(DateTime currentDate, int hoursOffset = 0)
            {
                //eDNA Initialization
                currentDate = currentDate.AddHours(hoursOffset);
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;
                nRet = History.DnaGetHistRaw(this.PointTag, currentDate, currentDate.AddYears(10), out uiKey);

                while (nRet == 0)
                {
                    //Pull the next point iteratively. Warning- sometimes eDNA pulls a value before the start time?
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                    //Check that the date of the point is correct- sometimes eDNA pulls too early or too far
                    if (dtTime < currentDate) { continue; }

                    //Three possibilities at this point in the code.
                    //1. If the status is "unreliable", the default value of the Tristate, Tristate.Unknown, will be returned
                    //2. If the value is within the limits, Tristate.True will be returned
                    //3. If the value is not within the limits, Tristate.False will be returned
                    if (strStatus == "UNRELIABLE")
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.Unknown);
                    }
                    else if (dValue >= this.LowerLimit && dValue <= this.HigherLimit)
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.True);
                    }
                    else
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.False);
                    }
                }

                //If the raw data point does not find a "last" point, assume that the current status is the correct one
                return new OpStatus(currentDate.AddHours(-hoursOffset), this.PullLimitStatusSingleSnap(currentDate).Status);
            }
            /// <summary>This method returns whether the signal is within the limit at a single point in time. It is
            /// primarily useful for edge cases (the initial and last points).</summary>
            /// <param name="currentDate">The point in time at which to find whether the signal is within the limit.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpStatus PullLimitStatusSingleSnap(DateTime currentDate, int hoursOffset = 0)
            {
                //eDNA Initialization
                currentDate = currentDate.AddHours(hoursOffset);
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;
                nRet = History.DnaGetHistSnap(this.PointTag, currentDate, currentDate, new TimeSpan(0, 0, 0, 1), out uiKey);

                //Ideally, eDNA will just return one point here, but that's not always the case (sometimes it pulls too early
                //or too late). The while loop is just to check to make sure that the time of the data point pulled is at
                //least greater than the user-supplied CurrentDate.
                while (nRet == 0)
                {
                    //Pull the next point iteratively. Warning- sometimes eDNA pulls a value before the start time?
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                    //Check that the date of the point is correct- sometimes eDNA pulls too early or too far
                    if (dtTime < currentDate) { continue; }

                    //Three possibilities at this point in the code.
                    //1. If the status is "unreliable", the default value of the Tristate, Tristate.Unknown, will be returned
                    //2. If the value is within the limits, Tristate.True will be returned
                    //3. If the value is not within the limits, Tristate.False will be returned
                    if (strStatus == "UNRELIABLE")
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.Unknown);
                    }
                    else if (dValue >= this.LowerLimit && dValue <= this.HigherLimit)
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.True);
                    }
                    else
                    {
                        return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.False);
                    }
                }

                //Return an unknown state by default, just in case the raw data pull fails (e.g. nRet is immediately != 0)
                return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.Unknown);
            }
            /// <summary>Find a list of limit status changes over time.</summary>
            /// <param name="startDate">The date at which to start the limit change status summary.</param>
            /// <param name="endDate">The date at which to end the limit change status summary.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public List<OpStatus> PullLimitStatusRawChanges(DateTime startDate, DateTime endDate, int hoursOffset = 0)
            {
                //eDNA Initialization
                var opList = new List<OpStatus>();
                startDate = startDate.AddHours(hoursOffset);
                endDate = endDate.AddHours(hoursOffset);
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime = DateTime.MinValue;
                string strStatus;
                nRet = History.DnaGetHistRaw(this.PointTag, startDate, endDate, out uiKey);

                //Pull the first value using Snap
                OpStatus startOpStatus = this.PullLimitStatusSingleSnap(startDate);
                opList.Add(new OpStatus(startDate.AddHours(-hoursOffset), startOpStatus.Status));

                //Iterate over all eDNA points in the raw data pull
                //If nRet == 0, there are still more points to return
                while (nRet == 0)
                {
                    //Pull the next point iteratively. Warning- sometimes eDNA pulls a value before the start time?
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                    //Check that the date of the point is correct- sometimes eDNA pulls too early or too far
                    if (dtTime < startDate || dtTime > endDate) { continue; }

                    //Three possibilities at this point in the code.
                    //1. If the status is "unreliable", the default value of the Tristate, Tristate.Unknown, will be used 
                    //2. If the value is within the limits, Tristate.True will be used
                    //3. If the value is not within the limits, Tristate.False will be used
                    if (strStatus == "UNRELIABLE")
                    {
                        opList.Add(new OpStatus(dtTime.AddHours(-hoursOffset), Tristate.Unknown));
                    }
                    else if (dValue >= this.LowerLimit && dValue <= this.HigherLimit)
                    {
                        opList.Add(new OpStatus(dtTime.AddHours(-hoursOffset), Tristate.True));
                    }
                    else
                    {
                        opList.Add(new OpStatus(dtTime.AddHours(-hoursOffset), Tristate.False));
                    }
                }

                //Now pull the last value using Snap, if necessary
                //dtTime still contains the last point pulled above, using Raw mode. If it's less than the EndDate, one
                //more point needs to be pulled to ensure that the entire data space is covered

                //**WARNING** CHECK THIS- DOES THE LOCAL ASSIGNMENT OF "out dtTime" OVERRIDE THE INITIAL ASSIGNMENT?
                //If not, it's not the end of the world- may be an extra point at the end of the opList
                if (dtTime < endDate)
                {
                    OpStatus finalOpStatus = this.PullLimitStatusLastRaw(endDate);
                    opList.Add(new OpStatus(endDate.AddHours(-hoursOffset), finalOpStatus.Status));
                }

                //Return the overall list
                return opList;
            }
            /// <summary>Find the amount of time spent within the signal limits over a period of time.</summary>
            /// <param name="startDate">The date at which to start the calculation of hours spent within the signal limits.</param>
            /// <param name="endDate">The date at which to end the calculation of hours spent within the signal limits.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpSummary PullLimitStatusSummary(DateTime startDate, DateTime endDate, int hoursOffset = 0)
            {
                //Initialization
                startDate = startDate.AddHours(hoursOffset);
                endDate = endDate.AddHours(hoursOffset);
                TimeSpan inLimit = TimeSpan.Zero;
                TimeSpan notInLimit = TimeSpan.Zero;
                TimeSpan unknown = TimeSpan.Zero;

                //First, pull the list of raw limit changes over time, so that they can be used in the iteration.
                List<OpStatus> olsList = this.PullLimitStatusRawChanges(startDate, endDate);

                //Iterate over all the raw changes and find the amount of time that was spent within the limit.
                //The end step is (olsList.Count - 2) because we need to access (the current index + 1) in the 
                //loop below to find the TimeSpan in between, so we don't want to include the final index in 
                //the iteration.
                int index = 0;
                do
                {
                    //Calculate the time spent in/out of the state by looking ahead at the DateTime of the next index.
                    TimeSpan timeToAdd = (olsList[index + 1].Time - olsList[index].Time);

                    //Now determine which type of status to add time to
                    if (olsList[index + 1].Status == Tristate.Unknown)
                    {
                        unknown = unknown.Add(timeToAdd);
                    }
                    else if (olsList[index].Status == Tristate.True)
                    {
                        inLimit = inLimit.Add(timeToAdd);
                    }
                    else if (olsList[index].Status == Tristate.False)
                    {
                        notInLimit = notInLimit.Add(timeToAdd);
                    }
                    else
                    {
                        unknown = unknown.Add(timeToAdd);
                    }

                    index++;
                }
                while (index < olsList.Count - 1);

                //Return the OpSummary object with the information obtained above
                return new OpSummary(inLimit, notInLimit, unknown, startDate.AddHours(-hoursOffset), endDate.AddHours(-hoursOffset));
            }
        }
        /// <summary>This class will calculate whether an equipment is in an operating state or not, based on user-supplied state definition 
        /// limits. Multiple limits with multiple eDNA tags may be specified- it is assumed that all defined limits must be met for the operational state
        /// to trigger (a logical AND).</summary>
        public class OpState
        {
            /// <summary>A description of the operating state. Example- "Running".</summary>
            public string Description { get; private set; }
            /// <summary>An instance of the OpLimit object that defines an eDNA tag which must be greater than
            /// or equal to a lower limit, and less than or equal to a lower limit.</summary>
            public OpLimit[] LimitDefinitions { get; private set; }
            /// <summary>This class will calculate whether an equipment is in an operating state or not, based
            /// on user-supplied state definition limits. Multiple limits with multiple eDNA tags may be
            /// specified- it is assumed that all defined limits must be met for the operational state
            /// to trigger (a logical AND).</summary>
            /// <param name="limitDefinitions">An instance of the OpLimit object that defines an eDNA tag which must be greater than
            /// or equal to a lower limit, and less than or equal to a lower limit.</param>
            /// <param name="description">A description of the operating state. Example- "Running".</param>
            public OpState(OpLimit[] limitDefinitions, string description = "Undefined")
            {
                this.Description = description;
                this.LimitDefinitions = limitDefinitions;
            }
            /// <summary>This method will calculate if the equipment is considered to be in the operating state at the
            /// time specified by the user parameter StartDate, and this.LimitDefinitions. WARNING- this 
            /// method uses the eDNA "Snap" function- see eDNA documentation for more information.</summary>
            /// <param name="currentDate">The date at which to calculate operating state.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpStatus PullStateStatusSingleSnap(DateTime currentDate, int hoursOffset = 0)
            {
                int numLimits = LimitDefinitions.Length;
                currentDate = currentDate.AddHours(hoursOffset);

                //For safety, this needs to be an array, not a list (we already know the exact size needed, so why not?).
                //The Tristate enum (as a byte) saves 1 byte of memory per allocation, and it's all we need.
                Tristate[] withinLimits = new Tristate[numLimits];

                //This needs to increment by index instead of a foreach. Reasoning is a little complicated. Basically, I have
                //encountered the possibility that eDNA returns a null value after a history call and then declares that the 
                //history pull is finished. (Not sure of why this occurs, communication issues maybe?) So just in case eDNA skips
                //the history call for a particular limit, the stored value will be the default value of Tristate, which is 
                //Tristate.Unknown (exactly the behavior that we want, because the actual state is unknown in this case).
                for (int limitIndex = 0; limitIndex < numLimits; limitIndex++)
                {
                    //Initialize the eDNA history call
                    OpLimit opLimit = this.LimitDefinitions[limitIndex];
                    withinLimits[limitIndex] = opLimit.PullLimitStatusSingleSnap(currentDate).Status;
                }

                // Return UNKNOWN if any of the values in WithinLimits were not set in the above logic (default value of Tristate is Tristate.Unknown), 
                //                or if any value was "UNRELIABLE".
                // Return FALSE if any of the values in WithinLimits are false.
                // Return TRUE only if (iff) all of the values in WithinLimits are true.
                if (withinLimits.Contains(Tristate.Unknown))
                {
                    return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.Unknown);
                }
                else if (withinLimits.Contains(Tristate.False))
                {
                    return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.False);
                }
                else
                {
                    return new OpStatus(currentDate.AddHours(-hoursOffset), Tristate.True);
                }
            }
            /// <summary>Find a list of state status changes over time.</summary>
            /// <param name="startDate">The date at which to start the state change status summary.</param>
            /// <param name="endDate">The date at which to end the state change status summary.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public List<OpStatus> PullStateStatusRawChanges(DateTime startDate, DateTime endDate, int hoursOffset = 0)
            {
                //This method is a little complicated. The overall idea is that at each "inflection point"
                //(a DateTime at which ANY of the OpLimits change their status) the overall equipment state
                //will be calculated using a logical AND. This is accomplished by:
                //
                //1. Building a "master" sorted, unique-valued list of all DateTimes across all OpLimits
                //2. Iterating over each DateTime in the "master" list
                //      2.1 Finding the last status of the limit BEFORE the current iterated DateTime, since it is
                //          ONLY possible for a status to change at an inflection point
                //      2.2 Using the same logic as in "PullStateStatusSingleSnap" to determine the overall state
                //          of the equipment (a logical AND across all limits)
                //3. Returning a list of the overall equipment state for each inflection DateTime

                //Initialization
                startDate = startDate.AddHours(hoursOffset);
                endDate = endDate.AddHours(hoursOffset);
                var opList = new List<List<OpStatus>>();
                var allRawDates = new List<DateTime>();
                var overallOpStatus = new List<OpStatus>();

                //Pull the raw status changes for each defined limit.
                foreach (OpLimit opLimit in this.LimitDefinitions)
                {
                    List<OpStatus> tempOpList = opLimit.PullLimitStatusRawChanges(startDate, endDate);
                    //Add to the overall opList.
                    opList.Add(tempOpList);
                    //Use a LINQ union call to keep track of all unique DateTimes.
                    allRawDates.Union(tempOpList.Select(f => f.Time));
                }

                //Sort the allRawDates list, since it's going to be used in a successive iteration
                //where the order matters.
                allRawDates.Sort();

                //Iterate over all the unique DateTimes, which will contain all inflection points.
                foreach (DateTime dt in allRawDates)
                {
                    //Initialization
                    int totalLimits = this.LimitDefinitions.Length;
                    Tristate[] limitStatus = new Tristate[totalLimits];

                    //Iterate over all the Limit Definitions to see if the equipment is within the limit.
                    for (int index = 0; index < totalLimits; index++)
                    {
                        //This LINQ statement will find the first element in the opList that is greater than
                        //the DateTime that is being iterated over- we know that the index that we want is
                        //(firstindex - 1) because it must be the status that the limit is in during 
                        //DateTime dt.
                        int firstindex = opList[index].FindIndex(f => f.Time > dt);
                        limitStatus[index] = opList[index][firstindex - 1].Status;
                    }

                    //Now, depending on the states that were obtained above, add the correct value to the 
                    //overall list using the following logic:
                    // Store UNKNOWN if any of the values in limitStatus were not set in the above logic (default value of Tristate is Tristate.Unknown), 
                    //                or if any value was "UNRELIABLE".
                    // Store FALSE if any of the values in limitStatus are false.
                    // Store TRUE only if (iff) all of the values in limitStatus are true.
                    if (limitStatus.Contains(Tristate.Unknown))
                    {
                        overallOpStatus.Add(new OpStatus(dt.AddHours(-hoursOffset), Tristate.Unknown));
                    }
                    else if (limitStatus.Contains(Tristate.False))
                    {
                        overallOpStatus.Add(new OpStatus(dt.AddHours(-hoursOffset), Tristate.False));
                    }
                    else
                    {
                        overallOpStatus.Add(new OpStatus(dt.AddHours(-hoursOffset), Tristate.True));
                    }
                }

                //Return the main list
                return overallOpStatus;
            }
            /// <summary>Find the amount of time spent within the operating state over a period of time.</summary>
            /// <param name="startDate">The date at which to start the calculation of hours spent within the operating state.</param>
            /// <param name="endDate">The date at which to end the calculation of hours spent within the operating state.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpSummary PullStateStatusSummary(DateTime startDate, DateTime endDate, int hoursOffset = 0)
            {
                //Initialization
                startDate = startDate.AddHours(hoursOffset);
                endDate = endDate.AddHours(hoursOffset);
                TimeSpan inLimit = TimeSpan.Zero;
                TimeSpan notInLimit = TimeSpan.Zero;
                TimeSpan unknown = TimeSpan.Zero;

                //First, pull the list of raw limit changes over time, so that they can be used in the iteration.
                List<OpStatus> ossList = this.PullStateStatusRawChanges(startDate, endDate);

                //Iterate over all the raw changes and find the amount of time that was spent within the operating state.
                //The end step is (ossList.Count - 2) because we need to access (the current index + 1) in the 
                //loop below to find the TimeSpan in between, so we don't want to include the final index in 
                //the iteration.
                for (int index = 0; index < ossList.Count - 2; index++)
                {
                    //Calculate the time spent in/out of the state by looking ahead at the DateTime of the next index.
                    TimeSpan timeToAdd = (ossList[index + 1].Time - ossList[index].Time);

                    //Now determine which type of status to add time to
                    if (ossList[index].Status == Tristate.True)
                    {
                        inLimit.Add(timeToAdd);
                    }
                    else if (ossList[index].Status == Tristate.False)
                    {
                        notInLimit.Add(timeToAdd);
                    }
                    else
                    {
                        unknown.Add(timeToAdd);
                    }
                }

                //Return the OpSummary object with the information obtained above
                return new OpSummary(inLimit, notInLimit, unknown, startDate.AddHours(-hoursOffset), endDate.AddHours(-hoursOffset));
            }
        }
        /// <summary>A class used to determine which operational state an equipment is in at a particular time.
        /// Multiple operating states are defined in this.StateDefinitions. It is assumed that the equipment
        /// can only be in one state at once.</summary>
        public class OpEvaluation
        {
            /// <summary>A description of the equipment associated with the list of state definitions.</summary>
            public string Equipment { get; private set; }
            /// <summary>A list of instances of the OpState class.</summary>
            public OpState[] StateDefinitions { get; private set; }
            /// <summary>A class used to determine which operational state an equipment is in at a particular time.
            /// Multiple operating states are defined in this.StateDefinitions. It is assumed that the equipment
            /// can only be in one state at once.</summary>
            /// <param name="stateDefinitions">A list of instances of the OpState class.</param>
            /// <param name="equipment">A description of the equipment associated with the list of state definitions.</param>
            public OpEvaluation(OpState[] stateDefinitions, string equipment = "Undefined")
            {
                this.StateDefinitions = stateDefinitions;
                this.Equipment = equipment;
            }
            /// <summary>This method will calculate which operating state the equipment is at a given time, based
            /// on this.StateDefinitions. It will return the index of the StateDefinition that triggers first.
            /// Warning- if the user-supplied state definitions are not exclusive (i.e. there are possible signal
            /// values which MIGHT trigger multiple states) this function will return the FIRST state that it finds
            /// that matches the criteria.</summary>
            /// <param name="currentDate">The date at which to calculate which operating state the equipment is in.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public OpSelectedState PullWhichStateSingleSnap(DateTime currentDate, int hoursOffset = 0)
            {
                currentDate = currentDate.AddHours(hoursOffset);
                int numstates = this.StateDefinitions.Length;

                //Iterate over each value in this.StateDefinitions
                for (int ii = 0; ii < numstates; ii++)
                {
                    OpState tempOp = this.StateDefinitions[ii];

                    //If the current status of the state is TRUE, then return that operating state
                    //as the one that triggers
                    if (tempOp.PullStateStatusSingleSnap(currentDate).Status == Tristate.True)
                    {
                        return new OpSelectedState(currentDate.AddHours(-hoursOffset), ii, tempOp);
                    }
                }

                //If no state is found, return -1
                return new OpSelectedState(currentDate.AddHours(-hoursOffset), -1, null);
            }
            /// <summary>NOT IMPLEMENTED. This method will return a list of raw changes in equipment operating state. Warning- 
            /// if the user-supplied state definitions are not exclusive (i.e. there are possible signal
            /// values which MIGHT trigger multiple states) this function will return (for each date) 
            /// the FIRST state that it finds that matches the criteria.</summary>
            /// <param name="startDate">The date at which to start the state change summary.</param>
            /// <param name="endDate">The date at which to end the state change summary.</param>
            public List<OpSelectedState> PullWhichStateRawChanges(DateTime startDate, DateTime endDate)
            {
                return null;
            }
            /// <summary>NOT IMPLEMENTED</summary>
            /// <param name="startDate">n/a</param>
            /// <param name="endDate">n/a</param>
            public List<OpSelectedStateSummary> PullWhichStateStatusSummary(DateTime startDate, DateTime endDate)
            {
                return null;
            }
        }

        //Data Structures
        /// <summary>A tristate equivalent to a bool?, except it uses less space.</summary>
        public enum Tristate : byte
        {
            ///<summary>The value is unknown.</summary>
            Unknown = 0,
            ///<summary>The value is true.</summary>
            True = 1,
            ///<summary>The value is false.</summary>
            False = 2
        }
        /// <summary>This data structure is useful for the OpLimit and OpState classes. It defines a status 
        /// (using a Tristate) at a particular time. For instance, this struct may be used to store 
        /// whether a signal is within a limit at a particular time (for an OpLimit) or whether an 
        /// equipment is running at a particular time (for an OpState).</summary>
        public struct OpStatus
        {
            /// <summary>The time at which the status is defined.</summary>
            public DateTime Time { get; private set; }
            /// <summary>The status value (as a Tristate enum).</summary>
            public Tristate Status { get; private set; }
            /// <summary>This data structure is useful for the OpLimit and OpState classes. It defines a status 
            /// (using a Tristate) at a particular time. For instance, this struct may be used to store 
            /// whether a signal is within a limit at a particular time (for an OpLimit) or whether an 
            /// equipment is running at a particular time (for an OpState).</summary>
            /// <param name="time">The time at which the status is defined.</param>
            /// <param name="status">The status value (as a Tristate enum).</param>
            public OpStatus(DateTime time, Tristate status)
                : this()
            {
                this.Time = time;
                this.Status = status;
            }
        }
        /// <summary>This data structure is useful for the OpLimit and OpState classes. It describes the amount of
        /// time spent in each possible value of a Tristate. For example, this struct may be used to store 
        /// the amount of time spent within a signal limit (for an OpLimit) or the amount of time spent
        /// outside of steady-state (for an OpState).</summary>
        public struct OpSummary
        {
            /// <summary>The starting date for the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The ending date for the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The amount of time in between the StartDate and EndDate. Note-ideally this should be equal to
            /// the sum of this.Inside, this.Outside, and this.Unknown. (Maybe this should be checked for and
            /// throw an exception otherwise?)</summary>
            public TimeSpan TimeDuration { get; private set; }
            /// <summary>A TimeSpan of the amount of time spent inside the OpLimit limit definitions or OpState state definitions.</summary>
            public TimeSpan Inside { get; private set; }
            /// <summary>A TimeSpan of the amount of time spent outside the OpLimit limit definitions or OpState state definitions.</summary>
            public TimeSpan Outside { get; private set; }
            /// <summary>A TimeSpan of the amount of time where the status was unknown.</summary>
            public TimeSpan Unknown { get; private set; }
            /// <summary>This data structure is useful for the OpLimit and OpState classes. It describes the amount of
            /// time spent in each possible value of a Tristate. For example, this struct may be used to store 
            /// the amount of time spent within a signal limit (for an OpLimit) or the amount of time spent
            /// outside of steady-state (for an OpState).</summary>
            /// <param name="startDate">The starting date for the summary.</param>
            /// <param name="endDate">The ending date for the summary.</param>
            /// <param name="inside">A TimeSpan of the amount of time spent inside the OpLimit limit definitions or OpState state definitions.</param>
            /// <param name="outside">A TimeSpan of the amount of time spent outside the OpLimit limit definitions or OpState state definitions.</param>
            /// <param name="unknown">A TimeSpan of the amount of time where the status was unknown.</param>
            public OpSummary(TimeSpan inside, TimeSpan outside, TimeSpan unknown, DateTime startDate, DateTime endDate)
                : this()
            {
                this.Inside = inside;
                this.Outside = outside;
                this.Unknown = unknown;
                this.StartDate = startDate;
                this.EndDate = endDate;
                this.TimeDuration = (endDate - startDate);
            }
        }
        /// <summary>This data structure is useful for determining which OpState an equipment is in
        /// at a particular time.</summary>
        public struct OpSelectedState
        {
            /// <summary>The time at which the equipment is in a particular OpState.</summary>
            public DateTime Time { get; private set; }
            /// <summary>The index of the selected state.</summary>
            public int SelectedStateIndex { get; private set; }
            /// <summary>The OpState itself (mainly for using its member methods if necessary).</summary>
            public OpState SelectedState { get; private set; }
            /// <summary>This data structure is useful for determining which OpState an equipment is in
            /// at a particular time.</summary>
            /// <param name="time">The time at which the equipment is in a particular OpState.</param>
            /// <param name="selectedStateIndex">The index of the selected state.</param>
            /// <param name="selectedState">The OpState itself (mainly for using its member methods if necessary).</param>
            public OpSelectedState(DateTime time, int selectedStateIndex, OpState selectedState)
                : this()
            {
                this.Time = time;
                this.SelectedStateIndex = selectedStateIndex;
                this.SelectedState = selectedState;
            }
        }
        /// <summary>This data structure is used to hold the time spent in each operating state
        /// over a period of time.</summary>
        public struct OpSelectedStateSummary
        {
            /// <summary>The start date of the summary.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The end date of the summary.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>The amount of time in between the StartDate and EndDate. Note-ideally this should be equal to
            /// the sum of this.Inside, this.Outside, and this.Unknown. (Maybe this should be checked for and
            /// throw an exception otherwise?)</summary>
            public TimeSpan TimeDuration { get; private set; }
            /// <summary>An array that contains the amount of time spent in each state, using the same 
            /// indices as this.StateDefinitions.</summary>
            public double[] TimeSpentInEachState { get; private set; }
            /// <summary>The OpStates associated with the summary (mainly so that their methods can be used if 
            /// necessary)</summary>
            public OpState[] StateDefinitions { get; private set; }
            /// <summary>This data structure is used to hold the time spent in each operating state
            /// over a period of time.</summary>
            /// <param name="timeSpentInEachState">An array that contains the amount of time spent in each state, using the same 
            /// indices as this.StateDefinitions.</param>
            /// <param name="stateDefinitions">The OpStates associated with the summary (mainly so that their methods can be used if necessary)</param>
            /// <param name="startDate">The start date of the summary.</param>
            /// <param name="endDate">The end date of the summary.</param>
            public OpSelectedStateSummary(double[] timeSpentInEachState, OpState[] stateDefinitions, DateTime startDate, DateTime endDate)
                : this()
            {
                this.StartDate = startDate;
                this.EndDate = endDate;
                this.TimeDuration = (endDate - startDate);
                this.TimeSpentInEachState = timeSpentInEachState;
                this.StateDefinitions = stateDefinitions;
            }
        }
    }
    namespace PullTools
    {
        //Initialization classes
        /// <summary>This class defines an overall data pull with a list of PullGroups.</summary>
        public class PullAllData
        {
            /// <summary>A list of PullGroups which will have data pulled.</summary>
            public List<PullGroup> Groups { get; private set; }
            /// <summary>The total number of points in the data pull.</summary>
            public int TotalPoints { get; private set; }
            /// <summary>Should unreliable points be filtered out?</summary>
            public bool FilterUnreliable { get; private set; }
            /// <summary>This class defines an overall data pull with a list of PullGroups.</summary>
            /// <param name="filePath">The fully-qualified filename for the location of the CSV file.</param>
            /// <param name="filterUnreliable">Whether unreliable data should be filtered (TRUE) or not filtered (FALSE).</param>
            public PullAllData(string filePath, bool filterUnreliable = false)
            {
                //Initialization
                var groups = new List<PullGroup>();
                var totalPoints = 0;

                //Read from the Point File
                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    //Iterate over each line
                    while (!reader.EndOfStream)
                    {
                        totalPoints++;
                        //Read from the input file
                        string[] splitLine = reader.ReadLine().Split(',');
                        //Read the correct values
                        string tag = splitLine[0];
                        string group = (splitLine.Length < 2) ? "Main" : splitLine[1];
                        string description = (splitLine.Length < 3) ? splitLine[0].Trim() : splitLine[2].Trim();
                        string ssTag = "";
                        string lowFilt = (splitLine.Length < 5) ? "-99999" : splitLine[4];
                        string highFilt = (splitLine.Length < 6) ? "99999" : splitLine[5];
                        //Steady-State filtering logic
                        double steadyStateDefLow = 1;
                        double steadyStateDefHigh = 1;
                        if (splitLine.Length > 3)
                        {
                            string[] ssTagSplit = splitLine[3].Split('|');
                            ssTag = ssTagSplit[0];
                            //This allows the user to define the value that determines equipment steady-state
                            //after a '|' in the steady-state tag cell
                            if (ssTagSplit.Length == 2)
                            {
                                steadyStateDefLow = Convert.ToDouble(ssTagSplit[1]);
                                steadyStateDefHigh = Convert.ToDouble(ssTagSplit[1]);
                            }
                            if (ssTagSplit.Length > 2)
                            {
                                steadyStateDefLow = Convert.ToDouble(ssTagSplit[1]);
                                steadyStateDefHigh = Convert.ToDouble(ssTagSplit[2]);
                            }
                        }
                        //Check if the group has already been defined
                        int index = groups.FindIndex(f => f.Description == group);
                        //If the group hasn't been defined yet, create it and add it to the group list
                        if (index < 0)
                        {
                            groups.Add(new PullGroup(group));
                            index = groups.Count - 1;
                        }
                        //Now add the PointInfo to the correct group
                        groups[index].AddTagsToPull(new PullTag(tag, description, filterUnreliable, ssTag, steadyStateDefLow, steadyStateDefHigh,
                            Convert.ToDouble(lowFilt), Convert.ToDouble(highFilt)));
                    }
                }

                //Now save the information obtained above.
                this.Groups = groups;
                this.TotalPoints = totalPoints;
                this.FilterUnreliable = filterUnreliable;
            }
            /// <summary>Writes a header for a metrics pull, based on the user-supplied parameters.</summary>
            /// <param name="mean">Should a header for the mean be written?</param>
            /// <param name="min">Should a header for the minimum be written?</param>
            /// <param name="max">Should a header for the maximum be written?</param>
            /// <param name="linearRegression">Should a header for linear regression be written?</param>
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
        }
        /// <summary>This class defines a group of eDNA points to pull at the same time. For instance, all the SSDG points may need to be pulled at the 
        /// same time so they can be directly compared, but PD points may not want to be included in the PointGroup. </summary>
        public class PullGroup
        {
            /// <summary>A description of the point grouping (Example- "SSDG").</summary>
            public string Description { get; private set; }
            /// <summary>A list of initialized tags to pull. Typically constructed using the ConstructDataPull class.</summary>
            public List<PullTag> PullTags { get; private set; }
            /// <summary>This class defines a group of eDNA points to pull at the same time. For instance, all the SSDG points may need to be pulled at the 
            /// same time so they can be directly compared, but PD points may not want to be included in the PointGroup. </summary>
            /// <param name="description">A parameter that describes the equipment or point grouping. For example, "SSDG". This parameter
            /// is for display/output purposes only, and the default value is "Not Grouped".</param>
            public PullGroup(string description = "Not Grouped")
            {
                this.Description = description;
                this.PullTags = new List<PullTag>();
            }
            /// <summary>Adds a list of PullTags to the overall list.</summary>
            /// <param name="pullTag">A list of instances of the PullTag class.</param>
            public void AddTagsToPull(List<PullTag> pullTag)
            {
                this.PullTags.AddRange(pullTag);
            }
            /// <summary>Adds a PullTag to the overall list.</summary>
            /// <param name="pullTag">An instance of the PullTag class.</param>
            public void AddTagsToPull(PullTag pullTag)
            {
                this.PullTags.Add(pullTag);
            }
            /// <summary>This method is designed to pull all of the data from all initialized points at the same time, to a 
            /// single time column. If "FillEmpty" is TRUE, then any missing data will be filled with the previous
            /// value. Else, missing data will be "null".</summary>
            /// <param name="startDate">The beginning date of the data pull.</param>
            /// <param name="endDate">The ending date of the data pull.</param>
            /// <param name="pullType">The type of data pull to perform. See the "PullMode" enum for a list of possibilities. The eDNA 
            /// documentation has more information about each of these modes.</param>
            /// <param name="timePeriod">If the mode is not "Raw", the TimePeriod will be used to determine data granularity. Please see
            /// eDNA documentation for more information.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public PullGroupData PullAllTags(DateTime startDate, DateTime endDate, PullMode pullType = PullMode.Raw, TimeSpan? timePeriod = null, int hoursOffset = 0)
            {
                TimeSpan timeSpan = (timePeriod == null) ? new TimeSpan(0, 0, 1) : timePeriod.Value;
                var tempData = new List<PullTagData>();

                //Iterate over each point and pull each tag individually
                foreach (PullTag pt in this.PullTags)
                {
                    tempData.Add(pt.PullData(startDate, endDate, pullType, timeSpan, hoursOffset));
                }

                //Construct the PullGroupData
                PullGroupData retGroupData = new PullGroupData(tempData, this.Description);

                return retGroupData;
            }
        }
        /// <summary>This class is used to initialize a data pull from eDNA and can extract to a specified data structure.</summary>
        public class PullTag
        {
            /// <summary>The fully-qualified eDNA tag (Site.Service.Point) used to perform the data pull.</summary>
            public string Tag { get; private set; }
            /// <summary>A description of the eDNA point. By default, this is equal to the point tag itself.</summary>
            public string Description { get; private set; }
            /// <summary>The fully-qualified eDNA tag (Site.Service.Point) that defines equipment steady-state.</summary>
            public string SteadyStateTag { get; private set; }
            /// <summary>The value that defines steady-state operation for the equipment.</summary>
            public double SteadyStateDefLow { get; private set; }
            /// <summary>The value that defines steady-state operation for the equipment.</summary>
            public double SteadyStateDefHigh { get; private set; }
            /// <summary>This optional parameter can be used to ignore all values less than the low value filter.</summary>
            public double LowFilter { get; private set; }
            /// <summary>This optional parameter can be used to ignore all values greater than the high value filter.</summary>
            public double HighFilter { get; private set; }
            /// <summary>Is value filtering being used?</summary>
            public bool ValueFiltering { get; private set; }
            /// <summary>Is steady-state filtering being used?</summary>
            public bool SteadyFiltering { get; private set; }
            /// <summary>Is unreliable filtering being used?</summary>
            public bool UnreliableFiltering { get; private set; }
            /// <summary>This class is used to initialize a data pull from eDNA and can extract to a specified data structure.</summary>
            /// <param name="pointTag">The fully-qualified eDNA tag (Site.Service.Point) used to perform the data pull.</param>
            /// <param name="pointDescription">A description of the eDNA point. By default, this is equal to the point tag itself.</param>
            /// <param name="filterUnreliable">Defines whether the unreliable values should be filtered out.</param>
            /// <param name="steadyStatePointTag">The fully-qualified eDNA tag (Site.Service.Point) that defines equipment steady-state.</param>
            /// <param name="steadyStateDefLow">The value that defines steady-state operation for the equipment.</param>
            /// <param name="steadyStateDefHigh">The value that defines steady-state operation for the equipment.</param>
            /// <param name="lowValueFilter">This optional parameter can be used to ignore all values less than the low value filter.</param>
            /// <param name="highValueFilter">This optional parameter can be used to ignore all values greater than the high value filter.</param>          
            public PullTag(string pointTag, string pointDescription = "", bool filterUnreliable = false,
                string steadyStatePointTag = "", double steadyStateDefLow = 1, double steadyStateDefHigh = 1, double lowValueFilter = -99999, double highValueFilter = 99999)
            {
                //Basic information
                this.Tag = pointTag.Trim();
                this.UnreliableFiltering = filterUnreliable;
                this.Description = (String.IsNullOrWhiteSpace(pointDescription)) ? this.Tag : pointDescription.Trim();
                //Steady-State filtering
                this.SteadyStateTag = steadyStatePointTag.Trim();
                this.SteadyStateDefLow = steadyStateDefLow;
                this.SteadyStateDefHigh = steadyStateDefHigh;
                this.SteadyFiltering = !String.IsNullOrWhiteSpace(this.SteadyStateTag);
                //Value Filtering
                this.LowFilter = lowValueFilter;
                this.HighFilter = highValueFilter;
                this.ValueFiltering = (lowValueFilter == -99999) ? false : true;
            }
            /// <summary>This method is designed to pull data from the initialized point pull instance. It returns a list 
            /// of values pulled. If you wish to store the values in this class, use "PointDataAdd" instead.</summary>
            /// <param name="startDate">The beginning date of the data pull.</param>
            /// <param name="endDate">The ending date of the data pull.</param>
            /// <param name="pullType">The type of data pull to perform. See the "PullMode" enum for a list of possibilities. The eDNA 
            /// documentation has more information about each of these modes.</param>
            /// <param name="timePeriod">If the mode is not "Raw", the TimePeriod will be used to determine data granularity. Please see
            /// eDNA documentation for more information.</param>
            /// <param name="hoursOffset">The number of hours (in integer form) to offset the pulled dates by. Useful for transforming between time
            /// zones, or daylight savings time.</param>
            public PullTagData PullData(DateTime startDate, DateTime endDate, PullMode pullType = PullMode.Raw, TimeSpan? timePeriod = null, int hoursOffset = 0)
            {
                TimeSpan timeSpan = (timePeriod == null) ? new TimeSpan(0, 0, 1) : timePeriod.Value;
                //Initialization               
                var tempList = new List<PullValue>();
                startDate = startDate.AddHours(hoursOffset);
                endDate = endDate.AddHours(hoursOffset);
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;

                //Select the correct eDNA history mode                            
                switch (pullType)
                {
                    case (PullMode.Totals):
                        nRet = History.DnaGetHistTotal(this.Tag, startDate, endDate, timeSpan, out uiKey);
                        break;
                    case (PullMode.Snap):
                        nRet = History.DnaGetHistSnap(this.Tag, startDate, endDate, timeSpan, out uiKey);
                        break;
                    case (PullMode.Avg):
                        nRet = History.DnaGetHistAvg(this.Tag, startDate, endDate, timeSpan, out uiKey);
                        break;
                    case (PullMode.Min):
                        nRet = History.DnaGetHistMin(this.Tag, startDate, endDate, timeSpan, out uiKey);
                        break;
                    case (PullMode.Max):
                        nRet = History.DnaGetHistMax(this.Tag, startDate, endDate, timeSpan, out uiKey);
                        break;
                    default:
                        nRet = History.DnaGetHistRaw(this.Tag, startDate, endDate, out uiKey);
                        break;
                }

                //Run the data pull
                while (nRet == 0)
                {
                    //Pull the next point
                    nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);
                    //Check that the point is within bounds- sometimes eDNA pulls too early or too far
                    if (dtTime < startDate || dtTime > endDate) { continue; }
                    //Add to the main list
                    tempList.Add(new PullValue(dtTime.AddHours(-hoursOffset), dValue, strStatus != "UNRELIABLE"));
                }
                History.DNACancelHistRequest(uiKey);

                //Turn the retlist into a PullTagData instance
                var retData = new PullTagData(tempList, startDate, endDate, this.Tag, this.Description);

                //Filtering, if selected
                if (this.UnreliableFiltering) retData.FilterUnreliable();
                if (this.ValueFiltering) retData.FilterByValue(this.LowFilter, this.HighFilter);
                if (this.SteadyFiltering)
                {
                    if (pullType == PullMode.Snap)
                    {
                        retData.FilterBySteadyState(this.SteadyStateTag, this.SteadyStateDefLow, this.SteadyStateDefHigh, true, Convert.ToInt16(timeSpan.TotalSeconds));
                    }
                    else
                    {
                        retData.FilterBySteadyState(this.SteadyStateTag, this.SteadyStateDefLow, this.SteadyStateDefHigh, false, 1);
                    }
                }

                //Return the list
                return retData;
            }
        }

        //Data structures
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
        /// <summary>An enum of possible ways to output data.</summary>
        public enum OutputMode
        {
            /// <summary>Pulls all values to a single file per group.</summary>
            SingleTimeColumn,
            /// <summary>Pulls all tags to a separate file.</summary>
            MultipleFiles,
            /// <summary>Pulls all tags to a separate file that can be read by StrategyStudio PointDefinitions.</summary>
            PointDefinitions
        };
        /// <summary>This class defines a single eDNA value at a particular timestamp.</summary>
        public struct PullValue
        {
            /// <summary>The timestamp of the value.</summary>
            public DateTime Time { get; set; }
            /// <summary>The value associated with the timestamp.</summary>
            public double? Value { get; set; }
            /// <summary>This parameter is a boolean value that indicates whether the value is reliable (TRUE), unreliable (FALSE), or unknown (NULL).</summary>
            public bool? Reliable { get; set; }
            /// <summary>This class defines a single eDNA value at a particular timestamp.</summary>
            /// <param name="time">The timestamp of the value.</param>
            /// <param name="value">The value associated with the timestamp.</param>
            /// <param name="reliable">This parameter is a boolean value that indicates whether the value is reliable (TRUE), unreliable (FALSE), or unknown (NULL).</param>
            public PullValue(DateTime time, double? value, bool? reliable)
                : this()
            {
                this.Time = time;
                this.Value = value;
                this.Reliable = reliable;
            }
        }
        /// <summary>This data structure is useful for holding pulled eDNA values for a single tag.</summary>
        public class PullTagData
        {
            /// <summary>The eDNA tag associated with the data.</summary>
            public string Tag { get; private set; }
            /// <summary>A description of the eDNA point associated with the data.</summary>
            public string Description { get; private set; }
            /// <summary>The StartDate of the initial data pull.</summary>
            public DateTime StartDate { get; private set; }
            /// <summary>The EndDate of the initial data pull.</summary>
            public DateTime EndDate { get; private set; }
            /// <summary>A list of PullValues associated with the tag.</summary>
            public List<PullValue> Values { get; private set; }
            /// <summary>This data structure is useful for holding pulled eDNA values for a single tag.</summary>
            /// <param name="values">A list of PullValues associated with the tag.</param>
            /// <param name="startDate">The StartDate of the initial data pull.</param>
            /// <param name="endDate">The EndDate of the initial data pull.</param>
            /// <param name="tag">The eDNA tag associated with the data.</param>
            /// <param name="description">The description of what the data contains.</param>
            public PullTagData(List<PullValue> values, DateTime startDate, DateTime endDate, string tag, string description)
            {
                this.Values = values;
                this.StartDate = startDate;
                this.EndDate = endDate;
                this.Tag = tag;
                this.Description = description;
            }
            /// <summary>This method adds a list of point values to the ones already stored.</summary>
            /// <param name="pullValues">A list of point values to be added to the overall list of point values stored in this class.</param>
            public void AddToValues(List<PullValue> pullValues)
            {
                this.Values.AddRange(pullValues);
            }
            /// <summary>A method to calculate the number of points pulled.</summary>
            public int CalculateNumPointsWithValues()
            {
                return this.Values.FindAll(f => f.Value.HasValue).Count;
            }
            /// <summary>A method to calculate the mean of the points.</summary>
            /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
            public double? CalculateMean(int analysisWindow = 0)
            {
                //Check if the values are empty and return null
                if (this.Values.Count == 0) return null;

                //Check if an Analysis Window was selected and skip the appropriate number of points
                List<PullValue> calcValues = (analysisWindow > 0) ?
                    this.Values.Skip(Math.Max(0, this.Values.Count() - analysisWindow)).ToList() :
                    this.Values;

                return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Average();
            }
            /// <summary>A method to calculate the minimum of the points.</summary>
            /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
            public double? CalculateMin(int analysisWindow = 0)
            {
                //Check if the values are empty and return null
                if (this.Values.Count == 0) return null;

                //Check if an Analysis Window was selected and skip the appropriate number of points
                List<PullValue> calcValues = (analysisWindow > 0) ?
                    this.Values.Skip(Math.Max(0, this.Values.Count() - analysisWindow)).ToList() :
                    this.Values;

                return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Min();
            }
            /// <summary>A method to calculate the maximum of the points.</summary>
            /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
            public double? CalculateMax(int analysisWindow = 0)
            {
                //Check if the values are empty and return null
                if (this.Values.Count == 0) return null;

                //Check if an Analysis Window was selected and skip the appropriate number of points
                List<PullValue> calcValues = (analysisWindow > 0) ?
                    this.Values.Skip(Math.Max(0, this.Values.Count() - analysisWindow)).ToList() :
                    this.Values;

                return calcValues.FindAll(f => f.Value.HasValue).Select(f => f.Value.Value).ToArray().Max();
            }
            /// <summary>A method to linearly regress the points and return the intercept and slope. The first value returned is the intercept, and the second is the slope.</summary>
            /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
            public Tuple<double, double> CalculateLinearRegression(int analysisWindow = 0)
            {
                //Check if the values are empty and return null
                if (this.Values.Count == 0) return null;

                //Check if an Analysis Window was selected and skip the appropriate number of points
                List<PullValue> calcValues = (analysisWindow > 0) ?
                    this.Values.Skip(Math.Max(0, this.Values.Count() - analysisWindow)).ToList() :
                    this.Values;

                //Create a Tuple (the input data type for the static regression method)
                var regvalues = new List<Tuple<double, double>>();
                for (int ii = 0; ii < calcValues.Count; ii++)
                {
                    regvalues.Add(new Tuple<double, double>((double)ii, calcValues[ii].Value.Value));
                }

                //Uses the MathNet library to regress the points to a line
                return SimpleRegression.Fit(regvalues);
            }
            /// <summary>This method clears all the stored point values.</summary>
            public void ClearValues()
            {
                this.Values.Clear();
            }
            /// <summary>This method will filter out all the unreliable values.</summary>
            public void FilterUnreliable()
            {
                //Uses LINQ to make this easy in a single line. However, could this be optimized to run 
                //more quickly? Probably doesn't matter enough to worry about
                if (this.Values.Count > 0) this.Values = this.Values.FindAll(f => f.Reliable == true);
            }
            /// <summary>This method will filter out all the values less than the Low Filter and all the values greater than the High Filter.</summary>
            /// <param name="lowFilter">Values less than this value will be filtered out.</param>
            /// <param name="highFilter">Values greater than this value will be filtered out.</param>
            public void FilterByValue(double lowFilter, double highFilter)
            {
                var newValues = new List<PullValue>();
                for (int ii = 0; ii < this.Values.Count; ii++)
                {
                    PullValue currentValue = this.Values[ii];
                    if (currentValue.Value < lowFilter || currentValue.Value > highFilter)
                    {
                        newValues.Add(new PullValue(currentValue.Time, null, null));
                    }
                    else
                    {
                        newValues.Add(currentValue);
                    }
                }
                this.Values = newValues;
            }
            /// <summary>This method is used to filter this.Values by whether the equipment is in steady-state or not.</summary>
            /// <param name="steadyStateTag">This is the fully-qualified eDNA tag (Site.Service.Tag) associated with the steady-state point.</param>
            /// <param name="steadyStateDefLow">The value that defines the low limit of whether the equipment is in steady-state or not. Typically this value will equal "1".</param>
            /// <param name="steadyStateDefHigh">The value that defines the high limit of whether the equipment is in steady-state or not. Typically this value will equal "1".</param>
            public void FilterBySteadyState(string steadyStateTag, double steadyStateDefLow = 1, double steadyStateDefHigh = 1,
                bool snapData = false, int snapSeconds = 1)
            {
                if (this.Values.Count > 0)
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
                    var newValues = new List<PullValue>();
                    for (int ii = 0; ii < this.Values.Count; ii++)
                    {
                        PullValue currentValue = this.Values[ii];
                        int indexSS = ssValues.FindIndex(f => f.Item1 > currentValue.Time) - 1;
                        if (indexSS >= 0 && ssValues[indexSS].Item2 == false)
                        {
                            newValues.Add(new PullValue(currentValue.Time, null, null));
                        }
                        else
                        {
                            newValues.Add(currentValue);
                        }
                    }
                    this.Values = newValues;
                }
            }
            /// <summary>Returns all the date times from the data pulled into this.Values.</summary>
            public List<DateTime> GetAllDateTimes()
            {
                if (this.Values.Count == 0) return null;
                else return this.Values.Select(f => f.Time).ToList();
            }
            /// <summary>This method will insert a value at the beginning of the list using "Snap". It is most useful for creating an initial point of reference for 
            /// filling data.</summary>
            public void InsertFirstValueSnap()
            {
                this.Values.Insert(0, this.PullFirstValueSnap(this.StartDate));
            }
            /// <summary>This method will pull the first data point using the "Snap" method. It is most useful to get an initial value that is guaranteed 
            /// to exist, since Snap will search backwards in the data until it finds a data point that is actually written. For more information about the "Snap"
            /// method, please refer to eDNA documentation.</summary>
            /// <param name="startDate">The starting date at which to pull data.</param>
            public PullValue PullFirstValueSnap(DateTime startDate)
            {
                //Initialization
                int nRet;
                uint uiKey = 0;
                double dValue;
                DateTime dtTime;
                string strStatus;

                //Pull the data
                History.DnaGetHistSnap(this.Tag, startDate, startDate, new TimeSpan(0, 0, 0, 1), out uiKey);
                nRet = History.DnaGetNextHist(uiKey, out dValue, out dtTime, out strStatus);

                //Return the results
                History.DNACancelHistRequest(uiKey);
                return new PullValue(dtTime, dValue, strStatus != "UNRELIABLE");
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
            public void WriteDataToPointDefs(string directory)
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

                    foreach (PullValue currentValue in this.Values)
                    {
                        if (currentValue.Value == null) continue;
                        file.WriteLine(String.Join(",", currentValue.Time.ToString(), currentValue.Value.ToString()));
                    }
                }
            }
            /// <summary>This method will write the data in this.Values to a StringBuilder which may be used as output to a CSV file.</summary>
            /// <param name="writeStatus">Defines whether a status column should be written (TRUE) or not (FALSE).</param>
            public StringBuilder WriteDataToString(bool writeStatus = true)
            {
                //Initialize
                var retSB = new StringBuilder();

                //I like the "if" outside the for loop so it's only checked once
                //Slight time improvement?
                if (writeStatus)
                {
                    foreach (PullValue dv in this.Values)
                    {
                        retSB.Append(dv.Time.ToString() + "," + dv.Value.ToString() + "," + dv.Reliable.ToString() + "\n");
                    }
                }
                else
                {
                    foreach (PullValue dv in this.Values)
                    {
                        retSB.Append(dv.Time.ToString() + "," + dv.Value.ToString() + "\n");
                    }
                }

                //Return the results
                return retSB;
            }
            /// <summary>Easily write selected metrics to a string.</summary>
            /// <param name="mean">Should the mean be calculated?</param>
            /// <param name="min">Should the minimum be calculated?</param>
            /// <param name="max">Should the maximum be calculated?</param>
            /// <param name="linearRegression">Should linear regression be calculated?</param>
            /// <param name="analysisWindow">The number of points to consider when calculating metrics.</param>
            public StringBuilder WriteMetricsToString(int analysisWindow = 0, bool mean = true, bool min = true, bool max = true, bool linearRegression = true)
            {
                //Initialization
                var retsb = new StringBuilder();
                retsb.Append(this.Tag + "," + this.Description + "," + this.CalculateNumPointsWithValues());
                //If selected, add each metric to the stringbuilder
                if (mean) { retsb.Append("," + this.CalculateMean(analysisWindow)); }
                if (min) { retsb.Append("," + this.CalculateMin(analysisWindow)); }
                if (max) { retsb.Append("," + this.CalculateMax(analysisWindow)); }
                if (linearRegression)
                {
                    Tuple<double, double> retparams = this.CalculateLinearRegression(analysisWindow);
                    if (retparams != null) retsb.Append("," + retparams.Item1 + "," + retparams.Item2);
                    else retsb.Append(",,");
                }
                return retsb;
            }
        }
        /// <summary>This data structure is useful for storing data from multiple eDNA tags simultaneously.</summary>
        public class PullGroupData
        {
            /// <summary>A description of the point grouping (Example- "SSDG").</summary>
            public string Description { get; private set; }
            /// <summary>A list of eDNA tag data.</summary>
            public List<PullTagData> TagData { get; private set; }
            /// <summary>This data structure is useful for storing data from multiple eDNA tags simultaneously.</summary>
            /// <param name="tagData">The PullTagData from each instance of a PullTag class.</param>
            /// <param name="description">A description of the data grouping (Example- "SSDG").</param>
            public PullGroupData(List<PullTagData> tagData, string description = "None")
            {
                this.TagData = tagData;
                this.Description = description;
            }
            /// <summary>Adds tag data to the overall list.</summary>
            /// <param name="tagData">A single instance of the PullTagData class.</param>
            public void AddTagData(PullTagData tagData)
            {
                this.TagData.Add(tagData);
            }
            /// <summary>This method will clear the stored list of tag values.</summary>
            public void ClearTagData()
            {
                this.TagData.Clear();
            }
            /// <summary>This method constructs a list of all DateTimes across all PullTags defined in this.PullTags.</summary>
            private List<DateTime> FindAllDateTimes()
            {
                var allDateTimes = new List<DateTime>();

                //Construct a "master" list with all DateTimes across all lists inside the TagValues. 
                //This is done by calling LINQ unions over the DateTimes from each of the DNATagValues. 
                //Should a better/faster method be used? Probably. But this is elegant, and I like it
                foreach (PullTagData pt in this.TagData)
                {
                    List<DateTime> tempDates = pt.GetAllDateTimes();
                    if (tempDates != null) allDateTimes.AddRange(tempDates);
                }

                //Sort and return the list
                allDateTimes = allDateTimes.Distinct().ToList();
                allDateTimes.Sort();
                return allDateTimes;
            }
            /// <summary>This method will pull the first data point using Snap mode, since the first data point is not  guaranteed to have a raw data point at 
            /// that exact time. It returns a List of tuples with structure: (double) FirstValue, (bool) FirstStatus.</summary>
            /// <param name="startDate">A DateTime used for the initialization of the Snap point pull. It should be the start date supplied by the user for the 
            /// overall point pull.</param>
            public List<PullValue> PullFirstValuesSnap(DateTime startDate)
            {
                var retList = new List<PullValue>();
                //Iterate over all the points in the PointsPull  
                foreach (PullTagData currentTagData in this.TagData)
                {
                    retList.Add(currentTagData.PullFirstValueSnap(startDate));
                }

                //Return the results
                return retList;
            }
            /// <summary>This method will construct a StringBuilder header which may be used as the first row of an output CSV file.</summary>
            /// <param name="writeStatusColumn">A boolean value that will determine if a status column is written or not. Default value is TRUE.</param>
            /// <param name="statusColumnName">A string that defines the name of the status column that will be written. Default value is "Reliable".</param>
            /// <param name="dateTimeName">A string that defines the name of the DateTime column that will be written. Default value is "DateTime".</param>
            public StringBuilder WriteFullHeader(bool writeStatusColumn = true, string statusColumnName = "Reliable",
                string dateTimeName = "DateTime")
            {
                //Initialization
                StringBuilder retSB = new StringBuilder();
                retSB.Append(dateTimeName + ",");

                //Iterate over all the points in the point group
                foreach (PullTagData currentTagData in this.TagData)
                {
                    retSB.Append(currentTagData.WritePointHeader(writeStatusColumn, statusColumnName));
                }

                //Return the StringBuilder
                return retSB;
            }
            /// <summary>This method returns a StringBuilder for the first data row of the output CSV file.</summary>
            /// <param name="startDate">The starting date of the data pull.</param>
            /// <param name="writeStatusColumn">A boolean value that will determine if a status column is written or not. Default value is TRUE.</param>
            public StringBuilder WriteFirstRow(DateTime startDate, bool writeStatusColumn = true)
            {
                //Initialization
                StringBuilder retSB = new StringBuilder();
                retSB.Append(",");

                //Pull the first value for each point using Snap
                List<PullValue> firstValues = this.PullFirstValuesSnap(startDate);

                //Iterate over each point
                foreach (PullValue currentValue in firstValues)
                {
                    if (writeStatusColumn) retSB.Append(currentValue.Value + "," + currentValue.Reliable + ",");
                    else retSB.Append(currentValue.Value + ",");
                }

                //Return the stringbuilder
                return retSB;
            }
            /// <summary>This method will write all of the pulled data to a string.</summary>
            /// <param name="writeStatus">If TRUE, a status column will be written.</param>
            /// <param name="fillData">Should "empty" data be filled using the last known value?</param>
            public StringBuilder WriteDataToString(bool writeStatus = true, bool fillData = false)
            {
                List<DateTime> allDateTimes = this.FindAllDateTimes();
                var retSB = new StringBuilder();

                //Insert a first data point
                foreach (PullTagData currentTagData in this.TagData)
                {
                    currentTagData.InsertFirstValueSnap();
                }

                //I like the "if" outside the for loop so it's only checked once
                //Slight time improvement? Less readable, though
                if (writeStatus)
                {
                    //Iterate over all the possible DateTimes
                    foreach (DateTime currentDateTime in allDateTimes)
                    {

                        //Append the DateTime
                        retSB.Append(currentDateTime.ToString() + ",");

                        //Iterate over each point
                        foreach (PullTagData currentTagData in this.TagData)
                        {
                            //Find the index of the point we are writing
                            int indexFound = (fillData) ? currentTagData.Values.FindIndex(f => f.Time > currentDateTime) - 1 :
                                currentTagData.Values.FindIndex(f => f.Time == currentDateTime);
                            if (indexFound > 0)
                            {
                                retSB.Append(currentTagData.Values[indexFound].Value.ToString() + "," +
                                    currentTagData.Values[indexFound].Reliable + ",");
                            }
                            else if (fillData)
                            {
                                retSB.Append(currentTagData.Values[0].Value.ToString() + "," +
                                    currentTagData.Values[0].Reliable + ",");
                            }
                            else
                            {
                                retSB.Append(",,");
                            }
                        }

                        //New line
                        retSB.Append("\n");
                    }
                }
                else
                {
                    //Iterate over all the possible DateTimes
                    foreach (DateTime currentDateTime in allDateTimes)
                    {

                        //Append the DateTime
                        retSB.Append(currentDateTime.ToString() + ",");

                        //Iterate over each point
                        foreach (PullTagData currentTagData in this.TagData)
                        {
                            //Find the index of the point we are writing
                            int? indexFound = (fillData) ? currentTagData.Values.FindIndex(f => f.Time > currentDateTime) - 1 :
                                currentTagData.Values.FindIndex(f => f.Time == currentDateTime);
                            if (indexFound > 0)
                            {
                                retSB.Append(currentTagData.Values[indexFound.Value].Value.ToString() + ",");
                            }
                            else if (fillData)
                            {
                                retSB.Append(currentTagData.Values[0].Value.ToString() + ",");
                            }
                            else
                            {
                                retSB.Append(",");
                            }
                        }

                        //New line
                        retSB.Append("\n");
                    }
                }

                //Return the results
                return retSB;
            }
        }
    }
    namespace PushTools
    {
        //Initialization classes
        /// <summary>This class contains a DataTable of values to push, obtained by importing from a CSV file.</summary>
        public class PushDataTable
        {
            /// <summary>The DataTable constructed during the import process. May be bound to a user control.</summary>
            public DataTable OverallDataTable { get; private set; }
            /// <summary>A string of the errors encountered during the import process.</summary>
            public string ImportErrors { get; private set; }
            /// <summary>This class contains a DataTable of values to push, obtained by importing from a CSV file.</summary>
            /// <param name="filePath">The full path of the CSV file.</param>
            public PushDataTable(string filePath)
            {
                var errorString = new StringBuilder();
                //Create the data table
                var tempDataTable = new DataTable();
                tempDataTable.Columns.Add("Type", typeof(string));
                tempDataTable.Columns.Add("Tag", typeof(string));
                tempDataTable.Columns.Add("StartTime", typeof(DateTime));
                tempDataTable.Columns.Add("EndTime", typeof(DateTime));
                tempDataTable.Columns.Add("UpdateRate", typeof(int));
                tempDataTable.Columns.Add("Param1", typeof(double));
                tempDataTable.Columns.Add("Param2", typeof(double));
                tempDataTable.Columns.Add("Param3", typeof(double));
                tempDataTable.Columns.Add("Param4", typeof(double));

                //Read From File
                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    int lineNum = 0;

                    //Iterate over each line
                    while (!reader.EndOfStream)
                    {
                        lineNum++;

                        //Construct the input string
                        string[] splitLine = reader.ReadLine().Split(',');

                        //Sanitize inputs using defaults
                        string tag = "NONE";
                        DateTime startTime = DateTime.MinValue;
                        DateTime endTime = DateTime.MinValue;
                        int updateRate = 60;
                        double param1 = 0;
                        double param2 = 0;
                        double param3 = 0;
                        double param4 = 0;

                        //Simulation Type
                        string type = "raw";
                        if (splitLine.Length > 0)
                        {
                            if (this.ValidateType(splitLine[0])) type = splitLine[0];
                            else errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 1 is not a valid simulation type.\n");

                            //Tag name                            
                            if (splitLine.Length > 1) tag = splitLine[1];

                            //StartDate                            
                            try { if (splitLine.Length > 2) startTime = Convert.ToDateTime(splitLine[2]); }
                            catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 3 is not a valid DateTime.\n***" + ex.Message + "\n"); }

                            //If the data type is "raw", do something different
                            if (type.ToLower() == "raw")
                            {
                                endTime = DateTime.MinValue;
                                updateRate = 0;

                                //Param1                            
                                try { if (splitLine.Length > 3) param1 = Convert.ToDouble(splitLine[3]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 4 is not a valid double.\n***" + ex.Message + "\n"); }
                            }
                            else
                            {
                                //EndDate                            
                                try { if (splitLine.Length > 3) endTime = Convert.ToDateTime(splitLine[3]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 4 is not a valid DateTime.\n***" + ex.Message + "\n"); }

                                //UpdateRate                            
                                try { if (splitLine.Length > 4) updateRate = Convert.ToInt32(splitLine[4]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 5 is not a valid integer.\n***" + ex.Message + "\n"); }

                                //Param1                            
                                try { if (splitLine.Length > 5) param1 = (String.IsNullOrEmpty(splitLine[5])) ? 0.0 : Convert.ToDouble(splitLine[5]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 6 is not a valid double.\n***" + ex.Message + "\n"); }

                                //Param2
                                try { if (splitLine.Length > 6) param2 = (String.IsNullOrEmpty(splitLine[6])) ? 0.0 : Convert.ToDouble(splitLine[6]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 7 is not a valid double.\n***" + ex.Message + "\n"); }

                                //Param3
                                try { if (splitLine.Length > 7) param3 = (String.IsNullOrEmpty(splitLine[7])) ? 0.0 : Convert.ToDouble(splitLine[7]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 8 is not a valid double.\n***" + ex.Message + "\n"); }

                                //Param4
                                try { if (splitLine.Length > 8) param4 = (String.IsNullOrEmpty(splitLine[8])) ? 0.0 : Convert.ToDouble(splitLine[8]); }
                                catch (Exception ex) { errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 9 is not a valid double.\n***" + ex.Message + "\n"); }
                            }
                        }
                        else
                        {
                            errorString.Append("-ERROR- Row " + lineNum.ToString() + " Column 1 does not have a simulation type defined.\n");
                        }
                        //Add the line to the point grid
                        tempDataTable.Rows.Add(type, tag, startTime, endTime, updateRate, param1, param2, param3, param4);
                    }
                }

                //Store the results from the import
                this.OverallDataTable = tempDataTable;
                this.ImportErrors = errorString.ToString();
            }
            /// <summary>Validates the user-supplied push type.</summary>
            /// <param name="typeString">The string to validate.</param>
            private bool ValidateType(string typeString)
            {
                string[] possibleTypes = { "raw", "ramp", "sine", "impulse", "step", "periodic", "periodicimpulse", "rand", "randn", "rande" };
                return possibleTypes.Contains(typeString.ToLower());
            }
        }
        /// <summary>This class contains a list of services to push historical and real-time data.</summary>
        public class PushAllData
        {
            /// <summary>The total number of points being pushed.</summary>
            public int TotalPushes { get; private set; }
            /// <summary>A list of services to push historical data to.</summary>
            public List<PushService> HistoryDataPush { get; private set; }
            /// <summary>A list of services to push real-time data to.</summary>
            public List<PushService> RealTimeDataPush { get; private set; }
            /// <summary>This class contains a list of services to push historical and real-time data.</summary>
            /// <param name="pushDataTable">The data table used to construct the data push.</param>
            public PushAllData(DataTable pushDataTable)
            {
                //Run the two main methods to construct a history and real-time data push.
                this.ConstructHistoryPush(pushDataTable);
                this.ConstructRealTimePush();
            }
            /// <summary>This class contains a list of services to push historical and real-time data.</summary>
            /// <param name="pushSummary">This method is useful when using the DataReview namespace.</param>
            public PushAllData(DataReview.OverallDataReviewSummary pushSummary)
            {

                //Run the two main methods to construct a history and real-time data push.
                this.ConstructHistoryPush(pushSummary);
                this.ConstructRealTimePush();
            }
            /// <summary>Constructs a list of services to push to history based on the user-supplied DataTable.</summary>
            private void ConstructHistoryPush(DataReview.OverallDataReviewSummary pushSummary)
            {
                //Initialize values
                var historyDataPush = new List<PushService>();
                this.TotalPushes = pushSummary.RunHourSummary.Count + pushSummary.StressEventSummary.Count +
                    pushSummary.StressHourSummary.Count;

                //Add Run hours to be pushed
                foreach (var rhSummary in pushSummary.RunHourSummary)
                {
                    //Determine the history service to write to
                    string service = "";
                    History.DnaHistResolveHistoryName(rhSummary.RunHourTag, out service);

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
                    var tempPushTag = new PushTag(rhSummary.RunHourTag, service);
                    double currentValue = rhSummary.CurrentValue.Item2;

                    //Iterate over all the granular summaries
                    foreach (var granSum in rhSummary.GranularSummaries)
                    {
                        currentValue += granSum.HoursRunning;
                        tempPushTag.AddTagValues(new PushValue(granSum.EndDate, currentValue));
                    }
                    historyDataPush[index].AddPushTag(tempPushTag);
                }

                //Add Stress hours to be pushed
                foreach (var shSummary in pushSummary.StressHourSummary)
                {
                    //Determine the history service to write to
                    string service = "";
                    History.DnaHistResolveHistoryName(shSummary.StressHourTag, out service);

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
                    var tempPushTag = new PushTag(shSummary.StressHourTag, service);
                    double currentValue = shSummary.CurrentValue.Item2;

                    //Iterate over all the granular summaries
                    foreach (var granSum in shSummary.GranularSummaries)
                    {
                        currentValue += granSum.NumberHours;
                        tempPushTag.AddTagValues(new PushValue(granSum.EndDate, currentValue));
                    }
                    historyDataPush[index].AddPushTag(tempPushTag);
                }

                //Add Stress events to be pushed
                foreach (var seSummary in pushSummary.StressEventSummary)
                {
                    //Determine the history service to write to
                    string service = "";
                    History.DnaHistResolveHistoryName(seSummary.StressEventTag, out service);

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
                    var tempPushTag = new PushTag(seSummary.StressEventTag, service);
                    double currentValue = seSummary.CurrentValue.Item2;

                    //Iterate over all the granular summaries
                    foreach (var granSum in seSummary.GranularSummaries)
                    {
                        currentValue += granSum.NumberEvents;
                        tempPushTag.AddTagValues(new PushValue(granSum.EndDate, currentValue));
                    }
                    historyDataPush[index].AddPushTag(tempPushTag);
                }

                //Save the values
                this.HistoryDataPush = historyDataPush;
            }
            /// <summary>Constructs a list of services to push to history based on the user-supplied DataTable.</summary>
            private void ConstructHistoryPush(DataTable pushDataTable)
            {
                //Initialize values
                var historyDataPush = new List<PushService>();
                this.TotalPushes = pushDataTable.Rows.Count;

                //Iterate over each line in the PointGrid
                foreach (DataRow tempDataRow in pushDataTable.Rows)
                {
                    var tempPushValueList = new List<PushValue>();
                    //Initialize Values
                    //These are all strongly typed of course- however, we imported the DataTable from
                    //a CSV file using very constrainted methods, and ideally any binded DataGridView
                    //will have constraints about data types. This may not be perfectly safe, but I 
                    //feel confident enough that it will work 99.9% of the time.                   
                    string pushType = tempDataRow.Field<string>(0).ToLower();
                    string pointTag = tempDataRow.Field<string>(1);
                    DateTime startTime = tempDataRow.Field<DateTime>(2);
                    DateTime endTime = tempDataRow.Field<DateTime>(3);
                    int updateRate = tempDataRow.Field<int>(4);
                    double param1 = tempDataRow.Field<double>(5);
                    double param2 = tempDataRow.Field<double>(6);
                    double param3 = tempDataRow.Field<double>(7);
                    double param4 = tempDataRow.Field<double>(8);

                    //Select which type of data pull              
                    switch (pushType)
                    {
                        case ("raw"):
                            tempPushValueList = SimulationMethods.SimulateRaw(startTime, param1);
                            break;
                        case ("ramp"):
                            tempPushValueList = SimulationMethods.SimulateRamp(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2);
                            break;
                        case ("sine"):
                            tempPushValueList = SimulationMethods.SimulateSine(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2, param3, param4);
                            break;
                        case ("impulse"):
                            tempPushValueList = SimulationMethods.SimulateImpulse(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2);
                            break;
                        case ("step"):
                            tempPushValueList = SimulationMethods.SimulateStep(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2);
                            break;
                        case ("periodic"):
                            tempPushValueList = SimulationMethods.SimulatePeriodic(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2, param3, param4);
                            break;
                        case ("periodicimpulse"):
                            tempPushValueList = SimulationMethods.SimulatePeriodicImpulse(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2, param3);
                            break;
                        case ("rand"):
                            tempPushValueList = SimulationMethods.SimulateRand(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2);
                            break;
                        case ("randn"):
                            tempPushValueList = SimulationMethods.SimulateRandn(new TimeSpan(0, 0, updateRate), startTime, endTime, param1, param2);
                            break;
                        case ("rande"):
                            tempPushValueList = SimulationMethods.SimulateRande(new TimeSpan(0, 0, updateRate), startTime, endTime, param1);
                            break;
                    }

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

                //Save the values
                this.HistoryDataPush = historyDataPush;
            }
            /// <summary>Constructs a real-time data push using the last value of the history data push.</summary>
            private void ConstructRealTimePush()
            {
                //Initialize values
                var realTimeDataPush = new List<PushService>();

                //Iterate over all the values necessary
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
        }
        /// <summary>This class is used to initialize a data push with points grouped together by the same service being pushed
        /// to. It will save time especially when pushing in "RealTime" mode.</summary>
        public class PushService
        {
            /// <summary>The fully-qualified eDNA history service (Site.Service) to push values to. Note- this is the history 
            /// service, not the real-time service.</summary>
            public string Service { get; private set; }
            /// <summary>The eDNA status to use for each point. A value of "3" indicates that the status is "OK". Ideally, the
            /// user should be able to select the status individually for each point. However, it seems unlikely that this
            /// functionality is necessary, and it would add complexity to the user experience. Since 99% of the time, the
            /// user will want the status to be "OK", I just left this as a global value that applies to all values defined
            /// in this.TagValues.</summary>
            public ushort GlobalStatus { get; private set; }
            /// <summary>A list of PushTag instances that will be pushed.</summary>
            public List<PushTag> PushTags { get; private set; }
            /// <summary>This class is used to initialize a data push with points grouped together by the same service being pushed
            /// to. It will save time especially when pushing in "RealTime" mode.</summary>
            /// <param name="service">The fully-qualified eDNA history service (Site.Service) to push values to. Note- this is the history 
            /// service, not the real-time service.</param>
            /// <param name="globalStatus">The eDNA status to use for each point. A value of "3" indicates that the status is "OK". Ideally, the
            /// user should be able to select the status individually for each point. However, it seems unlikely that this
            /// functionality is necessary, and it would add complexity to the user experience. Since 99% of the time, the
            /// user will want the status to be "OK", I just left this as a global value that applies to all values defined
            /// in this.TagValues.</param>
            public PushService(string service, ushort globalStatus = 3)
            {
                this.Service = service;
                this.PushTags = new List<PushTag>();
                this.GlobalStatus = globalStatus;
            }
            /// <summary>Adds a list of PushTags to the stored list inside the class.</summary>
            public void AddPushTag(List<PushTag> pushTagList)
            {
                this.PushTags.AddRange(pushTagList);
            }
            /// <summary>Adds a PushTag to the stored list inside the class.</summary>
            public void AddPushTag(PushTag pushTag)
            {
                this.PushTags.Add(pushTag);
            }
            /// <summary>Clears the list of PushTags.</summary>
            public void ClearPushTags()
            {
                this.PushTags.Clear();
            }
            /// <summary>This method will push the values in this.TagValues to eDNA history using "RealTime" mode. For more information
            /// about "RealTime" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push. Warning- this method is very slow and is not recommended to be used. As the name
            /// suggests, pushing to real-time seems to work inconsistently unless you are pushing values to the actual current time, although
            /// you can push "RealTime" to any DateTime later than the current DateTime in the eDNA CVT.</summary>
            /// <param name="realTimeService">The real-time service to push the values to. Technically, this could easily be found by calling DNAMiscMethods.FindService. 
            /// However, the whole point of using "PushService" to push real-time values instead of calling each "PushTag" individually is 
            /// to save time with the initialization of the service connection. By forcing the user to declare explicitly which RealTime service
            /// they wish to write to, I believe that user error will be reduced.</param>
            public StringBuilder PushRealTimeAllTags(string realTimeService)
            {
                //Initialize values
                int nRet = 1;
                uint uiKey1 = 0;
                string errorString = String.Empty;
                var progressString = new StringBuilder();
                string cacheFilename = "cache_" + this.Service;

                //Grab the defined "_INPADDR" point
                string inpAddr = String.Join(".", realTimeService, "_INPADDR");
                string description = MiscMethods.GetPointDescription(inpAddr);
                string ipAddress = description.Split(',')[0];
                ushort port = (ushort)Convert.ToInt32(description.Split(',')[1]);

                //If the "_INPADDR" point doesn't exist, this mode cannot be performed
                if (!String.IsNullOrEmpty(description))
                {
                    //Initialize rt push
                    nRet = LinkMX.eDnaMxUniversalInitialize(out uiKey1, true, true, true, (int)50000, cacheFilename, "C:\\ProgramData\\InStep\\");
                    progressString.Append("Initializing connection status= " + nRet + "\n");
                    //Connect to the service
                    nRet = LinkMX.eDnaMxUniversalDataConnect(uiKey1, ipAddress, port, "", (ushort)0);
                    progressString.Append("Connection result= " + nRet + "\n");

                    //Iterate over each point
                    foreach (PushTag currentPushTag in this.PushTags)
                    {
                        DateTime startWrite = DateTime.Now;
                        string pointID = MiscMethods.FindPointID(currentPushTag.Tag);
                        int numErrors = 0;

                        //Iterate over all the values and push to the queue
                        foreach (PushValue valueWrite in currentPushTag.TagValues)
                        {
                            nRet = LinkMX.eDnaMxAddRec(uiKey1, pointID, valueWrite.UTCTime, (ushort)0, (int)-1,
                                Convert.ToDouble(valueWrite.Value));
                            if (nRet != 0) { numErrors++; }
                            Thread.Sleep(25);
                        }

                        //Flush the values
                        nRet = LinkMX.eDnaMxFlushUniversalRecord(uiKey1, (int)1);
                        Thread.Sleep(50);

                        //Update the errorstring
                        string timeElapsed = (DateTime.Now - startWrite).TotalSeconds.ToString();
                        progressString.Append("Values for " + currentPushTag.Tag + " pushed in " + timeElapsed + " seconds with " + numErrors.ToString() + " errors\n");
                    }

                    //Close the connection
                    LinkMX.eDnaMxUniversalCloseSocket(uiKey1);

                    //Delete the cache file
                    LinkMX.eDnaMxDeleteCacheFiles(uiKey1);
                }
                else
                {
                    progressString.Append("-ERROR- Service " + realTimeService + " does not have an '_INPADDR' point defined.\n");
                }

                //Return the errors and progress
                return progressString;
            }
            /// <summary>This method will push all the values initialized in this.PushTags to eDNA history using "Insert" mode. For more information
            /// about "Insert" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push.</summary>
            public StringBuilder PushInsertAllTags()
            {
                var progressString = new StringBuilder();

                //Iterate over each point
                foreach (PushTag pointWrite in this.PushTags)
                {
                    progressString.Append(pointWrite.PushInsertAllValues());
                }

                //Return the errors and progress
                return progressString;
            }
            /// <summary>This method will push all the values initialized in this.PushTags to eDNA history using "Append" mode. For more information
            /// about "Append" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push.</summary>
            public StringBuilder PushAppendAllTags()
            {
                var progressString = new StringBuilder();

                //Iterate over each point
                foreach (PushTag pointWrite in this.PushTags)
                {
                    progressString.Append(pointWrite.PushAppendAllValues());
                }

                //Return the errors and progress
                return progressString;
            }
        }
        /// <summary>This class contains a list of values to push for a particular eDNA tag.</summary>
        public class PushTag
        {
            /// <summary>The fully-qualified (Site.Service.Point) tag name for the eDNA point associated with the data push.</summary>
            public string Tag { get; private set; }
            /// <summary>The fully-qualified eDNA service (Site.Service) to push values to.</summary>
            public string PushService { get; private set; }
            /// <summary>The eDNA status to use for each point. A value of "3" indicates that the status is "OK". Ideally, the
            /// user should be able to select the status individually for each point. However, it seems unlikely that this
            /// functionality is necessary, and it would add complexity to the user experience. Since 99% of the time, the
            /// user will want the status to be "OK", I just left this as a global value that applies to all values defined
            /// in this.TagValues.</summary>
            public ushort GlobalStatus { get; private set; }
            /// <summary>A list of eDNA values which will be pushed.</summary>
            public List<PushValue> TagValues { get; private set; }
            /// <summary>This class contains a list of values to push for a particular eDNA tag.</summary>
            /// <param name="tag">The fully-qualified (Site.Service.Point) tag name for the eDNA point associated with the data push.</param>
            /// <param name="pushService">The fully-qualified eDNA history service (Site.Service) to push values to. Note- this is the history 
            /// service, not the real-time service.</param>
            /// <param name="globalStatus">The eDNA status to use for each point. A value of "3" indicates that the status is "OK". Ideally, the
            /// user should be able to select the status individually for each point. However, it seems unlikely that this
            /// functionality is necessary, and it would add complexity to the user experience. Since 99% of the time, the
            /// user will want the status to be "OK", I just left this as a global value that applies to all values defined
            /// in this.TagValues.</param>
            public PushTag(string tag, string pushService, ushort globalStatus = 3)
            {
                this.Tag = tag.Trim();
                this.PushService = pushService.Trim();
                this.GlobalStatus = globalStatus;
                this.TagValues = new List<PushValue>();
            }
            /// <summary>Add a list of PushValues to the stored list, ready to be pushed.</summary>
            public void AddTagValues(List<PushValue> pushValueList)
            {
                this.TagValues.AddRange(pushValueList);
            }
            /// <summary>Add a PushValue to the stored list, ready to be pushed.</summary>
            public void AddTagValues(PushValue pushValue)
            {
                this.TagValues.Add(pushValue);
            }
            /// <summary>Clears the stored list of values to push.</summary>
            public void ClearValues()
            {
                this.TagValues.Clear();
            }
            /// <summary>Returns the total amount of values to be pushed.</summary>
            public int GetTotalValues()
            {
                return this.TagValues.Count;
            }
            /// <summary>This method will push the values in this.TagValues to eDNA history using "RealTime" mode. For more information
            /// about "RealTime" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push. Warning- this method is very slow and is not recommended to be used. If you have multiple
            /// tags that need to be pushed to the same real-time service, you should use "PushRealTimeAllTags" from the PushService class, since 
            /// it only connects to the service and initializes once, saving a lot of time. Also, pushing to real-time seems to work inconsistently
            /// unless you are pushing values to the actual current time.</summary>
            [Obsolete]
            public StringBuilder PushRealTimeAllValues()
            {
                //Initialization
                DateTime startwrite = DateTime.Now;
                int numerrors = 0;
                string errorstring = String.Empty;
                var progressstring = new StringBuilder();

                //eDNA Initialization
                int nRet = 1;
                uint uiKey1 = 0;
                string rtservice = MiscMethods.FindService(this.Tag);
                string pointid = MiscMethods.FindPointID(this.Tag);
                string description = MiscMethods.GetPointDescription(String.Join(".", rtservice, "_INPADDR"));
                string IPAddress = description.Split(',')[0];
                ushort Port = (ushort)Convert.ToInt32(description.Split(',')[1]);

                //If the "_INPADDR" point doesn't exist, this mode cannot be performed
                if (!String.IsNullOrEmpty(description))
                {
                    //Initialize rt push
                    nRet = LinkMX.eDnaMxUniversalInitialize(out uiKey1, true, true, true, (int)5000, "service_cache", "C:\\ProgramData\\InStep\\");
                    progressstring.Append("Initializing connection status= " + nRet + "\n");
                    //Connect to the service
                    nRet = LinkMX.eDnaMxUniversalDataConnect(uiKey1, IPAddress, Port, "", (ushort)0);
                    progressstring.Append("Connection result= " + nRet + "\n");

                    //Iterate over all the values and push to the queue
                    foreach (PushValue valuewrite in this.TagValues)
                    {
                        nRet = LinkMX.eDnaMxAddRec(uiKey1, pointid, valuewrite.UTCTime, (ushort)0, (int)-1,
                            Convert.ToDouble(valuewrite.Value));
                        if (nRet != 0) { numerrors++; }
                        //Flush the value
                        nRet = LinkMX.eDnaMxFlushUniversalRecord(uiKey1, (int)1);
                    }

                    //Update the logger
                    string timeelapsed = (DateTime.Now - startwrite).TotalSeconds.ToString();
                    progressstring.Append("Values for " + this.Tag + " pushed in " + timeelapsed + " seconds with " + numerrors.ToString() + " errors\n");

                    //Close the connection
                    LinkMX.eDnaMxUniversalCloseSocket(uiKey1);
                    //Delete the cache file
                    LinkMX.eDnaMxDeleteCacheFiles(uiKey1);
                }
                else
                {
                    progressstring.Append("-ERROR- Service " + this.PushService + " does not have an '_INPADDR' point defined.\n");
                }

                //Return the errors and progress
                return progressstring;
            }
            /// <summary>This method will push the values in this.TagValues to eDNA history using "Insert" mode. For more information
            /// about "Insert" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push.</summary>
            public StringBuilder PushInsertAllValues()
            {
                //Initialization
                int nRet = 1;
                var progressString = new StringBuilder();
                string errorString = String.Empty;
                int numErrors = 0;
                var startWrite = DateTime.Now;

                //Iterate over all the values and push to the queue
                foreach (PushValue valueWrite in this.TagValues)
                {
                    //Try to append the point to history
                    nRet = History.DnaHistQueueUpdateInsertValue(this.PushService, this.Tag, valueWrite.UTCTime, this.GlobalStatus,
                        valueWrite.Value, out errorString);
                    //Check for errors
                    if (nRet != 0) numErrors++;
                    if (!String.IsNullOrEmpty(errorString)) progressString.Append("-ERROR- " + errorString + "\n:");
                }

                //Flush the queue
                nRet = History.DnaHistFlushAppendValues(this.PushService, this.Tag, out errorString);
                if (!String.IsNullOrEmpty(errorString)) progressString.Append("-ERROR- " + errorString + "\n:");

                //Update the errorstring
                string timeElapsed = (DateTime.Now - startWrite).TotalSeconds.ToString();
                progressString.Append("Values for " + this.Tag + " pushed in " + timeElapsed + " seconds with " + numErrors.ToString() + " errors.\n");

                //Return the errors and progress
                return progressString;
            }
            /// <summary>This method will push the values in this.TagValues to eDNA history using "Append" mode. For more information
            /// about "Append" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
            /// errors encountered during the data push.</summary>
            public StringBuilder PushAppendAllValues()
            {
                //Initialization
                int nRet = 1;
                var progressString = new StringBuilder();
                string errorString = String.Empty;
                int numErrors = 0;
                var startWrite = DateTime.Now;

                //Iterate over all the values and push to the queue
                foreach (PushValue valueWrite in this.TagValues)
                {
                    //Try to append the point to history
                    nRet = History.DnaHistQueueAppendValue(this.PushService, this.Tag, valueWrite.UTCTime, this.GlobalStatus,
                        valueWrite.Value, out errorString);

                    //Check for errors
                    if (nRet != 0) numErrors++;
                    if (!String.IsNullOrEmpty(errorString)) progressString.Append("-ERROR- " + errorString + "\n:");
                }

                //Flush the queue
                nRet = History.DnaHistFlushAppendValues(this.PushService, this.Tag, out errorString);
                if (!String.IsNullOrEmpty(errorString)) progressString.Append("-ERROR- " + errorString + "\n:");

                //Update the errorstring
                string timeElapsed = (DateTime.Now - startWrite).TotalSeconds.ToString();
                progressString.Append("Values for " + this.Tag + " pushed in " + timeElapsed + " seconds with " + numErrors.ToString() + " errors.\n");

                //Return the errors and progress
                return progressString;
            }
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
        /// <summary>This enum is used to specify which data simulation type is being used.</summary>
        public enum SimulateType
        {
            /// <summary>Write data using the raw simulation method.</summary>
            raw,
            /// <summary>Write data using the ramp simulation method.</summary>
            ramp,
            /// <summary>Write data using the rand simulation method.</summary>
            rand,
            /// <summary>Write data using the randn simulation method.</summary>
            randn,
            /// <summary>Write data using the rande simulation method.</summary>
            rande,
            /// <summary>Write data using the sine simulation method.</summary>
            sine,
            /// <summary>Write data using the impulse simulation method.</summary>
            impulse,
            /// <summary>Write data using the step simulation method.</summary>
            step,
            /// <summary>Write data using the periodic simulation method.</summary>
            periodic,
            /// <summary>Write data using the periodicimpulse simulation method.</summary>
            periodicimpulse
        }
        /// <summary>This data structure is used to store an eDNA value which will be pushed.</summary>
        public struct PushValue
        {
            /// <summary>The DateTime (in UTC format) to push the eDNA value.</summary>
            public int UTCTime { get; private set; }
            /// <summary>The value to be pushed.</summary>
            public string Value { get; private set; }
            /// <summary>This data structure is used to store an eDNA value which will be pushed.</summary>
            /// <param name="utcTime">The DateTime (in UTC format) to push the eDNA value.</param>
            /// <param name="value">The value to be pushed.</param>
            public PushValue(DateTime utcTime, double value)
                : this()
            {
                this.Value = value.ToString();
                this.UTCTime = Utility.GetUTCTime(utcTime);
            }
        }

        //Static methods
        /// <summary>A class that contains method which are used to create simulated data.</summary>
        public static class SimulationMethods
        {
            /// <summary>This input setting is useful when pushing one data point at a time. It would typically be used to correct 
            /// erroneous values, not for simulation purposes.</summary>
            /// <param name="time">Date/Time (in correct format)</param>
            /// <param name="value">Value to be pushed</param>
            public static List<PushValue> SimulateRaw(DateTime time, double value)
            {
                //Only one time/value needs to be added
                var list = new List<PushValue>();
                list.Add(new PushValue(time, value));
                return list;
            }
            /// <summary> This input setting is useful to simulate data over a more extended time period, when relative change is
            /// more important than absolute values. For instance, if the run hours for equipment are known exactly at
            /// Time A and Time B, this input setting may be used to linearly interpolate the values in between.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="startVal">Starting Value</param>
            /// <param name="stopVal">Ending Value</param>
            public static List<PushValue> SimulateRamp(TimeSpan period, DateTime startDate, DateTime endDate, double startVal, double stopVal)
            {
                double totalDays = period.TotalDays;
                //Generate the dates and values
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                values = Generate.LinearSpaced(times.Length, startVal, stopVal);
                //Output results
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
            /// <summary>A uniform random distribution generates values uniformly along the interval [low value, high value].</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="lowVal">Low value</param>
            /// <param name="highVal">High value</param>
            public static List<PushValue> SimulateRand(TimeSpan period, DateTime startDate, DateTime endDate, double lowVal, double highVal)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                //Output results
                var pushList = new List<PushValue>();
                var rnd = new Random((int)DateTime.Now.Ticks);
                for (int ii = 0; ii < times.Length; ii++)
                {
                    double rand = rnd.NextDouble() * (highVal - lowVal) + lowVal;
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), rand));
                }
                return pushList;
            }
            /// <summary>A normal random distribution generates values using a mean and standard deviation.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="mean">Mean</param>
            /// <param name="std">Standard deviation</param>
            public static List<PushValue> SimulateRandn(TimeSpan period, DateTime startDate, DateTime endDate, double mean, double std)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                //Generate the dates and values
                var pushList = new List<PushValue>();
                var rnd = new Random((int)DateTime.Now.Ticks);
                for (int ii = 0; ii < times.Length; ii++)
                {
                    double randN = Math.Sqrt(-2.0 * Math.Log(rnd.NextDouble())) * Math.Sin(2.0 * Math.PI * rnd.NextDouble());
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), randN));
                }
                return pushList;
            }
            /// <summary>An exponential random distribution generates values using a rate.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="rate">The rate for the exponential distribution.</param>
            public static List<PushValue> SimulateRande(TimeSpan period, DateTime startDate, DateTime endDate, double rate)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                //Generate the dates and values
                var pushList = new List<PushValue>();
                var rnd = new Random((int)DateTime.Now.Ticks);
                for (int ii = 0; ii < times.Length; ii++)
                {
                    double randE = Math.Log(1 - rnd.NextDouble()) / (-rate);
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), randE));
                }
                return pushList;
            }
            /// <summary>Generates eDNA values to push using a sine function.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="frequency">The frequency of the sine wave, with units equal to the data update frequency selected.</param>
            /// <param name="amplitude">The amplitude of the sine wave (default value is 1).</param>
            /// <param name="mean">The mean of the sine wave (default value is 1).</param>
            /// <param name="phase">The phase of the sine wave (default value is 0).</param>
            public static List<PushValue> SimulateSine(TimeSpan period, DateTime startDate, DateTime endDate, double frequency, double amplitude = 1, double mean = 1, double phase = 0)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double totalSeconds = period.TotalSeconds;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                //Generate the dates and values
                values = Generate.Sinusoidal(times.Length, totalSeconds, frequency, amplitude, mean, phase);
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
            /// <summary>Generates an impulse in the data relative to the start date.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="amplitude">The amplitude of the impulse.</param>
            /// <param name="delay">The time period after which the impulse occurs, relative to the starting date, with units equal to the data update frequency selected.</param>
            public static List<PushValue> SimulateImpulse(TimeSpan period, DateTime startDate, DateTime endDate, double amplitude, double delay)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                //Generate the dates and values
                values = Generate.Impulse(times.Length, amplitude, Convert.ToInt32(delay));
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
            /// <summary>Generates a step function.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="amplitude">The amplitude of the step function.</param>
            /// <param name="delay">The amount of time after which the step occurs, with units equal to the data update frequency selected.</param>
            public static List<PushValue> SimulateStep(TimeSpan period, DateTime startDate, DateTime endDate, double amplitude, double delay)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                //Generate the dates and values
                values = Generate.Step(times.Length, amplitude, Convert.ToInt32(delay));
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
            /// <summary>Generates a periodic function.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="frequency">The frequency of the periodic function.</param>
            /// <param name="amplitude">The amplitude of the periodic function.</param>
            /// <param name="phase">The phase of the periodic function.</param>
            /// <param name="delay">The delay of the periodic function.</param>
            public static List<PushValue> SimulatePeriodic(TimeSpan period, DateTime startDate, DateTime endDate, double frequency, double amplitude, double phase, double delay)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double totalSeconds = period.TotalSeconds;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                //Generate the dates and values
                values = Generate.Periodic(times.Length, totalSeconds, frequency, amplitude, phase, Convert.ToInt32(delay));
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
            /// <summary>Generates a periodic impulse.</summary>
            /// <param name="period">Data Update Frequency</param>
            /// <param name="startDate">Starting Date/Time</param>
            /// <param name="endDate">Ending Date/Time</param>
            /// <param name="impulsePeriod">The period of the impulse, with units equal to the data update frequency selected.</param>
            /// <param name="amplitude">The amplitude of the impulse.</param>
            /// <param name="delay">The delay of the impulse, with units equal to the data update frequency selected.</param>
            public static List<PushValue> SimulatePeriodicImpulse(TimeSpan period, DateTime startDate, DateTime endDate, double impulsePeriod, double amplitude, double delay)
            {
                //Initialize parameters
                double totalDays = period.TotalDays;
                double[] times = Generate.LinearRange(startDate.ToOADate(), totalDays, endDate.ToOADate());
                double[] values = new double[times.Length];
                //Generate the dates and values
                values = Generate.PeriodicImpulse(times.Length, Convert.ToInt32(impulsePeriod), amplitude, Convert.ToInt32(delay));
                var pushList = new List<PushValue>();
                for (int ii = 0; ii < times.Length; ii++)
                {
                    pushList.Add(new PushValue(DateTime.FromOADate(times[ii]), values[ii]));
                }
                return pushList;
            }
        }
    }
    namespace TrendTools
    {
        /// <summary>A class useful for holding multiple instances of TrendInfos, found recursively through the directory structure.</summary>
        public class TrendDirectory
        {
            /// <summary>The base file path of the directory.</summary>
            public string BaseDirectory { get; private set; }
            /// <summary>A list of TrendInfos found recursively through the directory structure.</summary>
            public List<TrendInfo> TrendInfos { get; private set; }
            /// <summary>A class useful for holding multiple instances of TrendInfos, found recursively through the directory structure.</summary>
            /// <param name="directoryPath">The base file path of the directory.</param>
            public TrendDirectory(string directoryPath)
            {
                //Initialize values
                this.BaseDirectory = directoryPath;
                this.TrendInfos = new List<TrendInfo>();
                DirectoryInfo d = new DirectoryInfo(directoryPath);

                //Recursively find all the .eztx files    
                foreach (string file in Directory.EnumerateFiles(this.BaseDirectory, "*.xml", SearchOption.AllDirectories))
                {
                    this.TrendInfos.Add(new TrendInfo(file));
                }

                //Recursively find all the .eztx files               
                foreach (string file in Directory.EnumerateFiles(this.BaseDirectory, "*.eztx", SearchOption.AllDirectories))
                {
                    this.TrendInfos.Add(new TrendInfo(file));
                }
            }
            /// <summary>Writes all the associated tags of all this.TrendInfos to a CSV file.</summary>
            /// <param name="filePath">The file path to write the CSV file to.</param>
            public void WriteAllAssociatedTagsToFile(string filePath)
            {
                File.Delete(filePath);
                using (var file = new StreamWriter(filePath, true))
                {
                    foreach (TrendInfo currentTrend in this.TrendInfos)
                    {
                        file.Write(currentTrend.WriteAssociatedTags());
                    }
                }
            }
        }
        /// <summary>A class useful for determining information about eDNA trends.</summary>
        public class TrendInfo
        {
            /// <summary>The name of the trend</summary>
            public string Name { get; private set; }
            /// <summary>The path to the file.</summary>
            public FileInfo FilePath { get; private set; }
            /// <summary>The type of the trend (either eztx or xml).</summary>
            public string Extension { get; private set; }
            /// <summary>A list of tags associated with the trend.</summary>
            public List<string> AssociatedTags { get; private set; }
            /// <summary>A class useful for determining information about eDNA trends.</summary>
            /// <param name="filePath">The file path to the eDNA trend.</param>
            public TrendInfo(string filePath)
            {
                this.FilePath = new FileInfo(filePath);
                this.Name = Path.GetFileNameWithoutExtension(this.FilePath.Name);
                this.Extension = this.FilePath.Extension;
                this.AssociatedTags = new List<string>();

                //Read from the input file
                using (var reader = new StreamReader(filePath))
                {
                    //For the eztx format
                    if (this.Extension == ".eztx")
                    {
                        string config = reader.ReadToEnd();
                        IEnumerable<string> matchstrings = MiscMethods.GetSubStrings(config, "DataPointsOnAxis1=\"", "\" DataPointsOnAxis2");
                        foreach (string ms in matchstrings)
                        {
                            foreach (string tag in ms.Split('|'))
                            {
                                this.AssociatedTags.Add(tag);
                            }
                        }
                    }
                    //For the XML format
                    else if (this.Extension == ".xml")
                    {
                        XmlDocument xd = new XmlDocument();
                        xd.Load(filePath);
                        for (int ii = 0; ii < xd["ezTrend_Archive"]["Parameters"]["PointList"].ChildNodes.Count; ii++)
                        {
                            string tagText = xd["ezTrend_Archive"]["Parameters"]["PointList"].ChildNodes[ii].InnerText;
                            this.AssociatedTags.Add(tagText);
                        }
                    }
                }
            }
            /// <summary>Writes all the associated tags to a StringBuilder.</summary>
            public StringBuilder WriteAssociatedTags()
            {
                var retSB = new StringBuilder();

                foreach (string currentTag in this.AssociatedTags)
                {
                    retSB.AppendLine(String.Join(",", this.Name, this.Extension, this.FilePath, currentTag));
                }

                return retSB;
            }
        }
    }
}
