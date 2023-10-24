using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    class TestBinding
    {
        IBinding binding;

        [SetUp]
        public void Setup()
        {
            binding = new Binding();
        }

        [Test]
        public void TestKeyAsType()
        {
            binding.Bind<MarkerClass>();
            Assert.That(binding.Key as Type == typeof(MarkerClass));
        }
        [Test]
        public void TestKeyToAsTypes()
        {
            binding.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            Assert.That(binding.Key as Type == typeof(InjectableSuperClass));
            Assert.That((Type)binding.Value == typeof(InjectableDerivedClass));
        }

        [Test]
        public void TestNameAsType()
        {
            binding.Bind<InjectableSuperClass>().To<InjectableDerivedClass>().ToName("MarkerClass");
            Assert.AreEqual("MarkerClass", ((Binding)binding).Name);
        }

        [Test]
        public void TestKeyToWithMultipleChainedValues()
        {
            ClassWithConstructorParameters test1 = new ClassWithConstructorParameters(1, "abc");
            ClassWithConstructorParameters test2 = new ClassWithConstructorParameters(2, "def");
            ClassWithConstructorParameters test3 = new ClassWithConstructorParameters(3, "ghi");

            ((Binding)binding).ValueConstraint = BindingConstraintType.MANY;
            // binding.Unique = false;

            binding.Bind<ISimpleInterface>()
                    .ToValue(test1).ToName("abc")
                    .ToValue(test2).ToName("def")
                    .ToValue(test3).ToName("ghi");
            Assert.That(binding.Key as Type == typeof(ISimpleInterface));

            object[] values = ((Binding)binding).Value as object[];
            Assert.IsNotNull(values);
            Assert.That(values.Length == 3);
            for (int a = 0; a < values.Length; a++)
            {
                ISimpleInterface value = values[a] as ISimpleInterface;
                Assert.IsNotNull(value);
                Assert.That(value.intValue == a + 1);
            }
        }

    }
}