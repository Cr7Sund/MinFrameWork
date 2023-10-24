using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{

    public class TestInjectorFactory
    {
        IInjectorFactory factory;
        Binder.BindingResolver resolver;

        [SetUp]
        public void SetUp()
        {
            factory = new InjectorFactory();
            resolver = delegate (IBinding binding, object oldName) { };
        }

        [Test]
        public void TestInstantiation()
        {
            IInjectionBinding defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            InjectableDerivedClass testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
        }

        [Test]
        public void TestInstantiationFactory()
        {
            IInjectionBinding defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            InjectableDerivedClass testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
            int defaultValue = testResult.intValue;
            //Set a value
            testResult.intValue = 42;
            //Now get an instance again and ensure it's a different instance
            InjectableDerivedClass testResult2 = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.That(testResult2.intValue == defaultValue);
        }

        [Test]
        public void TestInstantiateSingleton()
        {
            IInjectionBinding defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().AsSingleton();
            InjectableDerivedClass testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;
            //Now get an instance again and ensure it's the same instance
            InjectableDerivedClass testResult2 = factory.Get(defaultBinding) as InjectableDerivedClass;
            Assert.That(testResult2.intValue == 42);
        }

        [Test]
        public void TestNamedInstances()
        {
            //Create two named instances
            IInjectionBinding defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.ONE);
            IInjectionBinding defaultBinding2 = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.TWO);

            InjectableDerivedClass testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            int defaultValue = testResult.intValue;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;

            //Now get an instance again and ensure it's a different instance
            InjectableDerivedClass testResult2 = factory.Get(defaultBinding2) as InjectableDerivedClass;
            Assert.IsNotNull(testResult2);
            Assert.That(testResult2.intValue == defaultValue);
        }

        //NOTE: Technically this test is redundant with the test above, since a named instance
        //is a de-facto Singleton
        [Test]
        public void TestNamedSingletons()
        {
            //Create two named singletons
            IInjectionBinding defaultBinding = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.ONE).AsSingleton();
            IInjectionBinding defaultBinding2 = new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName(SomeEnum.TWO).AsSingleton();

            InjectableDerivedClass testResult = factory.Get(defaultBinding) as InjectableDerivedClass;
            int defaultValue = testResult.intValue;
            Assert.IsNotNull(testResult);
            //Set a value
            testResult.intValue = 42;

            //Now get an instance again and ensure it's a different instance
            InjectableDerivedClass testResult2 = factory.Get(defaultBinding2) as InjectableDerivedClass;
            Assert.IsNotNull(testResult2);
            Assert.That(testResult2.intValue == defaultValue);
        }

        [Test]
        public void TestValueMap()
        {
            InjectableDerivedClass testvalue = new InjectableDerivedClass();
            testvalue.intValue = 42;
            IInjectionBinding binding = new InjectionBinding(resolver).Bind<InjectableSuperClass>()
            .To<InjectableDerivedClass>()
            .ToValue(testvalue);
            InjectableDerivedClass testResult = factory.Get(binding) as InjectableDerivedClass;
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
            binding.Bind<IPool<ClassToBeInjected>>().To<Pool<ClassToBeInjected>>().ToValue(pool);

            IPool<ClassToBeInjected> myPool = factory.Get(binding) as Pool<ClassToBeInjected>;
            Assert.NotNull(myPool);

            ClassToBeInjected instance1 = myPool.GetInstance() as ClassToBeInjected;
            Assert.NotNull(instance1);

            ClassToBeInjected instance2 = myPool.GetInstance() as ClassToBeInjected;
            Assert.NotNull(instance2);

            Assert.AreNotSame(instance1, instance2);
        }
    }
}