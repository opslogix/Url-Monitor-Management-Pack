using System;
using System.Diagnostics;

namespace OpsLogix.IMP.Url.Shared.Services
{
    /// <summary>
    /// Event Viewer logger implementation
    /// </summary>
    public class TraceLogger : ILogger
    {
        private readonly TraceSource _traceSource;

        public TraceLogger(string source, string logName)
        {
            if (!EventLog.SourceExists(source))
            {
                var sourceInfo = new EventSourceCreationData(source, logName);
                EventLog.CreateEventSource(sourceInfo);
            }

            _traceSource = new TraceSource(source);
            _traceSource.Listeners.Add(new EventLogTraceListener(source));
        }

        public void WriteLog(TraceEventType type, int id, string message)
        {
            if (message.Length > 30000)
                message = message.Substring(0, Math.Min(30000, message.Length));

            _traceSource.TraceEvent(type, id, message);
        }
    }
}
