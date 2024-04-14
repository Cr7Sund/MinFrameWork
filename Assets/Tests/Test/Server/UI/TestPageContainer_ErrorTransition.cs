using System;
using System.Threading.Tasks;
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

        [Test]
        public async Task PreparePage_Pending()
        {
            SampleThreeUIController.promise = Promise.Create();

            var task = _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            Assert.AreEqual(0, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);

            SampleThreeUIController.promise.Resolve();
            await task;
        }

        [Test]
        public async Task PreparePage_Rejected()
        {
            SampleThreeUIController.Rejected = true;
            LogAssert.Expect(UnityEngine.LogType.Error, "hello exception");
            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            }
            catch
            {

            }
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleThreeUIController.StartValue);
            Assert.AreEqual(0, SampleThreeUIController.EnableCount);
            Assert.AreEqual(1, SampleOneUIController.StartValue);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
        }

        [Test]
        public async Task PreparePage_Resolved()
        {
            SampleThreeUIController.promise = Promise.Create(); ;
            SampleThreeUIController.promise.Resolve();

            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleThreeUIController.EnableCount);
        }

        [Test]
        public async Task PreparePages_TransitionException()
        {

            SampleThreeUIController.promise = Promise.Create();
            MyException ex = null;

            await Task.Delay(2).ContinueWith(async (t) =>
            {
                try
                {
                    await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
                }
                catch (Exception e)
                {
                    if (e is MyException myException)
                        ex = myException;

                }
            });

            await Task.Delay(5).ContinueWith(async (t) =>
                              {
                                  try
                                  {
                                      await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
                                  }
                                  catch (MyException e)
                                  {
                                      ex = e;
                                  }
                              });


            await Task.Delay(10).ContinueWith((t) =>
            {
                SampleThreeUIController.promise.Resolve();
            });

            // Assert.AreEqual(UIExceptionType.OPEN_IN_TRANSITION, ex.Type);
        }

        [Test]
        public async Task Anim_fail_same()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, $"hello exception");
            SampleFivePanel.Rejected = true;

            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            }
            catch (Exception ex) { Console.Error(ex); }
            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            }
            catch (Exception ex) { Console.Error(ex); }

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }

        [Test]
        public async Task Anim_fail_switch()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, $"hello exception");

            SampleFivePanel.Rejected = true;

            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            }
            catch (Exception ex) { Console.Error(ex); }
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleFiveUIController.EnableCount);
        }

        [Test]
        public async Task Anim_fail_back()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, $"hello exception");
            LogAssert.Expect(UnityEngine.LogType.Error, $"hello exception");

            SampleFivePanel.Rejected = true;

            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            }
            catch (Exception ex) { Console.Error(ex); }
            await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);

            try
            {
                await _pageContainer.BackPage();
            }
            catch (Exception ex) { Console.Error(ex); }

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }
    }
}
