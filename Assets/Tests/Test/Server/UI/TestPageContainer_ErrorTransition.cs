using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Impl;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.UI.Api;
using NUnit.Framework;
using UnityEngine;
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
            SampleThreePanel.Rejected = true;
            // AssertHelper.Expect(LogType.Error, new Regex("hello exception"));
            try
            {
                await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "hello exception");
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

#pragma warning disable CS4014
        [Test]
        public async Task PreparePages_TransitionException()
        {
            // PLAN handle open events 
            SampleThreeUIController.promise = Promise.Create();
            MyException ex = null;

            if (Application.isPlaying)
            {
                _gameRoot.PromiseTimer.WaitFor(2, async () =>
                        {
                            try
                            {
                                await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                        });
                _gameRoot.PromiseTimer.WaitFor(50, async () =>
                {
                    try
                    {
                        await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
                    }
                    catch (MyException e)
                    {
                        ex = e;
                    }
                });
                await _gameRoot.PromiseTimer.WaitFor(100).AsTask();
                // we need to resolve or reject the push 
                SampleThreeUIController.promise.Resolve();
            }
            else
            {
               await Task.Delay(2).ContinueWith(async (t) =>
                            {
                                try
                                {
                                    await _pageContainer.PushPage(SampleUIKeys.SampleThreeUI);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(e);
                                }
                            });

               await Task.Delay(5).ContinueWith(async (t) =>
                                  {
                                      try
                                      {
                                          await _pageContainer.PushPage(SampleUIKeys.SampleFourUI);
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

            }


            Assert.AreEqual(UIExceptionType.OPEN_IN_TRANSITION, ex.Type);
        }

#pragma warning restore CS4014

        [Test]
        public async Task Anim_fail_same()
        {
            AssertHelper.Expect(LogType.Error, new Regex("hello exception"));
            AssertHelper.Expect(LogType.Warning, new Regex("the asset is already on the nodeTree"));
            SampleFivePanel.Rejected = true;

            await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);

            Assert.AreEqual(1, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }
        [Test]
        public async Task Anim_fail_switch()
        {
            AssertHelper.Expect(LogType.Error, new Regex("hello exception"));

            SampleFivePanel.Rejected = true;


            await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(1, SampleOneUIController.EnableCount);
            Assert.AreEqual(0, SampleFiveUIController.EnableCount);
        }

        [Test]
        public async Task Anim_fail_back()
        {
            AssertHelper.Expect(LogType.Error, new Regex("hello exception"));

            SampleFivePanel.Rejected = true;

            await _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            await _pageContainer.PushPage(SampleUIKeys.SampleFiveUI);

            Assert.AreEqual(2, _pageContainer.OperateNum);
            Assert.AreEqual(0, SampleOneUIController.EnableCount);
            Assert.AreEqual(1, SampleFiveUIController.EnableCount);
        }
    }
}
