using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    public class TestPoolBinder
    {
        [Test]
        public void GetInstance()
        {
            var poolBinder = new PoolBinder();
            var instance1 = poolBinder.AutoCreate<ClassToBeInjected>();
            var instance2 = poolBinder.AutoCreate<ClassToBeInjected>();

            Assert.AreEqual(2, poolBinder.Get<ClassToBeInjected>().Count);
            Assert.AreEqual(4, poolBinder.Get<ClassToBeInjected>().TotalLength);
            poolBinder.Return(instance1);
            poolBinder.Return(instance2);
        }

        [Test]
        public void ReturnInstance()
        {
            var poolBinder = new PoolBinder();
            var instance1 = poolBinder.AutoCreate<ClassToBeInjected>();
            var instance2 = poolBinder.AutoCreate<ClassToBeInjected>();


            poolBinder.Return(instance1);
            poolBinder.Return(instance2);
            Assert.AreEqual(0, poolBinder.Get<ClassToBeInjected>().Count);
            Assert.AreEqual(4, poolBinder.Get<ClassToBeInjected>().TotalLength);
        }

        [Test]
        public void GetInstance_Increase()
        {
            var poolBinder = new PoolBinder();
            var pool = poolBinder.GetOrCreate<ClassToBeInjected>();
            pool.InflationType = Api.PoolInflationType.INCREMENT;

            var instance1 = pool.GetInstance();
            var instance2 = poolBinder.AutoCreate<ClassToBeInjected>();

            Assert.AreEqual(2, poolBinder.Get<ClassToBeInjected>().Count);
            Assert.AreEqual(0, poolBinder.Get<ClassToBeInjected>().Available);
        }

        [Test]
        public void GetDifferentInstance()
        {
            var poolBinder = new PoolBinder();
            var instance1 = poolBinder.AutoCreate<ClassToBeInjected>();
            var instance2 = poolBinder.AutoCreate<SimpleInterfaceImplementer>();

            Assert.AreEqual(1, poolBinder.Get<ClassToBeInjected>().Count);
            Assert.AreEqual(1, poolBinder.Get<SimpleInterfaceImplementer>().Count);
        }

        [Test]
        public void ClearPoolBinder()
        {
            var poolBinder = new PoolBinder();
            var instance1 = poolBinder.AutoCreate<ClassToBeInjected>();
            var instance2 = poolBinder.AutoCreate<SimpleInterfaceImplementer>();

            poolBinder.Return(instance1);
            poolBinder.CleanUnreference();

            Assert.Null(poolBinder.Get<ClassToBeInjected>());
            Assert.AreEqual(1, poolBinder.Get<SimpleInterfaceImplementer>().Count);
        }

        [Test]
        public void ClearAllPoolBinder()
        {
            var poolBinder = new PoolBinder();
            var instance1 = poolBinder.AutoCreate<ClassToBeInjected>();
            var instance2 = poolBinder.AutoCreate<SimpleInterfaceImplementer>();

            poolBinder.Return(instance1);
            poolBinder.Return(instance2);
            poolBinder.CleanUnreference();

            Assert.AreEqual(0, poolBinder.Test_GetPoolCount());
        }
    }
}