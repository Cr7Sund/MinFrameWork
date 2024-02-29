using System;
namespace Cr7Sund.Logger
{
    internal static class LogWriterFactory
    {
        public static ILogWritable Create(FileLogType type)
        {
            LogFileManager.ExistOrCreate(LogFileUtil.GetFileDirector(type));
            LogWriterBuilder builder;

            switch (type)
            {
                case FileLogType.Code:
                    builder = new CodeLogWriterBuilder();
                    break;
                case FileLogType.Event:
                    builder = new EventLogWriterBuilder();
                    break;
                default:
                    throw new Exception("未支持的日志类型");
            }

            return LogWriterDirector.Construct(builder);
        }
    }
}
