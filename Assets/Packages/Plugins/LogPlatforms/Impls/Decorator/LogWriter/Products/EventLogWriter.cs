using System;
namespace Cr7Sund.Logger
{
    internal class EventLogWriter : LogWriter<LogEventData>
    {
        public EventLogWriter(ILogFileFormatting formatter, MMFile mmFile) : base(formatter, mmFile)
        {
        }

        protected override FileLogType FileLogType
        {
            get
            {
                return FileLogType.Event;
            }
        }

        protected override string Formatting(string level, string id, LogEventData obj)
        {
            jsonData.Clear();
            jsonData["event_time"] = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffzzzz");
            jsonData["event_type"] = level;
            jsonData["event_id"] = id;

            string preKey = string.Format("extra.{0}.{1}", level, id);

            foreach (var current in obj.info)
            {
                string key = string.Format("{0}.{1}", preKey, current.Key);
                jsonData[key] = current.Value;
            }
            return jsonData.ToJson();
        }
    }
}
