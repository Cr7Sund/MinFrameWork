using System;
namespace Cr7Sund.Logger
{
    internal interface ILog : IDisposable
    {
        void Initialize();
        string Format(LogLevel level, LogChannel logChannel, string format, params object[] args);
    }
}
