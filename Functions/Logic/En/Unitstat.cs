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
        public short FPMin { get; set; }
        public short FPMax { get; set; }
        public short FPInc { get; set; }
        private short Unk1;

        public short TPMin { get; set; }
        public short TPMax { get; set; }
        public short TPInc { get; set; }
        private short Unk2;

        public byte KickMin { get; set; }
        public byte KickMax { get; set; }
        public byte KickInc { get; set; }
        private byte Unk3;

        public byte BodyMin { get; set; }
        public byte BodyMax { get; set; }
        public byte BodyInc { get; set; }
        private byte Unk4;

        public byte GuardMin { get; set; }
        public byte GuardMax { get; set; }
        public byte GuardInc { get; set; }
        private byte Unk5;

        public byte ControlMin { get; set; }
        public byte ControlMax { get; set; }
        public byte ControlInc { get; set; }
        private byte Unk6;

        public byte SpeedMin { get; set; }
        public byte SpeedMax { get; set; }
        public byte SpeedInc { get; set; }
        private byte Unk7;

        public byte GutsMin { get; set; }
        public byte GutsMax { get; set; }
        public byte GutsInc { get; set; }
        private byte Unk8;

        public byte StaminaMin { get; set; }
        public byte StaminaMax { get; set; }
        public byte StaminaInc { get; set; }
        private byte Unk9;

        public short Move1ID { get; set; }
        public byte Move1Lvl { get; set; }
        private byte Unk10;

        public short Move2ID { get; set; }
        public byte Move2Lvl { get; set; }
        private byte Unk11;

        public short Move3ID { get; set; }
        public byte Move3Lvl { get; set; }
        private byte Unk12;

        public short Move4ID { get; set; }
        public byte Move4Lvl { get; set; }
        private byte Unk13;

        public short TotalStats { get; set; }

        private short Unk14;
        private short Unk15;
        private short Unk16;
        private short Unk17;
        private short Unk18;
        private int Unk19;
        private int Unk20;
    }
}
