using System;

using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleRootContext : CrossContext
    {
        public SampleRootContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }
        public override void AddComponents()
        {
            InjectionBinder.Bind<IPoolBinder>().To<PoolBinder>().AsCrossContext();
        }

        public override void RemoveComponents()
        {
        }
    }
}
