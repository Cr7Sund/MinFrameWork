using System;
namespace Cr7Sund.Logger
{
    public delegate void LogFormatterDelegate(LogLevel logLevel, string channel, ref string message);

    public interface ILogProvider : IDisposable
    {
        void Init(LogSinkType logSinkType, string logChannel);

        void WriteException(LogLevel logLevel, string message, Exception e);
        void WriteException<T0>(LogLevel logLevel, string message, Exception e, T0 propertyValue0);
        void WriteLine(LogLevel logLevel, string message);
        void WriteLine<T0>(LogLevel logLevel, string message, T0 propertyValue0);
        void WriteLine<T0, T1>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1);
        void WriteLine<T0, T1, T2>(LogLevel logLevel, string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);
    }
}
