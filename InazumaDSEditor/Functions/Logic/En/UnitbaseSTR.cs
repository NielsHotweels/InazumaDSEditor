using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UnitbaseSTR
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] Description;
        public string GetDescription()
        {
            return Encoding.UTF8.GetString(Description);
        }
        public void UpdateDescription(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            nameBytes = Enumerable.Range(0, 128)
            .Select(i => i < nameBytes.Length ? nameBytes[i] : (byte)0)
            .ToArray();
            this.Description = nameBytes;
        }
    }
}
