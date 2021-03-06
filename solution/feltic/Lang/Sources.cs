﻿using feltic.Library;
using System;
using System.IO;

namespace feltic.Language
{
    public class SourceList : ListCollection<SourceText>
    {
        public void AddDirectory(string SourceDirectory)
        {
            string[] files = Directory.GetFiles(SourceDirectory, "*." + Constants.SourceFileExtension, SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                SourceText source = SourceText.FromFile(files[i]);
                this.Add(source);
            }
        }
    }

    public class SourceText
    {
        public char[] CharArray;
        public string StringContent;

        public readonly string Filepath;
        
        private SourceText(string Filepath)
        {
            this.Filepath = Filepath;
        }

        public static SourceText FromFile(string Filepath)
        {
            if (!File.Exists(Filepath))
            {
                throw new Exception("source-filepath not exist");
            }
            SourceText sourceFile = new SourceText(Filepath);
            sourceFile.SetText(File.ReadAllText(Filepath));
            return sourceFile;
        }

        public void ToFile(string Filepath=null)
        {
            if(Filepath == null && this.Filepath == null)
            {
                throw new Exception("source-filepath can not null");
            }
            File.WriteAllText((Filepath != null ? Filepath : this.Filepath), new string(CharArray));
        }
        
        public SourceText SetText(string SourceText)
        {
            if(SourceText == null)
            {
                throw new Exception("source-text can not null");
            }
            this.StringContent = SourceText.Replace("\r", "");
            this.CharArray = StringContent.ToCharArray();
            return this;
        }

        public bool IsEqualFile(SourceText Compare)
        {
            return (Filepath == Compare.Filepath);
        }
    }
}
