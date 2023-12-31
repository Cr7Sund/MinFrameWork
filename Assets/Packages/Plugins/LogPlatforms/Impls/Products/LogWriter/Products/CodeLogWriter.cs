﻿using System;
namespace Cr7Sund.Logger
{
    internal class CodeLogWriter : LogWriter<string>
    {
        public CodeLogWriter(ILogFileFormatting formatter, MMFile mmFile) : base(formatter, mmFile)
        {
        }

        protected override LogType LogType
        {
            get
            {
                return LogType.Code;
            }
        }

        protected override string Formatting(string level, string id, string msg)
        {
            jsonData["log_time"] = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffzzzz");
            jsonData["log_level"] = level;
            jsonData["log_info"] = msg;
            return jsonData.ToJson();
        }
    }
}
