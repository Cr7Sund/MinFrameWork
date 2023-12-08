using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Tests;
using NUnit.Framework;
using System;
namespace Cr7Sund.Framework.PromiseTest
{
    public class PromiseChainingTest
    {
        private static IPromise<float> asyncFloatPromise;
        private ExampleCommand command;
        private float floatResult;
        private int intResult;
        private IPromise<int> promise;

        [SetUp]
        public void SetUp()
        {
            asyncFloatPromise = new Promise<float>();

            promise = new Promise<int>();
            intResult = 0;
            floatResult = 0f;
            command = new ExampleCommand();

        }

        [Test]
        public void chaining_value_simple()
        {
            promise
                .Then(v => v + 1)
                .Then(v =>
                {
                    intResult = v + 2;
                });

            promise.Resolve(1);
            Assert.AreEqual(4, intResult);
        }


        [Test]
        public void chaining_value_multiple()
        {
            promise
                .Then(v => v + 1)
                .Then(v => v + 2)
                .Then(v =>
                {
                    intResult = v + 3;
                });

            promise.Resolve(0);
            Assert.AreEqual(6, intResult);
        }


        [Test]
        public void chaining_value_async()
        {
            promise
                .Then(v =>
                {
                    return v + 1;
                })
                .Then(command.AsyncFunc)
                .Then(v =>
                {
                    floatResult = v + 3f;
                });

            promise.Resolve(0);
            asyncFloatPromise.Resolve(2);
            Assert.AreEqual(7, floatResult);
        }

        [Test]
        public void chaining_value_exception()
        {
            Exception e = null;
            Func<int, int> exceptionHandler = v => throw new Exception("west");
            promise
                .Then(v => v + 1)
                .Then(exceptionHandler)
                .Then(v =>
                {
                    intResult = v + 3;
                })
                .Catch(ex => e = ex)
                ;

            promise.Resolve(0);
            Assert.AreEqual(0, intResult);
            Assert.AreEqual(e.Message, "west");
        }


        [Test]
        public void chaining_value_convert_simple()
        {
            promise
                .Then(v => v + 1)
                .Then(command.ConvertFunc)
                .Then(v =>
                {
                    floatResult = v + 2f;
                });

            promise.Resolve(1);
            Assert.AreEqual(6f, floatResult);
        }

        [Test]
        public void chaining_value_convert_async()
        {
            promise
                .Then(command.Func)
                .Then(command.ConvertFunc)
                .Then(command.AsyncFunc)
                .Then(v =>
                {
                    floatResult = v + 2f;
                    return floatResult;
                });

            promise.Resolve(1);
            asyncFloatPromise.Resolve(2);
            Assert.AreEqual(9f, floatResult);
        }


        [Test]
        public void handle_rejected_catch_but_break_chain()
        {
            var promise = new Promise<int>();
            SimplePromise.simulatePromiseOne = new Promise<int>();

            var exampleCommand = new SimpleAsyncException_Generic();
            var exampleCommand1 = new SimpleCommandTwoGeneric();
            var finalPromise = promise.Then(exampleCommand.OnExecuteAsync, exampleCommand.OnCatchAsync)
                .Then(exampleCommand1.OnExecute)
                .Catch(ex => SimplePromise.result = -2);

            promise.Resolve(0);
            // SimplePromise.simulatePromiseOne.Resolve(7);
            Assert.AreEqual(-2, SimplePromise.result);
        }
        private class ExampleCommand
        {
            public int Func(int t)
            {
                return t + 1;
            }

            public float ConvertFunc(int t)
            {
                return t + 2f;
            }


            public int ConvertFunc(float t)
            {
                return (int)t + 2;
            }

            public IPromise<float> AsyncFunc(float t)
            {
                return asyncFloatPromise.Then(v =>
                {
                    return v + 1 + t;
                });
            }

            public IPromise<float> AsyncFunc(int t)
            {
                return asyncFloatPromise.Then(v =>
                {
                    return v + 1 + t;
                });
            }
        }
    }
}
