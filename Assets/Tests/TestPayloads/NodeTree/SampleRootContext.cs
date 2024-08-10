using Cr7Sund.NodeTree.Api;
using Cr7Sund.IocContainer;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.PackageTest.IOC
{
    public class SampleRootContext : CrossContext,INodeContext
    {
        public SampleRootContext() : base()
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }
        public  void AddComponents(INode self)
        {
            BindAsCrossAndSingleton<IPoolBinder,PoolBinder>();
        }

        public  void RemoveComponents()
        {
        }
    }
}
