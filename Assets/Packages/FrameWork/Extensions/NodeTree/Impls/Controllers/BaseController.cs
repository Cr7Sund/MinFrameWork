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

        public async PromiseTask Start(CancellationToken cancellation)
        {
            IsStarted = true;
            try
            {
                await OnStart(cancellation);
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    Debug.Error(e);
                }
            }
        }

        public async PromiseTask Stop()
        {
            try
            {
                await OnStop();
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    Debug.Error(e);
                }
            }
            finally
            {
                IsStarted = false;
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
                if (e is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    Debug.Error(e);
                }
            }
        }

        public async PromiseTask Disable()
        {
            try
            {
                await OnDisable();
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    Debug.Error(e);
                }
            }
            finally
            {
                IsActive = false;
            }
        }

        public virtual PromiseTask RegisterAddTask(CancellationToken cancellationToken) { return PromiseTask.CompletedTask; }
        public virtual void RegisterRemoveTask(CancellationToken cancellationToken) { }

        protected virtual PromiseTask OnStart(CancellationToken cancellation) { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnStop() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnDisable() { return PromiseTask.CompletedTask; }

    }
}
