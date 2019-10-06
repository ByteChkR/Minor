using System.Text;

namespace Common
{
    public interface IDebugSettings
    {
        bool Enabled { get; set; }
        bool SendInternalWarnings { get; set; }
        bool SearchForUpdates { get; set; }
        int InternalUpdateMask { get; set; }
        int PrefixLookupFlags { get; set; }
        string[] StageNames { get; set; }
        Encoding LogEncoding { get; set; }
        ILogStreamSettings[] Streams { get; set; }
    }
}