using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Convert_eDNA_To_CSV;
using Data_Flow_Analyzer;

namespace Tests
{
    [TestClass]
    public class DataFlowAnalyzerTests
    {
        //Overall, this section of unit tests is not that useful, since the testing files had data removed and are generally not complicated.
        [TestMethod]
        public void DFA_DataFile_AnalyzeCalcService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040008_23235.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040008_23235.dat,LCS03LC03CALC,2016/04/07 03:21:46,0.0004,5/12/2016 3:26:56 PM";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzeFTAService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040008_23236.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040008_23236.dat,LCS03LC03FTA,2016/04/07 03:27:32,0.0004,5/12/2016 3:27:16 PM";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzeMDCService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040011_23237.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040011_23237.dat,LCS03LC03MDC,2016/04/07 03:26:05,0.0004,5/12/2016 3:27:30 PM";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzeOPCService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040036_23238.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040036_23238.dat,LCS03LC03OPC,2016/04/07 03:20:00,0.0004,5/12/2016 3:27:42 PM";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzePRMAService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040037_23239.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040037_23239.dat,LCS03LC03PRMA,2016/04/07 03:24:39,0.0004,5/12/2016 3:27:54 PM";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzeRunService()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040038_23240.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040038_23240.dat,LCS03LC03RUN,2016/04/07 03:23:13,0.0004,5/12/2016 3:28:08 PM";
            Assert.AreEqual(testString, correctString);
        }
        //These tests are more useful, since the logic for the log files is a little complicated
        [TestMethod]
        public void DFA_DataFile_AnalyzeLogs_Output()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040038_23241.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-040038_23241.dat,Audit.log,,0.0002,5/12/2016 3:28:44 PM\r\n" +
                "PreMA-20160407-040038_23241.dat,System.log,,0.0002,5/12/2016 3:28:44 PM\r\n";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzeLogs_WriteLogs()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-040038_23241.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            df.AnalyzeArchive("LCS", testDir, true);
            string auditPath = Path.Combine(testDir, "Logs", "2016-05-12", "Audit.log");
            string sysPath = Path.Combine(testDir, "Logs", "2016-05-12", "System.log");
            Assert.IsTrue(File.Exists(auditPath));
            Assert.IsTrue(File.Exists(sysPath));
            //Now cleanup by deleting the files
            File.Delete(auditPath);
            File.Delete(sysPath);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzePreMALogs_Output()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-120035_23255.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            string testString = df.AnalyzeArchive("LCS", testDir, false).ToString();
            string correctString = "PreMA-20160407-120035_23255.dat,logs_APP20160407.7z,,0.0004,5/12/2016 3:30:36 PM\r\n";
            Assert.AreEqual(testString, correctString);
        }
        [TestMethod]
        public void DFA_DataFile_AnalyzePreMALogs_WriteLogs()
        {
            string testDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestFiles\\";
            string fileName = "PreMA-20160407-120035_23255.dat";
            string testPath = Path.Combine(testDir, fileName);
            var df = new DataFile(testPath);
            df.AnalyzeArchive("LCS", testDir, true);
            //Four files should have been extracting to the logs folder
            string applogPath = Path.Combine(testDir, "Logs", "2016-05-12", "applog.evtx");
            string syslogPath = Path.Combine(testDir, "Logs", "2016-05-12", "syslog.evtx");
            string premaStatusPath = Path.Combine(testDir, "Logs", "2016-05-12", "PreMAStatus.htm");
            string biglogPath = Path.Combine(testDir, "Logs", "2016-05-12", "logs_APP20160407.7z");
            Assert.IsTrue(File.Exists(applogPath));
            Assert.IsTrue(File.Exists(syslogPath));
            Assert.IsTrue(File.Exists(premaStatusPath));
            Assert.IsTrue(File.Exists(biglogPath));
            //Now cleanup by deleting the files
            File.Delete(applogPath);
            File.Delete(syslogPath);
            File.Delete(premaStatusPath);
        }
    }
    [TestClass]
    public class Convert_eDNA_To_CSV_Tests
    {
        [TestMethod]
        public void CETC_ConstructTagPull_GoodInput()
        {
            //This is testing the entire public method (which calls RetrievePointList and FilterEmptyPoints)
            string[] args = { "MDSS01.LC03OPC", "2011-01-01", "2012-01-01", "3" };
            var dpi = new DataPullInitialization(args);
            dpi.ConstructTagPull();
            //The point list for this service should always have some values and not be null
            Assert.IsNotNull(dpi.TagPullList);
            Assert.AreNotEqual(dpi.TagPullList.Count, 0);
        }
        //IMPORTANT NOTE
        //I would like to test the validate input methods here, but they loop until correct input is received. Solutions?
        [TestMethod]
        public void CETC_RetrievePointList_GoodInput()
        {
            string[] pointList = DataPullInitialization.RetrievePointListFromDNA("MDSS01.LC03OPC");
            //The point list for this service should always have some values and not be null
            Assert.IsNotNull(pointList);
            Assert.AreNotEqual(pointList.Length, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CETC_RetrievePointList_NonexistentService()
        {
            string[] pointList = DataPullInitialization.RetrievePointListFromDNA("UNDEFINED");
            //No points will be retrieved from the UNDEFINED service, so a NullReferenceException should 
            //be thrown, and the resulting list should have a length of 0
            Assert.AreEqual(pointList.Length, 0);
        }
        [TestMethod]
        public void CETC_FilterEmptyPoints_GoodInput()
        {
            string[] originalPointList = { "EXISTS", "YES", "", "OOPS THAT WAS EMPTY" };
            string[] testPoints = DataPullInitialization.FilterEmptyPoints(originalPointList);
            string[] correctPoints = { "EXISTS", "YES", "OOPS THAT WAS EMPTY" };
            //The empty string in the List should have been removed
            CollectionAssert.AreEqual(testPoints, correctPoints);
        }
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CETC_FilterEmptyPoints_NullInput()
        {
            string[] originalPointList = { "", "", "", "" };
            string[] testPoints = DataPullInitialization.FilterEmptyPoints(originalPointList);
            //All the strings in the original point list are empty, so they should all be removed
            //which throws a NullReferenceException and leads to a list length of 0
            Assert.AreEqual(testPoints.Length, 0);
        }
        [TestMethod]
        public void CETC_TagPull_ConstructorTesting()
        {
            //This is also covered under "TagPull_PathConstruction", but it tests the entire constructor,not just path construction
            //I may want to add additional properties later, so I checked it twice
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            var tp = new TagPull("MDSS01.LC03OPC.1S2SAVLW", outDir, new DateTime(2014, 11, 01), new DateTime(2014, 11, 02));
            string correctPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SAVLW_2014-11-01T000000_to_2014-11-02T000000.csv");
            Assert.AreEqual(tp.OutPath, correctPath);
        }
        [TestMethod]
        public void CETC_TagPull_PathConstruction()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string testOutPath = TagPull.CreateOutputPath(outDir, "MDSS01.LC03OPC.1S2SAVLW", new DateTime(2014, 11, 01), new DateTime(2014, 11, 02));
            string correctPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SAVLW_2014-11-01T000000_to_2014-11-02T000000.csv");
            Assert.AreEqual(testOutPath, correctPath);
        }
        [TestMethod]
        public void CETC_TagPull_GZipFile()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SAVLW_2014-11-01T000000_to_2014-11-02T000000.csv");
            //Create a dummy file for testing
            using (StreamWriter sw = new StreamWriter(outPath))
            {
                sw.WriteLine("THIS IS JUST FOR TESTING, PLEASE DELETE");
            }
            TagPull.GZipFile(outPath);
            //The file should be deleted and no longer exist once the GZipFile method is called
            Assert.IsFalse(File.Exists(outPath));
            //Check that the gzip file was created
            Assert.IsTrue(File.Exists(outPath + ".gz"));
            //Cleanup the unit test by deleting the file
            File.Delete(outPath + ".gz");
        }
        //These next few methods are the most important unit tests, because they relate to the data pull itself. Validation is difficult with eDNA
        //because of time zone differences and eccentricities with the eDNA API (sometimes points are pulled before the start time)
        //VERY IMPORTANT NOTE
        //Due to time zone changes, the dates are off by exactly five hours. For example, when I pull data from this program at midnight (00:00:00),
        //it will match data pulled from my eDNA Trend at 5 am (05:00:00)
        //Test1 is a test of the most basic functionality
        [TestMethod]
        public void CETC_TagPull_PullData1_Basics()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SAVLW_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.1S2SAVLW", sw, new DateTime(2015, 11, 11, 00, 00, 00), new DateTime(2015, 11, 11, 01, 00, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //I validated what the first 5 lines should look like by running eDNA Trend and running this program multiple times. Seems consistent. 
            //If the test fails for some reason, maybe try running it again, because the eDNA API does not always behave consistently.
            string[] correctOutput = {"1447218000,348.43997,OK",
                                    "1447218001,350.16998,OK",
                                    "1447218002,352.78,OK",
                                    "1447218003,345.66,OK",
                                    "1447218003,352.78,OK"};
            string[] testingArray = lineList.Take(5).ToArray();
            CollectionAssert.AreEqual(correctOutput, testingArray);
            //Finally, delete the testing file
            File.Delete(outPath);
        }
        //Test2 is mainly for testing time periods that don"t fall directly on the hour
        [TestMethod]
        public void CETC_TagPull_PullData2_NonHourTimePeriods()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.DG03SI01_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.DG03SI01", sw, new DateTime(2014, 12, 10, 03, 05, 00), new DateTime(2014, 12, 10, 04, 10, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //I validated what the first 5 lines should look like by running eDNA Trend and running this program multiple times. Seems consistent. 
            //If the test fails for some reason, maybe try running it again, because the eDNA API does not always behave consistently.
            string[] correctOutput = {"1418198700,1806,OK",
                                    "1418198702,1797,OK",
                                    "1418198704,1800,OK",
                                    "1418198705,1801,OK",
                                    "1418198707,1804,OK"};
            string[] testingArray = lineList.Take(5).ToArray();
            CollectionAssert.AreEqual(correctOutput, testingArray);
            //Finally, delete the testing file
            File.Delete(outPath);
        }
        //Test3 is for a tag that has a signficant number of unreliable points. It's also digital.
        [TestMethod]
        public void CETC_TagPull_PullData3_UnreliablePoints()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SCB1_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.1S2SCB1", sw, new DateTime(2015, 04, 01, 00, 00, 00), new DateTime(2015, 06, 01, 00, 00, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //I validated what the first 10 lines should look like by running eDNA Trend and running this program multiple times. Seems consistent. 
            //If the test fails for some reason, maybe try running it again, because the eDNA API does not always behave consistently.
            string[] correctOutput = {"1427860800,3,OK",
                                    "1427957402,3,OK",
                                    "1428728170,3,OK",
                                    "1428973644,3,UNRELIABLE",
                                    "1429081434,1,OK",
                                    "1429162272,1,UNRELIABLE",
                                    "1429162508,1,OK",
                                    "1429162965,1,UNRELIABLE",
                                    "1429163369,1,OK",
                                    "1429165423,1,UNRELIABLE"};
            string[] testingArray = lineList.Take(10).ToArray();
            CollectionAssert.AreEqual(correctOutput, testingArray);
            //Finally, delete the testing file
            File.Delete(outPath);
        }
        //Test4 is for a tag that does not exist. Nothing should be returned from the tag pull.
        [TestMethod]
        public void CETC_TagPull_PullData4_UndefTag()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.UNDEF_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.UNDEF", sw, new DateTime(2015, 04, 01, 00, 00, 00), new DateTime(2015, 06, 01, 00, 00, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //Since nothing was returned from the undefined point, the list should be empty
            Assert.AreEqual(lineList.Count, 0);
            //Finally, delete the testing file
            File.Delete(outPath);
        }
        //Test5 is testing that the number of points returned from a data pull is consistent.
        [TestMethod]
        public void CETC_TagPull_PullData5_NumPoints()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.SG01TI01_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.SG01TI01", sw, new DateTime(2015, 08, 01, 00, 00, 00), new DateTime(2015, 08, 01, 01, 00, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //I tested this multiple ways, and 263 points should be returned (remember, I'm writing the first value as SNAP, so that adds an extra data
            //point compared to eDNA Trend)
            Assert.AreEqual(lineList.Count, 263);
            //Finally, delete the testing file
            File.Delete(outPath);
        }
        //Test6 is testing that the final 5 points match
        [TestMethod]
        public void CETC_TagPull_PullData6_FinalPoints()
        {
            string outDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\TestData\\";
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "MDSS01.LC03OPC.1S2SAVLW_TESTING.csv");
            //Create a dummy file for testing
            using (var sw = new StreamWriter(outPath))
            {
                TagPull.PullDNAData("MDSS01.LC03OPC.1S2SAVLW", sw, new DateTime(2015, 11, 11, 00, 00, 00), new DateTime(2015, 11, 11, 01, 00, 00));
            }
            //Read from the file
            var lineList = new List<string>();
            using (var sr = new StreamReader(outPath))
            {
                while (!sr.EndOfStream) lineList.Add(sr.ReadLine());
            }
            //I validated what the first 5 lines should look like by running eDNA Trend and running this program multiple times. Seems consistent. 
            //If the test fails for some reason, maybe try running it again, because the eDNA API does not always behave consistently.
            string[] correctOutput = {"1447221596,330.56,OK",
                                    "1447221597,324.83002,OK",
                                    "1447221598,327.94998,OK",
                                    "1447221598,324.83002,OK",
                                    "1447221599,325.34998,OK"};
            string[] testingArray = lineList.Skip(Math.Max(0, lineList.Count() - 5)).ToArray();
            CollectionAssert.AreEqual(correctOutput, testingArray);
            //Finally, delete the testing file
            File.Delete(outPath);
        }

    }
}
