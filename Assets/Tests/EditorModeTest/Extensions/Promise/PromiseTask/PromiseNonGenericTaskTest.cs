using System;
using System.Threading.Tasks;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseTaskNoGenericTests
    {
        private int result = 0;

        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }

        [Test]
        public async Task ResolvePromise()
        {
            result = 0;
            async PromiseTask ResolveInternal()
            {
                var testPromise = Promise.Create();
                testPromise.Then(() => result++);
                testPromise.Resolve();
                await testPromise.AsTask();
            }

            await ResolveInternal();

            Assert.AreEqual(1, result);
        }


        [Test]
        public async Task RejectPromise()
        {
            // recommend use
            // await new Exception().ToPromiseTask();
            const string Message = "HelloWorld";

            async PromiseTask ResolveInternal()
            {
                var promise = Promise.Create();
                promise.RejectWithoutDebug(new System.NotImplementedException(Message));
                await promise.AsTask();
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
        public async Task RejectedValue()
        {
            const string Message = "HelloWorld";

            async PromiseTask ResolveInternal()
            {
                await PromiseTask.FromException(new Exception(Message));
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
        public async Task ResolvePromiseRepeatedly()
        {
            var promise = Promise.Create();
            string resultEx = null;

            async PromiseTask ResolveInternal()
            {
                await promise.ResolveAsync();
            }

            async PromiseTask RepeatResolveInternal()
            {
                await promise.ResolveAsync();
            }

            await ResolveInternal();
            try
            {
                await RepeatResolveInternal();
            }
            catch (Exception ex)
            {
                resultEx = ex.Message;
            }

            Assert.AreEqual("can resolve twice, since it has been recycled", resultEx);
        }

        [Test]
        public async Task AwaitTaskTwice()
        {
            var promise = Promise.Create();
            string resultEx = null;

            async PromiseTask ResolveInternal()
            {
                await promise.AsTask();
            }

            await promise.ResolveAsync();
            try
            {
                await ResolveInternal();
            }
            catch (Exception ex)
            {
                resultEx = ex.Message;
            }

            Assert.AreEqual("cant await twice since it has been recycled", resultEx);
        }


#if DEBUG
        [Test]
        public void PoolAction()
        {
            // setup
            var firstPromise = new Promise();
            firstPromise.Then(() => { });
            firstPromise.Resolve();

            int size = Promise.Test_GetResolveListPoolCount();
            var promise = new Promise();
            promise.Then(() => { });
            Assert.AreEqual(size - 1, Promise.Test_GetResolveListPoolCount());

            promise.Resolve();
            Assert.AreEqual(size, Promise.Test_GetResolveListPoolCount());
        }

        [Test]
        public async Task PoolTaskPromise()
        {
            //setup
            var firstPromise = Promise.Create();
            firstPromise.Then(() => { });
            await firstPromise.ResolveAsync();

            int promiseSize = Promise.Test_GetPoolCount();
            int actionSize = Promise.Test_GetResolveListPoolCount();

            var promise = Promise.Create();
            promise.Then(() => { });
            Assert.AreEqual(promiseSize - 1, Promise.Test_GetPoolCount());
            Assert.AreEqual(actionSize - 1, Promise.Test_GetResolveListPoolCount());
            await promise.ResolveAsync();
            Assert.AreEqual(promiseSize, Promise.Test_GetPoolCount());
            Assert.AreEqual(actionSize, Promise.Test_GetResolveListPoolCount());
        }

#endif

        [Test]
        public async Task sequentialStart()
        {
            result = 2;

            await resolveAfter2Seconds();
            Assert.AreEqual(result, 20);
            await resolveAfter1Second();

            Assert.AreEqual(result, 21);
        }


        [Test]
        public async Task sequentialWait()
        {
            await Task.Delay(1).ContinueWith(
                async _ =>
                {
                    result = 2;

                    var slow = resolveAfter2Seconds();
                    var fast = resolveAfter1Second();

                    await slow;
                    Assert.AreEqual(30, result);
                    await fast;
                    Assert.AreEqual(result, 30);
                }
            );
        }

        [Test]
        public async Task concurrent1()
        {
            await Task.Delay(1).ContinueWith(
                async _ =>
                {
                    result = 2;
                    await PromiseTask.WhenAll(
                       resolveAfter2Seconds(),
                       resolveAfter1Second()
                     );

                    Assert.AreEqual(30, result);
                });
        }

        private async PromiseTask resolveAfter2Seconds()
        {
            await Task.Delay(2);

            var promise = Promise.Create();
            result *= 10;
            promise.Resolve();

            await promise.AsTask();
        }

        private async PromiseTask resolveAfter1Second()
        {
            await Task.Delay(1);

            var promise = Promise.Create();
            result++;

            promise.Resolve();

            await promise.AsTask();
        }

    }
}
