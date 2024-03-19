using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace inzDS.Tools
{
    public class DataReader
    {
        public byte[] Stream;
        public string Path;
        public int Length;

        public DataReader(string file)
        {
            this.Path = file;
            this.Length = File.ReadAllBytes(file).Length;
            Stream = File.ReadAllBytes(file);
        }
        public DataReader(Byte[] _Stream)
        {
            Stream = _Stream;
            Length = _Stream.Length;
        }

        public string Read(int position, int length, bool littleEndian)
        {
            byte[] hexadecimalCode = new byte[length];
            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(Stream)))
            {
                binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
                binaryReader.Read(hexadecimalCode, 0, length);
                binaryReader.Close();
            }
            switch (littleEndian)
            {
                case true:
                    return LittleEndian(BitConverter.ToString(hexadecimalCode).Replace("-", ""));
                case false:
                    return BitConverter.ToString(hexadecimalCode).Replace("-", "");
                default:
                    return BitConverter.ToString(hexadecimalCode).Replace("-", "");
            }
        }
        public void Replace(int position, string replace)
        {
            byte[] StringToByteArray()
            {
                replace = Regex.Replace(replace, @"\s+", "");
                return Enumerable.Range(0, replace.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(replace.Substring(x, 2), 16))
                                 .ToArray();
            }
            byte[] hexCode = StringToByteArray();
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(this.Stream)))
            {
                bw.BaseStream.Seek(position, SeekOrigin.Begin);
                bw.Write(hexCode, 0, hexCode.Length);
                bw.Close();
            }
        }

        public void Append(string appendHex)
        {
            Stream = Stream.Concat(StringToByteArray(appendHex)).ToArray();
            this.Length = Stream.Length;
        }

        public int Search(int start, int deplacement, string search)
        {
            if (Length == 0) this.Length = File.ReadAllBytes(Path).Length;
            for (int i = start; i < Length; i += deplacement)
            {
                string find = Read(i, search.Length / 2, false);
                if (find == search)
                {
                    start = i;
                    break;
                }
                start = -1;
            }
            return start;
        }

        public void Save()
        {
            File.WriteAllBytes(Path, Stream);
        }

        public string LittleEndian(string hex)
        {
            hex = hex.Replace(" ", "");
            if (hex.Length < 3)
            {
                return hex;
            }
            else
            {
                if (hex.Length % 2 != 0)
                {
                    hex = "0" + hex;
                }
                List<String> hexList = new List<string>();
                for (int i = 0; i < hex.Length - 1; i = i + 2)
                {
                    hexList.Add(hex.Substring(i, 2));
                }
                hexList.Reverse();
                hex = null;
                foreach (string i in hexList)
                {
                    hex = hex + i;
                }
                return hex;
            }
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
