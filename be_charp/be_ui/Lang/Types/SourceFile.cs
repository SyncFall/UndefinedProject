using Be.Runtime.Parse;
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

        public UsingCollection Usings = new UsingCollection();
        public NamespaceCollection Namespaces = new NamespaceCollection();

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

        public void Set(string Source)
        {
            this.Source = Source;
        }

        public void Parse()
        {
            isParsed = false;
            if (string.IsNullOrEmpty(this.Source))
            {
                throw new Exception("no source-code available");
            }
            SourceParser sourceParser = new SourceParser(this);
            sourceParser.ParseSource();
            isParsed = true;
        }
    }
}
