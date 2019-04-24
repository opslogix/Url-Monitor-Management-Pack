using System.Diagnostics;

namespace OpsLogix.IMP.Url.Shared.Services
{
    public interface ILogger
    {
        void WriteLog(TraceEventType type, int id, string message);
    }
}
