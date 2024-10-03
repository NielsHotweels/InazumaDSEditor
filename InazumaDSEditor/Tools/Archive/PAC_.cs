using System;
using System.Collections.Generic;
using System.Linq;
using InazumaDSEditor.Tools;

namespace inzDS.Formats.Archive
{
    public class PAC_
    {
        public string Name = "PAC_";
        public string Extension = ".pac_";
        public string FileType = "Archive";
        public DataReader Data;
        public int FilesCount;
        public Dictionary<int, FilePKB> Files = new Dictionary<int, FilePKB>();

        public PAC_(byte[] _Data)
        {
            Data = new DataReader(_Data);
            FilesCount = Convert.ToInt32(Data.Read(0, 4, true), 16);
            for (int i = 0; i < FilesCount; i++)
            {
                FilePKB file = new FilePKB(
                    null,
                    Convert.ToInt32(Data.Read(8 + 8 * i, 4, true), 16),
                    Data.Stream.Skip(Convert.ToInt32(Data.Read(4 + 8 * i, 4, true), 16)).Take(Convert.ToInt32(Data.Read(8 + 8 * i, 4, true), 16)).ToArray()
                    );
                Files.Add(i, file);
            }
        }
        public PAC_(string _Path)
        {
            Data = new DataReader(_Path);
            FilesCount = Convert.ToInt32(Data.Read(0, 4, true), 16);
            for (int i = 0; i < FilesCount; i++)
            {
                FilePKB file = new FilePKB(
                    null,
                    Convert.ToInt32(Data.Read(8 + 8 * i, 4, true), 16),
                    Data.Stream.Skip(Convert.ToInt32(Data.Read(4 + 8 * i, 4, true), 16)).Take(Convert.ToInt32(Data.Read(8 + 8 * i, 4, true), 16)).ToArray()
                    );
                Files.Add(Convert.ToInt32(Data.Read(4 + 8 * i, 4, true), 16), file);
            }
        }
    }
}
