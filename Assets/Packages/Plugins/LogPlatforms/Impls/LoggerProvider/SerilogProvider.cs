using Serilog;
using System;

namespace Cr7Sund.Logger
{
    public class SerilogProvider : ILogProvider
    {
        private Serilog.Core.Logger _logger;


        public void Dispose()
        {
            _logger.Dispose();
        }

        public virtual void Init(LogSinkType logSinkType, string logChannel)
        {
            LoggerConfiguration loggerConfiguration = LoggerConfigFactory.Create(logSinkType, logChannel);
            _logger = loggerConfiguration.CreateLogger();
        }

        public void WriteException<T0>(LogLevel logLevel, string prefix, Exception ex, T0 propertyValue0)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Verbose(ex, prefix, propertyValue0, propertyValue0);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(ex, prefix, propertyValue0);
                    break;
                case LogLevel.Info:
                    _logger.Information(ex, prefix, propertyValue0);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(ex, prefix, propertyValue0);
                    break;
                case LogLevel.Error:
                    _logger.Error(ex, prefix, propertyValue0);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(ex, prefix, propertyValue0);
                    break;
                default:
                    break;
            }
        }

        public void WriteException(LogLevel logLevel, string prefix, Exception ex)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Verbose(ex, prefix);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(ex, prefix);
                    break;
                case LogLevel.Info:
                    _logger.Information(ex, prefix);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(ex, prefix);
                    break;
                case LogLevel.Error:
                    _logger.Error(ex, prefix);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(ex, prefix);
                    break;
                default:
                    break;
            }
        }

        public void WriteException(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Verbose(message);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Info:
                    _logger.Information(message);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message);
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
                    _logger.Verbose(message);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Info:
                    _logger.Information(message);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message);
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
                    _logger.Verbose(message, propertyValue0);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message, propertyValue0);
                    break;
                case LogLevel.Info:
                    _logger.Information(message, propertyValue0);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(message, propertyValue0);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, propertyValue0);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message, propertyValue0);
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
                    _logger.Verbose(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Info:
                    _logger.Information(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, propertyValue0, propertyValue1);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message, propertyValue0, propertyValue1);
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
                    _logger.Verbose(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Info:
                    _logger.Information(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Warn:
                    _logger.Warning(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message, propertyValue0, propertyValue1, propertyValue2);
                    break;
                default:
                    break;
            }
        }
    }
}