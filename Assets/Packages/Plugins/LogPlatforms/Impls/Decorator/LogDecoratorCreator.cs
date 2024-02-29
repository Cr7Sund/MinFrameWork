namespace Cr7Sund.Logger
{
    public static class LogDecoratorCreator
    {
        public static ILogDecorator Create()
        {

#if UNITY_EDITOR
            return new UnityLogDecorator(); //returns ConsoleLogger for default console output.
#elif UNITY_STANDALONE
            return new RpcLogDecorator();
#elif FINAL_RELEASE || PROFILER
            return new FileLogDecorator();
#else
            return new ConsoleLogDecorator();
#endif
        }
    }
}
