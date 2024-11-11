using Cr7Sund.FrameWork.Util;
using Cr7Sund.IocContainer;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using NUnit.Framework;
namespace Cr7Sund.PackageTest.IOC
{

    public class TestInjectorFactory
    {
        private IInjectorFactory factory;
        private Binder.BindingResolver resolver;

        [SetUp]
        public void SetUp()
        {
            factory = new InjectorFactory();
            resolver = delegate
            {
            };
        }

        [Test]
        public void TestInstantiation()
        {
            var defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            var testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
        }

        [Test]
        public void TestInstantiationFactory()
        {
            var defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            var testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
            int defaultValue = testResult.intValue;
            //Set a value
            testResult.intValue = 42;
            //Now get an instance again and ensure it's a different instance
            var testResult2 = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.That(testResult2.intValue == defaultValue);
        }

        [Test]
        public void TestInstantiateSingleton()
        {
            var defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().AsSingleton();
            var testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;
            //Now get an instance again and ensure it's the same instance
            var testResult2 = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.That(testResult2.intValue == 42);
        }

        [Test]
        public void TestNamedInstances()
        {
            //Create two named instances
            var defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.ONE);
            var defaultBinding2 = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.TWO);

            var testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            int defaultValue = testResult.intValue;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;

            //Now get an instance again and ensure it's a different instance
            var testResult2 = factory.Get(defaultBinding2) as InjectableDerivedClass;
            Assert.IsNotNull(testResult2);
            Assert.That(testResult2.intValue == defaultValue);
        }

        //NOTE: Technically this test is redundant with the test above, since a named instance
        //is a de-facto Singleton
        [Test]
        public void TestNamedSingletons()
        {
            //Create two named singletons
            var defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.ONE).AsSingleton();
            var defaultBinding2 = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.TWO).AsSingleton();

            var testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            int defaultValue = testResult.intValue;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;

            //Now get an instance again and ensure it's a different instance
            var testResult2 = factory.Get(defaultBinding2) as InjectableDerivedClass;
            Assert.IsNotNull(testResult2);
            Assert.That(testResult2.intValue == defaultValue);
        }

        [Test]
        public void TestValueMap()
        {
            var testvalue = new InjectableDerivedClass();
            testvalue.intValue = 42;
            var binding = new InjectionBinding(resolver).Bind<InjectableSuperClass>()
                .To<InjectableDerivedClass>()
                .To(testvalue);
            var testResult = factory.Get(binding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
            Assert.That(testResult.intValue == testvalue.intValue);
            Assert.That(testResult.intValue == 42);
        }

        // NOTE: Due to a limitation in the version of C# used by Unity,
        // IT IS NOT POSSIBLE TO MAP GENERICS ABSTRACTLY!!!!!
        // Therefore, pools must be mapped to concrete instance types. (Yeah, this blows.)
        [Test]
        public void TestGetFromPool()
        {
            IPool<ClassToBeInjected> pool = new Pool<ClassToBeInjected>();
            // Format the pool
            pool.SetSize(4);
            pool.InstanceProvider = new TestInstanceProvider();

            IInjectionBinding binding = new InjectionBinding(resolver);
            binding.Bind<IPool<ClassToBeInjected>>().To<Pool<ClassToBeInjected>>().To(pool);

            IPool<ClassToBeInjected> myPool = factory.Get(binding) as Pool<ClassToBeInjected>;
            Assert.NotNull(myPool);

            var instance1 = myPool.GetInstance();
            Assert.NotNull(instance1);

            var instance2 = myPool.GetInstance();
            Assert.NotNull(instance2);

            Assert.AreNotSame(instance1, instance2);
        }
    }
}
