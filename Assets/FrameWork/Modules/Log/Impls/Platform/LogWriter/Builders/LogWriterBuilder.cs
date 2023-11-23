namespace Cr7Sund.Logger
{
    internal abstract class LogWriterBuilder
    {

        protected ILogFileFormatting _formatter;
        protected MMFile _mmFile;
        protected ILogWritable _writer;

        public void BuildFormatter()
        {
            _formatter = new LogFileFormatter();
        }

        public abstract void BuildMMFile();
        public abstract void BuildLogWriter();

        public ILogWritable GetProduct()
        {
            return _writer;
        }
    }
}
