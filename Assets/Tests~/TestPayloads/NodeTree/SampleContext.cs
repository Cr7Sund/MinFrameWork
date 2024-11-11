using System;
using Cr7Sund.IocContainer;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleContext : CrossContext, INodeContext
    {
        public void AddComponents(INode self)
        {
        }

        public void RemoveComponents()
        {
        }
    }
}
