namespace Cr7Sund.Logger
{
    internal static class LogFormatUtility
    {
        public static string Format(string format, params object[] args)
        {
            if (args == null || args.Length <= 0)
                return format;
            return string.Format(format, args);
        }
    }
}
