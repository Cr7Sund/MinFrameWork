
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
            var logger = new Cr7Sund.Logger.LoggerProxy();
            logger.Init(logChannel);
            ILogProvider logProvider = new SerilogProvider();
            //  = new UnityEditorLogProvider();


            logProvider.Init(LogSinkType.File | LogSinkType.Net, logChannel.ToString());
            // logProvider.Init(LogSinkType.File);

            logger._logProvider = logProvider;
            return logger;
        }

    }

}
