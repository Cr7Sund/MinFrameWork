namespace Cr7Sund.Logger
{
    internal class EventLogWriterBuilder : LogWriterBuilder
    {
        public override void BuildLogWriter()
        {
            _writer = new EventLogWriter(_formatter, _mmFile);
        }

        public override void BuildMMFile()
        {
            _mmFile = new MMFile(LogFileUtil.GetMemoryPath(FileLogType.Event), LogFileUtil.LogMemorySize);
        }
    }
}
