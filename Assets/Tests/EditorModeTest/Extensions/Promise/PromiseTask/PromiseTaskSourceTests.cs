using System;
using System.Threading.Tasks;
using Cr7Sund.Package.Impl;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseTaskSourceTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }
        [TearDown]
        public void TearDown()
        {
            Assert.LessOrEqual(0, PromiseTaskSource<int>.Test_GetPoolCount());
        }

        [Test]
        public async Task ResolvePromise()
        {
            // recommend use
            //return await 2.ToPromiseTask();

            async PromiseTask<int> ResolveInternal()
            {
                var testPromise = PromiseTaskSource<int>.Create();
                testPromise.TryResolve(2);
                return await testPromise.Task;
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
                var promise = PromiseTaskSource<int>.Create();
                promise.TryReject(new System.NotImplementedException(Message));
                return await promise.Task;
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

#if DEBUG

        [Test]
        public async Task PoolTaskPromise()
        {
            //setup
            var firstPromise = PromiseTaskSource<int>.Create();
            firstPromise.TryResolve(2);
            await firstPromise.Task;

            int promiseSize = PromiseTaskSource<int>.Test_GetPoolCount();

            var promise = PromiseTaskSource<int>.Create();
            Assert.AreEqual(promiseSize - 1, PromiseTaskSource<int>.Test_GetPoolCount());
            promise.TryResolve(2);
            await promise.Task;
            Assert.AreEqual(promiseSize, PromiseTaskSource<int>.Test_GetPoolCount());
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

            var promise = PromiseTaskSource<int>.Create();
            result *= 10;
            promise.TryResolve(result);
            return await promise.Task;
        }

        private async PromiseTask<int> resolveAfter1Second(int result)
        {
            await Task.Delay(1);

            var promise = PromiseTaskSource<int>.Create();
            result++;
            promise.TryResolve(result);

            return await promise.Task;
        }

    }
}
