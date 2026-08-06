using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Item
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Name;
        public byte Unk1;
        public byte Type;
        public ushort ItemData1;
        public ushort Price;
        public ushort Image;
        public byte Unk2;
        public byte LockByte;
        public ushort ItemData2;
        public ushort Unk3;
        public ushort Description;


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
