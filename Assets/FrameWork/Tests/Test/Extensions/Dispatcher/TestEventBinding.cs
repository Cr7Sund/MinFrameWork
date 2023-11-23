using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;
using System;
namespace Cr7Sund.Framework.Tests
{
    [TestFixture]
    public class TestEventBinding
    {
        private const int INIT_VALUE = 42;
        private int confirmationValue = 42;


        [Test]
        public void TestMapNoArgumentCallback()
        {
            confirmationValue = INIT_VALUE;
            IEventBinding binding = new EventBinding();
            binding.Bind(SomeEnum.ONE).To(noArgumentCallback);
            var type = binding.TypeForCallback(noArgumentCallback);
            object[] value = binding.Value as object[];
            var extracted = value[0] as Delegate;

            Assert.AreEqual(EventCallbackType.NO_ARGUMENTS, type);

            extracted.DynamicInvoke();
            //Calling the method should change the confirmationValue
            Assert.AreNotEqual(confirmationValue, INIT_VALUE);
        }

        private void noArgumentCallback()
        {
            confirmationValue *= 2;
        }

        [Test]
        public void TestMapOneArgumentCallback()
        {
            confirmationValue = INIT_VALUE;
            IEventBinding binding = new EventBinding();
            binding.Bind(SomeEnum.ONE).To(oneArgumentCallback);
            var type = binding.TypeForCallback(oneArgumentCallback);
            object[] value = binding.Value as object[];
            var extracted = value[0] as Delegate;

            Assert.AreEqual(EventCallbackType.ONE_ARGUMENT, type);

            object[] parameters = new object[1];
            parameters[0] = new TestEvent("TEST", null, INIT_VALUE);
            extracted.DynamicInvoke(parameters);
            //Calling the method should change the confirmationValue
            Assert.AreEqual(confirmationValue, INIT_VALUE * INIT_VALUE);
        }

        private void oneArgumentCallback(IEvent o)
        {
            confirmationValue *= (int)o.Data;
        }


        private class TestEvent : IEvent
        {


            public TestEvent(object type, IEventDispatcher target, object data)
            {
                Type = type;
                Target = target;
                Data = data;
            }

            public object type { get { return Type; } set { Type = value; } }
            public IEventDispatcher target { get { return Target; } set { Target = value; } }
            public object data { get { return Data; } set { Data = value; } }
            public object Type { get; set; }
            public IEventDispatcher Target { get; set; }
            public object Data { get; set; }
        }
    }
}
