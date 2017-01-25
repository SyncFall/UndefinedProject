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
            SourceText source = SourceText.FromFile(@"D:\dev\UndefinedProject\output\test.bee-source");

            Thread.Sleep(2000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TokenContainer tokenContainer = new TokenContainer();
            SignatureContainer signatureContainer = new SignatureContainer();
            for (int i=0; i<20*1000; i++)
            {
                tokenContainer.SetSource(source);
                signatureContainer.SetContainer(tokenContainer);
            }
            
            stopWatch.Stop();

            Console.WriteLine("\nDone in: " + Utils.Format(stopWatch.Elapsed));
            Console.ReadLine();

            return 1;
        }
    }
}
