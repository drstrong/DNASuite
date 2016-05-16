using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Convert_eDNA_To_CSV;
using InStep.eDNA.EzDNAApiNet;

namespace Convert_eDNA_To_CSV
{
    public class DataPullInitialization
    {
        public List<TagPull> TagPullList { get; private set; }
        public string DNAService { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int BatchingInterval { get; private set; }
        public string OutputDirectory { get; private set; }
        public DataPullInitialization(string[] args)
        {
            this.ValidateArguments(args);    
        }
        //No unit testing done on the "Validate" functions- they will loop until proper input is received
        internal void ValidateArguments(string[] args)
        {
            //This part is a little tricky. It's possible that args contain absolutely nothing, or it may contain string input that can't
            //be correctly converted to the data type that we want. So we need to validate all possible input, and fill in defaults if necessary.
            this.TagPullList = new List<TagPull>();
            this.DNAService = (args.Length < 1) ? ValidateDNAService("") : ValidateDNAService(args[0]);
            this.StartDate = (args.Length < 2) ? ValidateDateTime("", "Start Date") : ValidateDateTime(args[1], "Start Date");
            this.EndDate = (args.Length < 3) ? ValidateDateTime("", "End Date") : ValidateDateTime(args[2], "End Date");
            if (this.StartDate > this.EndDate) throw new OverflowException("ERROR- StartDate is greater than EndDate.");
            this.BatchingInterval = (args.Length < 4) ? ValidateBatchingInterval("") : ValidateBatchingInterval(args[3]);
            this.OutputDirectory = (args.Length < 5) ? ValidatePath("") : ValidatePath(args[4]);
        }
        internal string ValidateDNAService(string testService)
        {
            string[] svcNames, svcDesc, svcType, svcStatus;
            Configuration.DnaGetServiceList(ushort.MaxValue, "SSERVER", "", out svcNames, out svcDesc, out svcType, out svcStatus);
            if (String.IsNullOrWhiteSpace(testService))
            {
                Console.WriteLine("eDNA Service: ");
                testService = Console.ReadLine();
            }
            while (!svcNames.Contains(testService))
            {
                Console.WriteLine(String.Format("ERROR- eDNA Service {0} was not found. Make sure you are connected to the right IP address, and check your spelling.",
                    testService));
                Console.WriteLine("eDNA Service: ");
                testService = Console.ReadLine();
            }
            return testService;
        }
        internal DateTime ValidateDateTime(string testDate, string userPrompt)
        {
            DateTime retDate = DateTime.MinValue;
            if (String.IsNullOrWhiteSpace(testDate))
            {
                Console.WriteLine(userPrompt + ": ");
                testDate = Console.ReadLine();
            }
            while (!DateTime.TryParse(testDate, out retDate))
            {
                Console.WriteLine(String.Format("ERROR- Could not convert {0} to DateTime. Use proper format (e.g. '2011-01-01'). Try again.",
                    userPrompt));
                Console.WriteLine(userPrompt + ": ");
                testDate = Console.ReadLine();
            }
            return retDate;
        }
        internal int ValidateBatchingInterval(string testInterval)
        {
            int retInterval = 1;
            if (String.IsNullOrWhiteSpace(testInterval))
            {
                Console.WriteLine("Batch Interval (months): ");
                testInterval = Console.ReadLine();
            }
            while (!int.TryParse(testInterval, out retInterval))
            {
                Console.WriteLine(String.Format("ERROR- Could not convert {0} to an integer. Use proper format (e.g. '3'). Try again.", testInterval));
                Console.WriteLine("Batch Interval (months): ");
                testInterval = Console.ReadLine();
            }
            return retInterval;
        }
        internal string ValidatePath(string testPath)
        {
            if (String.IsNullOrWhiteSpace(testPath)) testPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\Data\\";
            try { Directory.CreateDirectory(testPath); }
            catch (IOException) { throw new Exception("ERROR in directory name (parameter 4). The path is already defined as a file. Try another name."); }
            catch (UnauthorizedAccessException) { throw new Exception("ERROR in directory name (parameter 4). Access to directory is not authorized."); }
            catch (ArgumentException) { throw new Exception("ERROR in directory name (parameter 4).  Path is null or contains invalid characters."); }
            return testPath;
        }
        //Constructing and reading the tag list
        public void ConstructTagPull()
        {
            //This next bit of code just constructs a list of objects from the TagPull class- we will be iterating through it using a Parallel ForEach
            //later in the program
            foreach (string tagName in RetrievePointListFromDNA(this.DNAService))
            {
                //So why am I creating "batches" using the monthly batching interval? A few reasons. eDNA doesn't do well with long-term data pulls, often returning
                //an error if the pull goes on long enough. Also, using a batching interval means that more pulls can potentially be going on simultaneously in the
                //Parallel.ForEach. The downside is that the files will usually need to be concatenated after this program this complete. However, since I'm usually
                //just creating a database from these CSV files, it's really not that bad.
                for (DateTime curDate = this.StartDate; curDate < this.EndDate; curDate = curDate.AddMonths(this.BatchingInterval))
                {
                    DateTime curEndDate = (curDate.AddMonths(this.BatchingInterval) > this.EndDate) ? this.EndDate : curDate.AddMonths(this.BatchingInterval);
                    this.TagPullList.Add(new TagPull(tagName, this.OutputDirectory, curDate, curEndDate));
                }
            }
        }
        internal static string[] RetrievePointListFromDNA(string eDNAService)
        {
            //I'm only interested in the pointIDs; is there a way to just return null?
            string[] pointIDs, pointTime, pointStatus, pointDesc, pointUnits;
            double[] pointValues;
            int nRet = Configuration.DnaGetPointListEx(ushort.MaxValue, eDNAService, 0, out pointIDs, out pointValues, out pointTime, out pointStatus, out pointDesc, out pointUnits);
            //If no points are returned, stop the program. This usually only occurs when the user isn't connected to the right IP address or the service name is misspelled
            if (pointIDs == null) throw new NullReferenceException("ERROR- No points retrieved. Check that you are connected to the right IP address and check your spelling.");
            string[] newPointIDs = FilterEmptyPoints(pointIDs);
            Console.WriteLine(String.Format("Retrieved {0} points from {1}", newPointIDs.Length, eDNAService));
            return newPointIDs;
        }
        internal static string[] FilterEmptyPoints(string[] pointIDs)
        {
            //In this next line, I'm filtering out the elements that are null or whitespace. Got to be a better way to do this, eDNA API sucks, applicable methods don't work, etc.
            string[] newPointIDs = pointIDs.ToList().Where(f => !String.IsNullOrWhiteSpace(f)).ToArray();
            //Check that filtering out the empty points didn't reduce the size of the list to zero.
            if (newPointIDs.Length == 0) throw new NullReferenceException("ERROR- All points are empty strings. Check that you are connected to the right IP address and check your spelling.");
            return newPointIDs;
        }
    }
    public class TagPull
    {
        public string DNATag { get; private set; }
        public string OutPath { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TagPull(string eDNATag, string outDir, DateTime startDate, DateTime endDate)
        {
            this.DNATag = eDNATag;               
            this.StartDate = startDate;
            this.EndDate = endDate;
            //We're going to construct the entire output path from the start, since we already know all the necessary information supplied to the constructor
            //It is very important that the strings be formatted in a way that they can be used in the filename
            this.OutPath = CreateOutputPath(outDir, eDNATag, startDate, endDate);
        }
        public static string CreateOutputPath(string outDir, string eDNATag, DateTime startDate, DateTime endDate)
        {
            return Path.Combine(outDir,
                String.Join("_", eDNATag, startDate.ToString("yyyy-MM-ddTHHmmss"), "to", endDate.ToString("yyyy-MM-ddTHHmmss")) + ".csv");
        }
        public void PullWriteZip(bool zipFile)
        {          
            var startPullTime = DateTime.Now;
            Console.WriteLine(String.Format("Starting data pull for {0} between {1} and {2}...",
                this.DNATag, this.StartDate.ToString(), this.EndDate.ToString()));
            //The OutPath directory should always exist, because it is either the current executing directory, or it was created earlier in the code
            using (StreamWriter outfile = new StreamWriter(this.OutPath))
            {
                //Why do I supply all the parameters to a function internal to the class? Mainly for unit testing, I don't want to assume that the 
                //user MUST call the properties stored in the class. I want more generic
                PullDNAData(this.DNATag, outfile, this.StartDate, this.EndDate);
            }
            //This will call a function to gzip the file and remove the original file. These CSV files get really large, believe me, this is necessary
            if (zipFile) { GZipFile(this.OutPath); }
            Console.WriteLine("Finished data pull for {0} in {1} seconds.", this.DNATag, (DateTime.Now - startPullTime).TotalSeconds);
        }
        internal static void ConstructFirstDataPoint(string eDNATag, DateTime startDate, DateTime endDate, int dtTime, double dValue, string strStatus,
            out int retTime, out double retValue, out string retStatus)
        {
            //I initialize the values to be returned using the results from the "RAW" pull- this is the default. These values will only change if the "SNAP"
            //does not match the "RAW"
            retTime = dtTime;
            retValue = dValue;
            retStatus = strStatus;
            //These next few lines are just initialization for the required eDNA "out" parameters
            uint uiKey = 0;
            double dValueSnap = 0;
            int dtTimeSnap = 0;
            string strStatusSnap = "";
            //Pull the first value using "SNAP"- we want to ensure that a value is written for the very first time queried. However, this leads to problems
            //when the first value pulled using "RAW" is for the same time as we just pulled for "SNAP", which is what we will check during the next step
            int result = History.DnaGetHistSnapUTC(eDNATag, Utility.GetUTCTime(startDate), Utility.GetUTCTime(endDate), 1, out uiKey);
            result = History.DnaGetNextHistUTC(uiKey, out dValueSnap, out dtTimeSnap, out strStatusSnap);
            History.DNACancelHistRequest(uiKey);
            //If the point pulled using "SNAP" is earlier than the first data point pulled using "RAW", then use it. Otherwise just use the first point pulled using "RAW".
            //***IMPORTANT NOTE- This will lead to some unexpected behavior in two situations: 
            //Situation A- The very beginning of the eDNA database, with no records beforehand- this will create a duplicated point with two different timestamps.
            //I personally don't think that's a big deal- querying from the beginning of the database is only done once, and it's one small point in a sea of points.
            //Situation B- A data gap occurs DIRECTLY before start query time. Again, this will be a rare situation, and I don't think it will be a big deal.
            //***IMPORTANT NOTE 2- The second check is to ensure that the "RAW" first point was not pulled before the starting date. This sometimes happens (who knows
            //why the eDNA API doesn't check for that????) so I need to check for it.
            if (dtTimeSnap < dtTime || dtTime < Utility.GetUTCTime(startDate)) 
            {
                retTime = dtTimeSnap;
                retValue = dValueSnap;
                retStatus = strStatusSnap;
            }
        }
        internal static void PullDNAData(string eDNATag, StreamWriter outfile, DateTime startDate, DateTime endDate)
        {
            //IMPORTANT- TO CONVERT TO PACIFIC TIME. Our data is all stored in Pacific time. Yes, this should ABSOLUTELY
            //be a configurable setting. However, it's standard across company projects. I think at this point it will
            //just add to the confusion if I make it configurable.
            startDate = startDate.AddHours(-5.0);
            endDate = endDate.AddHours(-5.0);

            //These next few lines are just initialization for the required eDNA "out" parameters
            uint uiKey = 0;
            double dValue = 0;
            int dtTime = 0;
            string strStatus = "";
            //eDNA API performs threading inside, which is why I can wrap this in another threading layer (it takes time for the eDNA API to send
            //a network request, retrieve data back, etc.
            int result = History.DnaGetHistRawUTC(eDNATag, Utility.GetUTCTime(startDate), Utility.GetUTCTime(endDate), out uiKey);
            //The first point is special, we're going to handle it differently
            result = History.DnaGetNextHistUTC(uiKey, out dValue, out dtTime, out strStatus);  
            ConstructFirstDataPoint(eDNATag, startDate, endDate, dtTime, dValue, strStatus, out dtTime, out dValue, out strStatus);     
            //Any result other than 0 is "bad" or means that the data pull has ended. Eventually I should check for all options explicitly, maybe
            //allow a "re-start" of failed pulls?
            while (result == 0)
            {
                //Okay this is tricky. Why do I write the line before getting the result? Because the last function call 
                outfile.WriteLine(String.Join(",", dtTime.ToString(), dValue.ToString(), strStatus));
                //I want the function that retrieves the UTC time because that's exactly what I want to write. No need to waste time converting to UTC.
                result = History.DnaGetNextHistUTC(uiKey, out dValue, out dtTime, out strStatus);           
            }           
            //Success! It's important to cancel the histrequest, because it could cause slowdown on the server side checking to see if the requests are really closed
            History.DNACancelHistRequest(uiKey);         
        }
        internal static void GZipFile(string outPath)
        {
            //These next few lines were basically just copied from MDSN- look here for help:
            //https://msdn.microsoft.com/en-us/library/ms404280%28v=vs.110%29.aspx
            var fileToCompress = new FileInfo(outPath);
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) &
                   FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
            //We don't need the original file any longer, it just takes up space
            File.Delete(outPath);
        }
    }
}
