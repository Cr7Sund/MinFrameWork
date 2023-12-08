using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using NUnit.Framework;
namespace Cr7Sund.Framework.Tests
{
    public class TestEventDispatcher
    {
        private const int INIT_VALUE = 42;
        private const int INCREMENT = 4;
        private const int PAYLOAD = 8;
        private int confirmationValue = 42;
        private IEventDispatcher dispatcher;

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
            Assert.AreEqual(0, dispatcher.TriggerableCount);

            var anotherDispatcher1 = new EventDispatcher();
            dispatcher.AddTriggerable(anotherDispatcher1);

            Assert.AreEqual(1, dispatcher.TriggerableCount);

            var anotherDispatcher2 = new EventDispatcher();
            dispatcher.AddTriggerable(anotherDispatcher2);

            Assert.AreEqual(2, dispatcher.TriggerableCount);

            dispatcher.AddListener(SomeEnum.ONE, removeTriggerClientMethod);
            dispatcher.Dispatch(SomeEnum.ONE, anotherDispatcher1);

            Assert.AreEqual(1, dispatcher.TriggerableCount);

            dispatcher.AddListener(SomeEnum.ONE, removeTriggerClientMethod);
            dispatcher.Dispatch(SomeEnum.ONE, anotherDispatcher2);

            Assert.AreEqual(0, dispatcher.TriggerableCount);
        }

        private void removeTriggerClientMethod(IEvent evt)
        {
            var target = evt.Data as EventDispatcher;
            dispatcher.RemoveTriggerable(target);
        }

        [Test]
        public void TestBadlyFormedCallback()
        {
            confirmationValue = INIT_VALUE;
            // badArgumentMethod 's parameter should inherit from IEvent
            dispatcher.AddListener(SomeEnum.ONE, badArgumentMethod);

            TestDelegate testDelegate = delegate
            {
                dispatcher.Dispatch(SomeEnum.ONE, PAYLOAD);
            };

            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual( EventDispatcherExceptionType.TARGET_INVOCATION, ex.Type );
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
