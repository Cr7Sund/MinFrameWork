using System;
using System.Threading;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    public abstract class BaseController : IController
    {
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }

        protected virtual IInternalLog Debug { get => Console.Logger; }

        // No preload
        // since it should be existed one, but preload the node is still no created

        public async PromiseTask Start()
        {
            IsStarted = true;
            try
            {
                await OnStart();
            }
            catch (Exception e)
            {
                Debug.Error(e);
                if (e is OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public async PromiseTask Stop()
        {
            IsStarted = false;
            try
            {
                await OnStop();
            }
            catch (Exception e)
            {
                Debug.Error(e);
                if (e is OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public async PromiseTask Enable()
        {
            IsActive = true;
            try
            {
                await OnEnable();
            }
            catch (Exception e)
            {
                Debug.Error(e);
                if (e is OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public async PromiseTask Disable()
        {
            IsActive = false;
            try
            {
                await OnDisable();
            }
            catch (Exception e)
            {
                Debug.Error(e);
                if (e is OperationCanceledException)
                {
                    throw;
                }
            }
        }

        public virtual void RegisterAddTask(CancellationToken cancellationToken) { }
        public virtual void RegisterRemoveTask(CancellationToken cancellationToken) { }

        protected virtual PromiseTask OnStart() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnStop() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnDisable() { return PromiseTask.CompletedTask; }

    }
}
