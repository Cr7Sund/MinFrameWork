using System.Threading;

namespace Cr7Sund
{
    public interface IRunnable
    {
        bool IsStarted { get;  set; }

        PromiseTask Start();
        PromiseTask Stop();

        void RegisterAddTask(CancellationToken cancellationToken);
        void RegisterRemoveTask(CancellationToken cancellationToken);
    }
}
