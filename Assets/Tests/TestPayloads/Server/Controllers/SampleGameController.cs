using System.Threading;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Test
{
    public class SampleGameController : BaseGameController
    {
        protected override PromiseTask HandleHotfix()
        {
            return PromiseTask.CompletedTask;
        }

        protected override async PromiseTask InitGameEnv()
        {
            await PromiseTask.CompletedTask;
        }

        protected override PromiseTask RunLoginScene()
        {
            return PromiseTask.CompletedTask;
        }
    }
}