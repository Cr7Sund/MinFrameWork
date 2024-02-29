
using System;

namespace Cr7Sund.Logger
{
    public static class LogFormatUtil
    {
        public static string Format(string format, params object[] args)
        {
            if (args == null || args.Length <= 0)
                return format;
            return string.Format(format, args);
        }
        public static string ColorFormat(string msg, UnityEngine.Color color)
        {
            return string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), msg);
        }

        /// <summary>
        ///     Formats an exception for logging purposes.
        /// </summary>
        /// <param name="exception">The exception to format.</param>
        /// <returns>A formatted string representing the exception.</returns>
        public static string ParseException(Exception exception)
        {
            if (exception == null) return string.Empty;
            return $"{exception.GetType().FullName}: {exception.Message}\nStackTrace: {exception.StackTrace}";
        }
    }
}
