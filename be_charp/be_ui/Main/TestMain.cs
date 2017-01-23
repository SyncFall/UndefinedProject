using Bee.Language;
using Bee.Runtime;
using System;
using System.Diagnostics;

namespace Bee
{
    public class MainTest
    {
        public static int Main(string[] args)
        {
            SourceText SourceText = SourceText.FromFile(@"D:\dev\UndefinedProject\be-output\test.bee-source");

            //Thread.Sleep(3000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TokenContainer tokenContainer = new TokenContainer();
            SignatureContainer signatureContainer = new SignatureContainer();
            for (int i=0; i<20*1000; i++)
            {
                tokenContainer.SetSource(SourceText);
                signatureContainer.SetContainer(tokenContainer);
            }
            
            stopWatch.Stop();

            Console.WriteLine("\nDone in: " + Utils.FormatMilliseconds(stopWatch.ElapsedMilliseconds));
            Console.ReadLine();

            return 1;
        }
    }
}
