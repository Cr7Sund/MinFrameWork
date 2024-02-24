using System;

using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleRootContext : CrossContext
    {
        public SampleRootContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }
        public override void AddComponents(INode self)
        {
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext();
        }

        public override void RemoveComponents()
        {
        }
    }
}
