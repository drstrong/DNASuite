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
    class Program
    {   
        static void Main(string[] args)
        {
            Console.WriteLine(String.Format("Program initialized at {0}", DateTime.Now.ToString()));
            //These next two settings are hard-coded, but I should add them as additional options. Why do I set the max threads to 250? It has nothing to do with RAM, 
            //but from personal experience eDNA doesn't handle requests greater than 250 simultaneously very well. Again, should be added as user option.
            History.SetHistoryTimeout(2400);
            ThreadPool.SetMaxThreads(250,250);
            //The parameters for the entire data pull are contained in the "DataPullInitialization" class, which also contains a list of tags to pull.
            //See the "ConversionClasses.cs" for more information.
            Console.WriteLine("Validating user-supplied arguments...");
            var dpi = new DataPullInitialization(args);
            //Now that we've validated the parameters, we need to retrieve a list of points from eDNA.
            Console.WriteLine(String.Format("Retrieving the point list from eDNA service {0}...", dpi.DNAService));
            dpi.ConstructTagPull();
            //I know this Parallel ForEach loop gives me less control over the threading, but what I'm trying to do is really simple.
            Console.WriteLine(String.Format("Preparing to convert eDNA service {0} from {1} to {2} and writing to {3}...", 
                dpi.DNAService, dpi.StartDate.ToString(), dpi.EndDate.ToString(),dpi.OutputDirectory));
            Parallel.ForEach(dpi.TagPullList, (tp, loopState) => { tp.PullWriteZip(true); });
            //Success!
            Console.WriteLine("Pull completed successfully. Program will exit after next key press.");
            Console.ReadKey();
        }
    } 
}
