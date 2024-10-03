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
    public static class ShopName
    {
        public static string[] GetNames(string ShopNames)
        {
            string[] lines = ShopNames.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return lines;
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
