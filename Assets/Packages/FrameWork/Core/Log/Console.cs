using System;

namespace Cr7Sund
{
    public static class Console
    {
        public static IInternalLog Logger { get; private set; }


        public static void Init(IInternalLog logger)
        {
            Logger = logger;
        }

        public static void Dispose()
        {
            Logger.Dispose();
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Info(Exception ex)
        {
            Logger.Info(ex);
        }

        public static void Info<T0>(string message, T0 propertyValue0)
        {
            Logger.Info(message, propertyValue0);
        }

        public static void Info<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1)
        {
            Logger.Info(message, propertyValue0, propertyValue1);
        }

        public static void Info<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Logger.Info(message, propertyValue0, propertyValue1, propertyValue2);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Error<T0>(string message, T0 propertyValue0)
        {
            Logger.Error(message, propertyValue0);
        }

        public static void Error<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1)
        {
            Logger.Error(message, propertyValue0, propertyValue1);
        }

        public static void Error<T0, T1, T2>(string message, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Logger.Error(message, propertyValue0, propertyValue1, propertyValue2);
        }

        public static void Error(Exception ex, string prefix)
        {
            Logger.Error(ex, prefix);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void Fatal(Exception e)
        {
            Logger.Fatal(e);
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
        }
        public static void Warn(Exception e)
        {
            Logger.Warn(e);
        }
        public static void Warn<T0>(string message, T0 propertyValue0)
        {
            Logger.Warn(message, propertyValue0);
        }
        public static void Warn<T0, T1>(string message, T0 propertyValue0, T1 propertyValue1)
        {
            Logger.Warn(message, propertyValue0, propertyValue1);
        }
        public static void Error(Exception ex)
        {
            Logger.Error(ex);
        }
    }
}
