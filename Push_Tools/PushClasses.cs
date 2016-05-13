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
using MathNet.Numerics;
using MathNet.Filtering;
using InStep.eDNA.EzDNAApiNet;
using Push_Tools.Properties;

namespace Push_Tools
{
    public class PushToDNAService
    {
        public string Service { get; private set; }
        public List<Tuple<string, List<PushValue>>> TagsToPush { get; private set; }
        public PushToDNAService(string service)
        {
            this.Service = service;
            this.TagsToPush = new List<Tuple<string, List<PushValue>>>();
        }
        //The point of this function is to see if the tag has already been defined in the list of tags of push. If it hasn't been, that means the
        //index is less than 0, and it needs to be added to the overall list. If the tag already exists, I want to add it to the correct index,
        //and then sort the list (because the times will not necessarily be in order anymore).
        public void AddTagToPush(string tag, List<PushValue> values)
        {            
            int index = TagsToPush.FindIndex(f => f.Item1 == tag);
            if (index<0) TagsToPush.Add(new Tuple<string,List<PushValue>(tag,values));
            else
            {
                TagsToPush[index].Item2.AddRange(values);
                TagsToPush[index].Item2.OrderBy(f => f.UTCTime);
            }
        }
        public StringBuilder PushTagInsert(Tuple<string,List<PushValue>> tagAndValues, int roundDec = 6, ushort status = 3)
        {
            return PushToDNAService.PushTagInsert(this.Service, tagAndValues, roundDec, status);
        }
        public StringBuilder PushTagAppend(Tuple<string, List<PushValue>> tagAndValues, int roundDec = 6, ushort status = 3)
        {
            return PushToDNAService.PushTagAppend(this.Service, tagAndValues, roundDec, status);
        }
        public StringBuilder PushTagsRealtime(List<Tuple<string, List<PushValue>>> listTagsAndValues, int roundDec = 6, short status = 3, int sleepmSec = 100)
        {
            return PushToDNAService.PushTagsRealtime(this.Service, listTagsAndValues, roundDec, status, sleepmSec);
        }
        //Why do I separate out the static methods? It makes it easier to unit test
        public static StringBuilder PushTagInsert(string service, Tuple<string,List<PushValue>> tagAndValues, int roundDec = 6, ushort status = 3)
        {
            string historyService = String.Empty;        
            string tag = tagAndValues.Item1;
            History.DnaHistResolveHistoryName(tag, out historyService);
            //Initialization, these lines can be skipped
            int nRet = 0;
            string errorString = String.Empty;
            var progressString = new StringBuilder();
            var startWrite = DateTime.Now;
            //First, we are going to add all the points to the queue
            foreach(PushValue pv in tagAndValues.Item2)
            {
                nRet = History.DnaHistQueueUpdateInsertValue(historyService, tag, pv.UTCTime, status, Math.Round(pv.Value,roundDec).ToString(), out errorString);
                if (!String.IsNullOrEmpty(errorString)) progressString.AppendLine(String.Format("ERROR- {0}"));
            }
            //eDNA requires the queue to be flushed after all the points have been added to the queue
            nRet = History.DnaHistFlushUpdateInsertValues(historyService, tag, out errorString);
            if (!String.IsNullOrEmpty(errorString)) progressString.AppendLine(String.Format("ERROR- {0}"));
            progressString.Append(String.Format("Values for {0} appended in {1} seconds", tag, (DateTime.Now - startWrite).TotalSeconds.ToString()));
            return progressString;
        }      
        public static StringBuilder PushTagAppend(string service, Tuple<string,List<PushValue>> tagAndValues, int roundDec = 6, ushort status = 3)
        {
            string historyService = String.Empty;
            string tag = tagAndValues.Item1;
            History.DnaHistResolveHistoryName(tag, out historyService);
            //Initialization, these lines can be skipped
            int nRet = 0;
            string errorString = String.Empty;
            var progressString = new StringBuilder();
            var startWrite = DateTime.Now;
            //First, we are going to add all the points to the queue
            foreach(PushValue pv in tagAndValues.Item2)
            {
                nRet = History.DnaHistQueueAppendValue(historyService, tag, pv.UTCTime, status, Math.Round(pv.Value,roundDec).ToString(), out errorString);
                if (!String.IsNullOrEmpty(errorString)) progressString.AppendLine(String.Format("ERROR- {0}"));
            }
            //eDNA requires the queue to be flushed after all the points have been added to the queue
            nRet = History.DnaHistFlushAppendValues(historyService, tag, out errorString);
            if (!String.IsNullOrEmpty(errorString)) progressString.AppendLine(String.Format("ERROR- {0}"));
            progressString.Append(String.Format("Values for {0} inserted in {1} seconds", tag, (DateTime.Now - startWrite).TotalSeconds.ToString()));
            return progressString;
        }       
        public static StringBuilder PushTagsRealtime(string service, List<Tuple<string,List<PushValue>>> listTagsAndValues, 
            int roundDec = 6, short status = 3, int sleepmSec = 100)
        {
            //Initialization
            int nRet = 0;
            uint uiKey1 = 0;
            var progressString = new StringBuilder();
            string cacheFilename = "cache_" + service;           
            //Grab the defined "_INPADDR" point
            string inpAddr = String.Join(".", service, "_INPADDR");
            string description = MiscMethods.GetPointDescription(inpAddr);
            string ipAddress = description.Split(',')[0];
            ushort port = (ushort)Convert.ToInt32(description.Split(',')[1]);
            //If the "_INPADDR" point doesn't exist, this mode cannot be performed
            if (!String.IsNullOrEmpty(description))
            {
                nRet = LinkMX.eDnaMxUniversalInitialize(out uiKey1, true, true, true, (int)50000, cacheFilename, "C:\\ProgramData\\InStep\\");
                progressString.AppendLine(String.Format("Initializing connection, status= {0}",nRet));
                nRet = LinkMX.eDnaMxUniversalDataConnect(uiKey1, ipAddress, port, "", (ushort)0);
                progressString.AppendLine(String.Format("Connecting, results= {0}",nRet));
                foreach (Tuple<string,List<PushValue>> tagAndValue in listTagsAndValues)
                {
                    DateTime startWrite = DateTime.Now;
                    string tag = tagAndValue.Item1;
                    foreach(PushValue pv in tagAndValue.Item2)
                    {
                        string tagSite, tagService, tagID;
                        Configuration.SplitPointName(tag, out tagSite, out tagService, out tagID);
                        nRet = LinkMX.eDnaMxAddRec(uiKey1, tagID, pv.UTCTime, (ushort) 0, status, Math.Round(pv.Value, roundDec));
                        Thread.Sleep(sleepmSec);
                    }
                    nRet = LinkMX.eDnaMxFlushUniversalRecord(uiKey1, (int)1);
                    Thread.Sleep(sleepmSec);
                    string timeElapsed = (DateTime.Now - startWrite).TotalSeconds.ToString();
                    progressString.Append(String.Format("Values for {0} inserted in {1} seconds", tag, (DateTime.Now - startWrite).TotalSeconds.ToString()));
                }
                LinkMX.eDnaMxUniversalCloseSocket(uiKey1);
                LinkMX.eDnaMxDeleteCacheFiles(uiKey1);
            }
            else { progressString.AppendLine(String.Format("ERROR- Service {0} does not have an '_INPADDR' point defined.", service)); }
            return progressString;
        }
    }
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
        public double Value { get; private set; }
        /// <summary>This data structure is used to store an eDNA value which will be pushed.</summary>
        /// <param name="utcTime">The DateTime (in UTC format) to push the eDNA value.</param>
        /// <param name="value">The value to be pushed.</param>
        public PushValue(int utcTime, double value)
            : this()
        {
            this.Value = value;
            this.UTCTime = utcTime;
        }
    }
    /// <summary>A class that contains method which are used to create simulated data.</summary>
    public static class SimulationMethods
    {
        public const double TimeOffset = -5.0;
        private static double[] FindTimes(TimeSpan period, DateTime startDate, DateTime endDate, out int totalPoints)
        {
            //Setup- convert the dates to ints, find the total number of expected points
            int intStartDate = Utility.GetUTCTime(startDate.AddHours(TimeOffset));
            int intEndDate = Utility.GetUTCTime(endDate.AddHours(TimeOffset));
            int intPeriod = Convert.ToInt32(period.TotalSeconds);
            totalPoints = (int)Math.Floor(((decimal)intEndDate - (decimal)intStartDate) / (decimal)intPeriod);
            //Generate the times and values
            double[] times = Generate.LinearSpaced(totalPoints, intStartDate, intEndDate);
            //Return the results
            return times;
        }
        private static List<PushValue> ConstructPushValues(double[] times, double[] values)
        {
            var pushList = new List<PushValue>();
            for (int ii = 0; ii < times.Length; ii++) pushList.Add(new PushValue(Convert.ToInt32(times[ii]), values[ii]));
            return pushList;
        }
        /// <summary>This input setting is useful when pushing one data point at a time. It would typically be used to correct 
        /// erroneous values, not for simulation purposes.</summary>
        /// <param name="time">Date/Time (in correct format)</param>
        /// <param name="value">Value to be pushed</param>
        public static List<PushValue> SimulateRaw(DateTime time, double value)
        {
            //Need to adjust from local time to UTC time
            int newTime = Utility.GetUTCTime(time.AddHours(TimeOffset));
            var list = new List<PushValue>();
            list.Add(new PushValue(newTime, value));
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
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.LinearSpaced(totalPoints,startVal,stopVal);
            return ConstructPushValues(times, values);
        }
        /// <summary>A uniform random distribution generates values uniformly along the interval [low value, high value].</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="lowVal">Low value</param>
        /// <param name="highVal">High value</param>
        public static List<PushValue> SimulateRand(TimeSpan period, DateTime startDate, DateTime endDate, double lowVal, double highVal)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            var randDist = new MathNet.Numerics.Distributions.ContinuousUniform(lowVal,highVal);
            double[] values = Generate.Random(totalPoints, randDist);
            return ConstructPushValues(times, values);
        }
        /// <summary>A normal random distribution generates values using a mean and standard deviation.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="mean">Mean</param>
        /// <param name="std">Standard deviation</param>
        public static List<PushValue> SimulateRandn(TimeSpan period, DateTime startDate, DateTime endDate, double mean, double std)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            var randDist = new MathNet.Numerics.Distributions.Normal(mean, std);
            double[] values = Generate.Random(totalPoints, randDist);
            return ConstructPushValues(times, values);
        }
        /// <summary>An exponential random distribution generates values using a rate.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="rate">The rate for the exponential distribution.</param>
        public static List<PushValue> SimulateRande(TimeSpan period, DateTime startDate, DateTime endDate, double rate)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            var randDist = new MathNet.Numerics.Distributions.Exponential(rate);
            double[] values = Generate.Random(totalPoints, randDist);
            return ConstructPushValues(times, values);
        }
        /// <summary>Generates eDNA values to push using a sine function.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="frequency">The frequency of the sine wave, with units equal to the data update frequency selected.</param>
        /// <param name="amplitude">The amplitude of the sine wave (default value is 1).</param>
        /// <param name="mean">The mean of the sine wave (default value is 1).</param>
        /// <param name="phase">The phase of the sine wave (default value is 0).</param>
        public static List<PushValue> SimulateSine(TimeSpan period, DateTime startDate, DateTime endDate, double frequency, double amplitude = 1, double mean = 0, double phase = 0, int delay = 0)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.Sinusoidal(totalPoints, period.TotalSeconds, frequency, amplitude, mean, phase, delay);
            return ConstructPushValues(times, values);
        }
        /// <summary>Generates an impulse in the data relative to the start date.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="amplitude">The amplitude of the impulse.</param>
        /// <param name="delay">The time period after which the impulse occurs, relative to the starting date, with units equal to the data update frequency selected.</param>
        public static List<PushValue> SimulateImpulse(TimeSpan period, DateTime startDate, DateTime endDate, double amplitude, int delay)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.Impulse(totalPoints, amplitude, delay);
            return ConstructPushValues(times, values);
        }
        /// <summary>Generates a step function.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="amplitude">The amplitude of the step function.</param>
        /// <param name="delay">The amount of time after which the step occurs, with units equal to the data update frequency selected.</param>
        public static List<PushValue> SimulateStep(TimeSpan period, DateTime startDate, DateTime endDate, double amplitude, int delay)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.Step(totalPoints, amplitude, delay);
            return ConstructPushValues(times, values);
        }
        /// <summary>Generates a periodic function.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="frequency">The frequency of the periodic function.</param>
        /// <param name="amplitude">The amplitude of the periodic function.</param>
        /// <param name="phase">The phase of the periodic function.</param>
        /// <param name="delay">The delay of the periodic function.</param>
        public static List<PushValue> SimulatePeriodic(TimeSpan period, DateTime startDate, DateTime endDate, double frequency, double amplitude, double phase, int delay)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.Periodic(totalPoints, period.TotalSeconds, frequency, amplitude, phase, delay);
            return ConstructPushValues(times, values);
        }
        /// <summary>Generates a periodic impulse.</summary>
        /// <param name="period">Data Update Frequency</param>
        /// <param name="startDate">Starting Date/Time</param>
        /// <param name="endDate">Ending Date/Time</param>
        /// <param name="impulsePeriod">The period of the impulse, with units equal to the data update frequency selected.</param>
        /// <param name="amplitude">The amplitude of the impulse.</param>
        /// <param name="delay">The delay of the impulse, with units equal to the data update frequency selected.</param>
        public static List<PushValue> SimulatePeriodicImpulse(TimeSpan period, DateTime startDate, DateTime endDate, int impulsePeriod, double amplitude, int delay)
        {
            int totalPoints = 0;
            double[] times = SimulationMethods.FindTimes(period, startDate, endDate, out totalPoints);
            double[] values = Generate.PeriodicImpulse(totalPoints, impulsePeriod, amplitude, delay);
            return ConstructPushValues(times, values);
        }
    }
    /// <summary>Miscellaneous methods which are useful across all namespaces.</summary>
    public static class MiscMethods
    {
        /// <summary>Returns a description of an eDNA tag.</summary>
        /// <param name="fullTag">The fully-qualified (Site.Service.Point) eDNA tag.</param>
        public static string GetPointDescription(string fullTag)
        {
            double value;
            string valueString, statusString, description, units;
            DateTime time;
            ushort status;
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
            double value;
            string valueString, statusString, description, units;
            DateTime time;
            ushort status;
            RealTime.DNAGetRTFull(fullTag, out value, out valueString, out time, out status, out statusString, out description, out units);
            return new Tuple<DateTime, double>(time, value);
        }
    }
}
