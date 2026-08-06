using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Team
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Name;
        public ushort Sprites;
        public ushort Formation;
        public ushort Unk1;
        public ushort Kit;
        public ushort Unk2;
        public ushort Unk3;
        public ushort Unk4;
        public ushort Captain;
        public ushort BootsPlayer;
        public ushort BootsGoalkeeper;
        public ushort GlovesGoalkeeper;
        public ushort Unk5;
        public ushort Unk6;
        public ushort Unk7;
        public ushort Unk8;
        public ushort Unk9;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public PlayerData[] Players;

        public struct PlayerData
        {
            public ushort ID;
            public byte Position;
            public byte Number;
            public byte LevelBoost;
            public byte Unk;
            public ushort Unk2;
        }

        public string GetName()
        {
            return Encoding.UTF8.GetString(Name);
        }

        public void UpdateName(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            nameBytes = Enumerable.Range(0, 32)
            .Select(i => i < nameBytes.Length ? nameBytes[i] : (byte)0)
            .ToArray();
            this.Name = nameBytes;
        }
    }
}
