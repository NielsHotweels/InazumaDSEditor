namespace Inazuma
{
    using Inazuma.Functions.Tools;
    using SceneGate.Ekona.Containers.Rom;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Inazuma.Functions.Logic.En;
    using inzDS.Formats.Images;

    public partial class Form1 : Form
    {

        Node game { get; set; }

        Stream _Unitbase;
        Stream _Unitstat;
        Stream _UnitbaseSTR;

        Unitbase[] unitbase;
        Unitstat[] unitstat;
        UnitbaseSTR[] unitbaseSTR;
        List<Bitmap> palskin3D;
        List<Bitmap> palskin2D;
        List<Bitmap> palskinFace;

        string filepath;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "nds files (*.nds)|*.nds";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filepath = openFileDialog.FileName;
                game = NodeFactory.FromFile(filepath, FileOpenMode.ReadWrite);
                game.TransformWith<Binary2NitroRom>();
                Node logic = game.Children["data"].Children["data_iz"].Children["logic"];

                _Unitbase = logic.Children["en"].Children["unitbase.dat"].Stream;
                _Unitstat = logic.Children["en"].Children["unitstat.dat"].Stream;
                _UnitbaseSTR = logic.Children["en"].Children["unitbase.STR"].Stream;



                BinaryDataReader readerunitbase = new BinaryDataReader(_Unitbase);
                unitbase = readerunitbase.ReadMultipleStruct<Unitbase>((int)readerunitbase.Length / 0x60);
                comboBox1.Items.AddRange(unitbase.Select(x => x.GetName()).ToArray());
                
                using (BinaryDataReader reader = new BinaryDataReader(_Unitstat))
                {
                    unitstat = reader.ReadMultipleStruct<Unitstat>((int)reader.Length / 0x50);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_UnitbaseSTR))
                {
                    unitbaseSTR = reader.ReadMultipleStruct<UnitbaseSTR>((int)reader.Length / 0x80);
                }
                using (BinaryDataReader reader = new BinaryDataReader(logic.Children["palskin3d.dat"].Stream))
                {
                    NBFP palskin3D = new NBFP(reader.GetSection((int)reader.Length));

                    // Convert palette to images with 8 colors per image
                    this.palskin3D = palskin3D.ConvertPaletteToImages(8);

                    // Save the palette images or perform other operations as needed
                    for (int i = 0; i < this.palskin3D.Count; i++)
                    {
                        dropdownPalskin3D.Items.Add($"Palette {i + 1}");
                    }
                }
                using (BinaryDataReader reader = new BinaryDataReader(logic.Children["palskinface.dat"].Stream))
                {
                    NBFP palskinFace = new NBFP(reader.GetSection((int)reader.Length));

                    // Convert palette to images with 8 colors per image
                    this.palskinFace = palskinFace.ConvertPaletteToImages(4);

                    // Save the palette images or perform other operations as needed
                    for (int i = 0; i < this.palskinFace.Count; i++)
                    {
                        dropdownPalskinFace.Items.Add($"Palette {i + 1}");
                    }
                }
                using (BinaryDataReader reader = new BinaryDataReader(logic.Children["palskin2d.dat"].Stream))
                {
                    NBFP palskin2D = new NBFP(reader.GetSection((int)reader.Length));

                    // Convert palette to images with 8 colors per image
                    this.palskin2D = palskin2D.ConvertPaletteToImages(4);

                    // Save the palette images or perform other operations as needed
                    for (int i = 0; i < this.palskin2D.Count; i++)
                    {
                        dropdownPalskin2D.Items.Add($"Palette {i + 1}");
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int playerID = comboBox1.SelectedIndex;
            strPlayerID.Text = playerID.ToString();

            textName.Text = unitbase[playerID].GetName();
            textNickname.Text = unitbase[playerID].GetNickname();
            intExpCurve.Value = unitbase[playerID].ExpCurve;
            intBodyType2D.Value = unitbase[playerID].BodyType;
            intBodyType3D.Value = unitbase[playerID].BodyType3D;
            dropdownPalskinFace.SelectedIndex = unitbase[playerID].PalskinFace;
            dropdownPalskin3D.SelectedIndex = unitbase[playerID].SkinTone3D;
            dropdownPalskin2D.SelectedIndex = unitbase[playerID].SkinTone2D;
            dropdownGender.SelectedIndex = unitbase[playerID].Gender - 1;
            dropdownElement.SelectedIndex = unitbase[playerID].Element;
            dropdownPosition.SelectedIndex = (unitbase[playerID].Position / 0x20);
            intYear.Value = unitbase[playerID].Age;
            intHeadSprite.Value = unitbase[playerID].HeadSpriteOW;
            intHeadPalette.Value = unitbase[playerID].HeadPaletteOW;
            dropdownBodyType.SelectedIndex = unitbase[playerID].BodyType;
            intDescriptionID.Value = unitbase[playerID].DescriptionID;

            intFPMin.Value = unitstat[playerID].FPMin;
            intFPMax.Value = unitstat[playerID].FPMax;
            intFPInc.Value = unitstat[playerID].FPInc;

            intTPMin.Value = unitstat[playerID].TPMin;
            intTPMax.Value = unitstat[playerID].TPMax;
            intTPInc.Value = unitstat[playerID].TPInc;

            intKickMin.Value = unitstat[playerID].KickMin;
            intKickMax.Value = unitstat[playerID].KickMax;
            intKickInc.Value = unitstat[playerID].KickInc;

            intBodyMin.Value = unitstat[playerID].BodyMin;
            intBodyMax.Value = unitstat[playerID].BodyMax;
            intBodyInc.Value = unitstat[playerID].BodyInc;

            intGuardMin.Value = unitstat[playerID].GuardMin;
            intGuardMax.Value = unitstat[playerID].GuardMax;
            intGuardInc.Value = unitstat[playerID].GuardInc;

            intControlMin.Value = unitstat[playerID].ControlMin;
            intControlMax.Value = unitstat[playerID].ControlMax;
            intControlInc.Value = unitstat[playerID].ControlInc;

            intSpeedMin.Value = unitstat[playerID].SpeedMin;
            intSpeedMax.Value = unitstat[playerID].SpeedMax;
            intSpeedInc.Value = unitstat[playerID].SpeedInc;

            intGutsMin.Value = unitstat[playerID].GutsMin;
            intGutsMax.Value = unitstat[playerID].GutsMax;
            intGutsInc.Value = unitstat[playerID].GutsInc;

            intStaminaMin.Value = unitstat[playerID].StaminaMin;
            intStaminaMax.Value = unitstat[playerID].StaminaMax;
            intStaminaInc.Value = unitstat[playerID].StaminaInc;

            intStatTotal.Value = unitstat[playerID].TotalStats;

            intMoveID1.Value = unitstat[playerID].Move1ID; intMoveLvl1.Value = unitstat[playerID].Move1Lvl;
            intMoveID2.Value = unitstat[playerID].Move2ID; intMoveLvl2.Value = unitstat[playerID].Move2Lvl;
            intMoveID3.Value = unitstat[playerID].Move3ID; intMoveLvl3.Value = unitstat[playerID].Move3Lvl;
            intMoveID4.Value = unitstat[playerID].Move4ID; intMoveLvl4.Value = unitstat[playerID].Move4Lvl;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int playerID = comboBox1.SelectedIndex;
            unitbase[playerID].UpdateName(textName.Text, false);
            unitbase[playerID].UpdateName(textNickname.Text, true);
            unitbase[playerID].ExpCurve = (byte)intExpCurve.Value;
            unitbase[playerID].BodyType = (byte)intBodyType2D.Value;
            unitbase[playerID].BodyType3D = (byte)intBodyType3D.Value;
            unitbase[playerID].PalskinFace = (byte)dropdownPalskinFace.SelectedIndex;
            unitbase[playerID].SkinTone3D = (byte)dropdownPalskin3D.SelectedIndex;
            unitbase[playerID].SkinTone2D = (byte)dropdownPalskin2D.SelectedIndex;
            unitbase[playerID].Gender = (byte)(dropdownGender.SelectedIndex + 0x01);
            unitbase[playerID].Element = (byte)dropdownElement.SelectedIndex;
            unitbase[playerID].Position = (byte)(dropdownPosition.SelectedIndex * 0x20);
            unitbase[playerID].Age = (byte)intYear.Value;
            unitbase[playerID].HeadSpriteOW = (short)intHeadSprite.Value;
            unitbase[playerID].HeadPaletteOW = (short)intHeadPalette.Value;
            unitbase[playerID].BodyType = (byte)dropdownBodyType.SelectedIndex;
            unitbase[playerID].DescriptionID = (short)intDescriptionID.Value;

            unitbaseSTR[playerID].UpdateDescription(textboxDescription.Text);

            unitstat[playerID].FPMin = (short)intFPMin.Value;
            unitstat[playerID].FPMax = (short)intFPMax.Value;
            unitstat[playerID].FPInc = (short)intFPInc.Value;

            unitstat[playerID].TPMin = (short)intTPMin.Value;
            unitstat[playerID].TPMax = (short)intTPMax.Value;
            unitstat[playerID].TPInc = (short)intTPInc.Value;

            unitstat[playerID].KickMin = (byte)intKickMin.Value;
            unitstat[playerID].KickMax = (byte)intKickMax.Value;
            unitstat[playerID].KickInc = (byte)intKickInc.Value;

            unitstat[playerID].BodyMin = (byte)intBodyMin.Value;
            unitstat[playerID].BodyMax = (byte)intBodyMax.Value;
            unitstat[playerID].BodyInc = (byte)intBodyInc.Value;

            unitstat[playerID].GuardMin = (byte)intGuardMin.Value;
            unitstat[playerID].GuardMax = (byte)intGuardMax.Value;
            unitstat[playerID].GuardInc = (byte)intGuardInc.Value;

            unitstat[playerID].ControlMin = (byte)intControlMin.Value;
            unitstat[playerID].ControlMax = (byte)intControlMax.Value;
            unitstat[playerID].ControlInc = (byte)intControlInc.Value;

            unitstat[playerID].SpeedMin = (byte)intSpeedMin.Value;
            unitstat[playerID].SpeedMax = (byte)intSpeedMax.Value;
            unitstat[playerID].SpeedInc = (byte)intSpeedInc.Value;

            unitstat[playerID].GutsMin = (byte)intGutsMin.Value;
            unitstat[playerID].GutsMax = (byte)intGutsMax.Value;
            unitstat[playerID].GutsInc = (byte)intGutsInc.Value;

            unitstat[playerID].StaminaMin = (byte)intStaminaMin.Value;
            unitstat[playerID].StaminaMax = (byte)intStaminaMax.Value;
            unitstat[playerID].StaminaInc = (byte)intStaminaInc.Value;

            unitstat[playerID].TotalStats = (short)intStatTotal.Value;

            unitstat[playerID].Move1ID = (short)intMoveID1.Value; unitstat[playerID].Move1Lvl = (byte)intMoveLvl1.Value;
            unitstat[playerID].Move2ID = (short)intMoveID2.Value; unitstat[playerID].Move2Lvl = (byte)intMoveLvl2.Value;
            unitstat[playerID].Move3ID = (short)intMoveID3.Value; unitstat[playerID].Move3Lvl = (byte)intMoveLvl3.Value;
            unitstat[playerID].Move4ID = (short)intMoveID4.Value; unitstat[playerID].Move4Lvl = (byte)intMoveLvl4.Value;

        }

        private void saveRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an array of streams
            Stream[] streams = {
                _Unitbase,
                _Unitstat,
                _UnitbaseSTR
                };

            // Create an array of data structures
            Array[] dataStructures = {
                unitbase,
                unitstat,
                unitbaseSTR
                };

            // Loop through each stream and corresponding data structure
            for (int i = 0; i < streams.Length; i++)
            {
                using (DataStream dataStream = DataStreamFactory.FromStream(streams[i]))
                {
                    // Write the current data structure to the corresponding DataStream
                    BinaryDataWriter writer = new BinaryDataWriter(dataStream);
                    writer.WriteMultipleStruct((dynamic)dataStructures[i]);
                }
            }


        }

        private void intDescriptionID_ValueChanged(object sender, EventArgs e)
        {
            int descriptionID = (int)intDescriptionID.Value;
            intDescriptionID.Value = descriptionID;
            textboxDescription.Text = unitbaseSTR[descriptionID].GetDescription();
        }

        private void dropdownPalskin3D_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgPalskin3D.Image = palskin3D[dropdownPalskin3D.SelectedIndex];
        }

        private void imgPalskin3D_Click(object sender, EventArgs e)
        {

        }

        private void dropdownPalskinInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgPalskinInterface.Image = palskinFace[dropdownPalskinFace.SelectedIndex];
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void dropdownPalskin2D_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgPalskin2D.Image = palskin2D[dropdownPalskin2D.SelectedIndex];
        }
    }
}
