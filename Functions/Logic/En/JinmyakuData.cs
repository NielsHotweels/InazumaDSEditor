using InazumaDSEditor.Tools.Images;
using InazumaDSEditor.Tools;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Inazuma.Functions.Logic.En;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace InazumaDSEditor.Functions.Logic.En
{
    public enum ReqType : ushort
    {
        PlayerLock = 0x00,
        ChapterLock = 0x02,
    }
    public class JinmyakuData
    {
        public List<BaseNodeUI> Nodes { get; set; } = new List<BaseNodeUI>();
        public List<LinkData> Links { get; set; } = new List<LinkData>();
        public List<TextData> Texts { get; set; } = new List<TextData>();

        public class JinmyakuMapContainer
        {
            public List<JinmyakuData.BaseNodeUI> Nodes { get; set; }
            public List<JinmyakuData.LinkData> Links { get; set; }
            public List<TextData> Texts { get; set; }
        }

        [JsonDerivedType(typeof(PlayerNodeUI), typeDiscriminator: "Player")]
        [JsonDerivedType(typeof(RequirementNodeUI), typeDiscriminator: "Requirement")]
        public abstract class BaseNodeUI
        {
            public byte NodeType { get; set; }
            public byte PageID { get; set; }
        }

        public class PlayerNodeUI : BaseNodeUI
        {
            public RawPlayerNode Data { get; set; }
        }

        public class RequirementNodeUI : BaseNodeUI
        {
            public RequirementHeader Data { get; set; }
            public string RequirementText { get; set; } = "" ;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RawPlayerNode
        {
            public ushort Unk1 { get; set; }
            public ushort FPCost { get; set; }
            public ushort XPos { get; set; }
            public ushort YPos { get; set; }
            public ushort PlayerId { get; set; }
            public ushort Unk3 { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RequirementHeader
        {
            public ushort Unk1 { get; set; }
            public ushort Unk2 { get; set; }
            public ushort XPos { get; set; }
            public ushort YPos { get; set; }
            public ushort Unk5 { get; set; }
            public ushort PlayerId { get; set; }
            //00 = player lock 02 = chapter lock
            public ushort RequirementType { get; set; }

            public ushort Unk7 { get; set; }
            public ushort TextLength;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LinkData
        {
            public byte Source { get; set; }
            public byte Target { get; set; }
        }


        public class TextData
        {
            public TextHeader data { get; set; }
            public string text { get; set; } = "";
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TextHeader
        {
            public byte ID { get; set; }
            public byte Length;
        }

        public void ReadFile(Stream stream, Unitbase[] unitbase, Unitno[] unitno)
        {
            using (BinaryDataReader reader = new BinaryDataReader(stream))
            {
                byte pageID;
                byte nodeType;

                int nodeCount = reader.BaseStream.ReadByte();
                reader.Skip(1);
                Console.WriteLine("Jinmyaku node count: " + nodeCount);

                for (int i = 0; i < nodeCount; i++)
                {
                    pageID = reader.ReadValue<byte>();
                    nodeType = reader.ReadValue<byte>();
                    switch (nodeType)
                    {
                        case < 0x10:
                            string playerName = "";
                            RawPlayerNode playerNode = reader.ReadStruct<RawPlayerNode>();
                            playerName = unitbase[DataHandling.GetUnitIDFromPlayerID(playerNode.PlayerId, unitno)].GetName();
                            Nodes.Add(new PlayerNodeUI { NodeType = nodeType, Data = playerNode, PageID = pageID});
                            Console.WriteLine(
                               "node: Player" +
                               " ID: " + i +
                               " XPos: " + playerNode.XPos +
                               " YPos: " + playerNode.YPos +
                               " Name: " + playerName +
                               " Cost: " + playerNode.FPCost + "FP " +
                               " Unk1: " + playerNode.Unk1 +
                               " Unk3: " + playerNode.Unk3 +
                               " Page ID: " + pageID);
                                break;
                        case > 0x10:
                            RequirementHeader reqNode = reader.ReadStruct<RequirementHeader>();
                            for (int j = 0; j < unitbase.Length; j++)
                            {
                                if (unitbase[j].PlayerID1 == reqNode.PlayerId)
                                {
                                    playerName = unitbase[j].GetName();
                                    break;
                                }
                            }
                            string nodeText = Encoding.UTF8.GetString(reader.GetSection(reqNode.TextLength));
                            Nodes.Add(new RequirementNodeUI { NodeType = nodeType, Data = reqNode, PageID = pageID, RequirementText = nodeText });
                            Console.WriteLine(
                                "node: Requirement" +
                                " ID: " + i +
                                " Unk1: " + reqNode.Unk1 +
                                " Unk2: " + reqNode.Unk2 +
                                " XPos: " + reqNode.XPos +
                                " YPos: " + reqNode.YPos +
                                " Unk5: " + reqNode.Unk5 +
                                " ReqID: " + reqNode.PlayerId +
                                " Req Type: " + reqNode.RequirementType +
                                " Unk7: " + reqNode.Unk7 +
                                " Uncomp txt id: " + nodeType +
                                " Page ID: " + pageID);
                            Console.WriteLine(nodeText);
                            break;

                        default:
                            Console.WriteLine("Unable to read node: " + i + ", invalid node type: 0x" + nodeType.ToString("X"));
                            break;
                    }
                }

                int linkCount = reader.BaseStream.ReadByte();
                reader.Skip(1);
                Console.WriteLine("Jinmyaku link count: " + linkCount);
                
                for (int i = 0; i < linkCount; i++)
                {
                    LinkData link = reader.ReadStruct<LinkData>();

                    Console.WriteLine("Connection from " + link.Source + " to " + link.Target);

                    Links.Add(link);
                }

                int textCount = reader.BaseStream.ReadByte();


                for(int i = 0; i < textCount; i++) 
                {
                    TextHeader textData = reader.ReadStruct<TextHeader>();
                    string text = Encoding.UTF8.GetString(reader.GetSection(textData.Length));
                    Console.WriteLine(text + " ID: " + textData.ID);

                    Texts.Add(new TextData { data = textData, text = text});
                }
            }
        }
        public void JsonExport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Configure JSON to be pretty-printed/indented instead of one long line
                    var options = new JsonSerializerOptions { WriteIndented = true };

                    var combinedData = new
                    {
                        Nodes = Nodes,
                        Links = Links,
                        Texts = Texts
                    };

                    // 3. Serialize the combined object (creates a single clean json file!)
                    string completeJson = JsonSerializer.Serialize(combinedData, options);

                    // 4. Write it all in one go
                    File.WriteAllText(saveFileDialog.FileName, completeJson);

                    MessageBox.Show("Connection map exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export JSON file:\n{ex.Message}", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void JsonImport()
        {
            // 1. Set up the Open File Dialog
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Title = "Import Connection Map from JSON";
                openDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                openDialog.RestoreDirectory = true;

                // 2. If the user picked a file and clicked "Open"
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read the raw text strings from the file
                        string jsonString = File.ReadAllText(openDialog.FileName);
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        // Deserialize the text back into our strongly-typed polymorphic list
                        var importedMap = JsonSerializer.Deserialize<JinmyakuMapContainer>(jsonString, options);
                        if (importedMap != null)
                        {
                            this.Nodes = importedMap.Nodes ?? new List<BaseNodeUI>();
                            this.Links = importedMap.Links ?? new List<LinkData>();
                            this.Texts = importedMap.Texts ?? new List<TextData>();

                            MessageBox.Show($"Successfully imported {importedMap.Nodes.Count} nodes, {importedMap.Links.Count} links and {importedMap.Texts.Count} texts from JSON!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to import JSON file. Make sure the structure is correct!\n\nDetails: {ex.Message}", "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public Stream CompileToStream()
        {
            MemoryStream ms = new MemoryStream();

            // Instantiate your custom writer WITHOUT a using block so it doesn't kill the stream!
            BinaryDataWriter writer = new BinaryDataWriter(ms);

            // ==========================================
            // SECTION 1: WRITE NODES
            // ==========================================
            writer.Write((byte)Nodes.Count);
            writer.Write((byte)0x00);

            foreach (var baseNode in Nodes)
            {
                writer.Write(baseNode.PageID);
                writer.Write(baseNode.NodeType);

                if (baseNode is PlayerNodeUI playerNode)
                {
                    // BOOM! Your custom writer handles the whole struct in one line!
                    writer.WriteStruct(playerNode.Data);
                }
                else if (baseNode is RequirementNodeUI reqNode)
                {
                    byte[] textBytes = Encoding.UTF8.GetBytes(reqNode.RequirementText);

                    // Update the string length in the header dynamically
                    var header = reqNode.Data;
                    header.TextLength = (ushort)textBytes.Length;

                    // Write the header struct, then the text bytes immediately after
                    writer.WriteStruct(header);
                    writer.Write(textBytes);
                }
            }

            // ==========================================
            // SECTION 2: WRITE LINKS
            // ==========================================
            writer.Write((byte)Links.Count);
            writer.Write((byte)0x00);

            foreach (var link in Links)
            {
                writer.WriteStruct(link); // 1-line struct writing!
            }

            // ==========================================
            // SECTION 3: WRITE GLOBAL SCOUT TEXT POOL
            // ==========================================
            writer.Write((byte)Texts.Count);

            foreach (var textItem in Texts)
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(textItem.text);

                // Update the length dynamically just in case
                var header = textItem.data;
                header.Length = (byte)textBytes.Length;

                // Write header struct, then the string
                writer.WriteStruct(header);
                writer.Write(textBytes);
            }

            // Rewind the stream so the Form can read it from the beginning
            ms.Position = 0;

            // We do NOT call writer.Dispose() here! We let the garbage collector clean up the 
            // writer later, leaving our MemoryStream perfectly intact to be returned.
            return ms;
        }
    }
}
