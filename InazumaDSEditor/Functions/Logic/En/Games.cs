using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Games
    {
        public ushort TeamID;
        public byte Unk1;
        public byte TeamLevel;
        public byte Stadium;
        public byte Unk2;
        public ushort ForceKit;
        public ushort Unk3;
        public byte MatchType;
        public byte LootTable;
        public byte HalfStart;
        public byte DuplicateRemover;
        public ushort Unk4;

        public byte MatchScreens;
        public byte Unk5;
        public byte OST;
        public byte Unk6;
        public byte MatchEvent;
        public byte Unk7;
        public ushort Unk8;
        public uint Unk9;
        public ushort Unk10;
        public ushort Unk11;

        public ulong Unk12;
        public uint Unk13;
        public ushort Unk15;
        public ushort Unk16;
    }
}
