namespace Cr7Sund.Logger
{
    internal static class LogWriterDirector
    {
        public static ILogWritable Construct(LogWriterBuilder builder)
        {
            builder.BuildMMFile();
            builder.BuildFormatter();
            builder.BuildLogWriter();
            return builder.GetProduct();
        }
    }
}
