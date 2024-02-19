using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using NUnit.Framework;
using System;
using System.Collections;
namespace Cr7Sund.PackageTest.IOC
{
    public class TestPool
    {
        private Pool<ClassToBeInjected> pool;


        [SetUp]
        public void Setup()
        {
            pool = new Pool<ClassToBeInjected>();
            pool.InflationType = PoolInflationType.INCREMENT;
        }


        [Test]
        public void TestSetPoolProperties()
        {
            Assert.AreEqual(0, pool.TotalLength);

            pool.SetSize(100);
            Assert.AreEqual(0, pool.TotalLength);

            pool.OverflowBehavior = PoolOverflowBehavior.EXCEPTION;
            Assert.AreEqual(PoolOverflowBehavior.EXCEPTION, pool.OverflowBehavior);

            pool.OverflowBehavior = PoolOverflowBehavior.WARNING;
            Assert.AreEqual(PoolOverflowBehavior.WARNING, pool.OverflowBehavior);

            pool.OverflowBehavior = PoolOverflowBehavior.IGNORE;
            Assert.AreEqual(PoolOverflowBehavior.IGNORE, pool.OverflowBehavior);

            pool.InflationType = PoolInflationType.DOUBLE;
            Assert.AreEqual(PoolInflationType.DOUBLE, pool.InflationType);

            pool.InflationType = PoolInflationType.INCREMENT;
            Assert.AreEqual(PoolInflationType.INCREMENT, pool.InflationType);
        }

        [Test]
        public void TestAdd()
        {
            pool.SetSize(4);
            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.Add(new ClassToBeInjected());
                Assert.AreEqual(a + 1, pool.Available);
            }
        }

        [Test]
        public void TestAddList()
        {
            pool.SetSize(4);
            var list = new ClassToBeInjected[pool.TotalLength];
            for (int a = 0; a < pool.TotalLength; a++)
            {
                list[a] = new ClassToBeInjected();
            }
            pool.Add(list);
            Assert.AreEqual(pool.TotalLength, pool.Available);
        }

        [Test]
        public void TestGetInstance()
        {
            pool.SetSize(4);
            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            for (int a = pool.TotalLength; a > 0; a--)
            {
                Assert.AreEqual(a, pool.Available);
                var instance = pool.GetInstance();
                Assert.IsNotNull(instance);
                Assert.IsInstanceOf<ClassToBeInjected>(instance);
                Assert.AreEqual(a - 1, pool.Available);
            }
        }

        [Test]
        public void TestReturnInstance()
        {
            pool.SetSize(4);
            var stack = new Stack(pool.TotalLength);
            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            for (int a = 0; a < pool.TotalLength; a++)
            {
                stack.Push(pool.GetInstance());
            }

            Assert.AreEqual(pool.TotalLength, stack.Count);
            Assert.AreEqual(0, pool.Available);

            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.ReturnInstance(stack.Pop());
            }

