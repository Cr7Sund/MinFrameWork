using System;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.PackageTest.EventBus;
using NUnit.Framework;
using UnityEngine;

namespace Cr7Sund.PackageTest.EventBus
{
    class EventCircleTest
    {
        private TestEventBus eventBus;
        [SetUp]
        public void SetUp()
        {
            eventBus = new TestEventBus();
        }

        [Test]
        public void RaiseCircularly()
        {
            var listener = eventBus.TestListener<SampleTestEvent>();
            listener.Subscribe(RecursivelyRegister);
            TestDelegate handler = () => eventBus.Raise(new SampleTestEvent());

            Assert.Throws<MyException>(handler);
        }

        private void RecursivelyRegister(TestListener<SampleTestEvent> listener)
        {
            eventBus.Raise(new SampleTestEvent());
        }
        [Test]
        public void RegisterCircularly()
        {
            var listener = eventBus.TestListener<SampleTestEvent>();
            listener.Subscribe(RecursivelyRaise);

            TestDelegate handler = () => eventBus.Raise(new SampleTestEvent());
            Assert.Throws<MyException>(handler);
        }

        private void RecursivelyRaise(TestListener<SampleTestEvent> listener)
        {
            listener.Subscribe(RecursivelyRegister);
        }
    }
}