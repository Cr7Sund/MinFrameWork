using System;

namespace Cr7Sund
{
    public enum DefaultLogChannel
    {
        FrameWork = 1 << 1,
    }
    
    
    public static class Console
    {
        private static IInternalLog _logger;


        public static void Init(IInternalLog logger)
        {
            _logger = logger;
            _logger.Init(DefaultLogChannel.FrameWork);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(string prefix, Exception e)
        {
            _logger.Error(prefix, e);
        }

        public static void Error(Exception e)
        {
            _logger.Error(e);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Fatal(Exception e)
        {
            _logger.Fatal(e);
        }

        public static void Fatal(string prefix, Exception e)
        {
            _logger.Fatal(prefix, e);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}
