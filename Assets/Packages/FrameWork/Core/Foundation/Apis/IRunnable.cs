using System.Threading;

namespace Cr7Sund
{
    public interface IRunnable
    {
        bool IsStarted { get;  set; }

        PromiseTask Start(CancellationToken cancellation);
        PromiseTask Stop();
    }
}
