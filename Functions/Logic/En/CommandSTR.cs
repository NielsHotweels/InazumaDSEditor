using InazumaDSEditor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yarhl.FileSystem;

namespace Inazuma.Functions.Logic.En
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CommandSTR
    {
        public static string GetText(Stream commandSTR, int textID, int length = 0x20)
        {
            commandSTR.Position = textID * 0x20;
            BinaryReader reader = new BinaryReader(commandSTR);
            
            string text = Encoding.UTF8.GetString(reader.ReadBytes(length));
            return text;

        }
        public static void UpdateText(Stream commandSTR, int textID, string newText, int length = 0x20)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(newText);

            textBytes = Enumerable.Range(0, length)
            .Select(i => i < textBytes.Length ? textBytes[i] : (byte)0)
            .ToArray();

            commandSTR.Position = textID * 0x20;
            BinaryWriter writer = new BinaryWriter(commandSTR);
            writer.Write(textBytes);
        }
    }
}
