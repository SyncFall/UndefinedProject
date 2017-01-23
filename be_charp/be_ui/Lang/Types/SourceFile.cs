using Bee.Library;
using System;
using System.IO;

namespace Be.Runtime.Types
{
    public class SourceFileList : ListCollection<SourceFile>
    {
        public void AddDirectory(string SourceDirectory)
        {
            string[] files = Directory.GetFiles(SourceDirectory, "*." + BeConst.SOURCE_FILE_EXTENSION, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                SourceFile sourceType = new SourceFile();
                sourceType.LoadFile(files[i]);
                this.Add(sourceType);
            }
        }
    }

    public class SourceFile
    {
        public string Source;
        public string Filepath;

        public bool isParsed;

        public static SourceFile FromText(string text)
        {
            SourceFile sourceFile = new SourceFile();
            sourceFile.SetText(text);
            return sourceFile;
        }

        public SourceFile()
        {}

        public void LoadFile(string SourceFilepath)
        {
            if (!File.Exists(SourceFilepath))
            {
                throw new Exception("source-file not exist");
            }
            this.Filepath = SourceFilepath;
            this.Source = File.ReadAllText(this.Filepath).Replace("\r", "");
        }

        public SourceFile SetText(string Source)
        {
            this.Source = Source;
            return this;
        }
    }
}
