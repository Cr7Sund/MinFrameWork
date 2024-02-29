using System.Collections.Generic;
using System.Threading;
using Serilog.Events;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Logger
{
    public static class LogColorConfig
    {
        private static LogPalette logPalette = new LogPalette();
        private static readonly Dictionary<string, Color32> _colors = new Dictionary<string, Color32>();


        private static Color32 GetLocalColor(string key, Color32 defaultColor)
        {
            string str = EditorPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(str))
                return JsonUtility.FromJson<Color32>(str);
            return defaultColor;
        }

 
        public static string FormatMessage(LogLevel logLevel, string message)
        {
            Color32 color;
            // unity main thread's id is 1 by default
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                color = logLevel switch
                {
                    LogLevel.Trace => _colors["log trace"],
                    LogLevel.Debug => _colors["log debug"],
                    LogLevel.Info => _colors["log info"],
                    LogLevel.Warn => _colors["log warn"],
                    LogLevel.Error => _colors["log error"],
                    LogLevel.Fatal => _colors["log fatal"],
                    LogLevel.Event => _colors["log event"],
                    _ => Color.white
                };
            }
            else
            {
                color = logLevel switch
                {
                    LogLevel.Trace => GetLocalColor("log trace", Color.white),
                    LogLevel.Debug => GetLocalColor("log debug", LogColorHelp.HexToColor(logPalette.DebugHex)),
                    LogLevel.Info => GetLocalColor("log info", LogColorHelp.HexToColor(logPalette.InfoHex1)),
                    LogLevel.Warn => GetLocalColor("log warn", LogColorHelp.HexToColor(logPalette.WarnHex1)),
                    LogLevel.Error => GetLocalColor("log error", LogColorHelp.HexToColor(logPalette.ErrorHex)),
                    LogLevel.Fatal => GetLocalColor("log fatal", LogColorHelp.HexToColor(logPalette.FatalHex)),
                    LogLevel.Event => GetLocalColor("log event", Color.white),
                    _ => Color.white
                };
            }

            message = string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), message);
            return message;
        }


        public static string FormatMessage(LogEventLevel logLevel, string message)
        {
            Color32 color;
            color = GetColor(logLevel);

            message = string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), message);
            return message;
        }

        public static Color32 GetColor(LogEventLevel logLevel)
        {
            Color32 color;
            // unity main thread's id is 1 by default
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                color = GetCachedColor(logLevel);
            }
            else
            {
                color = GetLocalColor(logLevel);
            }

            return color;
        }

        private static Color32 GetCachedColor(LogEventLevel logLevel)
        {
            return logLevel switch
            {
                LogEventLevel.Verbose => Color.white,
                LogEventLevel.Debug => LogColorHelp.HexToColor(logPalette.DebugHex),
                LogEventLevel.Information => LogColorHelp.HexToColor(logPalette.InfoHex1),
                LogEventLevel.Warning => LogColorHelp.HexToColor(logPalette.WarnHex1),
                LogEventLevel.Error => LogColorHelp.HexToColor(logPalette.ErrorHex),
                LogEventLevel.Fatal => LogColorHelp.HexToColor(logPalette.FatalHex),
                _ => Color.white
            };
        }

        private static Color32 GetLocalColor(LogEventLevel logLevel)
        {
            return logLevel switch
            {
                LogEventLevel.Verbose => GetLocalColor("log trace", Color.white),
                LogEventLevel.Debug => GetLocalColor("log debug", LogColorHelp.HexToColor(logPalette.DebugHex)),
                LogEventLevel.Information => GetLocalColor("log info", LogColorHelp.HexToColor(logPalette.InfoHex1)),
                LogEventLevel.Warning => GetLocalColor("log warn", LogColorHelp.HexToColor(logPalette.WarnHex1)),
                LogEventLevel.Error => GetLocalColor("log error", LogColorHelp.HexToColor(logPalette.ErrorHex)),
                LogEventLevel.Fatal => GetLocalColor("log fatal", LogColorHelp.HexToColor(logPalette.FatalHex)),
                _ => Color.white
            };
        }
    }
}