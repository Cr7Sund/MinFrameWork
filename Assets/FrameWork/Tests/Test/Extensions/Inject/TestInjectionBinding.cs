using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    public class TestInjectionBinding
    {
        [Test]
        public void TestDefaultType()
        {
            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                (binding as IInjectionBinding).Type = InjectionBindingType.DEFAULT;
                Assert.That(typeof(SimpleInterfaceImplementer) == binding.Value as Type);
                Assert.That((binding as InjectionBinding).Type == InjectionBindingType.DEFAULT);
            };
            InjectionBinding defaultBinding = new InjectionBinding(resolver);
            defaultBinding.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>();
        }

        [Test]
        public void TestSingletonType()
        {
            var instance = new SimpleInterfaceImplementer();

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                (binding as IInjectionBinding).Type = InjectionBindingType.SINGLETON;
                Assert.That(instance == binding.Value);
                Assert.That((binding as InjectionBinding).Type == InjectionBindingType.SINGLETON);
            };
            InjectionBinding defaultBinding = new InjectionBinding(resolver);
            defaultBinding.Bind<ISimpleInterface>().ToValue(instance);
        }

        [Test]
        public void TestValueType()
        {
            var instance = new SimpleInterfaceImplementer();
            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                (binding as IInjectionBinding).Type = InjectionBindingType.VALUE;
                Assert.That(instance == binding.Value);
                Assert.That((binding as InjectionBinding).Type == InjectionBindingType.VALUE);
            };
            InjectionBinding defaultBinding = new InjectionBinding(resolver);
            defaultBinding.Bind<ISimpleInterface>().ToValue(instance);
        }

        [Test]
        public void TestSingletonChainBinding()
        {
            int a = 0;

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                Assert.That(binding.Value as Type == typeof(InjectableDerivedClass));
                InjectionBindingType correctType = (a == 0) ? InjectionBindingType.DEFAULT : InjectionBindingType.SINGLETON;
                Assert.That((binding as InjectionBinding).Type == correctType);
                a++;
            };
            new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().AsSingleton();
        }

        [Test]
        public void TestValueChainBinding()
        {
            int a = 0;
            InjectableDerivedClass testValue = new InjectableDerivedClass();

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                // if (a == 2)
                if (a == 0)
                {
                    Assert.AreEqual(typeof(InjectableDerivedClass), binding.Value as Type);
                    // Value Constraint is one
                    Assert.That(binding.Value != testValue);
                }
                if (a == 1)
                {
                    Assert.That(binding.Value == testValue);
                }

                InjectionBindingType correctType = (a == 0) ? InjectionBindingType.DEFAULT : InjectionBindingType.VALUE;
                Assert.That((binding as InjectionBinding).Type == correctType);

                a++;
            };
            new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToValue(testValue);
        }

        [Test]
        public void TestIllegalValueBinding()
        {
            MarkerClass illegalValue = new MarkerClass();

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName) { };
            TestDelegate testDelegate = delegate ()
            {
                new InjectionBinding(resolver).Bind<InjectableSuperClass>().ToValue(illegalValue);
            };
            InjectionException ex =
                Assert.Throws<InjectionException>(testDelegate);
            Assert.That(ex.type == InjectionExceptionType.ILLEGAL_BINDING_VALUE);
        }

        [Test]
        public void TestIllegalTypeBinding()
        {
            MarkerClass illegalValue = new MarkerClass();

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName) { };
            TestDelegate testDelegate = delegate ()
            {
                new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<SimpleInterfaceImplementer>();
            };
            InjectionException ex =
                Assert.Throws<InjectionException>(testDelegate);
            Assert.That(ex.type == InjectionExceptionType.ILLEGAL_BINDING_VALUE);
        }
    }
}