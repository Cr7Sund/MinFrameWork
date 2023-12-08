using System;
namespace Cr7Sund
{
    public class InternalLogger : IInternalLog
    {
        public void Info(string message)
        {
            Log.Info(message);
        }
        public void Error(string message)
        {
            Log.Error(message);
        }
        public void Error(Exception e)
        {
            Log.Error(e);
        }

        public void Error(string prefix, Exception e)
        {
            Log.Error(prefix, e);
        }
        public void Fatal(string message)
        {
            Log.Fatal(message);
        }
        public void Fatal(Exception e)
        {
            Log.Fatal(e);
        }
        public void Fatal(string prefix, Exception e)
        {
            Log.Fatal(prefix, e);
        }
    }

}
