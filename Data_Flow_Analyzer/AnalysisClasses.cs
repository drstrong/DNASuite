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
using SharpCompress;
using SharpCompress.Archive;
using SharpCompress.Archive.SevenZip;
using SharpCompress.Reader;
using SharpCompress.Common;
using SharpCompress.Compressor;
using SharpCompress.Compressor.BZip2;

namespace Data_Flow_Analyzer
{
    public class DataFlowAnalysis
    {
        public string OutputPath { get; private set; }
        public List<DataFile> DataFileList { get; private set; }
        public DataFlowAnalysis(string outDir, string fileName, ListBox.ObjectCollection fileList)
        {
            //Make sure the output directory exists
            Directory.CreateDirectory(outDir);
            this.OutputPath = Path.Combine(outDir, fileName);
            //Now, use the strings in the ListBox to create the data files to be analyzed
            this.DataFileList = new List<DataFile>();
            foreach (object ob in fileList) this.DataFileList.Add(new DataFile(Convert.ToString(ob)));
        }
        public int TotalDataFiles() { return DataFileList.Count; }
    }
    public class DataFile
    {
        public string Filename { get; private set; }
        public DataFile(string fileName)
        {
            this.Filename = fileName;
        }
        public StringBuilder AnalyzeArchive(string dataPrefix, string outDir, bool extractLogs)
        {
            //I need a stringBuilder here because the archive file might have multiple files within it, so I won't necessarily return just one line
            var csv = new StringBuilder();
            //This is not very readable, but I wanted to fit it all on a single line
            using (var reader = ReaderFactory.Open(new BZip2Stream(File.OpenRead(this.Filename), SharpCompress.Compressor.CompressionMode.Decompress)))
            {
                //I don't like using the try-catch loop here
                try
                {
                    while (reader.MoveToNextEntry())
                    {
                        IEntry curEntry = reader.Entry;
                        var fileInfo = new FileInfo(this.Filename);
                        //This part is 
                        string detectType = curEntry.Key.Remove(3);
                        //There are three options: a data file, a system.log file, and PreMA log files. 
                        if (detectType == dataPrefix)
                        {
                            csv.Append(AnalyzeDNAData(curEntry, fileInfo));
                        }
                        else if (detectType == "Aud" || detectType == "Sys")
                        {
                            csv.AppendLine(AnalyzeLogFile(curEntry, fileInfo, reader, outDir, extractLogs, false));
                        }
                        else if (detectType == "log")
                        {
                            csv.AppendLine(AnalyzeLogFile(curEntry, fileInfo, reader, outDir, extractLogs, true));
                        }
                        else 
                        { 
                            csv.AppendLine(curEntry.Key); 
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                    //Why does this exception keep being thrown 
                }
            }
            return csv;
        }
        //The following methods are used within the "AnalyzeArchive" method
        internal string AnalyzeDNAData(IEntry entry, FileInfo fileInfo)
        {
            //The date is in the filename, so we need to get it. This is a convoluted way. I find where "20" starts (since it will very likely be the start of the DateTime),
            //then I find the rest of the file string starting at that index and remove the extension. It will be formated below in the "return"
            int dateIndex = entry.Key.IndexOf("20");
            string dateString = Path.GetFileNameWithoutExtension(entry.Key.Substring(dateIndex));
            //This may hurt readability a little, but I didn't give the variables "local" names. The comments give more information about the output of this method
            return String.Join(",",
                fileInfo.Name, //This is the short name of the file, instead of the full path
                entry.Key.Remove(dateIndex), //The eDNA site and service (everything before the dateIndex)
                String.Format("{0:####/##/## ##:##:##}", Convert.ToInt64(dateString)), //This is the date of the data file, formatted from dateString
                Math.Round((double)fileInfo.Length / 1000000.0, 4).ToString(), //The file size in MB
                entry.LastModifiedTime.ToString() //The last modification date of the file
                );
        }
        internal string AnalyzeLogFile(IEntry entry, FileInfo fileInfo, IReader reader, string outDir, bool extractLogs, bool premaLog)
        {
            //If selected in user settings, extract the file to the output directory
            if (extractLogs)
            {
                //The default directory that the logs will be exported to is a folder called "Logs" in the specified output directory
                string formatDate = String.Format("{0:yyyy-MM-dd}", entry.LastModifiedTime.Value);
                string logDir = Path.Combine(outDir, "Logs", formatDate);
                Directory.CreateDirectory(logDir);
                string logPath = Path.Combine(logDir, entry.Key);
                //If the log file already exists, make sure it's deleted first. This is also because the "latest" log file for a day is the most updated one
                File.Delete(logPath);
                reader.WriteEntryToFile(logPath);
                //Special case if it's a PreMA log file, because it needs to be extracted again
                if (premaLog) WritePreMALog(logPath, logDir);
            }
            //This may hurt readability a little, but I didn't give the variables "local" names. The comments give more information about the output of this method
            return String.Join(",",
                fileInfo.Name, //This is the short name of the file, instead of the full path
                entry.Key, //Instead of site.service, this will be the name of the log file (e.g. type)
                String.Empty, //The eDNA service time doesn't apply here, so use an empty string
                Math.Round((double) fileInfo.Length / 1000000.0,4).ToString(), //The file size in MB
                entry.LastModifiedTime.ToString() //The last modification date of the file
                );
        }
        //This method is optionally used within the AnalyzeLogFile method
        internal void WritePreMALog(string logPath, string logDir)
        {
            //Since the PreMA logs are in an archive of their own, we need to extract again
            using (var szipreader = ArchiveFactory.Open(File.OpenRead(logPath)))
            {
                foreach (var innerentry in szipreader.Entries)
                {
                    string innerLogPath = logDir + @"\" + innerentry.Key;
                    //If the log file already exists, make sure it's deleted first. This is also because the "latest" log file for a day is the most updated one
                    File.Delete(innerLogPath);
                    innerentry.WriteToFile(innerLogPath);
                }
            }
        }
    }
}
