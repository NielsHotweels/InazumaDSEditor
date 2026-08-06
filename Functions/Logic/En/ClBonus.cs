using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ClBonus
    {
        public ushort Unk1;
        public ushort Empty1;
        public ushort Unk3;
        public ushort Empty2;
        public ushort Unk5;
        public ushort Drop1;
        public ushort Drop2;
        public ushort Drop3;
        public byte Drop1Chance;
        public byte Drop2Chance;
        public byte Drop3Chance;
        public byte Drop1Unk1;
        public byte Drop2Unk1;
        public byte Drop3Unk1;
        public ushort Empty3;
    }
}
