﻿using Be;
using Be.Runtime;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bee.Test
{
    public class TestMain
    {
        public static int Main(string[] args)
        {

            SourceFile source = new SourceFile();
            source.LoadFile(@"D:\dev\UndefinedProject\be-output\test.be-src");

            Thread.Sleep(3000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TokenContainer tokenContainer = new TokenContainer();
            for(int i=0; i<10*1000; i++)
            {
                tokenContainer.SetSourceFile(source);
            }
            
            stopWatch.Stop();

            Console.WriteLine("\nDone in: " + Utils.FormatMilliseconds(stopWatch.ElapsedMilliseconds));
            Console.ReadLine();

            return 1;
        }
    }
}