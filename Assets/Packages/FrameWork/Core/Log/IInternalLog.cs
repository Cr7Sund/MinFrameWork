using System;
namespace Cr7Sund
{
    public interface IInternalLog
    {
        void Init(Enum logChannel);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(string prefix, Exception e);
        void Error(Exception e);
        void Fatal(string message);
        void Fatal(Exception e);
        void Fatal(string prefix, Exception e);
    }
}
