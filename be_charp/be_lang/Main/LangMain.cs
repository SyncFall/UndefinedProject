using Be.Runtime.Types;
using Be.Runtime;
using System;
using System.Diagnostics;

namespace Be
{
    public class BeEntry
    {
        public static long LONG_COUNT = 1000*1000*75;
        public static long INDEX_COUNT = 0;
        public static Stopwatch StopWatch = new Stopwatch();

        [STAThread]
        public static int Main(string[] args)
        { 
            ObjectLoader loader = new ObjectLoader();

            Utils.PrintSourceTreeStatistics(@"..\..");
#if(TRACK)
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            SourceFileList source = new SourceFileList();
            source.AddDirectory(@"..\..\..\..\");
            try
            {
                loader.Add(source);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError:\n");
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return 0;
            }

            loader.StartProgramm();
            stopWatch.Stop();
            Console.WriteLine("\n");
            //Console.WriteLine("Performing " + LONG_COUNT.ToString("#,#", CultureInfo.InvariantCulture) + " logical instructions");
            Console.WriteLine("\nDone in: " + Utils.FormatMilliseconds(stopWatch.ElapsedMilliseconds));
            Console.ReadLine();

            
#endif
#if (SPEED)
            SourceFileList source = new SourceFileList();
            source.AddDirectory(@"D:\dev\UndefinedProject\");
            Stopwatch stopWatch = new Stopwatch();
            try
            {
                int runningCount = (100*1000);
                Console.WriteLine("Processing " + ((runningCount / 1000) + "").Replace(".", "") + ".000 Times an Collection of "+source.Size()+" Source-Files..");
                stopWatch.Start();
                SourceFile sourceType;
                for(int i=0; i<runningCount; i++){
                    sourceType = new SourceFile();
                    sourceType.Set(source.Get(0).Source);
                    loader.Add(sourceType);
                }
                stopWatch.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError:\n");
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                return 0;
            }

            //loader.StartProgramm();

            Console.WriteLine("\nDone in: "+Utils.FormatMilliseconds(stopWatch.ElapsedMilliseconds));
            Console.ReadLine();
#endif
            return 1;
        }
    }

}
