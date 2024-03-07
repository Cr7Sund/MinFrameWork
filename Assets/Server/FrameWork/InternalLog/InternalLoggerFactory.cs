
using Cr7Sund.Logger;

namespace Cr7Sund
{
    public class InternalLoggerFactory
    {
        public static IInternalLog Create()
        {
            return Create("Test");
        }

        public static IInternalLog Create(string logChannel)
        {
            var logSinkType = LogSinkType.File | LogSinkType.Net | LogSinkType.LogPlatform;
            var logger = new LoggerProxy(logSinkType, logChannel);
            return logger;
        }

    }

}
