#nullable enable
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using UnityEngine;

namespace Cr7Sund.Logger
{
    public sealed class UnityLogEventSink : ILogEventSink
    {
        private readonly UnityEngine.ILogger _unityLogger;
        private readonly ITextFormatter _formatter;

        public UnityLogEventSink(ITextFormatter formatter, UnityEngine.ILogger unityLogger)
        {
            _formatter = formatter;
            _unityLogger = unityLogger;
        }

        public void Emit(LogEvent logEvent)
        {
            var logType = logEvent.Level switch
            {
                LogEventLevel.Verbose or LogEventLevel.Debug or LogEventLevel.Information => LogType.Log,
                LogEventLevel.Warning => LogType.Warning,
                LogEventLevel.Error or LogEventLevel.Fatal => LogType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level), "Unknown log level"),
            };

            string message = string.Empty;
#if UNITY_EDITOR
            using var stringWriter = new StringWriter();
            _formatter.Format(logEvent, stringWriter);
            message = stringWriter.ToString();
#else
            message = logEvent.RenderMessage();
#endif

            UnityEngine.Object? unityContext = null;
            if (logEvent.Properties.TryGetValue(UnityObjectEnricher.UnityContextKey, out var contextPropertyValue) && contextPropertyValue is ScalarValue contextScalarValue)
            {
                unityContext = contextScalarValue.Value as UnityEngine.Object;
            }
            string? unityTag = null;
            if (logEvent.Properties.TryGetValue(UnityTagEnricher.UnityTagKey, out var tagPropertyValue) && tagPropertyValue is ScalarValue tagScalarValue)
            {
                unityTag = tagScalarValue.Value as string;
            }
            if (logEvent.Properties.ContainsKey(UnityColorEnricher.PropertyName))
            {
                message = LogColorConfig.FormatMessage(logEvent.Level, message);
            }

            if (logEvent.Exception != null)
            {
                if (message != string.Empty)
                    _unityLogger.LogError(message, logEvent.Exception.ToString());
                else
                    _unityLogger.LogException(logEvent.Exception);
            }
            else
            {
                if (unityContext != null)
                {
                    if (unityTag != null)
                    {
                        _unityLogger.Log(logType, unityTag, message, unityContext);
                    }
                    else
                    {
                        _unityLogger.Log(logType, string.Empty, message, unityContext);
                    }
                }
                else if (unityTag != null)
                {
                    _unityLogger.Log(logType, unityTag, message);
                }
                else
                {
                    _unityLogger.Log(logType, message);
                }
            }
        }
    }
}