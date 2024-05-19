using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Api;

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

        public async PromiseTask Run()
        {
            AssertUtil.NotNull(_context, NodeTreeExceptionType.EMPTY_CONTEXT);

            Inject();
            Init();
            SetAdding();
            await LoadAsync(AddCancellation.Token);
            await Start(AddCancellation.Token);
            await SetActive(true);
            SetReady();
        }

        public async PromiseTask DestroyAsync()
        {
            if (!IsInit) return;

            if (NodeState == NodeState.Adding)
            {
                CancelLoad();
            }

            if (IsActive)
            {
                await Disable(true);
            }
            if (IsStarted)
            {
                await Stop();
            }
            if (LoadState == LoadState.Loaded)
            {
                await UnloadAsync(RemoveCancellation.Token);
            }

            Destroy(null);
            Dispose();
        }

        public override string ToString()
        {
            return "GameNode";
        }
    }

}
