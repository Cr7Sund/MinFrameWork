using System;

namespace Cr7Sund.Selector.Impl
{
    public static class EntranceConsole
    {
        private static IInternalLog _logger;


        public static void Init(IInternalLog logger)
        {
            _logger = logger;
        }
        public static void Dispose()
        {
            _logger?.Dispose();
            EntranceConsole.Dispose();
            Console.Dispose();
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }
    }
}