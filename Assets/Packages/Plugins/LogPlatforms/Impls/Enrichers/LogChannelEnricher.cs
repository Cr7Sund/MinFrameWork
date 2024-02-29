using System;
using Serilog.Core;
using Serilog.Events;

namespace Cr7Sund.Logger
{
    public class LogChannelEnricher : ILogEventEnricher
    {
        public const string PropertyName = "LogChannel";
        private string _logChannel;

        public LogChannelEnricher(string logChannel)
        {
            _logChannel = logChannel;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var channelProp = propertyFactory.CreateProperty(PropertyName, _logChannel);
            logEvent.AddPropertyIfAbsent(channelProp);
        }
    }
}
