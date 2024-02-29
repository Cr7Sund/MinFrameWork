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



    public enum LogSinkType
    {
        Local,
        Console,
        File,
        DB,
        Net,
        ELK,
    }
}
