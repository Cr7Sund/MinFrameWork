using System;
namespace Cr7Sund
{
    public interface IInternalLog : IDisposable
    {
        void Init(string logChannel);

        void Debug(string message);
        void Debug<T0>(string message, T0 propertyValue0);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1);
        void Error<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);
        void Error(Exception e, string prefix);
        void Error<T0>(Exception e, string prefix, T0 propertyValue0);
        void Error(Exception e);
        void Fatal(string message);
        void Fatal(Exception e);
        void Fatal(Exception e, string prefix);
        void Warn<T0>(string message, T0 propertyValue0);
        void Error<T0>(string message, T0 propertyValue0);
    }
}
