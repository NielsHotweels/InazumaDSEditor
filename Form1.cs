namespace Inazuma
{
    using Inazuma.Functions.Logic.En;
    using InazumaDSEditor.Functions;
    using InazumaDSEditor.Functions.JSON_Structures;
    using InazumaDSEditor.Functions.Logic.En;
    using InazumaDSEditor.Tools;
    using InazumaDSEditor.Tools.Images;
    using InazumaDSEditor.Tools.JSON;
    using Microsoft.VisualBasic.Devices;
    using Org.BouncyCastle.Asn1.Cmp;
    using Org.BouncyCastle.Utilities.IO;
    using SceneGate.Ekona.Compression;
    using SceneGate.Ekona.Containers.Rom;
    using SceneGate.Ekona.Security;
    using ScriptInazumaElevenDS.Level5.Archive.PKB;
    using ScriptInazumaElevenDS.Level5.Archive.PKH;
    using ScriptInazumaElevenDS.Level5.Compression.LZ10;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Runtime.Intrinsics.Arm;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Texim;
    using Texim.Images;
    using Texim.Images.Standard;
    using Texim.Palettes;
    using Texim.Pixels;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using static InazumaDSEditor.Functions.GameInfo;

    public partial class Form1 : Form
    {
        bool checkCancel = true; // tab control




        LZ10 compression = new LZ10();
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
        Stream _FieldInf;
        Stream _ItemSortList;
        Stream _I_Detail;
        Stream _JinmyakuData;

        //images
        PKB FacPKB;
        PKB IhPKB;
        PKB EffPKB;



        //logic
        Unitbase[] unitbase { get; set; }
        Unitstat[] unitstat { get; set; }
        UnitbaseSTR[] unitbaseSTR;

        List<Unitchange> unitchangelist = new List<Unitchange>();
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
        FieldInf[] fieldInf;
        ItemSortList[] itemSortList;
        I_Detail[] i_Detail;
        JinmyakuData jinmyakuData = new JinmyakuData();

        BinaryFormat binaryPng;

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
            if (game != null)
            {
                MessageBox.Show("Game already opened!");
                return;
            }
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

                if (gameInfo.Game == GameVerions.IE3)
                {
                    MessageBox.Show("no ie3 compatibility lol\nthink again sucker");
                }


                // 1. Retrieve the banner as an IndexedPaletteImage
                IndexedPaletteImage icon = game.Children["system"].Children["banner"].Children["icon"].GetFormatAs<IndexedPaletteImage>();

                // 2. FIX: Use the file format parameters class
                var bitmapConverter = new IndexedImage2BinaryPng(icon);

                // 3. FIX: Use the Bitmap converter to wrap the raw pixels into a file container
                using BinaryFormat binaryPng = bitmapConverter.Convert(icon);
                
                // 5. The stream can now be seamlessly processed by WinForms
                pictureBoxgameimg.Image = new Bitmap(binaryPng.Stream);

                Node data_iz = game.Children["data"].Children["data_iz"];
                Node effect3d = data_iz.Children["effect3d"];
                Node face2d = data_iz.Children["face2d"];
                Node logic = data_iz.Children["logic"];
                Node logiclang = logic.Children[gameInfo.Language.ToString()];
                Node Pic3d = data_iz.Children["pic3d"];

                if (gameInfo.Game == GameVerions.IE2)
                {
                    _Games = logic.Children["games.dat"].Stream;
                    _FieldInf = logic.Children["fieldinf.dat"].Stream;
                    PKH EffPKH = new PKH(effect3d.Children[gameInfo.Language.ToString()].Children["eff.pkh"].Stream);
                    EffPKB = new PKB(effect3d.Children[gameInfo.Language.ToString()].Children["eff.pkb"].Stream, EffPKH);
                }
                else if (gameInfo.Game == GameVerions.IE1)
                {
                    _Games = logiclang.Children["games.dat"].Stream;
                    _FieldInf = logiclang.Children["fieldinf.dat"].Stream;
                    PKH EffPKH = new PKH(effect3d.Children["eff.pkh"].Stream);
                    EffPKB = new PKB(effect3d.Children["eff.pkb"].Stream, EffPKH);
                }

                _Unitbase = logiclang.Children["unitbase.dat"].Stream;
                _Unitstat = logiclang.Children["unitstat.dat"].Stream;
                _UnitbaseSTR = logiclang.Children["unitbase.STR"].Stream;
                _Unitno = logiclang.Children["unitno.dat"].Stream;
                _Command = logiclang.Children["command.dat"].Stream;
                _CommandSTR = logiclang.Children["command.STR"].Stream;
                _Item = logiclang.Children["item.dat"].Stream;
                _ItemSTR = logiclang.Children["item.STR"].Stream;
                _ShopName = logiclang.Children["ShopName.dat"].Stream;
                _Shop = logiclang.Children["shop.dat"].Stream;
                _Team = logiclang.Children["team.pkb"].Stream;
                _TeamPKH = logiclang.Children["team.pkh"].Stream;
                _ItemSortList = logiclang.Children["itemsortlist.dat"].Stream;
                _I_Detail = logiclang.Children["i_detail.dat"].Stream;
                _JinmyakuData = logiclang.Children["JinmyakuData.dat"].Stream;



                PKH FacPKH = new PKH(face2d.Children["fac.pkh"].Stream);
                FacPKB = new PKB(face2d.Children["fac.pkb"].Stream, FacPKH);

                PKH IhPKH = new PKH(Pic3d.Children["ih.pkh"].Stream, 0x08);
                IhPKB = new PKB(Pic3d.Children["ih.pkb"].Stream, IhPKH, 608);

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
                    dropdownConnMapPlayer.Items.AddRange(players);
                    for (int i = 0; i < dropdownTeamPlayers.Length; i++)
                    {
                        dropdownTeamPlayers[i].DataSource = (string[])players.Clone();
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
                    unitchangelist.AddRange(unitchange);

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
                    dropdownBinderTeam.Items.AddRange(teams);
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
                    intMatchID.Maximum = games.Length - 1;
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



                using (BinaryDataReader reader = new BinaryDataReader(_FieldInf))
                {
                    fieldInf = reader.ReadMultipleStruct<FieldInf>((int)reader.Length / 0x180);
                    dropdownMatchStadium.Items.AddRange(fieldInf.Select(x => x.GetName()).ToArray());
                }

                using (BinaryDataReader reader = new BinaryDataReader(_ItemSortList))
                {
                    itemSortList = reader.ReadMultipleStruct<ItemSortList>((int)reader.Length / 0x02);
                }

                using (BinaryDataReader reader = new BinaryDataReader(_I_Detail))
                {
                    i_Detail = reader.ReadMultipleStruct<I_Detail>((int)reader.Length / 0x10);
                    intEquipmentID.Maximum = i_Detail.Length;
                }

                var properties = typeof(Unitstat).GetProperties(); // Retrieve all public properties
                foreach (var property in properties)
                {
                    dropdownMassEditVariable.Items.Add(property.Name); // Use property.Name to get the property name
                }

                Console.WriteLine("reading Jinmyaku data");
                jinmyakuData.ReadFile(_JinmyakuData, unitbase, unitno);
                for (int i = 0; i < jinmyakuData.Nodes.Count; i++)
                {
                    var currentNode = jinmyakuData.Nodes[i];

                    if (currentNode.NodeType <= 0x10)
                    {
                        var playerNode = (JinmyakuData.PlayerNodeUI)currentNode;
                        listBox1.Items.Add(unitbase[DataHandling.GetUnitIDFromPlayerID(playerNode.Data.PlayerId, unitno)].GetName());
                    }
                    else if (currentNode.NodeType > 0x10)
                    {
                        var reqNode = (JinmyakuData.RequirementNodeUI)currentNode;
                        listBox1.Items.Add(reqNode.RequirementText);
                    }
                }
                MessageBox.Show("Rom succesfully loaded\nGame: " + gameInfo.Game);
            }
        }

        private void dropdownPlayerID_SelectedIndexChanged(object sender, EventArgs e)
        {
            int playerID = dropdownPlayerID.SelectedIndex;
            strUnitIndex.Text = playerID.ToString();
            intPlayerID.Value = unitno[playerID].PlayerID;

            textName.Text = unitbase[playerID].GetName();
            textNickname.Text = unitbase[playerID].GetNickname();
            intExpCurve.Value = unitbase[playerID].ExpCurve;
            intStandardKit.Value = unitbase[playerID].StandardKit;
            intBodyType2D.Value = unitbase[playerID].BodyType2D;
            intBodyType3D.Value = unitbase[playerID].BodyType3D;
            dropdownPalskinFace.SelectedIndex = unitbase[playerID].PalskinFace;
            dropdownPalskin3D.SelectedIndex = unitbase[playerID].SkinTone3D;
            dropdownPalskin2D.SelectedIndex = unitbase[playerID].SkinTone2D;
            dropdownGender.SelectedIndex = unitbase[playerID].Gender - 1;
            dropdownElement.SelectedIndex = unitbase[playerID].Element;
            dropdownPosition.SelectedIndex = (unitbase[playerID].Position / 0x20);
            for (int i = 0; i < teamPKH.Length; i++)
            {
                if (unitbase[playerID].BinderTeam == 0) break;
                if (teamPKH[i].TeamID == unitbase[playerID].BinderTeam)
                {
                    dropdownBinderTeam.SelectedIndex = i;
                    break;
                }
            }

            intYear.Value = unitbase[playerID].Age;
            intHeadSprite.Value = unitbase[playerID].HeadSpriteOW;
            intHeadPalette.Value = unitbase[playerID].HeadPaletteOW;
            dropdownBodyType.SelectedIndex = unitbase[playerID].BodyType;
            intDescriptionID.Value = unitbase[playerID].DescriptionID;
            intPlayerSprite.Value = unitbase[playerID].SpriteandModel;

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

            try
            {
                byte[] imgcomp = FacPKB.Directory.GetFileFromFullPath((unitbase[playerID].SpriteandModel * 100).ToString());
                byte[] img = compression.Decompress(imgcomp);
                Face2d fac = new Face2d();
                using (BinaryDataReader reader = new BinaryDataReader(img))
                {
                    fac = reader.ReadStruct<Face2d>();
                }
                NBFP nbfp = new NBFP(fac.nbfp);
                NBFC nbfc = new NBFC(fac.nbfc, nbfp);
                pictureBoxPlayerFac.Image = nbfc.ConvertToPNG(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Player has no sprite: {unitbase[playerID].GetName()}");
                pictureBoxPlayerFac.Image = null;
            }

            try
            {
                byte[] imgcomp = IhPKB.Directory.GetFileFromFullPath((unitbase[playerID].PlayerID1).ToString());
                byte[] img = compression.Decompress(imgcomp);
                MenuIcon ih = new MenuIcon();
                using (BinaryDataReader reader = new BinaryDataReader(img))
                {
                    ih = reader.ReadStruct<MenuIcon>();
                }
                NBFP nbfp = new NBFP(ih.nbfp);
                NBFC nbfc = new NBFC(ih.nbfc, nbfp);
                pictureBoxPlayerIcon.Image = nbfc.ConvertToPNG(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Player has no menu sprite: {unitbase[playerID].GetName()}");
                pictureBoxPlayerIcon.Image = null;
            }
        }

        private void intPlayerSprite_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                byte[] imgcomp = FacPKB.Directory.GetFileFromFullPath(((short)intPlayerSprite.Value * 100).ToString());
                byte[] img = compression.Decompress(imgcomp);
                Face2d fac = new Face2d();
                using (BinaryDataReader reader = new BinaryDataReader(img))
                {
                    fac = reader.ReadStruct<Face2d>();
                }
                NBFP nbfp = new NBFP(fac.nbfp);
                NBFC nbfc = new NBFC(fac.nbfc, nbfp);
                pictureBoxPlayerFac.Image = nbfc.ConvertToPNG(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spriteset does not exist: {intPlayerSprite.Value}");
                pictureBoxPlayerFac.Image = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int playerID = dropdownPlayerID.SelectedIndex;
            unitbase[playerID].UpdateName(textName.Text, false);
            unitbase[playerID].UpdateName(textNickname.Text, true);
            unitbase[playerID].ExpCurve = (byte)intExpCurve.Value;
            unitbase[playerID].StandardKit = (short)intStandardKit.Value;

            unitbase[playerID].BodyType2D = (byte)intBodyType2D.Value;
            unitbase[playerID].BodyType3D = (byte)intBodyType3D.Value;
            unitbase[playerID].PalskinFace = (byte)dropdownPalskinFace.SelectedIndex;
            unitbase[playerID].SkinTone3D = (byte)dropdownPalskin3D.SelectedIndex;
            unitbase[playerID].SkinTone2D = (byte)dropdownPalskin2D.SelectedIndex;
            unitbase[playerID].Gender = (byte)(dropdownGender.SelectedIndex + 0x01);
            unitbase[playerID].Element = (byte)dropdownElement.SelectedIndex;
            unitbase[playerID].Position = (byte)(dropdownPosition.SelectedIndex * 0x20);
            unitbase[playerID].Age = (byte)intYear.Value;
            unitbase[playerID].BinderTeam = (byte)dropdownBinderTeam.SelectedIndex;
            unitbase[playerID].HeadSpriteOW = (short)intHeadSprite.Value;
            unitbase[playerID].HeadPaletteOW = (short)intHeadPalette.Value;
            unitbase[playerID].BodyType = (byte)dropdownBodyType.SelectedIndex;
            unitbase[playerID].DescriptionID = (short)intDescriptionID.Value;
            unitbase[playerID].SpriteandModel = (ushort)intPlayerSprite.Value;

            unitno[playerID].PlayerID = (ushort)intPlayerID.Value;
            unitbase[playerID].PlayerID1 = (ushort)intPlayerID.Value;


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
                _Games,
                _I_Detail,
                _Unitno,
                };

            // Create an array of data structures
            Array[] dataStructures = {
                unitbase,
                unitstat,
                unitbaseSTR,
                command,
                unitchangelist.ToArray(),
                item,
                shop,
                team,
                games,
                i_Detail,
                unitno,
                };
            // Loop through each stream and corresponding data structure
            for (int i = 0; i < streams.Length; i++)
            {
                using (DataStream dataStream = DataStreamFactory.FromStream(streams[i]))
                {
                    int structSize = Marshal.SizeOf(dataStructures[i].GetType().GetElementType());

                    // Check if we need to extend the stream length
                    if (dataStream.Length != structSize * dataStructures[i].Length)
                    {
                        // Set the stream length to accommodate the data
                        dataStream.SetLength(structSize * dataStructures[i].Length);
                    }

                    // Write the current data structure to the corresponding DataStream
                    BinaryDataWriter writer = new BinaryDataWriter(dataStream);
                    writer.WriteMultipleStruct((dynamic)dataStructures[i]);
                }

            }

            using (Stream compiledStream = jinmyakuData.CompileToStream())
            {
                using (DataStream dataStream = DataStreamFactory.FromStream(_JinmyakuData))
                    {
                        // Clear out the old file payload completely
                        dataStream.SetLength(0);

                        // FIX: Reset the wrapper position pointer directly so it starts writing at index 0
                        dataStream.Position = 0;

                        // Safely stream the raw changes directly into the virtual game file record
                        compiledStream.CopyTo(dataStream);
                        Console.WriteLine($"Jinmyaku data saved successfully! Stream length: {dataStream.Length} bytes");
                    }
                }

            MessageBox.Show("ROM data saved successfully!", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            dropdownBasePlayer.SelectedIndex = DataHandling.GetUnitIDFromPlayerID(unitchangelist[egoID].BasePlayer, unitno);
            dropdownAlterEgo.SelectedIndex = DataHandling.GetUnitIDFromPlayerID(unitchangelist[egoID].AlterEgo, unitno);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            int egoID = dropdownAlterEgoID.SelectedIndex;
            unitchangelist[egoID] = new Unitchange { 
                BasePlayer = unitno[dropdownBasePlayer.SelectedIndex].PlayerID, 
                AlterEgo = unitno[dropdownAlterEgo.SelectedIndex].PlayerID
            };
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
            dropdownItemType.SelectedIndex = item[itemID].Type;
            intItemData1.Value = item[itemID].ItemData1;
            intItemData2.Value = item[itemID].ItemData2;
            intItemPrice.Value = item[itemID].Price;
            intItemLockCondition.Value = item[itemID].LockByte;
            intItemDescriptionID.Value = item[itemID].Description;
            intItemIcon.Value = item[itemID].Image;
            if (tabControlItemData.SelectedTab == Equipment)
            {
                UpdateEquipmentStatsTab((byte)(item[dropdownItemID.SelectedIndex].ItemData1 & 0xFF));
            }
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
            item[itemID].Type = (byte)dropdownItemType.SelectedIndex;
            item[itemID].ItemData1 = (ushort)intItemData1.Value;
            item[itemID].ItemData2 = (ushort)intItemData2.Value;
            item[itemID].Price = (ushort)intItemPrice.Value;
            item[itemID].LockByte = (byte)intItemLockCondition.Value;
            item[itemID].Description = (ushort)intItemDescriptionID.Value;
            item[itemID].Image = (ushort)intItemIcon.Value;

            //reload dropdown
            //var objSelected = dropdownItemID.SelectedText;
            //for (int i = 0; i < item.Length; i++)
            //{
            //    dropdownItemID.Items.Remove(i);
            //
            //}
            //dropdownItemID.Items.AddRange(item.Select(x => x.GetName()).ToArray());
            //dropdownItemID.SelectedText = objSelected;

            //save equipment data if selected item type
            if (tabControlItemData.SelectedTab == Equipment)
            {
                item[itemID].ItemData1 = (ushort)(
                    ((ushort)intItemData1.Value & ~0xFFFF) | ((int)intEquipmentID.Value & 0xFFFF));
                SaveEquipmentStatsTab();
            }
        }
        private void buttonAddAlterEgo_Click(object sender, EventArgs e)
        {
            unitchangelist.Add(new Unitchange());
            dropdownAlterEgoID.Items.Add("Alter Ego" + unitchangelist.Count);
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

            dropdownTeamCaptain.SelectedIndex = DataHandling.GetUnitIDFromPlayerID(team[TeamID].Captain, unitno);
            for (int i = 0; i < dropdownTeamPlayers.Length; i++)
            {
                dropdownTeamPlayers[i].SelectedIndex = DataHandling.GetUnitIDFromPlayerID(team[TeamID].Players[i].ID, unitno);
                intTeamPlayerLvl[i].Value = team[TeamID].Players[i].LevelBoost;
                intTeamPlayerNumber[i].Value = team[TeamID].Players[i].Number;
                intTeamPlayerPosition[i].Value = team[TeamID].Players[i].Position;
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
            dropdownMatchStadium.SelectedIndex = games[GameID].Stadium;
            intMatchForceKit.Value = games[GameID].ForceKit;
            intMatchDrops.Value = games[GameID].LootTable;
            dropdownMatchHalfStart.SelectedIndex = games[GameID].HalfStart - 1;
            dropdownMatchDuplicateRemover.SelectedIndex = games[GameID].DuplicateRemover;
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
            games[GameID].Stadium = (byte)dropdownMatchStadium.SelectedIndex;
            games[GameID].ForceKit = (ushort)intMatchForceKit.Value;
            games[GameID].LootTable = (byte)intMatchDrops.Value;
            games[GameID].HalfStart = (byte)(dropdownMatchHalfStart.SelectedIndex + 1);
            games[GameID].DuplicateRemover = (byte)dropdownMatchDuplicateRemover.SelectedIndex;
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

        private void label109_Click(object sender, EventArgs e)
        {

        }

        private void buttonExportPlayer_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var combinedData = new
                {
                    _unitbase = unitbase[dropdownPlayerID.SelectedIndex],
                    _unitstat = unitstat[dropdownPlayerID.SelectedIndex],
                    _unitbaseSTR = unitbaseSTR[dropdownPlayerID.SelectedIndex],
                };

                string json = JsonSerializer.Serialize(combinedData, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(saveFileDialog.FileName, json);

                MessageBox.Show("Player data exported successfully!");
            }
        }

        private void buttonImportPlayer_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var importedData = JsonSerializer.Deserialize<PlayerData>(json);

                if (importedData == null)
                {
                    MessageBox.Show("Failed to import player data. Please check the JSON format.");
                    return;
                }

                int index = dropdownPlayerID.SelectedIndex;

                if (index < 0 || index >= unitbase.Length || index >= unitstat.Length)
                {
                    MessageBox.Show("Invalid player selection.");
                    return;
                }

                PropertyUpdater.UpdateProperties(ref unitbase[index], importedData._unitbase);
                PropertyUpdater.UpdateProperties(ref unitstat[index], importedData._unitstat);
                PropertyUpdater.UpdateProperties(ref unitbaseSTR[index], importedData._unitbaseSTR);

                dropdownPlayerID_SelectedIndexChanged(sender, e);

                MessageBox.Show($"Imported Player Data:\nName: {unitbase[index].GetName()}\nTotal Stats: {unitstat[index].TotalStats}");
            }
        }

        private void comboBox65_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonExportBanner_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG files (*.png)|*.png",
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                binaryPng.Stream.WriteTo(saveFileDialog.FileName);
                MessageBox.Show("Player data exported successfully!");
            }

        }

        private void buttonImportBanner_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG files (*.png)|*.png",
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmap = new Bitmap(openFileDialog.FileName);
            }
        }

        private void label114_Click(object sender, EventArgs e)
        {

        }

        private void buttonExportSprite_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog { };
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < FacPKB.Directory.Files.Count; i++)
                {
                    byte[] imgcomp = FacPKB.Directory.Files.ElementAt(i).Value.ByteContent;
                    byte[] img = compression.Decompress(imgcomp);
                    Face2d fac = new Face2d();
                    using (BinaryDataReader reader = new BinaryDataReader(img))
                    {
                        fac = reader.ReadStruct<Face2d>();
                    }
                    NBFP nbfp = new NBFP(fac.nbfp);
                    NBFC nbfc = new NBFC(fac.nbfc, nbfp);
                    Bitmap bitmap = new Bitmap(nbfc.ConvertToPNG(false));
                    string filepath = Path.Combine(folderDialog.SelectedPath, FacPKB.Directory.Files.ElementAt(i).Key.ToString() + ".png");
                    bitmap.Save(filepath, ImageFormat.Png);
                }
            }
        }

        private void buttonExportPlayerIcon_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog { };
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < IhPKB.Directory.Files.Count; i++)
                {
                    byte[] imgcomp = IhPKB.Directory.Files.ElementAt(i).Value.ByteContent;
                    byte[] img = compression.Decompress(imgcomp);
                    MenuIcon fac = new MenuIcon();
                    using (BinaryDataReader reader = new BinaryDataReader(img))
                    {
                        fac = reader.ReadStruct<MenuIcon>();
                    }
                    NBFP nbfp = new NBFP(fac.nbfp);
                    NBFC nbfc = new NBFC(fac.nbfc, nbfp);
                    Bitmap bitmap = new Bitmap(nbfc.ConvertToPNG(false));
                    string filepath = Path.Combine(folderDialog.SelectedPath, IhPKB.Directory.Files.ElementAt(i).Key.ToString() + ".png");
                    bitmap.Save(filepath, ImageFormat.Png);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < EffPKB.Directory.Files.Count; i++)
                {
                    byte[] imgcomp = IhPKB.Directory.Files.ElementAt(i).Value.ByteContent;
                    byte[] img = compression.Decompress(imgcomp);

                    // Define the output file path with a ".bin" extension or any other appropriate extension
                    string filepath = Path.Combine(folderDialog.SelectedPath, IhPKB.Directory.Files.ElementAt(i).Key.ToString() + ".nsbmd");

                    // Write the binary data directly to a file
                    File.WriteAllBytes(filepath, img);
                }
            }

        }

        private void buttonExportMovePlayerAnimation_Click(object sender, EventArgs e)
        {

        }

        private void dropdownItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateItemTypeTab();
        }
        private void UpdateItemTypeTab()
        {
            checkCancel = false;
            switch (dropdownItemType.SelectedIndex)
            {
                case 1 or 2 or 3:
                    tabControlItemData.SelectedTab = Healing;
                    intItemHealingAmount.Value = item[dropdownItemID.SelectedIndex].ItemData1;
                    break;
                case 14 or 17:
                    tabControlItemData.SelectedTab = Formation; break;
                case 15:
                    tabControlItemData.SelectedTab = Move; break;
                case 21:
                    tabControlItemData.SelectedTab = Kit; break;
                case 22 or 23 or 24 or 25:
                    tabControlItemData.SelectedTab = Equipment;
                    UpdateEquipmentStatsTab((byte)(item[dropdownItemID.SelectedIndex].ItemData1 & 0xFF));
                    break;
                default:
                    tabControlItemData.SelectedTab = None; break;
            }
            checkCancel = true;
        }

        private void UpdateEquipmentStatsTab(int statsID)
        {
            intEquipmentID.Value = statsID;
            intEquipmentFP.Value = i_Detail[statsID].FP;
            intEquipmentTP.Value = i_Detail[statsID].TP;
            intEquipmentKick.Value = i_Detail[statsID].Kick;
            intEquipmentBody.Value = i_Detail[statsID].Body;
            intEquipmentGuard.Value = i_Detail[statsID].Guard;
            intEquipmentSpeed.Value = i_Detail[statsID].Speed;
            intEquipmentStamina.Value = i_Detail[statsID].Stamina;
            intEquipmentGuts.Value = i_Detail[statsID].Guts;
            intEquipmentControl.Value = i_Detail[statsID].Control;
        }
        private void SaveEquipmentStatsTab()
        {
            int statsID = (int)intEquipmentID.Value;
            i_Detail[statsID].FP = (ushort)intEquipmentFP.Value;
            i_Detail[statsID].TP = (ushort)intEquipmentTP.Value;
            i_Detail[statsID].Kick = (byte)intEquipmentKick.Value;
            i_Detail[statsID].Body = (byte)intEquipmentBody.Value;
            i_Detail[statsID].Guard = (byte)intEquipmentGuard.Value;
            i_Detail[statsID].Speed = (byte)intEquipmentSpeed.Value;
            i_Detail[statsID].Stamina = (byte)intEquipmentStamina.Value;
            i_Detail[statsID].Guts = (byte)intEquipmentGuts.Value;
            i_Detail[statsID].Control = (byte)intEquipmentControl.Value;
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
        private void tabControlItemData_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = checkCancel;
            checkCancel = true;
        }

        private void intEquipmentID_ValueChanged(object sender, EventArgs e)
        {
            UpdateEquipmentStatsTab((int)intEquipmentID.Value);
        }

        private void buttonSaveEquipment_Click(object sender, EventArgs e)
        {
            SaveEquipmentStatsTab();
        }

        private void buttonMassEditApply_Click(object sender, EventArgs e)
        {
            var properties = typeof(Unitstat).GetProperties();

            var property = properties[dropdownMassEditVariable.SelectedIndex];
            for (int i = 0; i < unitstat.Length; i++)
            {
                property.SetValue(unitstat[i], (byte)intMassEditSetPoint.Value);

            }

        }

        private void comboBox65_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void ConnectionMap_Selecting(object sender, TabControlCancelEventArgs e)
        {

        }

        private void label132_Click(object sender, EventArgs e)
        {

        }
        
        private void numericUpDown97_ValueChanged_2(object sender, EventArgs e)
        {

        }

        private void label136_Click(object sender, EventArgs e)
        {

        }

        private void label138_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var node = jinmyakuData.Nodes[(byte)listBox1.SelectedIndex];
            intNodeType.Value = node.NodeType;
            intConnMapPageID.Value = node.PageID;

            if (node.NodeType <= 0x10)
            {
                var playerNode = (JinmyakuData.PlayerNodeUI)node;
                tabControlConnMapNodeType.SelectedIndex = 0;
                intConnMapXPos.Value = playerNode.Data.XPos;
                intConnMapYPos.Value = playerNode.Data.YPos;
                IntNodeFPCost.Value = playerNode.Data.FPCost;
                dropdownConnMapPlayer.SelectedIndex = DataHandling.GetUnitIDFromPlayerID(playerNode.Data.PlayerId, unitno);
            }
            else
            {
                var reqNode = (JinmyakuData.RequirementNodeUI)node;
                tabControlConnMapNodeType.SelectedIndex = 1;
                intConnMapXPos.Value = reqNode.Data.XPos;
                intConnMapYPos.Value = reqNode.Data.YPos;
            }
        }

        private void tabPage18_Click(object sender, EventArgs e)
        {

        }

        private void buttonExportConnJson_Click(object sender, EventArgs e)
        {
            jinmyakuData.JsonExport();
        }

        private void buttonImportConnMap_Click(object sender, EventArgs e)
        {
            jinmyakuData.JsonImport();

            listBox1.Items.Clear();
            for (int i = 0; i < jinmyakuData.Nodes.Count; i++)
            {
                var currentNode = jinmyakuData.Nodes[i];

                if (currentNode.NodeType <= 0x10)
                {
                    var playerNode = (JinmyakuData.PlayerNodeUI)currentNode;
                    listBox1.Items.Add(unitbase[DataHandling.GetUnitIDFromPlayerID(playerNode.Data.PlayerId, unitno)].GetName());
                }
                else if (currentNode.NodeType > 0x10)
                {
                    var reqNode = (JinmyakuData.RequirementNodeUI)currentNode;
                    listBox1.Items.Add(reqNode.RequirementText);
                }
            }
        }

        private void Testbutton_Click(object sender, EventArgs e)
        {
            // Sanity check: ensure data exists
            if (jinmyakuData == null || jinmyakuData.Nodes.Count == 0)
            {
                MessageBox.Show("No connection map data loaded to compile!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
    }
}

