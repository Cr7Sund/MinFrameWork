using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using NUnit.Framework;
using System;
namespace Cr7Sund.PackageTest.IOC
{
    internal class TestBinder
    {
        private IBinder binder;

        [SetUp]
        public void SetUp()
        {
            binder = new Binder();
        }

        [Test]
        public void TestBindType()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            var binding = binder.GetBinding<InjectableSuperClass>();

            Assert.IsNotNull(binding);
            Assert.AreEqual(typeof(InjectableSuperClass), binding.Key.SingleValue);
            Assert.AreNotEqual(typeof(InjectableDerivedClass), binding.Key.SingleValue);
            Assert.AreEqual(typeof(InjectableDerivedClass), binding.Value.SingleValue);
        }

        [Test]
        public void TestNameBinding()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClassOne>();
            binder.Bind<InjectableSuperClass>().ToName("marker").To<InjectableDerivedClassTwo>();
            binder.Bind<InjectableSuperClass>().ToName("strange").To<InjectableDerivedClassThree>();

            var binding = binder.GetBinding<InjectableSuperClass>();
            Assert.IsNotNull(binding);
            Assert.AreEqual(binding.Key.SingleValue, typeof(InjectableSuperClass));

            var unNameValue = binding.Value.SingleValue as Type;
            Assert.AreEqual(typeof(InjectableDerivedClassOne), unNameValue);
            Assert.AreNotEqual(typeof(InjectableDerivedClassTwo), unNameValue);

            binding = binder.GetBinding<InjectableSuperClass>("marker");
            var nameValue = binding.Value.SingleValue as Type;
            Assert.AreEqual(typeof(InjectableDerivedClassTwo), nameValue);
            Assert.AreNotEqual(typeof(InjectableDerivedClassOne), nameValue);

            binding = binder.GetBinding<InjectableSuperClass>("strange");
            nameValue = binding.Value.SingleValue as Type;
            Assert.AreEqual(typeof(InjectableDerivedClassThree), nameValue);
            Assert.AreNotEqual(typeof(InjectableDerivedClassOne), nameValue);
            Assert.AreNotEqual(typeof(InjectableDerivedClassTwo), nameValue);
        }


        [Test]
        public void TestUnBind()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClass>();
            var binding = binder.GetBinding<InjectableSuperClass>();
            Assert.IsNotNull(binding);
            binder.Unbind<InjectableSuperClass>();
            binding = binder.GetBinding<InjectableSuperClass>();
            Assert.IsNull(binding);
        }

        [Test]
        public void TestOverwriteBinding()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClassOne>().ToName("duplicate").Weak();
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClassTwo>().ToName("duplicate");

            Assert.AreEqual(typeof(InjectableDerivedClassTwo), binder.GetBinding<InjectableSuperClass>("duplicate").Value.SingleValue as Type);
        }

        [Test]
        public void TestConflict_Binder_exception()
        {
            binder.Bind<InjectableSuperClass>().To<InjectableDerivedClassOne>();


            var ex = Assert.Throws<MyException>(() => { binder.Bind<InjectableSuperClass>().To<InjectableDerivedClassTwo>(); });
            UnityEngine.Assertions.Assert.AreEqual(BinderExceptionType.CONFLICT_IN_BINDER, ex.Type);
        }
    }
}
