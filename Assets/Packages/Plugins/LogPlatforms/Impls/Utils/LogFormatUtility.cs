
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
        public static string ColorFormat(string msg, UnityEngine.Color color)
        {
            return string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), msg);
        }
    }
}
