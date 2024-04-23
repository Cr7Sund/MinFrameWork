using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.Test
{
    public class SampleGameContext : GameContext
    {
        protected override string Channel => "SampleGame";

        protected override void OnMappedBindings()
        {
            base.OnMappedBindings();
        }

        protected override void OnUnMappedBindings()
        {
            base.OnUnMappedBindings();
        }

    }
}