using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//this file is probably used for the player requirement for special moves, coop player requirements are different

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Cmd_Cond
    {
        public byte Unk1;
        public byte Unk2;
        public byte RequirementType;
        public byte Unk3;
        public ushort Req1;
        public ushort Req2;
        public ushort Req3;
        public ushort Req4;
    }
}
