using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using InazumaDSEditor.Tools;

namespace ScriptInazumaElevenDS.Level5.Archive.PKH
{
    public class PKH
    {
        public string Name => "PKH";

        public Stream BaseStream;

        public List<byte[]> Table = new List<byte[]>();

        public PKHSupport.Header Header;

        public PKH(Stream stream)
        {
            BaseStream = stream;
            Table = Open();
        }
        public PKH(Stream stream, int constSize)
        {
            BaseStream = stream;
            Table = Open(constSize);
        }

        public PKH(byte[] fileByteArray)
        {
            BaseStream = new MemoryStream(fileByteArray);
            Table = Open();
        }

        public List<byte[]> Open(int constSize = 0)
        {
            BinaryDataReader data = new BinaryDataReader(BaseStream);
            Header = data.ReadStruct<PKHSupport.Header>();

            data.Seek(0x30);

            
            int elementLength = (Header.Length - 0x30) / Header.FileCount;
            if(constSize != 0) { elementLength = constSize; }
            Console.WriteLine(Header.Length);

            return Enumerable.Range(0, Header.FileCount)
                              .Select(_ => data.GetSection(elementLength))
                              .ToList();
        }

        public void Close()
        {
            BaseStream?.Dispose();
            BaseStream = null;
            Table.Clear();
        }
    }
}
