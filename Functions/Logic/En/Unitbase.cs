using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Unitbase
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Nickname;
        public byte ExpCurve;
        private byte Unk1;
        public short PlayerID1;
        public short PlayerID2;
        public byte BodyType2D;
        public byte PalskinFace;
        public short HeadSpriteOW;
        public short HeadPaletteOW;
        public byte SkinTone2D;
        public byte Unk2;
        public byte BodyType3D;
        public byte SkinTone3D;
        public short Unk3;
        public byte Gender;
        public byte Age;
        public byte BodyType;
        public byte Position;
        public int Unk4;
        public byte Element;
        public short Unk5;
        public byte Unk6;
        public short DescriptionID;
        public string GetName()
        {
            return Encoding.UTF8.GetString(Name);
        }
        public string GetNickname()
        {
            return Encoding.UTF8.GetString(Nickname);
        }

        public void UpdateName(string name, bool isNickname)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            nameBytes = Enumerable.Range(0, 32)
            .Select(i => i < nameBytes.Length ? nameBytes[i] : (byte)0)
            .ToArray();

            if (isNickname)
            {
                this.Nickname = nameBytes;
            }
            else
            {
                this.Name = nameBytes;
            }
        }
    }
}
