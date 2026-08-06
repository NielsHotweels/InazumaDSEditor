using Inazuma.Functions.Logic.En;
using SceneGate.Ekona.Containers.Rom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaDSEditor.Functions
{
    public class DataHandling()
    {
        public static int GetUnitIDFromPlayerID(int playerID, Unitno[] unitno)
        {
            for (int i = 0; i < unitno.Length; i++)
            {
                if (unitno[i].PlayerID == playerID)
                {
                    return i;
                }
            }
            return 0;
        }
    }

    public enum Element
    {
        none,
        Wind,
        Wood,
        Fire,
        Earth,
    }
    public enum Positions
    {
        Goalkeeper,
        Defender,
        Midfielder,
        Forward
    }

    
}
