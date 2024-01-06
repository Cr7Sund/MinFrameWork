using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Framework.Tests
{
    public class SampleNode : Node
    {
        public SampleNode()
        {
            _context = new SampleContext();
        }

        public void AssignContext(IContext context)
        {
            _context = context;
        }
    }
}
