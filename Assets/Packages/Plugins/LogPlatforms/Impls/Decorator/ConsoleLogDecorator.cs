using System;
using System.Text;
namespace Cr7Sund.Logger
{
    internal class ConsoleLogDecorator : ILogDecorator
    {

        public string Format(LogLevel level, string logChannel, string format, params object[] args)
        {
            string result = LogFormatUtil.Format(format, args);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);

            return result;
        }

        public void Initialize()
        {

        }
        public void Dispose()
        {
        }

        public string Format(LogEventData data)
        {
#if UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            var str = sb.Append(string.Format("[{0}] Type : {1}, ID : {2} ", LogLevel.Event, data.type, data.id));
            sb.Append("Info : {");

            foreach (var current in data.info)
                sb.Append($"{current.Key} : {current.Value}");

            sb.Append("}");
            return sb.ToString();
#else
            return string.Empty;
#endif
        }
    }
}
