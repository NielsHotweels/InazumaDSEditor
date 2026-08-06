using System;
using System.IO;
using ScriptInazumaElevenDS.Tools;
using InazumaDSEditor.Tools;

namespace ScriptInazumaElevenDS.Level5.Archive.PKB
{
    public class PKB
    {
        public string Name => "PKB";

        public Stream BaseStream;

        public PKH.PKH PKHTable { get; set; }

        public VirtualDirectory Directory { get; set; }

        public PKB()
        {
            Directory = new VirtualDirectory("/");
        }

        public PKB(Stream stream, PKH.PKH pkh)
        {
            BaseStream = stream;
            Directory = Open(pkh);
        }
        public PKB(Stream stream, PKH.PKH pkh, int constSize)
        {
            BaseStream = stream;
            Directory = Open(pkh, constSize);
        }

        public PKB(byte[] fileByteArray, PKH.PKH pkh)
        {
            BaseStream = new MemoryStream(fileByteArray);
            Directory = Open(pkh);
        }

        public VirtualDirectory Open(PKH.PKH pkh)
        {
            PKHTable = pkh;
            VirtualDirectory folder = new VirtualDirectory("");

            for (int i = 0; i < PKHTable.Table.Count; i++)
            {
                BinaryDataReader tableReader = new BinaryDataReader(PKHTable.Table[i]);

                int index = tableReader.ReadValue<int>();
                int offset = tableReader.ReadValue<int>();
                int size = tableReader.ReadValue<int>();

                // swap the bytes of target
                //index = (int)((int)((index << 24) & 0xff000000) |
                    //((index << 8) & 0xff0000) |
                    //((index >> 8) & 0xff00) |
                    //((index >> 24) & 0xff));
                folder.AddFile(index.ToString(), new SubMemoryStream(BaseStream, offset, size));
            }
            return folder;
        }
        public VirtualDirectory Open(PKH.PKH pkh, int constSize)
        {
            PKHTable = pkh;
            VirtualDirectory folder = new VirtualDirectory("");

            for (int i = 0; i < PKHTable.Table.Count; i++)
            {
                BinaryDataReader tableReader = new BinaryDataReader(PKHTable.Table[i]);

                int index = tableReader.ReadValue<int>();
                int size = tableReader.ReadValue<int>();

                // swap the bytes of target
                //index = (int)((int)((index << 24) & 0xff000000) |
                //((index << 8) & 0xff0000) |
                //((index >> 8) & 0xff00) |
                //((index >> 24) & 0xff));
                folder.AddFile(index.ToString(), new SubMemoryStream(BaseStream, constSize * i, constSize));
            }
            return folder;
        }
    }
}
