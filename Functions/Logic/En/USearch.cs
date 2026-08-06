using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct USearch
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Name;
        // Properties for serialization
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
        public byte[] Nickname;

        public byte Unk1;
        public byte Unk2;
        public byte Unk3;
        public byte Unk4;
        public byte Unk5;
        public byte Unk6;
        public byte Area;
        public byte Unk8;
        public byte Unk9;
        public short BefriendedVia;
        public byte Unk11;
        public byte Unk12;
        public long RecruitScript;

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
