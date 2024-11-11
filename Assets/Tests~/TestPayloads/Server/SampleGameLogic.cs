using Cr7Sund.IocContainer;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.AssetContainers;

namespace Cr7Sund.Server.Test
{
    public class SampleGameLogic : GameLogic.GameLogic
    {
        protected override GameBuilder CreateBuilder()
        {
            return new SampleGameBuilder();
        }

        public IInjector GetContextInjector()
        {
            var context = _gameNode.Context as Context;
            return context.InjectionBinder.Injector;
        }
    }
}