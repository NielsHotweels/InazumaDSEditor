using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PracticeGame
    {
        public byte Type;
        public byte Page;
        public ushort LocalID;
        public ushort xPos;
        public ushort yPos;
        public ushort Unk1;
        public ushort DisplayTeamID;
        public int Unk2;
        public int Unk3;
        public int Unk4;
        public int Unk5;
        public int Unk6;
        public int Unk7;
        public int Unk8;
        public int Unk9;
        public int Unk10;
        public int Unk11;
        public ushort Unk12;
        public ushort TeamID;
        public ushort Connection1;
        public ushort Connection2;
        public ushort Connection3;
        public ushort Connection4;
        public ushort Unk14;

    }
}
