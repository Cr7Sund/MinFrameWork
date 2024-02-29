using System;
using UnityEngine;

namespace Cr7Sund.Logger
{
    public class LoggerProxy : IInternalLog
    {
        public ILogProvider _logProvider;
        private LogLevel _miniumLogLevel;


        private void Log(LogLevel logLevel, string prefix, Exception ex)
        {
            if (logLevel >= _miniumLogLevel)
            {
                _logProvider?.WriteException(logLevel, prefix, ex);
            }
        }

        private void Log<T0>(LogLevel logLevel, string prefix, Exception ex, T0 propertyValue0)
        {
            if (logLevel >= _miniumLogLevel)
            {
                _logProvider?.WriteException(logLevel, prefix, ex, propertyValue0);
            }
        }
        private void Log(LogLevel logLevel, string message)
        {
            if (logLevel >= _miniumLogLevel)
            {
                _logProvider?.WriteLine(logLevel, message);
            }
        }

        private void Log<T0>(LogLevel logLevel, string message, T0 propertyValue0)
        {
            if (logLevel >= _miniumLogLevel)
            {
                _logProvider?.WriteLine(logLevel, message, propertyValue0);
            }
        }
        private void Log<T0, T1>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1)
        {
            if (logLevel >= _miniumLogLevel)
            {

                _logProvider?.WriteLine(logLevel, message, propertyValue0, propertyValue1);
            }
        }

        private void Log<T0, T1, T2>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            if (logLevel >= _miniumLogLevel)
            {

                _logProvider?.WriteLine(logLevel, message, propertyValue0, propertyValue1, propertyValue2);
            }
        }



        #region Public
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Error(Exception e, string prefix)
        {
            Log(LogLevel.Error, prefix, e);
        }

        public void Error(Exception e)
        {
            Log(LogLevel.Error, string.Empty, e);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public void Fatal(Exception e)
        {
            Log(LogLevel.Error, string.Empty, e);
        }

        public void Fatal(Exception e, string prefix)
        {
            Log(LogLevel.Error, prefix, e);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Init(string logChannel)
        {
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Debug<T0>(string message, T0 propertyValue0)
        {
            Log(LogLevel.Debug, message, propertyValue0);
        }

        public void Error<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Log(LogLevel.Error, message, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1)
        {
            Log(LogLevel.Error, message, propertyValue0, propertyValue1);
        }

        public void Warn<T0>(string message, T0 propertyValue0)
        {
            Log(LogLevel.Warn, message, propertyValue0);
        }

        public void Error<T0>(string message, T0 propertyValue0)
        {
            Log(LogLevel.Error, message, propertyValue0);
        }

        public void Error<T0>(Exception e, string prefix, T0 propertyValue0)
        {
            Log(LogLevel.Error, prefix, e, propertyValue0);

        }

        public void Dispose()
        {
            _logProvider?.Dispose();
        }

        #endregion
    }
}
