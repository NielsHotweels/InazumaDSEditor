using System;
using System.Linq;
using System.Drawing;
using inzDS.Tools;
using System.Collections.Generic;

namespace inzDS.Formats.Images
{
    public class NBFP
    {
        public string Name = "NBFP";
        public string Extension = ".NBFP";
        public string FileType = "Palette";
        public DataReader Data;
        public Dictionary<int, Color> Palette = new Dictionary<int, Color>();

        Color BGR555ToColor(byte byte1, byte byte2)
        {
            int r, b, g;
            short bgr = BitConverter.ToInt16(new Byte[] { byte1, byte2 }, 0);

            r = (bgr & 0x001F) * 0x08;
            g = ((bgr & 0x03E0) >> 5) * 0x08;
            b = ((bgr & 0x7C00) >> 10) * 0x08;

            return Color.FromArgb(r, g, b);
        }
        Byte[] ColorToBGR555(Color color)
        {
            byte[] d = new byte[2];

            int r = color.R / 8;
            int g = (color.G / 8) << 5;
            int b = (color.B / 8) << 10;

            ushort bgr = (ushort)(r + g + b);
            Array.Copy(BitConverter.GetBytes(bgr), d, 2);

            return d;
        }

        public NBFP(string _Path)
        {
            Data = new DataReader(_Path);
            for (int i = 0; i < Data.Length / 2; i++)
            {
                Palette.Add(i, BGR555ToColor(Data.Stream.Skip(i * 2).Take(1).First(), Data.Stream.Skip(i * 2 + 1).Take(1).First()));
            }
        }
        public NBFP(byte[] _Data)
        {
            Data = new DataReader(_Data);
            for (int i = 0; i < Data.Length / 2; i++)
            {
                Palette.Add(i, BGR555ToColor(Data.Stream.Skip(i * 2).Take(1).First(), Data.Stream.Skip(i * 2 + 1).Take(1).First()));
            }
        }
        public NBFP(Image image)
        {
            Bitmap img = new Bitmap(image);
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);
                    if (!Palette.ContainsValue(pixel) & Palette.Count < 16)
                    {
                        Palette.Add(Palette.Count, pixel);
                    }
                }
            }
            while (Palette.Count < 16)
            {
                Palette.Add(Palette.Count, Color.FromArgb(248, 248, 248));
            }
        }

        // Method to split the palette into groups of specified number of colors per palette
        public List<Dictionary<int, Color>> SplitPaletteIntoGroups(int colorsPerPalette)
        {
            List<Dictionary<int, Color>> paletteGroups = new List<Dictionary<int, Color>>();

            for (int i = 0; i < Palette.Count; i += colorsPerPalette)
            {
                Dictionary<int, Color> paletteGroup = Palette.Skip(i).Take(colorsPerPalette).ToDictionary(kv => kv.Key, kv => kv.Value);
                paletteGroups.Add(paletteGroup);
            }

            return paletteGroups;
        }

        // Method to convert palette group into an image
        public Bitmap ConvertPaletteGroupToImage(Dictionary<int, Color> paletteGroup)
        {
            Bitmap image = new Bitmap(paletteGroup.Count * 16, 16);
            using (Graphics g = Graphics.FromImage(image))
            {
                int x = 0;
                foreach (var color in paletteGroup.Values)
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, x, 0, 16, 16);
                    }
                    x += 16;
                }
            }
            return image;
        }

        // Method to convert entire palette into images
        public List<Bitmap> ConvertPaletteToImages(int colorsPerPalette)
        {
            List<Bitmap> images = new List<Bitmap>();

            List<Dictionary<int, Color>> paletteGroups = SplitPaletteIntoGroups(colorsPerPalette);
            foreach (var group in paletteGroups)
            {
                Bitmap image = ConvertPaletteGroupToImage(group);
                images.Add(image);
            }

            return images;
        }
    }
}
