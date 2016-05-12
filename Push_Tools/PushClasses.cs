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
        public void AddPushTag(List<PushTag> pushTagList) { this.PushTags.AddRange(pushTagList); }
        /// <summary>Adds a PushTag to the stored list inside the class.</summary>
        public void AddPushTag(PushTag pushTag) { this.PushTags.Add(pushTag); }
        /// <summary>Clears the list of PushTags.</summary>
        public void ClearPushTags() { this.PushTags.Clear(); }
        /// <summary>This method will push the values in this.TagValues to eDNA history using "RealTime" mode. For more information
        /// about "RealTime" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
        /// errors encountered during the data push. Warning- this method is very slow and is not recommended to be used. As the name
        /// suggests, pushing to real-time seems to work inconsistently unless you are pushing values to the actual current time, although
        /// you can push "RealTime" to any DateTime later than the current DateTime in the eDNA CVT.</summary>
        /// <param name="realTimeService">The real-time service to push the values to. Technically, this could easily be found by calling DNAMiscMethods.FindService. 
        /// However, the whole point of using "PushService" to push real-time values instead of calling each "PushTag" individually is 
        /// to save time with the initialization of the service connection. By forcing the user to declare explicitly which RealTime service
        /// they wish to write to, I believe that user error will be reduced.</param>
        public StringBuilder PushRealTimeAllTags(string realTimeService, int roundDec = 6, int sleepmSec = 100)
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
                progressString.AppendLine("Initializing connection status= " + nRet);
                //Connect to the service
                nRet = LinkMX.eDnaMxUniversalDataConnect(uiKey1, ipAddress, port, "", (ushort)0);
                progressString.AppendLine("Connection result= " + nRet);

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
                            Math.Round(valueWrite.Value,roundDec));
                        if (nRet != 0) { numErrors++; }
                        Thread.Sleep(sleepmSec);
                    }

                    //Flush the values
                    nRet = LinkMX.eDnaMxFlushUniversalRecord(uiKey1, (int)1);
                    Thread.Sleep(sleepmSec);

                    //Update the errorstring
                    string timeElapsed = (DateTime.Now - startWrite).TotalSeconds.ToString();
                    progressString.AppendLine("Values for " + currentPushTag.Tag + " pushed in " + timeElapsed + " seconds with " + numErrors.ToString() + " errors");
                }

                //Close the connection
                LinkMX.eDnaMxUniversalCloseSocket(uiKey1);

                //Delete the cache file
                LinkMX.eDnaMxDeleteCacheFiles(uiKey1);
            }
            else
            {
                progressString.AppendLine("-ERROR- Service " + realTimeService + " does not have an '_INPADDR' point defined.");
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
        public void AddTagValues(List<PushValue> pushValueList) { this.TagValues.AddRange(pushValueList); }
        /// <summary>Add a PushValue to the stored list, ready to be pushed.</summary>
        public void AddTagValues(PushValue pushValue) { this.TagValues.Add(pushValue); }
        /// <summary>Clears the stored list of values to push.</summary>
        public void ClearValues() { this.TagValues.Clear(); }
        /// <summary>Returns the total amount of values to be pushed.</summary>
        public int GetTotalValues() { return this.TagValues.Count; }
        /// <summary>This method will push the values in this.TagValues to eDNA history using "Insert" mode. For more information
        /// about "Insert" mode, please refer to eDNA documentation. The output of this method is a StringBuilder containing the 
        /// errors encountered during the data push.</summary>
        public StringBuilder PushInsertAllValues(int roundDec = 6)
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
                string value = Math.Round(valueWrite.Value, roundDec).ToString();
                //Try to append the point to history
                nRet = History.DnaHistQueueUpdateInsertValue(this.PushService, this.Tag, valueWrite.UTCTime, this.GlobalStatus,
                    value, out errorString);
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
        public StringBuilder PushAppendAllValues(int roundDec = 6)
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
                string value = Math.Round(valueWrite.Value, roundDec).ToString();
                nRet = History.DnaHistQueueAppendValue(this.PushService, this.Tag, valueWrite.UTCTime, this.GlobalStatus,
                    value, out errorString);

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
        public double Value { get; private set; }
        /// <summary>This data structure is used to store an eDNA value which will be pushed.</summary>
        /// <param name="utcTime">The DateTime (in UTC format) to push the eDNA value.</param>
        /// <param name="value">The value to be pushed.</param>
        public PushValue(DateTime utcTime, double value)
            : this()
        {
            this.Value = value;
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
