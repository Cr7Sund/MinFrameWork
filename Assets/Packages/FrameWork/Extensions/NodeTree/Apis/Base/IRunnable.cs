namespace Cr7Sund.NodeTree.Api
{
    public interface IRunnable
    {
        bool IsStarted { get; }


        void Start();
        void Stop();
    }
}
