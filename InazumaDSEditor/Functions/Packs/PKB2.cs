using BitMiracle.LibTiff.Classic;
using InazumaDSEditor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileSystem;

namespace InazumaDSEditor.Functions.Packs
{
    public class PKB2
    {
        public Node pkb;
        public Node pkh;
        public PKB2(Node _pkb, Node _pkh)
        {
            pkb = _pkb;
            pkh = _pkh;
        }




        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PKH1(int files)
        {
            public uint data; 
        }

        public struct PKHeader
        {
            public ushort fileCount;
            public ushort fileSize;
        }
        public static PKHeader GetHeader(Node pkh)
        {
            PKHeader header = new PKHeader();

            using (BinaryReader dataReader = new BinaryReader(pkh.Stream))
            {
                dataReader.BaseStream.Position = 0x16;
                header.fileCount = dataReader.ReadUInt16();
                dataReader.BaseStream.Position = 0x1c;
                header.fileSize = dataReader.ReadUInt16();
            }

            return header;
        }
    }
}
