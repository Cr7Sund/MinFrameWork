using System;

namespace Cr7Sund
{
    public static class Debug
    {
        public static IInternalLog Logger;

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Error(string prefix, Exception e)
        {
            Logger.Error(prefix, e);
        }

        public static void Error(Exception e)
        {
            Logger.Error(e);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void Fatal(Exception e)
        {
            Logger.Fatal(e);
        }

        public static void Fatal(string prefix, Exception e)
        {
            Logger.Fatal(prefix, e);
        }
    }
}
