using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//amount of exp that is required to level up per exp curve

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LVup_exp
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x62)]
        public int ExpPoints;
    }
}
