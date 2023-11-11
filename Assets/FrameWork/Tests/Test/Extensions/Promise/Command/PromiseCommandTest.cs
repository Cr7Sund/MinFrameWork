using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;
using UnityEditor.VersionControl;

namespace Cr7Sund.Framework.PromiseCommandTest
{

    public class PromiseCommandTests
    {
        [SetUp]
        public void SetUp()
        {
            SimplePromise.simulatePromiseT = new Promise<int>();
        }

        [TearDown]
        public void Cleanup()
        {
            SimplePromise.simulatePromiseT = null;
            SimplePromise.result = 0;
            SimplePromise.simulatePromise = null;
            SimplePromise.exceptionStr = null;
        }


        [Test]
        public void command_all_resolve()
        {
            // equal to
            // var promise1 = new Promise();
            // promise1.Then(new PromiseCommand_1().Resolve).Then(new PromiseCommand_1().Resolve);

            var promise = new SimplePromiseCommandOne<int>();
            var donePromise = promise.Then<SimplePromiseCommandTwo<int>>().Then<SimplePromiseCommandOne<int>>().Then((value) => value - 200);
            promise.Resolve(0);
            Assert.AreEqual((((0 + 2) * 3) + 1) * 2 - 200, ((Promise<int>)donePromise).Test_GetResolveValue());

        }

        [Test]
        public void command_with_async_operation()
        {
            var promise = new SimplePromiseCommandOne<int>();
            var donePromise1 = promise.Then<SimplePromiseCommandTwo<int>>();
            var donePromise2 = donePromise1.Then<SimpleAsyncPromiseCommandOne<int>>().Then<SimplePromiseCommandOne<int>>();

            promise.Resolve(0);
            Assert.AreEqual(6, ((Promise<int>)donePromise1).Test_GetResolveValue());

            SimplePromise.simulatePromiseT.Resolve(0);
            Assert.AreEqual(((((0 + 2) * 3) + 3) * 5 + 1) * 2, ((Promise<int>)donePromise2).Test_GetResolveValue());
        }


        [Test]
        public void command_convert()
        {
            var promise = new SimplePromiseCommandOne<int>();
            var donePromise = promise.Then<SimplePromiseCommandTwo<int>>().Then<ConvertPromiseCommand<int, float>,float>();
            promise.Resolve(0);

            var promise1 = new Promise<int>();
            var promise2 = new Promise<float>();
            promise1.Then((t) => promise2);
            Assert.AreEqual(((0 + 2) * 3 + 3) * 4.2f, ((PromiseCommand<int, float>)donePromise).Test_GetResolveValue());
        }
    }

}