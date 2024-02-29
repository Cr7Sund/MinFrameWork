namespace Cr7Sund.Logger
{
    public static class LogProviderFactory
    {
        public static ILogProvider Create()
        {
            // return new UnityLogProvider();
            
            return new SerilogProvider();
        }
    }
}