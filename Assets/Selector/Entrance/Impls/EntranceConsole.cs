
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
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}