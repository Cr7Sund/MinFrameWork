#nullable enable
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;
using System;

namespace Cr7Sund.Logger
{
    public static class UnitySinkExtensions
    {
        private const string DefaultDebugOutputTemplate = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Writes log events to <see cref="UnityEngine.ILogger"/>. Defaults to <see cref="UnityEngine.Debug.unityLogger"/>.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="unityLogger">Specify a Unity-native logger. Defaults to <see cref="UnityEngine.Debug.unityLogger"/>.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration Unity(
            this LoggerSinkConfiguration sinkConfiguration,
            UnityEngine.ILogger? unityLogger = null,
             string outputTemplate = DefaultDebugOutputTemplate,
             IFormatProvider? formatProvider = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

#pragma warning disable IDE0074 // Use compound assignment
            if (unityLogger == null) unityLogger = UnityEngine.Debug.unityLogger;
#pragma warning restore IDE0074 // Use compound assignment

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new UnityLogEventSink(formatter, unityLogger));
        }


    }
}