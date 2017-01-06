using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Be
{
    public static class Utils
    {
        public static string FormatMilliseconds(double milliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",  t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
        }

        public static string EnumToString(Enum enum_instance)
        {
            return enum_instance.ToString().ToLower();
        }

        public static int LogBranchCount = 0;
        public static int LogItemCount = 0;
        public static void LogBranch(string branch)
        {
            Console.WriteLine("branch | " + ((10*1000) + (++LogBranchCount)) + " | " + branch);
        }
        public static void LogItem(string item)
        {
            Console.WriteLine("item | " + ((1000*1000) + (++LogItemCount)) + " | " + item);
        }

        public static void PrintSourceTreeStatistics(string ProjectDirectory)
        {
            string[] files = Directory.GetFiles(ProjectDirectory, "*.cs", SearchOption.AllDirectories);
            int byteCount = 0;
            int lineCount = 0;
            int objectCount = 0;
            int blockCount = 0;
            int statementCount = 0;
            int commentCount = 0;
            foreach (string file in files)
            {
                byteCount += (int)new FileInfo(file).Length;
                string[] lineArray = File.ReadAllLines(file);
                for (int i = 0; i < lineArray.Length; i++)
                {
                    string line = lineArray[i].Trim();
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    lineCount++;
                    if (line.Contains(" class ") || line.Contains(" enum "))
                    {
                        objectCount++;
                    }
                    if (line.Contains("if") || line.Contains("else if") || line.Contains("else") || line.Contains("for") || line.Contains("foreach") || line.Contains("do") || line.Contains("while") || line.Contains("switch") || line.Contains("case"))
                    {
                        blockCount++;
                    }
                    if (line.Contains(";"))
                    {
                        statementCount++;
                    }
                    if(line.Contains("//"))
                    {
                        commentCount++;
                    }
                }
            }
            Utils.LogBranch("Project-Size: " + (byteCount / 1024) + " KBytes | Source-Files: " + files.Length + " | Line-Count: " + lineCount + " | Class-Objects: " + objectCount + " | Control-Blocks: "+ blockCount + " | Code-Statements: "+ statementCount + " | Comments: "+commentCount);
        }
    }
}
