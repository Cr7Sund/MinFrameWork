
using Serilog.Events;
using Serilog.Formatting;
using System.IO;
using System.Xml;
using UnityEngine;


namespace Cr7Sund.Logger
{
    /// <summary>
    /// Text formatter serializing log events into json
    /// </summary>
    public partial class JsonTextFormatter : ITextFormatter
    {
        private static readonly string SourceContextPropertyName = "SourceContext";
        private static readonly string ThreadIdPropertyName = "ThreadId";

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            LogEntryData data = new LogEntryData();

            var milliseconds = (logEvent.Timestamp.UtcDateTime.Ticks - 621355968000000000) / 10000000;
            data.TimeStamp = milliseconds;
            data.Level = (ushort)logEvent.Level;
            data.Message = logEvent.RenderMessage();

            WriteException(logEvent, data);
            WriteColor(logEvent, data);
            WriteLevel(logEvent, data);

            string result = JsonUtility.ToJson(data);
            output.Write(result);
        }


        private void WriteColor(LogEvent logEvent, LogEntryData output)
        {
            if (logEvent.Properties.ContainsKey(UnityColorEnricher.PropertyName))
            {
                string color = ColorUtility.ToHtmlStringRGBA(LogColorConfig.GetColor(logEvent.Level));
                output.Color = color;
            }
        }

        private void WriteLevel(LogEvent logEvent, LogEntryData output)
        {
            if (logEvent.Properties.TryGetValue(LogChannelEnricher.PropertyName, out var propertyValue)
            && propertyValue is ScalarValue scalarValue)
            {
                output.LogChannel = scalarValue.Value.ToString();
            }
        }

        private void WriteException(LogEvent logEvent, LogEntryData output)
        {
            if (logEvent.Exception != null)
            {
                output.ExceptionMsg = logEvent.Exception.ToString();
            }
            else
            {
                output.ExceptionMsg = string.Empty;
            }
        }
    }

}