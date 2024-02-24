using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleNode : Node
    {
        public SampleNode(IAssetKey assetKey) : base(assetKey)
        {
            _context = new SampleContext();
        }

        public SampleNode() : this(null)
        {

        }
    }
}
