using Cr7Sund.IocContainer;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
using System;
namespace Cr7Sund.PackageTest.IOC
{
    public class TestBinding
    {
        private IBinding binding;

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
            Assert.That((Type)binding.Value.SingleValue == typeof(InjectableDerivedClass));
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
            var test1 = new ClassWithConstructorParameters(1, "abc");
            var test2 = new ClassWithConstructorParameters(2, "def");
            var test3 = new ClassWithConstructorParameters(3, "ghi");

            ((Binding)binding).ValueConstraint = BindingConstraintType.MANY;
            // binding.Unique = false;

            binding.Bind<ISimpleInterface>()
                .To(test1).ToName("abc")
                .To(test2).ToName("def")
                .To(test3).ToName("ghi");
            Assert.That(binding.Key as Type == typeof(ISimpleInterface));

            ISemiBinding values = ((Binding)binding).Value ;
            Assert.IsNotNull(values);
            Assert.That(values.Count == 3);
            for (int a = 0; a < values.Count; a++)
            {
                var value = values[a] as ISimpleInterface;
                Assert.IsNotNull(value);
                Assert.That(value.intValue == a + 1);
            }
        }
    }
}