            Assert.AreEqual(0, stack.Count);
            Assert.AreEqual(pool.TotalLength, pool.Available);
        }

        [Test]
        public void TestDispose()
        {
            pool.SetSize(4);
            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.Add(new ClassToBeInjected());
            }
            pool.Dispose();
            Assert.AreEqual(0, pool.Available);
        }

        [Test]
        public void TestPoolOverflowException()
        {
            pool.SetSize(4);
            pool.InstanceProvider = new TestInstanceProvider();
            for (int a = 0; a < 4; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            for (int a = pool.TotalLength; a > 0; a--)
            {
                Assert.AreEqual(a, pool.Available);
                pool.GetInstance();
            }

            TestDelegate testDelegate = delegate
            {
                pool.GetInstance();
            };
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(PoolExceptionType.OVERFLOW, ex.Type);
        }

        [Test]
        public void TestOverflowWithoutException()
        {
            pool.SetSize(4);
            pool.OverflowBehavior = PoolOverflowBehavior.IGNORE;
            pool.InstanceProvider = new TestInstanceProvider();
            for (int a = 0; a < 4; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            for (int a = pool.TotalLength; a > 0; a--)
            {
                Assert.AreEqual(a, pool.Available);
                pool.GetInstance();
            }

            TestDelegate testDelegate = delegate
            {
                object shouldBeNull = pool.GetInstance();
                Assert.IsNull(shouldBeNull);
            };
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void TestPoolTypeMismatchException()
        {
            pool.SetSize(4);
            pool.Add(new ClassToBeInjected());

            TestDelegate testDelegate = delegate
            {
                pool.Add(new InjectableDerivedClass());
            };
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(PoolExceptionType.TYPE_MISMATCH, ex.Type);
        }

        [Test]
        public void TestRemoveFromPool()
        {
            pool.SetSize(4);
            for (int a = 0; a < pool.TotalLength; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            for (int a = pool.TotalLength; a > 0; a--)
            {
                Assert.AreEqual(a, pool.Available);
                var instance = pool.GetInstance();
                pool.Remove(instance);
            }

            Assert.AreEqual(0, pool.Available);
        }

        [Test]
        public void TestRemoveList1()
        {
            pool.SetSize(4);
            for (int a = 0; a < 4; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            var removalList = new ClassToBeInjected[3];
            for (int a = 0; a < pool.TotalLength - 1; a++)
            {
                removalList[a] = new ClassToBeInjected();
            }
            pool.Remove(removalList);
            Assert.AreEqual(1, pool.Available);
        }

        [Test]
        public void TestRemoveList2()
        {
            pool.SetSize(4);
            for (int a = 0; a < 4; a++)
            {
                pool.Add(new ClassToBeInjected());
            }

            var removalList = new ClassToBeInjected[3];
            for (int a = 0; a < pool.TotalLength - 1; a++)
            {
                removalList[a] = pool.GetInstance();
            }
            pool.Remove(removalList);
            Assert.AreEqual(1, pool.Available);
        }

        [Test]
        public void TestRemovalException()
        {
            pool.SetSize(4);
            pool.Add(new ClassToBeInjected());
            TestDelegate testDelegate = delegate
            {
                pool.Remove(new InjectableDerivedClass());
            };
            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(PoolExceptionType.TYPE_MISMATCH, ex.Type);
        }

        [Test]
        public void TestReleaseOfPoolable()
        {
            var anotherPool = new Pool<PooledInstance>();

            anotherPool.SetSize(4);
            anotherPool.Add(new PooledInstance());
            var instance = anotherPool.GetInstance();
            instance.someValue = 42;
            Assert.AreEqual(42, instance.someValue);
            anotherPool.ReturnInstance(instance);
            Assert.AreEqual(0, instance.someValue);
        }

        //Double is default
        [Test]
        public void TestAutoInflationDouble()
        {
            pool.InstanceProvider = new TestInstanceProvider();

            var instance1 = pool.GetInstance();
            Assert.IsNotNull(instance1);
            Assert.AreEqual(1, pool.TotalLength); //First call creates one instance
            Assert.AreEqual(0, pool.Available); //Nothing Available

            var instance2 = pool.GetInstance();
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2);
            Assert.AreEqual(2, pool.TotalLength); //Second call doubles. We have 2
            Assert.AreEqual(0, pool.Available); //Nothing Available

            var instance3 = pool.GetInstance();
            Assert.IsNotNull(instance3);
            Assert.AreEqual(3, pool.TotalLength); //Third call doubles. We have 4
            Assert.AreEqual(0, pool.Available); //One allocated. One Available.

            var instance4 = pool.GetInstance();
            Assert.IsNotNull(instance4);
            Assert.AreEqual(4, pool.TotalLength); //Fourth call. No doubling since one was Available.
            Assert.AreEqual(0, pool.Available);

            var instance5 = pool.GetInstance();
            Assert.IsNotNull(instance5);
            Assert.AreEqual(5, pool.TotalLength); //Fifth call. Double to 8.
            Assert.AreEqual(0, pool.Available); //Three left unallocated.
        }

        [Test]
        public void TestAutoInflationIncrement()
        {
            pool.InstanceProvider = new TestInstanceProvider();
            pool.InflationType = PoolInflationType.INCREMENT;

            int testCount = 10;

            var stack = new Stack();

            //Calls should simply increment. There will never be unallocated
            for (int a = 0; a < testCount; a++)
            {
                var instance = pool.GetInstance();
                Assert.IsNotNull(instance);
                Assert.AreEqual(a + 1, pool.TotalLength);
                Assert.AreEqual(0, pool.Available);
                stack.Push(instance);
            }

            //Now return the instances
            for (int a = 0; a < testCount; a++)
            {
                var instance = stack.Pop() as ClassToBeInjected;
                pool.ReturnInstance(instance);

                Assert.AreEqual(a + 1, pool.Available, "This one");
                Assert.AreEqual(testCount, pool.TotalLength, "Or this one");
            }
        }

        [Test]
        public void TestPoolClear()
        {
            var anotherPool = new Pool<PooledInstance>();

            anotherPool.SetSize(4);
            anotherPool.Add(new PooledInstance());
            var instance = anotherPool.GetInstance();
            instance.someValue = 42;
            Assert.AreEqual(42, instance.someValue);
            anotherPool.Dispose();
            Assert.AreEqual(0, instance.someValue);
        }
    }

    internal class PooledInstance : IPoolable
    {
        public int someValue;

        public bool IsRetain
        {
            get;
        }

        public void Restore()
        {
            someValue = 0;
        }

        public void Retain()
        {
        }

        public void Release()
        {
        }
    }

    internal class TestInstanceProvider : IInstanceProvider
    {
        public T GetInstance<T>()
        {
            object instance = Activator.CreateInstance(typeof(T));
            var retv = (T)instance;
            return retv;
        }

        public object GetInstance(Type key)
        {
            return Activator.CreateInstance(key);
        }
    }
}
