using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Framework.Tests
{
    public class SampleRootContext : CrossContext
    {
        public SampleRootContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }
        public override void MapBindings()
        {
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext();
        }
        public override void UnMappedBindings()
        {
            throw new NotImplementedException();
        }
    }
}
