using System;
namespace Cr7Sund.Logger
{
    public interface ILogDecorator : IDisposable
    {
        void Initialize();
        string Format(LogLevel level, Enum logChannel, string format, params object[] args);
    }
}
