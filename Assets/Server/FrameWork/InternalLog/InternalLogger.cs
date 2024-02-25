using System;
namespace Cr7Sund
{
    public class InternalLogger : IInternalLog
    {
        private Enum _logChannel;

        public void Init(Enum logChannel = null)
        {
            _logChannel = logChannel;
            Log.Initialize();
        }

        public void Info(string message)
        {
            Log.Info(_logChannel, message);
        }

        public void Warn(string message)
        {
            Log.Warn(_logChannel, message);
        }

        public void Error(string message)
        {
            Log.Error(_logChannel, message);
        }
        public void Error(Exception e)
        {
            Log.Error(_logChannel, "", e);
        }

        public void Error(string prefix, Exception e)
        {
            Log.Error(_logChannel, prefix, e);
        }
        public void Fatal(string message)
        {
            Log.Fatal(_logChannel, message);
        }
        public void Fatal(Exception e)
        {
            Log.Fatal(_logChannel, "", e);
        }
        public void Fatal(string prefix, Exception e)
        {
            Log.Fatal(_logChannel, prefix, e);
        }


    }

}
