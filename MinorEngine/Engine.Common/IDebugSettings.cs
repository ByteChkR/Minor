using System.Text;

namespace Engine.Common
{
    public interface IDebugSettings
    {
        bool Enabled { get; set; }
        bool SendInternalWarnings { get; set; }
        bool SearchForUpdates { get; set; }
        int InternalUpdateMask { get; set; }
        int PrefixLookupFlags { get; set; }
        int SeverityFilter { get; set; }

        string[] StageNames { get; set; }
        ILogStreamSettings[] Streams { get; set; }
    }
}