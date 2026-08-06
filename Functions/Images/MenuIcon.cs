using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MenuIcon
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x200)]
        public byte[] nbfc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] nbfp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] unk1;
    }
}