using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using System;
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
            var defaultBinding = new InjectionBinding(resolver);
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
            var defaultBinding = new InjectionBinding(resolver);
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
            var defaultBinding = new InjectionBinding(resolver);
            defaultBinding.Bind<ISimpleInterface>().ToValue(instance);
        }

        [Test]
        public void TestSingletonChainBinding()
        {
            int a = 0;

            Binder.BindingResolver resolver = delegate (IBinding binding, object oldName)
            {
                Assert.That(binding.Value as Type == typeof(InjectableDerivedClass));
                var correctType = a == 0 ? InjectionBindingType.DEFAULT : InjectionBindingType.SINGLETON;
                Assert.That((binding as InjectionBinding).Type == correctType);
                a++;
            };
            new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().AsSingleton();
        }

        [Test]
        public void TestValueChainBinding()
        {
            int a = 0;
            var testValue = new InjectableDerivedClass();

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

                var correctType = a == 0 ? InjectionBindingType.DEFAULT : InjectionBindingType.VALUE;
                Assert.That((binding as InjectionBinding).Type == correctType);

                a++;
            };
            new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToValue(testValue);
        }

        [Test]
        public void TestIllegalValueBinding()
        {
            var illegalValue = new MarkerClass();

            Binder.BindingResolver resolver = delegate
            {
            };
            TestDelegate testDelegate = delegate
            {
                new InjectionBinding(resolver).Bind<InjectableSuperClass>().ToValue(illegalValue);
            };
            var ex =
                Assert.Throws<InjectionException>(testDelegate);
            Assert.That(ex.Type == InjectionExceptionType.ILLEGAL_BINDING_VALUE);
        }

        [Test]
        public void TestIllegalTypeBinding()
        {
            var illegalValue = new MarkerClass();

            Binder.BindingResolver resolver = delegate
            {
            };
            TestDelegate testDelegate = delegate
            {
                new InjectionBinding(resolver).Bind<InjectableSuperClass>().To<SimpleInterfaceImplementer>();
            };
            var ex =
                Assert.Throws<InjectionException>(testDelegate);
            Assert.That(ex.Type == InjectionExceptionType.ILLEGAL_BINDING_VALUE);
        }
    }
}
