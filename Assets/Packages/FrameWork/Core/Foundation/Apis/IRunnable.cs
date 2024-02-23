namespace Cr7Sund
{
    public interface IRunnable
    {
        bool IsStarted { get; }


        void Start();
        void Stop();
    }
}
