// #if UNITY_STANDALONE
using System;
using System.Diagnostics;
using UnityEngine;
namespace Cr7Sund.Logger
{
    public class RpcLogDecorator : ILogDecorator
    {
        private LogServer server;

        public string Format(LogLevel level, string logChannel, string format, params object[] args)
        {
            string result = LogFormatUtil.Format(format, args);
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
                TimeStamp = ConvertToTimestamp(DateTime.Now),
                Info = logString,
                StackTrace = stackTrace
            };
            string logMsg = JsonUtility.ToJson(logInfo);
            server?.SendAsync(logMsg);
        }

        public static long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
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
