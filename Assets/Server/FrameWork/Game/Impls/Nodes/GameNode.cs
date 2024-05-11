using System;
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
            await LoadAsync();
            await Start(AddCancellation.Token);
            await SetActive(true);
        }

        public async PromiseTask DestroyAsync()
        {
            if (!IsInit) return;
            if (IsActive)
            {
                await SetActive(false);
            }
            if (IsStarted)
            {
                await Stop();
            }

            if (LoadState == LoadState.Loading || LoadState == LoadState.Loaded)
            {
                await UnloadAsync();
            }

            Destroy(null);
            Dispose();
        }

    }

}
