using System;
using System.Threading.Tasks;
using Cr7Sund.Package.Impl;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseTaskTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }

        [Test]
        public async Task ResolvePromise()
        {
            // recommend use
            //return await 2.ToPromiseTask();

            async PromiseTask<int> ResolveInternal()
            {
                var testPromise = Promise<int>.Create();
                testPromise.Resolve(2);
                return await testPromise.AsTask();
            }

            var result = await ResolveInternal();
            Assert.AreEqual(2, result);

        }

        [Test]
        public async Task RejectPromise()
        {
            // AssertHelper.IgnoreFailingMessages();
            // recommend use
            //return await new Exception().ToPromiseTask<int>();
            const string Message = "HelloWorld";

            async PromiseTask<int> ResolveInternal()
            {
                var promise = Promise<int>.Create();
                promise.RejectWithoutDebug(new System.NotImplementedException(Message));
                return await promise.AsTask();
            }

            string resultEx = null;
            try
            {
                await ResolveInternal();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    resultEx = ex.InnerException.Message;
                }
                else
                {
                    resultEx = ex.Message;
                }
            }

            Assert.AreEqual(Message, resultEx);
        }

        [Test]
        public async Task ResolveValue()
        {
            async PromiseTask<int> ResolveInternal()
            {
                return await PromiseTask<int>.FromResult(2);
            }

            var result = await ResolveInternal();
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task RejectedValue()
        {
            const string Message = "HelloWorld";

            async PromiseTask<int> ResolveInternal()
            {
                return await PromiseTask<int>.FromException(new Exception(Message));
            }

            string resultEx = null;
            try
            {
                await ResolveInternal();
            }
            catch (Exception ex)
            {
                resultEx = ex.Message;
            }

            Assert.AreEqual(Message, resultEx);
        }


        [Test]
        public async Task ChainPromise()
        {
            var promise = Promise<int>.Create();
            var completed = 0;
            var result = 0;

            promise.Then<int>((v) => result += 1);
            await promise.ResolveAsync(result);
            completed++;

            Assert.AreEqual(1, completed);
            Assert.AreEqual(1, result);
        }

#if DEBUG
        [Test]
        public void PoolAction()
        {
            // setup
            var firstPromise = new Promise<int>();
            firstPromise.Then(_ => { });
            firstPromise.Resolve(2);

            int size = Promise<int>.Test_GetResolveListPoolCount();
            var promise = new Promise<int>();
            promise.Then(_ => { });
            Assert.AreEqual(size - 1, Promise<int>.Test_GetResolveListPoolCount());

            promise.Resolve(2);
            Assert.AreEqual(size, Promise<int>.Test_GetResolveListPoolCount());
        }

        [Test]
        public async Task PoolTaskPromise()
        {
            //setup
            var firstPromise = Promise<int>.Create();
            firstPromise.Then(_ => { });
            await firstPromise.ResolveAsync(2);

            int promiseSize = Promise<int>.Test_GetPoolCount();
            int actionSize = Promise<int>.Test_GetResolveListPoolCount();

            var promise = Promise<int>.Create();
            promise.Then(_ => { });
            Assert.AreEqual(promiseSize - 1, Promise<int>.Test_GetPoolCount());
            Assert.AreEqual(actionSize - 1, Promise<int>.Test_GetResolveListPoolCount());
            await promise.ResolveAsync(2);
            Assert.AreEqual(promiseSize, Promise<int>.Test_GetPoolCount());
            Assert.AreEqual(actionSize, Promise<int>.Test_GetResolveListPoolCount());
        }

#endif
        [Test]
        public async Task sequentialStart()
        {
            int result = 2;

            result = await resolveAfter2Seconds(result);
            result = await resolveAfter1Second(result);

            Assert.AreEqual(result, 21);
        }

        [Test]
        public async Task sequentialWait()
        {
            int result = 2;

            var slow = resolveAfter2Seconds(result);
            var fast = resolveAfter1Second(result);

            var result1 = await slow;
            var result2 = await fast;

            Assert.AreEqual(result2, 3);
            Assert.AreEqual(result1, 20);
        }

        [Test]
        public async Task concurrent1()
        {
            var result = 2;
            var results = await PromiseTask<int>.WhenAll(
                resolveAfter2Seconds(result),
                resolveAfter1Second(result)
              );

            // 2. Log the results together
            Assert.AreEqual(20, results[0]);
            Assert.AreEqual(3, results[1]);
        }
        private async PromiseTask<int> resolveAfter2Seconds(int result)
        {
            await Task.Delay(2);

            var promise = Promise<int>.Create();
            result *= 10;
            promise.Resolve(result);
            return await promise.AsTask();
        }

        private async PromiseTask<int> resolveAfter1Second(int result)
        {
            await Task.Delay(1);

            var promise = Promise<int>.Create();
            result++;
            promise.Resolve(result);

            return await promise.AsTask();
        }

    }
}
