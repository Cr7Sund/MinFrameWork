namespace Cr7Sund
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Event
    }

    public enum LogType : byte
    {
        /// <summary> 代码日志 </summary>
        Code = 1,

        /// <summary> 埋点日志 </summary>
        Event = 1 << 1
    }
}
