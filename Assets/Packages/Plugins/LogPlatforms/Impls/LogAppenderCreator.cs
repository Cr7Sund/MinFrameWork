namespace Cr7Sund.Logger
{
    public static class LogAppenderCreator
    {
        public static ILogAppender Create()
        {

#if UNITY_EDITOR
            return new UnityLogAppender(); //returns ConsoleLogger for default console output.
#elif UNITY_STANDALONE
            return new RpcLogAppender();
#elif FINAL_RELEASE || PROFILER
            return new FileLog();
#else
            return new ConsoleLog();
#endif
        }
    }
}
