using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class BaseSceneController : UpdateController, ILateUpdate
    {
        [Inject(ServerBindDefine.SceneLogger)] protected IInternalLog _log;
        protected override IInternalLog Debug => _log;

        public void LateUpdate(int millisecond)
        {
            OnLateUpdate(millisecond);
        }

        protected virtual void OnLateUpdate(int millisecond) { }
        protected override void OnUpdate(int millisecond)
        {
        }
    }
}
