using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct I_Detail
    {
        public ushort FP;
        public ushort TP;

        public byte Kick;
        public byte Body;
        public byte Guard;
        public byte Control;
        public byte Speed;
        public byte Guts;
        public byte Stamina;
        public byte Unk1;
        public ushort Unk2;
        public ushort Unk3;
    }
}
