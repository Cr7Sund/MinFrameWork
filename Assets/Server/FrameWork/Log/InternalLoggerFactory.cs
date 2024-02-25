using System;
namespace Cr7Sund
{
    public class InternalLoggerFactory
    {
        public static IInternalLog Create(LogChannel logChannel)
        {
            var logger = new InternalLogger();
            logger.Init(logChannel);

            return logger;
        }

    }

}
