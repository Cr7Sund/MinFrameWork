using System.Threading;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    public abstract class BaseController : IController
    {
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }


        // No preload
        // since it should be existed one, but preload the node is still no created

        public async PromiseTask Start()
        {
            IsStarted = true;
            if (MacroDefine.NoCatchMode)
            {
                await OnStart();
            }
            else
            {
                try
                {
                    await OnStart();
                }
                catch (System.Exception e)
                {
                    Console.Error(e, "{TypeName}.OnStart Error: ", GetType().FullName);
                }
            }
        }

        public async PromiseTask Stop()
        {
            if (MacroDefine.NoCatchMode)
            {
                await OnStop();
            }
            else
            {
                try
                {
                    await OnStop();
                }
                catch (System.Exception e)
                {
                    Console.Error(e, "{TypeName}.OnStop Error: ", GetType().FullName);
                }
            }

            IsStarted = false;
        }

        public async PromiseTask Enable()
        {
            IsActive = true;

            if (MacroDefine.NoCatchMode)
            {
                await OnEnable();
            }
            else
            {
                try
                {
                    await OnEnable();
                }
                catch (System.Exception e)
                {
                    Console.Error(e, "{@TypeName}.OnEnable Error: ", GetType().FullName);
                }
            }
        }

        public async PromiseTask Disable()
        {
            if (MacroDefine.NoCatchMode)
            {
                await OnDisable();
            }
            else
            {
                try
                {
                    await OnDisable();
                }
                catch (System.Exception e)
                {
                    Console.Error(e, "{TypeName}.OnDisable Error: ", GetType().FullName);
                }
            }
            IsActive = false;
        }

        public virtual void RegisterAddTask(CancellationToken cancellationToken) { }
        public virtual void RegisterRemoveTask(CancellationToken cancellationToken) { }

        protected virtual PromiseTask OnStart() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnStop() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnEnable() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnDisable() { return PromiseTask.CompletedTask; }

    }
}
