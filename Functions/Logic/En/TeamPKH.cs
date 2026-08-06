using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TeamPKH
    {
        public uint TeamID;

        public static ushort FindPlayerID(Unitno[] units, int unitIndex)
        {
            for (int i = 0; i == units.Count(); i++)
            {
                Debug.WriteLine(units[i].PlayerID);
                if (units[i].PlayerID == unitIndex)
                {
                    break;
                }
                return (ushort)i;
            }
            Console.Error.WriteLine("Could not find player ");
            return 0;
        }
    }
}
