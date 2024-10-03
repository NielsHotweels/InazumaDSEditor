using System;
using System.IO;
using System.Linq;
using InazumaDSEditor.Tools;
using System.Collections.Generic;

namespace inzDS.Formats.Archive
{
    public class PKB
    {
        public string Name = "PKB";
        public string Extension = ".pkb";
        public string FileType = "Archive";
        public DataReader Data;
        public PKH FilePKH;
        public Dictionary<int, FilePKB> Files = new Dictionary<int, FilePKB>();

        public PKB(string _Path)
        {
            if (!File.Exists(_Path.Replace(".pkb", ".pkh")))
            {
                throw new FileNotFoundException(Path.GetFileNameWithoutExtension(_Path) + ".pkh not found");
            }
            Data = new DataReader(_Path);
            FilePKH = new PKH(_Path.Replace(".pkb", ".pkh"));
        }

        public FilePKB GetFile(int number, int shift)
        {
            if (number > FilePKH.FilesCount) return null;
            int position = 48 + shift * number;
            if (shift == 4)
            {
                return new FilePKB(
                        FilePKH.Data.Read(position, 4, false),
                        32,
                        Data.Stream.Skip(number * 32).Take(32).ToArray()
                        );
            } 
            else if (shift == 8)
            {
                return new FilePKB(
                        FilePKH.Data.Read(position, 4, false),
                        Convert.ToInt32(FilePKH.Data.Read(position + 4, 4, true), 16),
                        Data.Stream.Skip(number * 608).Take(608).ToArray()
                        );
            } else
            {
                return new FilePKB(
                        FilePKH.Data.Read(position, 4, false),
                        Convert.ToInt32(FilePKH.Data.Read(position + 8, 4, true), 16),
                        Data.Stream.Skip(Convert.ToInt32(FilePKH.Data.Read(position + 4, 4, true), 16)).Take(Convert.ToInt32(FilePKH.Data.Read(position + 8, 4, true), 16)).ToArray()
                        );
            }
        }
        public FilePKB GetFileByKey(string key, int shift)
        {
            int position = 48;
            for (int i = 0; i < FilePKH.FilesCount; i++)
            {
                if (FilePKH.Data.Read(position, 4, false) == key)
                {
                    if (shift == 8)
                    {
                        return new FilePKB(
                                FilePKH.Data.Read(position, 4, false),
                                Convert.ToInt32(FilePKH.Data.Read(position + 4, 4, true), 16),
                                Data.Stream.Skip((position-48)/8 * 608).Take(608).ToArray()
                                );
                    } else
                    {
                        return new FilePKB(
                                FilePKH.Data.Read(position, 4, false),
                                Convert.ToInt32(FilePKH.Data.Read(position + 8, 4, true), 16),
                                Data.Stream.Skip(Convert.ToInt32(FilePKH.Data.Read(position + 4, 4, true), 16)).Take(Convert.ToInt32(FilePKH.Data.Read(position + 8, 4, true), 16)).ToArray()
                                );
                    }
                }
                if (shift == 8)
                {
                    position += 8;
                } else
                {
                    position += 12;
                }
            }
            return null;
        }

        public int IndexOf(string key, int shift)
        {
            int position = 48;
            int fileCount = 0;
            for (int i = 0; i < FilePKH.FilesCount; i++)
            {
                if (FilePKH.Data.Read(position, 4, false) == key)
                {
                    return fileCount;
                }
                position += shift;
                fileCount += 1;
            }
            return -1;
        }

        public void Add(string _Path)
        {
            DataReader fileAdded = new DataReader(_Path);
            Files.Values.OrderBy(x => x.Key).Last();
            byte[] emptyBlock = new byte[Files.Values.OrderBy(x => x.Key).Last().Length % 16];
            for (int i = 0; i < emptyBlock.Length; i++)
            {
                emptyBlock[i] = 0x79;
            }
            IEnumerable<byte> newPKB = Data.Stream.Concat(emptyBlock).Concat(fileAdded.Stream);
            Data.Stream = newPKB.ToArray();
            FilePKH.FilesCount += 1;
            FilePKH.Data.Replace(22, Data.LittleEndian(FilePKH.FilesCount.ToString("X4")));
        }
        public void Add(DataReader file, int shift, string key)
        {
            if (shift == 12)
            {
                int emptyBlock2 = 16 - ( 48 + FilePKH.FilesCount * 12 + 12) % 16;
                if (emptyBlock2 != 0)
                {
                    FilePKH.Data.Append("".PadRight(emptyBlock2 * 2, 'F'));
                }

                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift, key);
                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift + 4, Data.LittleEndian(Data.Length.ToString("X8")));
                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift + 8, Data.LittleEndian(file.Length.ToString("X8")));
                FilePKH.FilesCount += 1;
                FilePKH.Data.Replace(22, Data.LittleEndian(FilePKH.FilesCount.ToString("X4")));
                FilePKH.Data.Save();

                int emptyBlock = 16 - (Data.Length + file.Length) % 16;
                Data.Append(file.Read(0, file.Length, false));
                Data.Append("".PadRight(emptyBlock * 2, 'F'));
                Data.Save();
            }
            else if (shift == 8)
            {
                int emptyBlock2 = 16 - (48 + FilePKH.FilesCount * 8 + 8) % 16;
                if (emptyBlock2 != 0)
                {
                    FilePKH.Data.Append("".PadRight(emptyBlock2 * 2, 'F'));
                }

                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift, key);
                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift + 4, Data.LittleEndian(file.Length.ToString("X8")));
                FilePKH.FilesCount += 1;
                FilePKH.Data.Replace(22, Data.LittleEndian(FilePKH.FilesCount.ToString("X4")));
                FilePKH.Data.Save();

                int emptyBlock = 16 - (Data.Length + file.Length) % 16;
                Data.Append(file.Read(0, file.Length, false));
                Data.Append("".PadRight(emptyBlock * 2, 'F'));
                Data.Save();
            }
            else if (shift == 4)
            {
                int emptyBlock2 = 16 - (48 + FilePKH.FilesCount * 4 + 4) % 16;
                if (emptyBlock2 != 0)
                {
                    FilePKH.Data.Append("".PadRight(emptyBlock2 * 2, 'F'));
                }

                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift, key);
                FilePKH.Data.Replace(48 + FilePKH.FilesCount * shift + 8, Data.LittleEndian(file.Length.ToString("X8")));
                FilePKH.FilesCount += 1;
                FilePKH.Data.Replace(22, Data.LittleEndian(FilePKH.FilesCount.ToString("X4")));
                FilePKH.Data.Save();

                int emptyBlock = 16 - (Data.Length + file.Length) % 16;
                Data.Append(file.Read(0, file.Length, false));
                Data.Append("".PadRight(emptyBlock * 2, 'F'));
                Data.Save();
            }
        }

    }

    public class FilePKB
    {
        public string Key;
        public int Length;
        public DataReader Data;

        public FilePKB(string _Key, int _Length, byte[] _Content)
        {
            Key = _Key;
            Length = _Length;
            Data = new DataReader(_Content);
        }
    }
}
