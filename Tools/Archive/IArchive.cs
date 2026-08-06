using System.Windows.Forms;
using ScriptInazumaElevenDS.Tools;

namespace ScriptInazumaElevenDS.Level5.Compression
{
    public interface IArchive
    {
        string Name { get; }

        VirtualDirectory Directory { get; set; }

        void Save(string path, ProgressBar progressBar);

        void Close();
    }
}
