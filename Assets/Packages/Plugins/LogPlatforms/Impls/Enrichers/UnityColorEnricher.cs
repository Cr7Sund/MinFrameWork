using Serilog.Core;
using Serilog.Events;

namespace Cr7Sund.Logger
{
    public sealed class UnityColorEnricher : ILogEventEnricher
    {
        public const string PropertyName = "%UNITY_LOG_COLOR%";


        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) =>
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(PropertyName, true));
    }
}