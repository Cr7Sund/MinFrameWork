using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.IOC;

namespace Cr7Sund.PackageTest.IOC
{
    public interface IUsesPool
    {
        ISimpleInterface Instance1 { get; set; }
        ISimpleInterface Instance2 { get; set; }
    }
    public class UsesPool : IUsesPool
    {
        [Inject]
        public IPool<SimpleInterfaceImplementer> pool;

        public ISimpleInterface Instance1 { get; set; }
        public ISimpleInterface Instance2 { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            Instance1 = pool.GetInstance();
            Instance2 = pool.GetInstance();
        }
    }
}
