using System;
using System.Threading.Tasks;
using Cr7Sund.CompilerServices;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseNonGenericTaskSourceTests
    {
        private int result = 0;

        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }
        [TearDown]
        public void TearDown()
        {
            Assert.LessOrEqual(0, PromiseTaskSource.Test_GetPoolCount());
        }


        [Test]
        public async Task ResolvePromise()
        {
            result = 0;
            async PromiseTask ResolveInternal()
            {
                var testPromise = PromiseTaskSource.Create();
                testPromise.TryResolve();
                await testPromise.Task;
                result++;
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
                var promise = PromiseTaskSource.Create();
                promise.TryReject(new System.NotImplementedException(Message));
                await promise.Task;
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
            var promise = PromiseTaskSource.Create();
            MyException resultEx = null;

            async PromiseTask ResolveInternal()
            {
                promise.TryResolve();
                await promise.Task;
            }

            async PromiseTask RepeatResolveInternal()
            {
                promise.TryResolve();
                await promise.Task;
            }

            await ResolveInternal();
            try
            {
                await RepeatResolveInternal();
            }
            catch (Exception ex) when (ex is MyException myException)
            {
                resultEx = myException;
            }

            Assert.AreEqual(PromiseTaskExceptionType.CAN_VISIT_VALID_VERSION, resultEx.Type);
        }

        [Test]
        public async Task AwaitTaskTwice()
        {
            var promise = PromiseTaskSource.Create();
            MyException resultEx = null;

            async PromiseTask ResolveInternal()
            {
                await promise.Task;
            }

            promise.TryResolve();
            await promise.Task;
            try
            {
                await ResolveInternal();
            }
            catch (Exception ex) when (ex is MyException myException)
            {
                resultEx = myException;
            }

            //Expected 0  but it's 1
            Assert.AreEqual(PromiseTaskExceptionType.CAN_VISIT_VALID_VERSION, resultEx.Type);
        }

#if DEBUG
        [Test]
        public async Task PoolTaskPromise()
        {
            var firstPromise = PromiseTaskSource.Create();
            firstPromise.TryResolve();
            await firstPromise.Task;

            int promiseSize = PromiseTaskSource.Test_GetPoolCount();
            var promise = PromiseTaskSource.Create();
            Assert.AreEqual(promiseSize - 1, PromiseTaskSource.Test_GetPoolCount());
            promise.TryResolve();
            await promise.Task;
            Assert.AreEqual(promiseSize, PromiseTaskSource.Test_GetPoolCount());
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

            var promise = PromiseTaskSource.Create();
            result *= 10;
            promise.TryResolve();

            await promise.Task;
        }

        private async PromiseTask resolveAfter1Second()
        {
            await Task.Delay(1);

            var promise = PromiseTaskSource.Create();
            result++;

            promise.TryResolve();

            await promise.Task;
        }

    }
}
