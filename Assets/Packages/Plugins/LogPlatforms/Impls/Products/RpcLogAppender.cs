// #if UNITY_STANDALONE
using Cr7Sund.FrameWork.Util;
using System;
using System.Diagnostics;
using UnityEngine;
namespace Cr7Sund.Logger
{
    public class RpcLogAppender : ILogAppender
    {
        private LogServer server;

        public string Format(LogLevel level, Enum logChannel, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);

            var st = new StackTrace();
            WriteToDevice(logMessage, st.ToString(), level);

            return logMessage;
        }

        public async void Initialize()
        {
            server = new LogServer();
            await server.StartServer();
        }

        public void Dispose()
        {
            server?.DisConnect();
        }

        public string Format(LogEventData data)
        {
            return string.Empty;
        }

        private void WriteToDevice(string logString, string stackTrace, LogLevel type)
        {
            var logInfo = new LogInfo
            {
                TimeStamp = TimeUtils.ConvertToTimestamp(DateTime.Now),
                Info = logString,
                StackTrace = stackTrace
            };
            string logMsg = JsonUtility.ToJson(logInfo);
            server?.SendAsync(logMsg);
        }
    }

    public class LogInfo
    {
        public string Info;
        public string StackTrace;
        public long TimeStamp;
    }
}
// #endif
