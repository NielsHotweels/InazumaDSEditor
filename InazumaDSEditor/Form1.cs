namespace Inazuma
{
    using SceneGate.Ekona.Containers.Rom;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Inazuma.Functions.Logic.En;
    using InazumaDSEditor.Functions.Packs;
    using Yarhl.FileFormat;
    using System.Runtime.Intrinsics.Arm;
    using InazumaDSEditor.Tools;
    using InazumaDSEditor.Tools.Images;
    using InazumaDSEditor.Functions;
    using Microsoft.VisualBasic.Devices;
    using System.Diagnostics;
    using System.Drawing.Imaging;
    using System.Text.Encodings.Web;
    using Org.BouncyCastle.Utilities.IO;
    using inzDS.Formats.Archive;
    using System.Text.RegularExpressions;
    using Org.BouncyCastle.Asn1.Cmp;
    using static InazumaDSEditor.Functions.GameInfo;

    public partial class Form1 : Form
    {

        Node game { get; set; }
        GameInfo gameInfo;

        //logic
        Stream _Unitbase;
        Stream _Unitstat;
        Stream _UnitbaseSTR;
        Stream _Unitchange;
        Stream _Unitno;
        Stream _Command;
        Stream _CommandSTR;
        Stream _Item;
        Stream _ItemSTR;
        Stream _ShopName;
        Stream _Shop;
        Stream _Team;
        Stream _TeamPKH;
        Stream _Games;

        //images
        Stream _ihPKB;
        Stream _ihPKH;

        //logic
        Unitbase[] unitbase;
        Unitstat[] unitstat;
        UnitbaseSTR[] unitbaseSTR;
        Unitchange[] unitchange;
        Unitno[] unitno;
        Command[] command;
        Item[] item;
        ItemSTR[] itemSTR;
        Shop[] shop;
        string[] shopName;
        Team[] team;
        TeamPKH[] teamPKH;
        Games[] games;

        //images
        PKB2 ihPKB;
        PKB2 ihPKHeader;
        PKB2.PKH1[] ihPKH;


        List<Bitmap> palskin3D;
        List<Bitmap> palskin2D;
        List<Bitmap> palskinFace;

        ComboBox[] dropdownShopItems;
        ComboBox[] dropdownTeamPlayers;
        NumericUpDown[] intTeamPlayerLvl;
        NumericUpDown[] intTeamPlayerNumber;
        NumericUpDown[] intTeamPlayerPosition;
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
                ProgramInfo programInfo = Navigator.SearchNode(game, "system/info").GetFormatAs<ProgramInfo>();

                gameInfo = new GameInfo().GetInfo(programInfo);
                textGameName.Text = programInfo.GameTitle;
                textGameType.Text = gameInfo.Game.ToString();
                textGameLanguage.Text = gameInfo.Language.ToString();
                textGameCode.Text = programInfo.GameCode;


                Node logic = game.Children["data"].Children["data_iz"].Children["logic"];
                Node Pic3d = game.Children["data"].Children["data_iz"].Children["pic3d"];


                _Unitbase = logic.Children[gameInfo.Language.ToString()].Children["unitbase.dat"].Stream;
                _Unitstat = logic.Children[gameInfo.Language.ToString()].Children["unitstat.dat"].Stream;
                _UnitbaseSTR = logic.Children[gameInfo.Language.ToString()].Children["unitbase.STR"].Stream;
                _Unitno = logic.Children[gameInfo.Language.ToString()].Children["unitno.dat"].Stream;
                _Command = logic.Children[gameInfo.Language.ToString()].Children["command.dat"].Stream;
                _CommandSTR = logic.Children[gameInfo.Language.ToString()].Children["command.STR"].Stream;
                _Item = logic.Children[gameInfo.Language.ToString()].Children["item.dat"].Stream;
                _ItemSTR = logic.Children[gameInfo.Language.ToString()].Children["item.STR"].Stream;
                _ShopName = logic.Children[gameInfo.Language.ToString()].Children["ShopName.dat"].Stream;
                _Shop = logic.Children[gameInfo.Language.ToString()].Children["shop.dat"].Stream;
                _Team = logic.Children[gameInfo.Language.ToString()].Children["team.pkb"].Stream;
                _TeamPKH = logic.Children[gameInfo.Language.ToString()].Children["team.pkh"].Stream;
                _Games = logic.Children["games.dat"].Stream;
                _ihPKB = Pic3d.Children["ih.pkb"].Stream;
                _ihPKH = Pic3d.Children["ih.pkh"].Stream;

                _Unitchange = logic.Children["unitchange.dat"].Stream;

                ComboBox[] dropdownShopItemList = { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8, comboBox9, comboBox10, comboBox11, comboBox12, comboBox13, comboBox14, comboBox15, comboBox16, comboBox17, comboBox18, comboBox19, comboBox20, comboBox21, comboBox22, comboBox23, comboBox24, comboBox25, comboBox26, comboBox27, comboBox28, comboBox29, comboBox30, comboBox31, comboBox32 };
                ComboBox[] dropdownTeamPlayersList = { comboBox33, comboBox34, comboBox35, comboBox36, comboBox37, comboBox38, comboBox39, comboBox40, comboBox41, comboBox42, comboBox43, comboBox44, comboBox45, comboBox46, comboBox47, comboBox48, comboBox49, comboBox50, comboBox51, comboBox52, comboBox53, comboBox54, comboBox55, comboBox56, comboBox57, comboBox58, comboBox59, comboBox60, comboBox61, comboBox62, comboBox63, comboBox64 };

                NumericUpDown[] intTeamPlayerLvlList = { numericUpDown1, numericUpDown2, numericUpDown3, numericUpDown4, numericUpDown5, numericUpDown6, numericUpDown7, numericUpDown8, numericUpDown9, numericUpDown10, numericUpDown11, numericUpDown12, numericUpDown13, numericUpDown14, numericUpDown15, numericUpDown16, numericUpDown17, numericUpDown18, numericUpDown19, numericUpDown20, numericUpDown21, numericUpDown22, numericUpDown23, numericUpDown24, numericUpDown25, numericUpDown26, numericUpDown27, numericUpDown28, numericUpDown29, numericUpDown30, numericUpDown31, numericUpDown32 };
                NumericUpDown[] intTeamPlayerNumberList = { numericUpDown33, numericUpDown34, numericUpDown35, numericUpDown36, numericUpDown37, numericUpDown38, numericUpDown39, numericUpDown40, numericUpDown41, numericUpDown42, numericUpDown43, numericUpDown44, numericUpDown45, numericUpDown46, numericUpDown47, numericUpDown48, numericUpDown49, numericUpDown50, numericUpDown51, numericUpDown52, numericUpDown53, numericUpDown54, numericUpDown55, numericUpDown56, numericUpDown57, numericUpDown58, numericUpDown59, numericUpDown60, numericUpDown61, numericUpDown62, numericUpDown63, numericUpDown64 };
                NumericUpDown[] intTeamPlayerPositionList = { numericUpDown65, numericUpDown66, numericUpDown67, numericUpDown68, numericUpDown69, numericUpDown70, numericUpDown71, numericUpDown72, numericUpDown73, numericUpDown74, numericUpDown75, numericUpDown76, numericUpDown77, numericUpDown78, numericUpDown79, numericUpDown80, numericUpDown81, numericUpDown82, numericUpDown83, numericUpDown84, numericUpDown85, numericUpDown86, numericUpDown87, numericUpDown88, numericUpDown89, numericUpDown90, numericUpDown91, numericUpDown92, numericUpDown93, numericUpDown94, numericUpDown95, numericUpDown96 };

                dropdownShopItems = dropdownShopItemList;
                dropdownTeamPlayers = dropdownTeamPlayersList;

                intTeamPlayerLvl = intTeamPlayerLvlList;
                intTeamPlayerNumber = intTeamPlayerNumberList;
                intTeamPlayerPosition = intTeamPlayerPositionList;

                DataGridViewComboBoxColumn playerBox = new DataGridViewComboBoxColumn();

                using (BinaryDataReader reader = new BinaryDataReader(_Unitbase))
                {
                    unitbase = reader.ReadMultipleStruct<Unitbase>((int)reader.Length / 0x60);
                    var players = unitbase.Select(x => x.GetName()).ToArray();
                    playerBox.Items.AddRange(players);
                    dropdownPlayerID.Items.AddRange(players);
                    dropdownBasePlayer.Items.AddRange(players);
                    dropdownAlterEgo.Items.AddRange(players);
                    dropdownTeamCaptain.Items.AddRange(players);
                    for (int i = 0; i < dropdownTeamPlayers.Length; i++)
                    {
                        dropdownTeamPlayers[i].Items.AddRange(players);
                    }
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Unitstat))
                {
                    unitstat = reader.ReadMultipleStruct<Unitstat>((int)reader.Length / 0x50);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_UnitbaseSTR))
                {
                    unitbaseSTR = reader.ReadMultipleStruct<UnitbaseSTR>((int)reader.Length / 0x80);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Unitchange))
                {
                    unitchange = reader.ReadMultipleStruct<Unitchange>((int)reader.Length / 0x04);
                    for (int i = 0; i < unitchange.Length; i++)
                    {
                        dropdownAlterEgoID.Items.Add($"Alter Ego {i + 1}");
                    }
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Unitno))
                {
                    unitno = reader.ReadMultipleStruct<Unitno>((int)reader.Length / 0x02);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Command))
                {
                    command = reader.ReadMultipleStruct<Command>((int)reader.Length / 0x1C);
                    var moves = command.Select(x => CommandSTR.GetText(_CommandSTR, x.MoveName)).ToArray();
                    dropdownMoveID.Items.AddRange(moves);
                    dropdownMove1.Items.AddRange(moves);
                    dropdownMove2.Items.AddRange(moves);
                    dropdownMove3.Items.AddRange(moves);
                    dropdownMove4.Items.AddRange(moves);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Item))
                {
                    item = reader.ReadMultipleStruct<Item>((int)(reader.Length) / 0x30);
                    var items = item.Select(x => x.GetName()).ToArray();
                    dropdownItemID.Items.AddRange(items);
                    dropdownTeamPlayerBoots.Items.AddRange(items);
                    dropdownTeamKeeperBoots.Items.AddRange(items);
                    dropdownTeamKeeperGloves.Items.AddRange(items);
                    for (int i = 0; i < dropdownShopItems.Length; i++)
                    {
                        dropdownShopItems[i].Items.AddRange(items);
                    }
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Shop))
                {
                    shop = reader.ReadMultipleStruct<Shop>((int)(reader.Length) / 0x40);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_ShopName))
                {
                    string text = reader.ReadString(System.Text.Encoding.UTF8).ToString();
                    shopName = ShopName.GetNames(text); // Assuming GetNames method is defined in ShopName class
                    dropdownShopID.Items.AddRange(shopName);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Team))
                {
                    team = reader.ReadMultipleStruct<Team>((int)(reader.Length) / 0x140);
                    var teams = team.Select(x => x.GetName()).ToArray();
                    dropdownTeamID.Items.AddRange(teams);
                    dropdownMatchTeamID.Items.AddRange(teams);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_TeamPKH))
                {
                    reader.Skip(0x30);
                    teamPKH = reader.ReadMultipleStruct<TeamPKH>((int)(reader.Length) / 0x4);
                }
                using (BinaryDataReader reader = new BinaryDataReader(_Games))
                {
                    games = reader.ReadMultipleStruct<Games>((int)(reader.Length) / 0x30);
                    intMatchID.Maximum = games.Length;
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

                using (BinaryDataReader reader = new BinaryDataReader(_ihPKH))
                {
                    reader.BaseStream.Position = 0x30;
                    ihPKH = reader.ReadMultipleStruct<PKB2.PKH1>((int)reader.Length / 0x4);
                    Console.WriteLine(ihPKH[1].data);
                }
            }
        }

        private void dropdownPlayerID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int playerID = dropdownPlayerID.SelectedIndex;
            strUnitIndex.Text = playerID.ToString();
            stringPlayerID.Text = unitno[playerID].PlayerID.ToString();

            textName.Text = unitbase[playerID].GetName();
            textNickname.Text = unitbase[playerID].GetNickname();
            intExpCurve.Value = unitbase[playerID].ExpCurve;
            intBodyType2D.Value = unitbase[playerID].BodyType2D;
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

            dropdownMove1.SelectedIndex = unitstat[playerID].Move1ID;
            dropdownMove2.SelectedIndex = unitstat[playerID].Move2ID;
            dropdownMove3.SelectedIndex = unitstat[playerID].Move3ID;
            dropdownMove4.SelectedIndex = unitstat[playerID].Move4ID;


            intMoveLvl1.Value = unitstat[playerID].Move1Lvl;
            intMoveLvl2.Value = unitstat[playerID].Move2Lvl;
            intMoveLvl3.Value = unitstat[playerID].Move3Lvl;
            intMoveLvl4.Value = unitstat[playerID].Move4Lvl;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int playerID = dropdownPlayerID.SelectedIndex;
            unitbase[playerID].UpdateName(textName.Text, false);
            unitbase[playerID].UpdateName(textNickname.Text, true);
            unitbase[playerID].ExpCurve = (byte)intExpCurve.Value;
            unitbase[playerID].BodyType2D = (byte)intBodyType2D.Value;
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

            unitstat[playerID].Move1ID = (short)dropdownMove1.SelectedIndex;
            unitstat[playerID].Move2ID = (short)dropdownMove2.SelectedIndex;
            unitstat[playerID].Move3ID = (short)dropdownMove3.SelectedIndex;
            unitstat[playerID].Move4ID = (short)dropdownMove4.SelectedIndex;

            unitstat[playerID].Move1Lvl = (byte)intMoveLvl1.Value;
            unitstat[playerID].Move2Lvl = (byte)intMoveLvl2.Value;
            unitstat[playerID].Move3Lvl = (byte)intMoveLvl3.Value;
            unitstat[playerID].Move4Lvl = (byte)intMoveLvl4.Value;

        }

        private void saveRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an array of streams
            Stream[] streams = {
                _Unitbase,
                _Unitstat,
                _UnitbaseSTR,
                _Command,
                _Unitchange,
                _Item,
                _Shop,
                _Team,
                _Games
                };

            // Create an array of data structures
            Array[] dataStructures = {
                unitbase,
                unitstat,
                unitbaseSTR,
                command,
                unitchange,
                item,
                shop,
                team,
                games
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
        private void dropdownPalskinInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgPalskinInterface.Image = palskinFace[dropdownPalskinFace.SelectedIndex];
        }
        private void dropdownPalskin2D_SelectedIndexChanged(object sender, EventArgs e)
        {
            imgPalskin2D.Image = palskin2D[dropdownPalskin2D.SelectedIndex];
        }
        private void label43_Click(object sender, EventArgs e)
        {

        }
        private void label48_Click(object sender, EventArgs e)
        {

        }
        private void dropdownMoveID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int moveID = dropdownMoveID.SelectedIndex;
            intMoveID.Text = moveID.ToString();
            dropdownMoveType.SelectedIndex = command[moveID].MoveType;
            intMoveFoulRate.Value = command[moveID].FoulRate;
            intMoveTPCost.Value = command[moveID].TpCost;
            intMovePower.Value = command[moveID].Power;
            intMovePlayerRequirement.Value = command[moveID].PlayerRequirement;
            dropdownMoveElement.SelectedIndex = command[moveID].Element;
            intMoveCoopAmount.Value = command[moveID].CoopAmount;
            intMoveCoopRequirementType.Value = command[moveID].CoopRecuirementCategory;
            intMoveCoopRequirement1.Value = command[moveID].CoopPlayerRequirement1;
            intMoveCoopRequirement2.Value = command[moveID].CoopPlayerRequirement2;
            dropdownMoveAttribute.SelectedIndex = command[moveID].MoveAttribute;
            intMoveLeveling.Value = command[moveID].Leveling;
            intMoveNameID.Value = command[moveID].MoveName;
            intMoveDescriptionID.Value = command[moveID].MoveDescription;
            intMoveAnimationID.Value = command[moveID].MoveAnimation;
            textMoveName.Text = CommandSTR.GetText(_CommandSTR, (int)intMoveNameID.Value, 0x20);
            textMoveDescription.Text = CommandSTR.GetText(_CommandSTR, (int)intMoveDescriptionID.Value, 0x80);
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        private void intMoveLvl3_ValueChanged(object sender, EventArgs e)
        {

        }
        private void buttonSaveMove_Click(object sender, EventArgs e)
        {
            int moveID = dropdownMoveID.SelectedIndex;
            CommandSTR.UpdateText(_CommandSTR, (int)intMoveNameID.Value, textMoveName.Text);
            CommandSTR.UpdateText(_CommandSTR, (int)intMoveDescriptionID.Value, textMoveDescription.Text, 0x80);
            command[moveID].MoveType = (byte)dropdownMoveType.SelectedIndex;
            command[moveID].FoulRate = (byte)intMoveFoulRate.Value;
            command[moveID].TpCost = (ushort)intMoveTPCost.Value;
            command[moveID].Power = (byte)intMovePower.Value;
            command[moveID].PlayerRequirement = (byte)intMovePlayerRequirement.Value;
            command[moveID].Element = (byte)dropdownMoveElement.SelectedIndex;
            command[moveID].CoopAmount = (byte)intMoveCoopAmount.Value;
            command[moveID].CoopRecuirementCategory = (byte)intMoveCoopRequirementType.Value;
            command[moveID].CoopPlayerRequirement1 = (byte)intMoveCoopRequirement1.Value;
            command[moveID].CoopPlayerRequirement2 = (byte)intMoveCoopRequirement2.Value;
            command[moveID].MoveAttribute = (byte)dropdownMoveAttribute.SelectedIndex;
            command[moveID].Leveling = (byte)intMoveLeveling.Value;
            command[moveID].MoveName = (ushort)intMoveNameID.Value;
            command[moveID].MoveDescription = (ushort)intMoveDescriptionID.Value;
            command[moveID].MoveAnimation = (int)intMoveAnimationID.Value;
        }
        private void dropdownAlterEgoID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int egoID = dropdownAlterEgoID.SelectedIndex;

            for (int j = 0; j < unitno.Length; j++)
            {
                if (unitchange[egoID].BasePlayer == unitno[j].PlayerID)
                {
                    dropdownBasePlayer.SelectedIndex = j;
                    break;
                }
            }
            for (int j = 0; j < unitno.Length; j++)
            {
                if (unitchange[egoID].AlterEgo == unitno[j].PlayerID)
                {
                    dropdownAlterEgo.SelectedIndex = j;
                    break;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int egoID = dropdownAlterEgoID.SelectedIndex;
            unitchange[egoID].BasePlayer = unitno[dropdownBasePlayer.SelectedIndex].PlayerID;
            unitchange[egoID].AlterEgo = unitno[dropdownAlterEgo.SelectedIndex].PlayerID;
        }
        private void label71_Click(object sender, EventArgs e)
        {

        }
        private void dropdownItemID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int itemID = dropdownItemID.SelectedIndex;
            intItemID.Text = itemID.ToString();
            textItemName.Text = item[itemID].GetName();
            int descriptionLength = (item[itemID + 1].Description - item[itemID].Description) * 0x20;
            textItemDescription.Text = ItemSTR.GetText(_ItemSTR, item[itemID].Description, descriptionLength);
            intItemType.Value = item[itemID].Type;
            intItemData1.Value = item[itemID].ItemData1;
            intItemData2.Value = item[itemID].ItemData2;
            intItemPrice.Value = item[itemID].Price;
            intItemLockCondition.Value = item[itemID].LockByte;
            intItemDescriptionID.Value = item[itemID].Description;
            intItemIcon.Value = item[itemID].Image;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < item.Length; i++)
            {
                item[i].LockByte = 0x00;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < command.Length; i++)
            {
                command[i].PlayerRequirement = 0x00;
            }
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < command.Length; i++)
            {
                command[i].CoopRecuirementCategory = 0x00;
                command[i].CoopPlayerRequirement1 = 0x00;
                command[i].CoopPlayerRequirement2 = 0x00;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < command.Length; i++)
            {
                command[i].Element = 0x00;
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < unitbase.Length; i++)
            {
                unitbase[i].Element = 0x00;
            }
        }

        private void dropdownShopID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ShopID = dropdownShopID.SelectedIndex;
            textShopName.Text = shopName[ShopID];
            for (int i = 0; i < dropdownShopItems.Length; i++)
            {
                dropdownShopItems[i].SelectedIndex = shop[ShopID].Item[i];
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int ShopID = dropdownShopID.SelectedIndex;
            for (int i = 0; i < dropdownShopItems.Length; i++)
            {
                shop[ShopID].Item[i] = (ushort)dropdownShopItems[i].SelectedIndex;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int itemID = dropdownItemID.SelectedIndex;
            int descriptionLength = (item[itemID + 1].Description - item[itemID].Description) * 0x20;
            item[itemID].UpdateName(textItemName.Text);
            ItemSTR.UpdateText(_ItemSTR, itemID, textItemDescription.Text, descriptionLength);
            item[itemID].Type = (byte)intItemType.Value;
            item[itemID].ItemData1 = (ushort)intItemData1.Value;
            item[itemID].ItemData2 = (ushort)intItemData2.Value;
            item[itemID].Price = (ushort)intItemPrice.Value;
            item[itemID].LockByte = (byte)intItemLockCondition.Value;
            item[itemID].Description = (ushort)intItemDescriptionID.Value;
            item[itemID].Image = (ushort)intItemIcon.Value;
        }
        private void buttonAddAlterEgo_Click(object sender, EventArgs e)
        {
            Array.Resize(ref unitchange, unitchange.Length + 1);
            dropdownAlterEgoID.Items.Clear();
            for (int i = 0; i < unitchange.Length; i++)
            {
                dropdownAlterEgoID.Items.Add($"Alter Ego {i + 1}");
            }
        }

        private void dropdownTeamID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int TeamID = dropdownTeamID.SelectedIndex;
            stringTeamID.Text = teamPKH[TeamID].TeamID.ToString();
            intTeamEmblem.Value = team[TeamID].Sprites;
            dropdownTeamPlayerBoots.SelectedIndex = team[TeamID].BootsPlayer;
            dropdownTeamKeeperBoots.SelectedIndex = team[TeamID].BootsGoalkeeper;
            dropdownTeamKeeperGloves.SelectedIndex = team[TeamID].GlovesGoalkeeper;
            intTeamFormation.Value = team[TeamID].Formation;
            intTeamKit.Value = team[TeamID].Kit;

            for (int j = 0; j < unitno.Length; j++)
            {
                if (team[TeamID].Captain == unitno[j].PlayerID)
                {
                    dropdownTeamCaptain.SelectedIndex = j;
                    break;
                }
            }
            for (int i = 0; i < dropdownTeamPlayers.Length; i++)
            {
                for (int j = 0; j < unitno.Length; j++)
                {
                    if (team[TeamID].Players[i].ID == unitno[j].PlayerID)
                    {
                        dropdownTeamPlayers[i].SelectedIndex = j;
                        intTeamPlayerLvl[i].Value = team[TeamID].Players[i].LevelBoost;
                        intTeamPlayerNumber[i].Value = team[TeamID].Players[i].Number;
                        intTeamPlayerPosition[i].Value = team[TeamID].Players[i].Position;
                        break;
                    }
                }
            }
        }

        private void label86_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {

        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            int TeamID = dropdownTeamID.SelectedIndex;

            team[TeamID].Sprites = (ushort)intTeamEmblem.Value;
            team[TeamID].BootsPlayer = (ushort)dropdownTeamPlayerBoots.SelectedIndex;
            team[TeamID].BootsGoalkeeper = (ushort)dropdownTeamKeeperBoots.SelectedIndex;
            team[TeamID].GlovesGoalkeeper = (ushort)dropdownTeamKeeperGloves.SelectedIndex;
            team[TeamID].Formation = (ushort)intTeamFormation.Value;
            team[TeamID].Kit = (ushort)intTeamKit.Value;

            for (int i = 0; i < dropdownTeamPlayers.Length; i++)
            {
                team[TeamID].Players[i].ID = unitno[dropdownTeamPlayers[i].SelectedIndex].PlayerID;
                team[TeamID].Players[i].LevelBoost = (byte)intTeamPlayerLvl[i].Value;
                team[TeamID].Players[i].Number = (byte)intTeamPlayerNumber[i].Value;
                team[TeamID].Players[i].Position = (byte)intTeamPlayerPosition[i].Value;
            }
        }

        private void numericUpDown97_ValueChanged(object sender, EventArgs e)
        {
            int GameID = (int)intMatchID.Value;

            for (int j = 0; j < teamPKH.Length; j++)
            {
                if (games[GameID].TeamID == teamPKH[j].TeamID)
                {
                    try
                    {
                        dropdownMatchTeamID.SelectedIndex = j;
                    }
                    catch (Exception)
                    {
                        dropdownMatchTeamID.SelectedIndex = 0;
                    }
                    break;
                }
            }

            intMatchTeamLevel.Value = games[GameID].TeamLevel;
            intMatchType.Value = games[GameID].MatchType;
            intMatchStadium.Value = games[GameID].Stadium;
            intMatchForceKit.Value = games[GameID].ForceKit;
            intMatchDrops.Value = games[GameID].LootTable;
            intMatchHalfStart.Value = games[GameID].HalfStart;
            intMatchDuplicateRemover.Value = games[GameID].DuplicateRemover;
            intMatchScreen.Value = games[GameID].MatchScreens;
            intMatchOST.Value = games[GameID].OST;
            intMatchEvent.Value = games[GameID].MatchEvent;
        }

        private void buttonSaveMatch_Click(object sender, EventArgs e)
        {
            int GameID = (int)intMatchID.Value;
            games[GameID].TeamID = (ushort)teamPKH[dropdownMatchTeamID.SelectedIndex].TeamID;

            games[GameID].TeamLevel = (byte)intMatchTeamLevel.Value;
            games[GameID].MatchType = (byte)intMatchType.Value;
            games[GameID].Stadium = (byte)intMatchStadium.Value;
            games[GameID].ForceKit = (ushort)intMatchForceKit.Value;
            games[GameID].LootTable = (byte)intMatchDrops.Value;
            games[GameID].HalfStart = (byte)intMatchHalfStart.Value;
            games[GameID].DuplicateRemover = (byte)intMatchDuplicateRemover.Value;
            games[GameID].MatchScreens = (byte)intMatchScreen.Value;
            games[GameID].OST = (byte)intMatchOST.Value;
            games[GameID].MatchEvent = (byte)intMatchEvent.Value;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < command.Length; i++)
            {
                command[i].Leveling = 0x01;
            }
        }

        private void textItemDescription_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
