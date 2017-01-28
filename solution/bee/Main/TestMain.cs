using Bee.Language;
using System;
using System.Diagnostics;
using System.Threading;

namespace Bee
{
    public class MainTest
    {
        public static int Main(string[] args)
        {
            SourceList sourceList = new SourceList();
            sourceList.Add(SourceText.FromFile("test1.bee-source"));
            sourceList.Add(SourceText.FromFile("test2.bee-source"));
            sourceList.Add(SourceText.FromFile("test3.bee-source"));
            sourceList.Add(SourceText.FromFile("test4.bee-source"));
            sourceList.Add(SourceText.FromFile("test5.bee-source"));

            Thread.Sleep(2000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i=0; i<200; i++)
            {
                Registry registry = new Registry();
                registry.AddSourceList(sourceList);
            }
            
            stopWatch.Stop();

            Console.WriteLine("\nDone in: " + Utils.Format(stopWatch.Elapsed));
            Console.ReadLine();

            return 1;
        }
    }
}
