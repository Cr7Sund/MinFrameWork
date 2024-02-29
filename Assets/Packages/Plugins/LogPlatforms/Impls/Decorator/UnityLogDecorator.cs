#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
namespace Cr7Sund.Logger
{
    public class UnityLogDecorator : ILogDecorator
    {
        private LogPalette logPalette = new LogPalette();

        private readonly Dictionary<string, Color32> _colors = new Dictionary<string, Color32>();

        public UnityLogDecorator()
        {

            var color = GetLocalColor("log trace", Color.white);
            _colors["log trace"] = color;

            color = GetLocalColor("log debug", LogColorHelp.HexToColor(logPalette.DebugHex));
            _colors["log debug"] = color;

            color = GetLocalColor("log info", LogColorHelp.HexToColor(logPalette.InfoHex1));
            _colors["log info"] = color;

            color = GetLocalColor("log warn", LogColorHelp.HexToColor(logPalette.WarnHex1));
            _colors["log warn"] = color;

            color = GetLocalColor("log error", LogColorHelp.HexToColor(logPalette.ErrorHex));
            _colors["log error"] = color;

            color = GetLocalColor("log fatal", LogColorHelp.HexToColor(logPalette.FatalHex));
            _colors["log fatal"] = color;

            color = GetLocalColor("log event", Color.white);
            _colors["log event"] = color;
        }

        public string Format(LogLevel level, string logChannel, string format, params object[] args)
        {
            string result = LogFormatUtil.Format(format, args);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);

            result = DecorateColor(level, logMessage);
            return result;
        }

        public void Initialize()
        {

        }

        public void Dispose()
        {
        }
        private Color32 GetLocalColor(string key, Color32 defaultColor)
        {
            string str = EditorPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(str))
                return JsonUtility.FromJson<Color32>(str);
            return defaultColor;
        }

        private string DecorateColor(LogLevel level, string msg)
        {
            Color32 color;
            //unity主线程默认为1
            if (Thread.CurrentThread.ManagedThreadId != 1)
            {
                color = level switch
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
                color = level switch
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

            return string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), msg);
        }
    }
}
#endif
