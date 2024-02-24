using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Scene.Apis;
namespace Cr7Sund.Server.Impl
{
    public class GameNode : ModuleNode, IGameNode
    {
        private GameNode(IAssetKey assetKey) : base(assetKey)
        {

        }
        
        public GameNode() : this(null)
        {

        }

        public void Run()
        {
            AssertUtil.NotNull(_context, NodeTreeExceptionType.EMPTY_CONTEXT);

            Inject();
            Init();
            LoadAsync(this).Then(_ =>
            {
                Start();
                SetActive(true);
            });
        }

        public IPromise<INode> Destroy()
        {
            Deject();

            return UnloadAsync(this).Then(node =>
            {
                SetActive(false);
                Stop();
                Dispose();
                return node;
            });
        }
    }
}
