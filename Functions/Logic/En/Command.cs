using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Command
    {
        public byte MoveType;
        public byte FoulRate;
        public ushort Unk1;
        public ushort TpCost;
        public byte Power;
        public byte PlayerRequirement;
        public byte Element;
        public byte Unk7;
        public byte CoopAmount;
        public byte CoopRecuirementCategory;
        public byte CoopPlayerRequirement1;
        public byte CoopPlayerRequirement2;
        public byte Unk6;
        public byte MoveAttribute;
        public byte Leveling;
        public byte Unk2;
        public byte Unk3;
        public byte Unk4;
        public ushort MoveName;
        public ushort MoveDescription;
        public int MoveAnimation;
    }
}
