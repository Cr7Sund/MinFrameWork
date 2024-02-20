using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.FrameWork.Util;
using NUnit.Framework;
using System;
namespace Cr7Sund.PackageTest.Inject
{
    internal class TestInjector
    {
        private IInjectionBinder binder;

        [SetUp]
        public void SetUp()
        {
            binder = new InjectionBinder();
        }

        [Test]
        public void TestNoParamsConstructor()
        {
            binder.Bind<ClassWithConstructorParameters>().To<ClassWithConstructorParameters>();
            var instance =
                binder.GetInstance<ClassWithConstructorParameters>();
            Assert.IsNotNull(instance);
            Assert.AreEqual(42, instance.intValue);
            Assert.AreEqual("Liberator", instance.stringValue);
        }

        [Test]
        public void TestPostConstruct()
        {
            binder.Bind<PostConstructClass>().To<PostConstructClass>();
            binder.Bind<float>().To((float)Math.PI);
            var instance = binder.GetInstance<PostConstructClass>();
            Assert.IsNotNull(instance);
            Assert.AreEqual((float)Math.PI * 2f, instance.floatVal);
        }

        // A value-mapped object must never attempt to re-instantiate
        [Test]
        public void TestValueMappingWithNoReInstantiate()
        {
            string stringVal = "Ender Wiggin";
            var instance = new ClassWithConstructorParametersOnlyOneConstructor(stringVal);

            binder.Bind<ClassWithConstructorParametersOnlyOneConstructor>().To(instance);
            //If this class attempts to construct, with no string mapped, there'll be an error
            var instance2 = binder.GetInstance<ClassWithConstructorParametersOnlyOneConstructor>();
            Assert.AreSame(instance, instance2);
            Assert.AreEqual(stringVal, instance2.stringVal);
        }

        [Test]
        public void TestNameInstances()
        {
            var testValue = new InjectableSuperClass();
            float defaultFloatValue = testValue.floatValue;
            testValue.floatValue = (float)Math.PI;

            binder.Bind<int>().To(20);
            var binding = binder.Bind<InjectableSuperClass>()
                .To<InjectableSuperClass>()
                .ToName(SomeEnum.ONE)
                .AsSingleton();
            binder.Bind<InjectableSuperClass>().To(testValue).ToName(SomeEnum.TWO);
            binder.Bind<HasNamedInjections>().To<HasNamedInjections>();

            var instance = binder.GetInstance<HasNamedInjections>();
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.injectionOne);
            Assert.IsNotNull(instance.injectionTwo);
            Assert.AreEqual(20, instance.injectionOne.intValue);
            Assert.AreEqual(20, instance.injectionTwo.intValue);
            Assert.AreEqual(defaultFloatValue, instance.injectionOne.floatValue);
            Assert.AreEqual((float)Math.PI, instance.injectionTwo.floatValue);
        }

        [Test]
        public void TestNamedFactories()
        {
            binder.Bind<int>().To(20);
            binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementer>().ToName(SomeEnum.ONE);
            binder.Bind<ISimpleInterface>().To<SimpleInterfaceImplementerTwo>().ToName(SomeEnum.TWO);

            var valueOneOne = binder.GetInstance<ISimpleInterface>(SomeEnum.ONE);
            var valueOneTwo = binder.GetInstance<ISimpleInterface>(SomeEnum.ONE);

            var valueTwoOne = binder.GetInstance<ISimpleInterface>(SomeEnum.TWO);
            var valueTwoTwo = binder.GetInstance<ISimpleInterface>(SomeEnum.TWO);

            //Of course nothing should return null
            Assert.NotNull(valueOneOne);
            Assert.NotNull(valueOneTwo);
            Assert.NotNull(valueTwoOne);
            Assert.NotNull(valueTwoTwo);

            //All four instances should be unique.
            Assert.AreNotSame(valueOneOne, valueOneTwo);
            Assert.AreNotSame(valueOneTwo, valueTwoOne);
            Assert.AreNotSame(valueTwoOne, valueTwoTwo);
            Assert.AreNotSame(valueOneOne, valueTwoTwo);
            //First pair should be of type SimpleInterfaceImplementer.
            Assert.IsInstanceOf<SimpleInterfaceImplementer>(valueOneOne);
            Assert.IsInstanceOf<SimpleInterfaceImplementer>(valueOneTwo);
            //Second pair should be of type SimpleInterfaceImplementerTwo.
            Assert.IsInstanceOf<SimpleInterfaceImplementerTwo>(valueTwoOne);
            Assert.IsInstanceOf<SimpleInterfaceImplementerTwo>(valueTwoTwo);
        }

        [Test]
        public void TestOnlyParamConstructorException()
        {
            binder.Bind<IMapConfig>().To(new MapConfig());
            binder.Bind<IMap>().To<Map>().AsSingleton();

            TestDelegate testDelegate = () =>
            {
                binder.GetInstance<IMap>();
            };
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(InjectionExceptionType.NONEMPTY_CONSTRUCTOR, ex.Type);
        }
    }
}
