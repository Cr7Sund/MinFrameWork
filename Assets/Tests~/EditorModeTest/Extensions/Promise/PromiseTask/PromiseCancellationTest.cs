using System;
using System.Threading.Tasks;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Impl;
using NUnit.Framework;

namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseCancellationTest
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }

        [Test]
        public void CancelPromise()
        {
            var cancellation = UnsafeCancellationTokenSource.Create();
            var promise = new Promise();
            cancellation.Token.Register(promise.Cancel);

            AsyncTestDelegate testDelegate = async () =>
            {
                cancellation.Cancel();
                await promise.Task;
            };

            Assert.ThrowsAsync<TaskCanceledException>(testDelegate);
        }

        [Test]
        public void ReCancel_Fail()
        {
            var cancellation = UnsafeCancellationTokenSource.Create();
            var promise = new Promise();
            cancellation.Token.Register(promise.Cancel);
            cancellation.Cancel();

            TestDelegate testDelegate = cancellation.Cancel;

            var ex = Assert.Throws<System.Exception>(testDelegate);
            Assert.AreEqual("token is already canceled or cancelling", ex.Message);
        }

        [Test]
        public void ReCancel_DisposeFirst()
        {
            var cancellation = UnsafeCancellationTokenSource.Create();
            var promise = new Promise();
            cancellation.Token.Register(promise.Cancel);
            cancellation.Cancel();
            cancellation.Dispose();
            TestDelegate testDelegate = cancellation.Cancel;

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void ReuseCancellation()
        {
            var cancellation = UnsafeCancellationTokenSource.Create();
            var promise = new Promise();
            cancellation.Token.Register(promise.Cancel);

            cancellation.Dispose();
            cancellation.Token.Register(promise.Cancel);
            TestDelegate testDelegate = cancellation.Cancel;

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void JoinTaskCancellation()
        {
            var cancellation = UnsafeCancellationTokenSource.Create();
            var source = cancellation.Join();
            Task task1 = Task.Delay(2, source.Token);

            AsyncTestDelegate testDelegate = async () =>
                {
                    cancellation.Cancel();
                    await task1;
                };

            Assert.ThrowsAsync<TaskCanceledException>(testDelegate);
        }

        [Test]
        public void UnitTokenTest()
        {
            TestDelegate testDelegate = () => UnsafeCancellationToken.None.Register(() => { });

            var ex = Assert.Throws<Exception>(testDelegate);
            Assert.AreEqual("try to use default token", ex.Message);
            AssertUtil.IsFalse(UnsafeCancellationToken.None.IsCancellationRequested);
        }
    }
}