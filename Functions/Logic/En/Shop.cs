using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Shop
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public ushort[] Item;
    }
}
