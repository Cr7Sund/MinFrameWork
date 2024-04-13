using System;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.UI.Api;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Cr7Sund.ServerTest.UI
{
    public partial class TestPageContainer
    {

      //[Test]
        public void PreparePage_Pending()
        {
            SampleThreeUIController.promise = new Promise();

            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(0, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
        }

      //[Test]
        public void PreparePage_Rejected()
        {
            LogAssert.ignoreFailingMessages = true;
            SampleThreeUIController.Rejected = true;

            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleOneUIController.StartValue);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

      //[Test]
        public void PreparePage_Resolved()
        {
            SampleThreeUIController.promise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            SampleThreeUIController.promise.Resolve();
            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }

      //[Test]
        public void PreparePages_TransitionException()
        {
            SampleThreeUIController.promise = new Promise();
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            TestDelegate testDelegate = () => _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            var ex = Assert.Throws<MyException>(testDelegate);
            Assert.AreEqual(UIExceptionType.OPEN_IN_TRANSITION, ex.Type);
        }


      //[Test]
        public void Anim_fail_same()
        {
            LogAssert.ignoreFailingMessages = true;
            SampleFivePanel.Rejected = true;

            _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }

      //[Test]
        public void Anim_fail_switch()
        {
            LogAssert.ignoreFailingMessages = true;

            SampleFivePanel.Rejected = true;

            _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleFiveUIController.EnableCount);
        }

      //[Test]
        public void Anim_fail_back()
        {
            LogAssert.ignoreFailingMessages = true;

            SampleFivePanel.Rejected = true;

            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            _pageContainer.BackPage();

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }
    }
}
