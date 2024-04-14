using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace Cr7Sund.PackageTest.IOC
{
    [TestFixture]
    public class TestinjectionBinder : IPrebuildSetup, IPostBuildCleanup
    {

        [SetUp]
        public void Setup()
        {
            binder = new InjectionBinder();
        }
        private IInjectionBinder binder;

        public void Cleanup()
        {
            GuaranteedUniqueInstances.Reset();
        }

        [Test]
        public void TestInjectorExists()
        {
            Assert.That(binder.Injector != null);
        }

        [Test]
        public void TestGetBindingFlat()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableSuperClass>();
            var binding = binder.GetBinding<InjectableSuperClass>();
            Assert.IsNotNull(binding);
        }

        [Test]
        public void TestGetBindingAbstract()
        {
            binder.Bind<ISimpleInterface>().To<ClassWithConstructorParameters>();
            var binding = binder.GetBinding<ISimpleInterface>();
            Assert.IsNotNull(binding);
        }

        [Test]
        public void TestGetNamedBinding()
        {
            binder.Bind<ISimpleInterface>().To<ClassWithConstructorParameters>().ToName(nameof(MarkerClass));
            var binding = binder.GetBinding<ISimpleInterface>(nameof(MarkerClass));
            Assert.IsNotNull(binding);
        }

        [Test]
        public void TestGetInstance1()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

            var instance = binder.GetInstance(typeof(ClassToBeInjected)) as ClassToBeInjected;

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestGetInstance2()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();

            var instance = binder.GetInstance<ClassToBeInjected>();

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestGetNamedInstance1()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>().ToName(nameof(MarkerClass));

            var instance = binder.GetInstance(typeof(ClassToBeInjected), nameof(MarkerClass)) as ClassToBeInjected;

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestGetNamedInstance2()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>().ToName(nameof(MarkerClass));

            var instance = binder.GetInstance<ClassToBeInjected>(nameof(MarkerClass));

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestGetNamedInstance3()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>().ToName(SomeEnum.ONE);

            var instance = binder.GetInstance(typeof(ClassToBeInjected), SomeEnum.ONE) as ClassToBeInjected;

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestGetNamedInstance4()
        {
            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>().ToName(SomeEnum.ONE);

            var instance = binder.GetInstance<ClassToBeInjected>(SomeEnum.ONE);

            Assert.IsNotNull(instance);
            Assert.That(instance is ClassToBeInjected);
        }

        [Test]
        public void TestInjectionErrorFailureToProvideDependency()
        {
            TestDelegate testDelegate = delegate
            {
                binder.GetInstance<InjectableSuperClass>();
            };
            binder.Bind<InjectableSuperClass>().To<InjectableSuperClass>();
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(InjectionExceptionType.NULL_BINDING_GET_INJECT, ex.Type);
        }

        [Test]
        public void TestInjectionProvideIntDependency()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableSuperClass>();
            binder.Bind<int>().To(42);
            var testValue = binder.GetInstance<InjectableSuperClass>();
            Assert.IsNotNull(testValue);
            Assert.That(testValue.intValue == 42);
        }

        [Test]
        public void TestRemoveDependency()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableSuperClass>();
            binder.Bind<int>().To(42);
            var testValueBeforeUnbinding = binder.GetInstance<InjectableSuperClass>();
            Assert.IsNotNull(testValueBeforeUnbinding);
            Assert.That(testValueBeforeUnbinding.intValue == 42);

            binder.Unbind<int>();

            TestDelegate testDelegate = delegate
            {
                binder.GetInstance<InjectableSuperClass>();
            };

            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(InjectionExceptionType.NULL_BINDING_GET_INJECT, ex.Type);
        }

        [Test]
        public void TestDefaultInjection()
        {
            GuaranteedUniqueInstances.Reset();

            binder.Bind<GuaranteedUniqueInstances>().To<GuaranteedUniqueInstances>().ToName(SomeEnum.ONE);
            var instance1 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance2 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance3 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance4 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            Assert.AreNotEqual(instance1.uid, instance2.uid);
            Assert.AreNotSame(instance1, instance2);
            Assert.AreEqual(1, instance1.uid);
            Assert.AreEqual(2, instance2.uid);
            Assert.AreEqual(3, instance3.uid);
            Assert.AreEqual(4, instance4.uid);
        }


        [Test]
        public void TestInjectionToPool()
        {
            GuaranteedUniqueInstances.Reset();

            binder.Bind<GuaranteedUniqueInstances>().To<GuaranteedUniqueInstances>().ToName(SomeEnum.ONE).AsPool();
            var instance1 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance2 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            Assert.AreNotEqual(instance1.uid, instance2.uid);
            Assert.AreNotSame(instance1, instance2);
            Assert.AreEqual(1, instance1.uid);
            Assert.AreEqual(2, instance2.uid);

            binder.ReleaseInstance(instance1, SomeEnum.ONE);
            binder.ReleaseInstance(instance2, SomeEnum.ONE);
            var instance3 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance4 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            Assert.LessOrEqual(instance3.uid, 2);
            Assert.LessOrEqual(instance4.uid, 2);
        }

        [Test]
        public void TestReleaseNullToPool()
        {
            GuaranteedUniqueInstances.Reset();

            binder.Bind<GuaranteedUniqueInstances>().To<GuaranteedUniqueInstances>().ToName(SomeEnum.ONE).AsPool();
            var instance1 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance2 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            Assert.AreNotEqual(instance1.uid, instance2.uid);
            Assert.AreNotSame(instance1, instance2);
            Assert.AreEqual(1, instance1.uid);
            Assert.AreEqual(2, instance2.uid);

            AssignNull(ref instance1);
            Assert.IsNull(instance1);

            TestDelegate testDelegate = delegate
            {
                binder.ReleaseInstance(instance1, SomeEnum.ONE);
                binder.ReleaseInstance(instance2, SomeEnum.ONE);
            };
            Assert.Throws<MyException>(testDelegate);
            var instance3 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            var instance4 = binder.GetInstance<GuaranteedUniqueInstances>(SomeEnum.ONE);
            Assert.LessOrEqual(instance3.uid, 4); // since we dont actually return , we will expand the pool to adjust
            Assert.LessOrEqual(instance4.uid, 4);
        }

        private void AssignNull(ref GuaranteedUniqueInstances instances)
        {
            instances = null;
        }

        [Test]
        public void TestValueToSingleton()
        {
            var uniqueInstance = new GuaranteedUniqueInstances();
            binder.Bind<GuaranteedUniqueInstances>().To(uniqueInstance);
            var instance1 = binder.GetInstance<GuaranteedUniqueInstances>();
            var instance2 = binder.GetInstance<GuaranteedUniqueInstances>();
            Assert.AreEqual(instance1.uid, instance2.uid);
            Assert.AreSame(instance1, instance2);
        }

        //RE: Issue #23. A value-mapping trumps a Singleton mapping
        [Test]
        public void TestValueToSingletonBinding()
        {
            var instance = new InjectableSuperClass();
            var binding = binder.Bind<InjectableSuperClass>().To(instance).AsSingleton() as InjectionBinding;
            Assert.AreEqual(InjectionBindingType.VALUE, binding.Type);
        }

        //RE: Issue #23. A value-mapping trumps a Singleton mapping
        [Test]
        public void TestSingletonToValueBinding()
        {
            var instance = new InjectableSuperClass();
            var binding = binder.Bind<InjectableSuperClass>().AsSingleton().To(instance) as InjectionBinding;
            Assert.AreEqual(InjectionBindingType.VALUE, binding.Type);
        }


        //RE:Issue #34. Ensure that a Singleton instance can properly use constructor injection
        [Test]
        public void TestConstructorToSingleton()
        {
            ConstructorInjectsClassToBeInjected.Value = 0;

            binder.Bind<ClassToBeInjected>().To<ClassToBeInjected>();
            binder.Bind<ConstructorInjectsClassToBeInjected>().To<ConstructorInjectsClassToBeInjected>().AsSingleton();
            var instance = binder.GetInstance<ConstructorInjectsClassToBeInjected>();
            Assert.IsNotNull(instance.injected);
            Assert.AreEqual(1, ConstructorInjectsClassToBeInjected.Value);
        }

        [Test]
        public void TestPolymorphicBinding()
        {
            binder.Bind<ISimpleInterface>().Bind<IAnotherSimpleInterface>().To<PolymorphicClass>();

            var callOnce = binder.GetInstance<ISimpleInterface>();
            Assert.NotNull(callOnce);
            Assert.IsInstanceOf<PolymorphicClass>(callOnce);

            var callAgain = binder.GetInstance<IAnotherSimpleInterface>();
            Assert.NotNull(callAgain);
            Assert.IsInstanceOf<PolymorphicClass>(callAgain);
        }

        [Test]
        public void TestPolymorphicSingleton()
        {
            binder.Bind<ISimpleInterface>().Bind<IAnotherSimpleInterface>().To<PolymorphicClass>().AsSingleton();

            var callOnce = binder.GetInstance<ISimpleInterface>();
            Assert.NotNull(callOnce);
            Assert.IsInstanceOf<PolymorphicClass>(callOnce);

            var callAgain = binder.GetInstance<IAnotherSimpleInterface>();
            Assert.NotNull(callAgain);
            Assert.IsInstanceOf<PolymorphicClass>(callAgain);

            callOnce.intValue = 42;

            Assert.AreSame(callOnce, callAgain);
            Assert.AreEqual(42, (callAgain as ISimpleInterface).intValue);
        }

        [Test]
        public void TestNamedInstanceBeforeUnnamedInstance()
        {
            binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToName(SomeEnum.ONE);
            binder.Bind<ISimpleInterface>().To<PolymorphicClass>();

            var instance1 = binder.GetInstance<ISimpleInterface>(SomeEnum.ONE);
            var instance2 = binder.GetInstance<ISimpleInterface>();

            Assert.That(instance1 is SimpleInterfaceImplementer);
            Assert.That(instance2 is PolymorphicClass);
        }


        [Test]
        public void TestUnnamedInstanceBeforeNamedInstance()
        {
            binder.Bind<ISimpleInterface>().To<PolymorphicClass>();
            binder.Bind<ISimpleInterface>().ToName(SomeEnum.ONE).To<SimpleInterfaceImplementer>();

            var instance1 = binder.GetInstance<ISimpleInterface>(SomeEnum.ONE);
            var instance2 = binder.GetInstance<ISimpleInterface>();

            Assert.That(instance1 is SimpleInterfaceImplementer);
            Assert.That(instance2 is PolymorphicClass);
        }

        [Test]
        public void TestPrereflectOne()
        {
            binder.Bind<ISimpleInterface>().Bind<IAnotherSimpleInterface>().To<PolymorphicClass>();

            var list = new List<Type>();
            list.Add(typeof(PolymorphicClass));
            int count = binder.Reflect(list);

            Assert.AreEqual(1, count);

            var reflected = binder.Injector.Reflector.Get<PolymorphicClass>();
            Assert.True(((ReflectedClass)reflected).PreGenerated);
        }

        [Test]
        public void TestPrereflectMany()
        {
            binder.Bind<HasNamedInjections>().To<HasNamedInjections>();
            binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToName(SomeEnum.ONE);
            binder.Bind<ISimpleInterface>().To<PolymorphicClass>();
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            binder.Bind<int>().To(42);
            binder.Bind<string>().To("zaphod"); //primitives won't get reflected...

            var list = new List<Type>();
            list.Add(typeof(HasNamedInjections));
            list.Add(typeof(SimpleInterfaceImplementer));
            list.Add(typeof(PolymorphicClass));
            list.Add(typeof(InjectableDerivedClass));
            list.Add(typeof(int));

            int count = binder.Reflect(list);
            Assert.AreEqual(4, count); //...so list length will not include primitives

            var reflected1 = binder.Injector.Reflector.Get<HasNamedInjections>();
            Assert.True(((ReflectedClass)reflected1).PreGenerated);

            var reflected2 = binder.Injector.Reflector.Get<SimpleInterfaceImplementer>();
            Assert.True(((ReflectedClass)reflected2).PreGenerated);

            var reflected3 = binder.Injector.Reflector.Get<PolymorphicClass>();
            Assert.True(((ReflectedClass)reflected3).PreGenerated);
            Assert.AreNotEqual(reflected2.Constructor, reflected3.Constructor);

            var reflected4 = binder.Injector.Reflector.Get<InjectableDerivedClass>();
            Assert.True(((ReflectedClass)reflected4).PreGenerated);
        }

        [Test]
        public void TestPrereflectAll()
        {
            binder.Bind<HasNamedInjections>().To<HasNamedInjections>();
            binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToName(SomeEnum.ONE);
            binder.Bind<ISimpleInterface>().To<PolymorphicClass>();
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            binder.Bind<int>().To(42);
            binder.Bind<string>().To("zaphod"); //primitives won't get reflected...

            int count = binder.ReflectAll();
            Assert.AreEqual(4, count); //...so list length will not include primitives

            var s = binder.GetInstance<ISimpleInterface>();
            Assert.IsTrue(s is PolymorphicClass);

            var reflected1 = binder.Injector.Reflector.Get<HasNamedInjections>();
            Assert.True(((ReflectedClass)reflected1).PreGenerated);

            var reflected2 = binder.Injector.Reflector.Get<SimpleInterfaceImplementer>();
            Assert.True(((ReflectedClass)reflected2).PreGenerated);

            var reflected3 = binder.Injector.Reflector.Get<PolymorphicClass>();
            Assert.True(((ReflectedClass)reflected3).PreGenerated);
            Assert.AreNotEqual(reflected2.Constructor, reflected3.Constructor);

            var reflected4 = binder.Injector.Reflector.Get<InjectableDerivedClass>();
            Assert.True(((ReflectedClass)reflected4).PreGenerated);

        }


    }


}
