using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    public class TestEventDispatcher
    {
        IEventDispatcher dispatcher;
        private const int INIT_VALUE = 42;
        private const int INCREMENT = 4;
        private const int PAYLOAD = 8;
        private int confirmationValue = 42;

        [SetUp]
        public void SetUp()
        {
            dispatcher = new EventDispatcher();
        }

        [Test]
        public void TestAddListenerNoArgs()
        {
            dispatcher.AddListener(SomeEnum.ONE, noArgumentsMethod);
            Assert.IsTrue(dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
        }

        [Test]
        public void TestAddListenerOneArg()
        {
            dispatcher.AddListener(SomeEnum.ONE, oneArgumentMethod);
            Assert.IsTrue(dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
        }

        [Test]
        public void TestRemoveListenerNoArgs()
        {
            dispatcher.AddListener(SomeEnum.ONE, noArgumentsMethod);
            dispatcher.RemoveListener(SomeEnum.ONE, noArgumentsMethod);
            Assert.IsFalse(dispatcher.HasListener(SomeEnum.ONE, noArgumentsMethod));
        }

        [Test]
        public void TestRemoveListenerOneArg()
        {
            dispatcher.AddListener(SomeEnum.ONE, oneArgumentMethod);
            dispatcher.RemoveListener(SomeEnum.ONE, oneArgumentMethod);
            Assert.IsFalse(dispatcher.HasListener(SomeEnum.ONE, oneArgumentMethod));
        }


        [Test]
        public void TestDispatchNoArgs()
        {
            confirmationValue = INIT_VALUE;
            dispatcher.AddListener(SomeEnum.ONE, noArgumentsMethod);
            dispatcher.Dispatch(SomeEnum.ONE);
            Assert.AreEqual(INIT_VALUE + INCREMENT, confirmationValue);
        }

        [Test]
        public void TestDispatchOneArg()
        {
            confirmationValue = INIT_VALUE;
            dispatcher.AddListener(SomeEnum.ONE, oneArgumentMethod);
            dispatcher.Dispatch(SomeEnum.ONE, PAYLOAD);
            Assert.AreEqual(INIT_VALUE + PAYLOAD, confirmationValue);
        }

        [Test]
        public void TestMultipleListeners()
        {
            confirmationValue = INIT_VALUE;
            dispatcher.AddListener(SomeEnum.ONE, noArgumentsMethod);
            dispatcher.AddListener(SomeEnum.ONE, oneArgumentMethod);
            dispatcher.Dispatch(SomeEnum.ONE, PAYLOAD);

            Assert.AreEqual(INIT_VALUE + PAYLOAD + INCREMENT, confirmationValue);
        }

        [Test]
        public void TestTriggerClientRemoval()
        {
            Assert.AreEqual(0, (dispatcher as ITriggerProvider).TriggerableCount);

            EventDispatcher anotherDispatcher1 = new EventDispatcher();
            (dispatcher as ITriggerProvider).AddTriggerable(anotherDispatcher1);

            Assert.AreEqual(1, (dispatcher as ITriggerProvider).TriggerableCount);

            EventDispatcher anotherDispatcher2 = new EventDispatcher();
            (dispatcher as ITriggerProvider).AddTriggerable(anotherDispatcher2);

            Assert.AreEqual(2, (dispatcher as ITriggerProvider).TriggerableCount);

            dispatcher.AddListener(SomeEnum.ONE, removeTriggerClientMethod);
            dispatcher.Dispatch(SomeEnum.ONE, anotherDispatcher1);

            Assert.AreEqual(1, (dispatcher as ITriggerProvider).TriggerableCount);

            dispatcher.AddListener(SomeEnum.ONE, removeTriggerClientMethod);
            dispatcher.Dispatch(SomeEnum.ONE, anotherDispatcher2);

            Assert.AreEqual(0, (dispatcher as ITriggerProvider).TriggerableCount);
        }

        private void removeTriggerClientMethod(IEvent evt)
        {
            EventDispatcher target = evt.Data as EventDispatcher;
            (dispatcher as ITriggerProvider).RemoveTriggerable(target);
        }

        [Test]
        public void TestBadlyFormedCallback()
        {
            confirmationValue = INIT_VALUE;
            // badArgumentMethod 's parameter should inherit from IEvent
            dispatcher.AddListener(SomeEnum.ONE, badArgumentMethod);

            TestDelegate testDelegate = delegate ()
            {
                dispatcher.Dispatch(SomeEnum.ONE, PAYLOAD);
            };

            EventDispatcherException ex = Assert.Throws<EventDispatcherException>(testDelegate);
            Assert.That(ex.Type == EventDispatcherExceptionType.TARGET_INVOCATION);
        }

        [Test]
        public void TestMidpointRemoval()
        {
            confirmationValue = INIT_VALUE;
            // yeah , you can achieve it .
            // But I dont recommend 
            dispatcher.AddListener(SomeEnum.ONE, interruptMethod);
            dispatcher.AddListener(SomeEnum.ONE, noArgumentsMethod);

            dispatcher.Dispatch(SomeEnum.ONE);

            Assert.AreEqual(INIT_VALUE, confirmationValue);
        }

        private void noArgumentsMethod()
        {
            confirmationValue += INCREMENT;
        }

        private void oneArgumentMethod(IEvent evt)
        {
            int data = (int)evt.Data;

            confirmationValue += data;
        }

        private void badArgumentMethod(object payload)
        {
            int data = (int)payload;

            confirmationValue += data;
        }

        private void interruptMethod()
        {
            dispatcher.RemoveListener(SomeEnum.ONE, noArgumentsMethod);
        }
    }
}

