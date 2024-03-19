using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Unitstat
    {
        public short FPMin;
        public short FPMax;
        public short FPInc;
        private short Unk1;

        public short TPMin;
        public short TPMax;
        public short TPInc;
        private short Unk2;

        public byte KickMin;
        public byte KickMax;
        public byte KickInc;
        private byte Unk3;

        public byte BodyMin;
        public byte BodyMax;
        public byte BodyInc;
        private byte Unk4;

        public byte GuardMin;
        public byte GuardMax;
        public byte GuardInc;
        private byte Unk5;

        public byte ControlMin;
        public byte ControlMax;
        public byte ControlInc;
        private byte Unk6;

        public byte SpeedMin;
        public byte SpeedMax;
        public byte SpeedInc;
        private byte Unk7;

        public byte GutsMin;
        public byte GutsMax;
        public byte GutsInc;
        private byte Unk8;

        public byte StaminaMin;
        public byte StaminaMax;
        public byte StaminaInc;
        private byte Unk9;

        public short Move1ID;
        public byte Move1Lvl;
        private byte Unk10;

        public short Move2ID;
        public byte Move2Lvl;
        private byte Unk11;

        public short Move3ID;
        public byte Move3Lvl;
        private byte Unk12;

        public short Move4ID;
        public byte Move4Lvl;
        private byte Unk13;

        public short TotalStats;

        private short Unk14;
        private short Unk15;
        private short Unk16;
        private short Unk17;
        private short Unk18;
        private int Unk19;
        private int Unk20;
    }
}
