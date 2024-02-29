
using System;

namespace Cr7Sund.Logger
{
    public class UnityEditorLogProvider : ILogProvider, IDisposable
    {
        private ILogDecorator _logDecorator;
        private string _logChannel;
        private static ILogDecorator _logAppender;
        private static ILogDecorator LogAppender
        {
            get
            {
                // Lazy initialization of log decorator using LogDecoratorCreator.
                return _logAppender ??= LogDecoratorCreator.Create();
            }
        }
        public void Dispose()
        {
            _logDecorator?.Dispose();
        }

        public virtual void Init(LogSinkType logSinkType, string logChannel)
        {
            _logChannel = logChannel;

            if ((logSinkType & LogSinkType.Net) == LogSinkType.Net)
            {
                _logDecorator = new RpcLogDecorator();
            }
            if ((logSinkType & LogSinkType.File) == LogSinkType.File)
            {
                _logDecorator = new FileLogDecorator();
            }
        }

        public void WriteException(LogLevel logLevel, string prefix, Exception ex)
        {
            var result = LogFormatUtil.ParseException(ex);
            result = $"{prefix} \n Exception {result}";
            WriteException(logLevel, prefix);
        }

        public void WriteException(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message);
                    break;
                default:
                    break;
            }
        }

        public void WriteException<T0>(LogLevel logLevel, string message, Exception e, T0 propertyValue0)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message, e, propertyValue0);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message, e, propertyValue0);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message, e, propertyValue0);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message, e, propertyValue0);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message, e, propertyValue0);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message, e, propertyValue0);
                    break;
                default:
                    break;
            }
        }

        public void WriteLine(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message);
                    break;
                default:
                    break;
            }
        }


        public void WriteLine<T0>(LogLevel logLevel, string message, T0 propertyValue0)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message, propertyValue0);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message, propertyValue0);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message, propertyValue0);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message, propertyValue0);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message, propertyValue0);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message, propertyValue0);
                    break;
                default:
                    break;
            }
        }

        public void WriteLine<T0, T1>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message, propertyValue0, propertyValue1);
                    break;
                default:
                    break;
            }
        }

        public void WriteLine<T0, T1, T2>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    UnityEditorDebug(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Debug:
                    UnityEditorDebug(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Info:
                    UnityEditorDebug(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Warn:
                    UnityEditorWarning(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Error:
                    UnityEditorError(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Fatal:
                    UnityEditorError(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                default:
                    break;
            }
        }

        #region Debug
        private void UnityEditorDebug(string format, params object[] args)
        {
            format = LogAppender.Format(LogLevel.Trace, _logChannel, format, args);

            if (args.Length <= 0)
                UnityEngine.Debug.Log(format);
            else
                UnityEngine.Debug.LogFormat(format, args);
        }

        private void UnityEditorWarning(string format, params object[] args)
        {
            format = LogAppender.Format(LogLevel.Trace, _logChannel, format, args);

            if (args.Length <= 0)
                UnityEngine.Debug.LogWarning(format);
            else
                UnityEngine.Debug.LogWarningFormat(format, args);
        }

        private void UnityEditorError(string format, params object[] args)
        {
            format = LogAppender.Format(LogLevel.Trace, _logChannel, format, args);

            if (args.Length <= 0)
                UnityEngine.Debug.LogError(format);
            else
                UnityEngine.Debug.LogErrorFormat(format, args);
        }
        #endregion
    }
}