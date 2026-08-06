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
        public byte ExpCurve { get; set; }
        public byte BinderTeam;
        public ushort PlayerID1;
        public ushort SpriteandModel;
        public byte BodyType2D { get; set; }
        public byte PalskinFace { get; set; }
        public short HeadSpriteOW { get; set; }
        public short HeadPaletteOW { get; set; }
        public byte SkinTone2D { get; set; }
        public byte Unk2;
        public byte BodyType3D { get; set; }
        public byte SkinTone3D { get; set; }
        public short StandardKit { get; set; }
        public byte Gender { get; set; }
        public byte Age { get; set; }
        public byte BodyType { get; set; }
        public byte Position { get; set; }
        public int Unk4;
        public byte Element { get; set; }
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
