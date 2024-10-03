using System;
using InazumaDSEditor.Tools;

namespace inzDS.Formats.Archive
{
    public class PKH
    {
        public string Name = "PKH";
        public string Extension = ".pkh";
        public string FileType = "ArchiveTable";
        public DataReader Data;
        public int FilesCount;
        public PKH(string _Path)
        {
            Data = new DataReader(_Path);
            FilesCount = Convert.ToInt32(Data.Read(22, 2, true), 16);
        }
    }
}
