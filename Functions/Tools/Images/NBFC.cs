using System;
using System.Linq;
using System.Drawing;
using Inazuma.Functions.Tools;
using System.Collections.Generic;
using inzDS.Tools;

namespace inzDS.Formats.Images
{
    public class NBFC
    {
        public string Name = "NBFC";
        public string Extension = ".NBFC";
        public string FileType = "Tiles";
        public DataReader Data;
        NBFP Nbfp;

        public NBFC(string _Path, NBFP _Nbfp)
        {
            Data = new DataReader(_Path);
            Nbfp = _Nbfp;
        }
        public NBFC(byte[] _Data, NBFP _Nbfp)
        {
            Data = new DataReader(_Data);
            Nbfp = _Nbfp;
        }
        public NBFC(Image image, NBFP _Nbfp)
        {
            Nbfp = _Nbfp;

            Bitmap img = new Bitmap(image);
            if (img.Width == 64)
            {
                byte[] data = new byte[2048];

                int x = 0;
                int y = 0;
                int offset = 0;

                int x2 = 0;
                int y2 = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (i % 4 == 0 && i > 3)
                    {
                        x += 8;
                        x2 = x;
                        y = y2;
                    }
                    if (i % (img.Width / 2) == 0 && i > img.Width / 2 - 1)
                    {
                        x = 0;
                        x2 = x;
                        y += 8;
                        y2 = y;
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            int pixelKey1 = Nbfp.Palette.FirstOrDefault(s => s.Value == img.GetPixel(x, y)).Key;
                            int pixelKey2 = Nbfp.Palette.FirstOrDefault(s => s.Value == img.GetPixel(x + 1, y)).Key;
                            data[offset] = (byte)Convert.ToInt32(pixelKey2.ToString("X") + pixelKey1.ToString("X"), 16);
                            offset++;
                            x += 2;
                        }
                        y += 1;
                        x = x2;
                    }
                }
            }
            else if (img.Width == 32)
            {
                byte[] data = new byte[512];
                int offset = 0;
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x += 2)
                    {
                        int pixelKey1 = Nbfp.Palette.FirstOrDefault(s => s.Value == img.GetPixel(x, y)).Key;
                        int pixelKey2 = Nbfp.Palette.FirstOrDefault(s => s.Value == img.GetPixel(x + 1, y)).Key;
                        data[offset] = (byte)Convert.ToInt32(pixelKey2.ToString("X") + pixelKey1.ToString("X"), 16);
                        offset++;
                    }
                }
                Data = new DataReader(data);
            }
        }

        public Image ConvertToPNG(bool transparent)
        {
            Bitmap img = new Bitmap(1, 1);
            if (Data.Length == 2048)
            {
                img = new Bitmap(64, 64);
                int x = 0;
                int y = 0;
                int offset = 0;

                int x2 = 0;
                int y2 = 0;
                for (int i = 0; i < Data.Length / 8; i++)
                {
                    if (i % 4 == 0 && i > 3)
                    {
                        x += 8;
                        x2 = x;
                        y = y2;
                    }
                    if (i % (img.Width / 2) == 0 && i > img.Width / 2 - 1)
                    {
                        x = 0;
                        x2 = x;
                        y += 8;
                        y2 = y;
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            img.SetPixel(x, y, Nbfp.Palette[Convert.ToInt32(Data.Read(offset + j * 4 + k, 1, false)[1].ToString(), 16)]);
                            img.SetPixel(x + 1, y, Nbfp.Palette[Convert.ToInt32(Data.Read(offset + j * 4 + k, 1, false)[0].ToString(), 16)]);
                            x += 2;
                        }
                        y += 1;
                        x = x2;
                    }
                    offset += 8;
                }
            }
            else if (Data.Length == 512)
            {
                img = new Bitmap(32, 32);
                int x = 0;
                int y = 0;
                int offset = 0;
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        img.SetPixel(x, y, Nbfp.Palette[Convert.ToInt32(Data.Read(offset, 1, false)[1].ToString(), 16)]);
                        img.SetPixel(x + 1, y, Nbfp.Palette[Convert.ToInt32(Data.Read(offset, 1, false)[0].ToString(), 16)]);
                        offset += 1;
                        x += 2;
                    }
                    x = 0;
                    y += 1;
                }
            }

            if (transparent == true)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        Color pixel = img.GetPixel(x, y);
                        if (pixel == Nbfp.Palette[0])
                        {
                            img.SetPixel(x, y, Color.FromArgb(0, pixel));
                        }
                    }
                }
            }
            return img;
        }
    }
}
