using System;
namespace Cr7Sund
{
    public class InternalLogger : IInternalLog
    {
        public void Info(string message)
        {
            Log.Info(LogChannel.Framework, message);
        }
        public void Error(string message)
        {
            Log.Error(LogChannel.Framework, message);
        }
        public void Error(Exception e)
        {
            Log.Error(LogChannel.Framework, "", e);
        }

        public void Error(string prefix, Exception e)
        {
            Log.Error(LogChannel.Framework, prefix, e);
        }
        public void Fatal(string message)
        {
            Log.Fatal(LogChannel.Framework, message);
        }
        public void Fatal(Exception e)
        {
            Log.Fatal(LogChannel.Framework, "", e);
        }
        public void Fatal(string prefix, Exception e)
        {
            Log.Fatal(LogChannel.Framework, prefix, e);
        }
    }

}
