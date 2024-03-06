using System;

namespace Cr7Sund
{
    public static class Console
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

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Info<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Info(message, propertyValue0, propertyValue1, propertyValue2);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(string prefix, Exception e)
        {
            _logger.Error(prefix, e);
        }

        public static void Error<T0>(Exception e, string prefix, T0 propertyValue0)
        {
            _logger.Error(e, prefix, propertyValue0);
        }

        public static void Error(Exception e, string message)
        {
            _logger.Error(e, message);
        }

        public static void Error<T0>(string message, T0 propertyValue0)
        {
            _logger.Error(message, propertyValue0);
        }

        public static void Error<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1)
        {
            _logger.Error(message, propertyValue0, propertyValue1);
        }

        public static void Error<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _logger.Error(message, propertyValue0, propertyValue1, propertyValue2);
        }

        public static void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public static void Fatal(Exception e)
        {
            _logger.Fatal(e);
        }

        public static void Fatal(Exception e, string prefix)
        {
            _logger.Fatal(e, prefix);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Warn<T0>(string message, T0 propertyValue0)
        {
            _logger.Warn(message, propertyValue0);
        }

        public static void Error(Exception ex)
        {
            _logger.Error(ex);
        }
    }
}
