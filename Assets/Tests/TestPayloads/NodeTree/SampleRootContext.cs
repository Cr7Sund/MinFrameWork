using System;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.NodeTree.Impl;
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
