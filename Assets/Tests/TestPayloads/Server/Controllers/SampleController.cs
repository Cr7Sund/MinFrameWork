using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Tests
{
    public class SampleController : BaseGameController
    {
        protected override IPromise HandleHotfix()
        {
            return Promise.Resolved();
        }

        protected override void InitGameEnv()
        {
        }

        protected override IPromise<INode> RunLoginScene()
        {
            return Promise<INode>.Resolved(null);
        }
    }
}