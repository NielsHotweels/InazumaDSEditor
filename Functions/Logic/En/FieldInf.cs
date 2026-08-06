using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FieldInf
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x170)]
        public byte[] Data;
        public string GetName()
        {
            return Encoding.UTF8.GetString(Name);
        }
    }
}
