using SceneGate.Ekona.Containers.Rom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InazumaDSEditor.Functions
{
    public class GameInfo
    {
        public enum Languages
        {
            en, fr, jp, it, sp
        }
        public enum GameVerions
        {
            IE1, IE2, IE3,
        }
        public GameVerions Game { get; private set; }
        public Languages Language { get; private set; }
        public GameInfo GetInfo(ProgramInfo programInfo)
        {
            //games
            switch (programInfo.GameCode)
            {
                case "YEEP" or "YEEF" or "YEEI" or "YEES":
                    Game = GameVerions.IE1;
                    break;
                case "BEEP" or "BEBP" or "BEEF" or "BEBF" or "BEEJ" or "BEBI" or "BEEI" or "BEBS" or "BEES":
                    Game = GameVerions.IE2;
                    break;
            }
            //languages
            switch (programInfo.GameCode)
            {
                case "YEEP" or "BEEP" or "BEBP":
                    Language = Languages.en;
                    break;
                case "BEEF" or "BEBF" or "BEBJ":
                    Language = Languages.fr;
                    break;
                case "BEEJ":
                    Language = Languages.jp;
                    break;
                case "BEBI" or "BEEI" or "YEEI":
                    Language = Languages.it;
                    break;
                case "BEBS" or "BEES" or "YEES":
                    Language = Languages.sp;
                    break;
            }
            return this;
        }
    }
}
