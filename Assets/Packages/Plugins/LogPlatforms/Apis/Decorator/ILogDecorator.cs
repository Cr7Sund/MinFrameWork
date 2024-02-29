using System;
namespace Cr7Sund.Logger
{
    public interface ILogDecorator : IDisposable
    {
        void Initialize();
        string Format(LogLevel level, string logChannel, string format, params object[] args);
    }
}
