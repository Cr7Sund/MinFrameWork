using System;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class BaseSceneController : UpdateController, ILateUpdate, IPreloadable
    {
        [Inject(ServerBindDefine.SceneLogger)] protected IInternalLog _log;
        protected override IInternalLog Debug => _log ?? Console.Logger;


        public virtual PromiseTask Prepare(UnsafeCancellationToken cancellation)
        {
            try
            {
                return OnPrepare(cancellation);
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
                    return PromiseTask.CompletedTask;
                }
            }
        }

        public void LateUpdate(int millisecond)
        {
            OnLateUpdate(millisecond);
        }

        protected virtual void OnLateUpdate(int millisecond)
        {

        }
        protected override void OnUpdate(int millisecond)
        {
        }
        protected virtual PromiseTask OnPrepare(UnsafeCancellationToken cancellation)
        {
            return PromiseTask.CompletedTask;
        }
    }
}
