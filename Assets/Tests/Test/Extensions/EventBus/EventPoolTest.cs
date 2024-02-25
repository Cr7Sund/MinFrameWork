using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.EventBus
{
    public class EventPoolTest
    {
        private TestEventBus eventBus;
        private IPoolBinder poolBinder;
        private IInjectionBinder injectionBinder;


        [SetUp]
        public void SetUp()
        {
            eventBus = new TestEventBus();
            poolBinder = new PoolBinder();
            injectionBinder = new InjectionBinder();
            injectionBinder.Bind<IInjectionBinder>().To(injectionBinder);
            injectionBinder.Bind<IPoolBinder>().To(poolBinder);

            Console.Init(new InternalLogger());
            injectionBinder.Injector.Inject(eventBus);
        }

        [Test]
        public void TestPool_Register()
        {
            var listener = eventBus.TestListener<ClassTestEvent>();

            listener.Subscribe();
            var @event = poolBinder.AutoCreate<ClassTestEvent>();
            eventBus.Raise(@event);
            Assert.AreEqual(4, poolBinder.Get<ClassTestEvent>().TotalLength);
            Assert.AreEqual(4, poolBinder.Get<ClassTestEvent>().Available);
        }


    }
}