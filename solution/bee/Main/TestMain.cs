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
            //.Add(SourceText.FromFile("test1.bee-source"));
            sourceList.Add(SourceText.FromFile("test2.bee-source"));
            //sourceList.Add(SourceText.FromFile("test3.bee-source"));
            //sourceList.Add(SourceText.FromFile("test4.bee-source"));
            //sourceList.Add(SourceText.FromFile("test5.bee-source"));

            Thread.Sleep(2500);

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            Registry registry = new Registry();
            registry.AddSourceList(sourceList);
            for (int i=0; i<10000; i++)
            {
                registry.UpdateSource(sourceList[0]);
            }
            
            stopWatch.Stop();

            Console.WriteLine("\nDone in: " + Utils.Format(stopWatch.Elapsed));
            Console.ReadLine();

            return 1;
        }
    }
}
