using System.Runtime.InteropServices;

namespace ScriptInazumaElevenDS.Level5.Archive.PKH
{
    public class PKHSupport
    {
        public struct Header
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] Name;
            public int Length;
            public short Unk1;
            public short FileCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
            public byte[] Unk2;
        }
    }
}
