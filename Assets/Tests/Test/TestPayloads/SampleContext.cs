using System;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Framework.Tests
{
    public class SampleContext : CrossContext
    {
        public override void MapBindings()
        {
        }
        public override void UnMappedBindings()
        {
            throw new NotImplementedException();
        }
    }
}
