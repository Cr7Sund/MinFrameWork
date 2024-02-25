using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class BaseSceneController : BaseController
    {
        [Inject(ServerBindDefine.SceneLogger)] protected IInternalLog Debug;

    }
}
