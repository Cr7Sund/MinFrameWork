using Serilog.Events;
using Serilog.Core;

namespace Cr7Sund.Logger
{
    /// <summary>
    /// An instance of this sink may be substituted when an instance of the <see cref="UDPSink"/>
    /// is unable to be constructed.
    /// </summary>
    internal class NullSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
        }
    }
}