using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Needle.Console
{
    public static class LogEntriesExtension
    {
        private static Regex _filePathRegex = new Regex(@"at\s+(?<FilePath>[^:\s]+)");
        private static Regex _fileLineRegex = new Regex(@":([^:]+)\)$");


        internal static bool GetStripEntry(int row, LogEntry outputEntry)
        {
            bool result = LogEntries.GetEntryInternal(row, outputEntry);
            if (result)
            {
                StripStackTrace(outputEntry);
            }
            return result;
        }

        private static void StripStackTrace(LogEntry logEntry)
        {
            string message = logEntry.message;
            if (!string.IsNullOrEmpty(message))
            {
                string[] lines = message.Split(new string[1]
                {
                        "\n"
                }, StringSplitOptions.None);
                var sb = new StringBuilder();
                bool startLog = true;
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    if (startLog && lines[i + 1].StartsWith("void Serilog.Core.Sinks.SafeAggregateSink.Emit(LogEvent"))
                    {
                        sb.AppendLine("-");
                        startLog = false;
                    }
                    if (!startLog)
                    {
                        if (lines[i].StartsWith("void Cr7Sund.Logger.Logger.Log"))
                        {
                            startLog = true;
                            i += 2;
                            if (lines[i].StartsWith("void Cr7Sund.Console"))
                            {
                                i += 1;
                            }

                            string filePath = _filePathRegex.Match(lines[i]).Groups["FilePath"].Value;
                            int fileLine = Convert.ToInt32(_fileLineRegex.Match(lines[i]).Groups[1].Value);
                            logEntry.file = filePath;
                            logEntry.line = fileLine;
                        }

                    }
                    if (startLog)
                    {
                        sb.AppendLine(lines[i]);
                    }
                }
                if (startLog)
                {
                    message = sb.ToString();
                }
            }

            logEntry.message = message;
        }


    }
}