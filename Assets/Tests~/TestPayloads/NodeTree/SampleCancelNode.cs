using System.Threading;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework.Constraints;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleCancelNode : Node
    {
        public static IPromise LoadPromise;
        public SampleCancelNode(IAssetKey assetKey) : base(assetKey)
        {
            _context = new SampleContext();
        }

        public SampleCancelNode() : this(null)
        {

        }
   
        protected override async PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation)
        {
            cancellation.Register(LoadPromise.Cancel);
            await LoadPromise.Task;
        }
    }
}
