using NUnit.Framework;
using System;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using UnityEngine.TestTools;
namespace Cr7Sund.PackageTest.PromiseTest
{
    public class PromiseProgressTests
    {
        [SetUp]
        public void SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
        }
        
        [Test]
        public void can_report_simple_progress()
        {
            const float expectedStep = 0.25f;
            float currentProgress = 0f;
            var promise = new Promise<int>();

            promise.Progress(v =>
            {
                AssertUtil.InRange(expectedStep - (v - currentProgress), -Math.E, Math.E);
                currentProgress = v;
            });

            for (float progress = 0.25f; progress < 1f; progress += 0.25f)
                promise.ReportProgress(progress);
            promise.ReportProgress(1f);

            Assert.AreEqual(1f, currentProgress);
        }

        [Test]
        public void can_handle_onProgress()
        {
            var promise = new Promise<int>();
            float progress = 0f;

            promise.Then(null, null, v => progress = v);

            promise.ReportProgress(1f);

            Assert.AreEqual(1f, progress);
        }

        [Test]
        public void can_handle_chained_onProgress()
        {
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            float progressA = 0f;
            float progressB = 0f;
            int result = 0;

            promiseA
                .Then(v => promiseB, null, v => progressA = v)
                .Progress(v => progressB = v)
                .Then(v => result = v)
                .Done();

            promiseA.ReportProgress(1f);
            promiseA.Resolve(-17);
            promiseB.ReportProgress(2f);
            promiseB.Resolve(17);

            Assert.AreEqual(1f, progressA);
            Assert.AreEqual(2f, progressB);
            Assert.AreEqual(17, result);
        }

        [Test]
        public void can_do_progress_weighted_average()
        {
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            var promiseC = new Promise<int>();

            float[] expectedProgress =
            {
                0.1f, 0.2f,
                0.6f, 1f
            };
            float currentProgress = 0f;
            int currentStep = 0;
            int result = 0;

            promiseC.Progress(v =>
                {
                    AssertUtil.InRange(currentStep, 0, expectedProgress.Length - 1);
                    Assert.AreEqual(v, expectedProgress[currentStep]);
                    currentProgress = v;
                    ++currentStep;
                })
                .Then(v => result = v)
                .Done()
                ;

            promiseA.Progress(v => promiseC.ReportProgress(v * 0.2f))
                .Then(v => promiseB, null)
                .Progress(v => promiseC.ReportProgress(0.2f + 0.8f * v))
                .Then(v => promiseC.Resolve(v))
                .Catch(ex => promiseC.RejectWithoutDebug(ex))
                ;

            promiseA.ReportProgress(0.5f);
            promiseA.ReportProgress(1f);
            promiseA.Resolve(-17);
            promiseB.ReportProgress(0.5f);
            promiseB.ReportProgress(1f);
            promiseB.Resolve(17);

            Assert.AreEqual(expectedProgress.Length, currentStep);
            Assert.AreEqual(1f, currentProgress);
            Assert.AreEqual(17, result);
        }


        [Test]
        public void chain_multiple_promises_reporting_progress()
        {
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            float progressA = 0f;
            float progressB = 0f;
            int result = 0;

            promiseA
                .Progress(v => progressA = v)
                .Then(v => promiseB, null)
                .Progress(v => progressB = v)
                .Then(v => result = v)
                .Done()
                ;

            promiseA.ReportProgress(1f);
            promiseA.Resolve(-17);
            promiseB.ReportProgress(2f);
            promiseB.Resolve(17);

            Assert.AreEqual(1f, progressA);
            Assert.AreEqual(2f, progressB);
            Assert.AreEqual(17, result);
        }

        [Test]
        public void exception_is_thrown_for_progress_after_resolve()
        {
            var promise = new Promise<int>();
            promise.Resolve(17);

            var promiseException = Assert.Throws<MyException>(() => promise.ReportProgress(1f));
            Assert.AreEqual(PromiseExceptionType.Valid_PROGRESS_STATE, promiseException.Type);
        }

        [Test]
        public void exception_is_thrown_for_progress_after_reject()
        {
            LogAssert.ignoreFailingMessages = true;
            var promise = new Promise<int>();
            promise.RejectWithoutDebug(new Exception());

            var promiseException = Assert.Throws<MyException>(() => promise.ReportProgress(1f));
            Assert.AreEqual(PromiseExceptionType.Valid_PROGRESS_STATE, promiseException.Type);
        }

        [Test]
        public void first_progress_is_averaged()
        {
            LogAssert.ignoreFailingMessages = true;
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            var promiseC = new Promise<int>();
            var promiseD = new Promise<int>();

            int currentStep = 0;
            float[] expectedProgress =
            {
                0.25f, 0.50f,
                0.75f, 1f
            };

            Promise<int>.First(() => promiseA, () => promiseB, () => promiseC, () => promiseD)
                .Progress(progress =>
                {
                    AssertUtil.InRange(currentStep, 0, expectedProgress.Length - 1);
                    Assert.AreEqual(expectedProgress[currentStep], progress);
                    ++currentStep;
                });

            var exception = new Exception();
            promiseA.RejectWithoutDebug(exception);
            promiseC.RejectWithoutDebug(exception);
            promiseB.RejectWithoutDebug(exception);
            promiseD.RejectWithoutDebug(exception);

            Assert.AreEqual(expectedProgress.Length, currentStep);
        }

        [Test]
        public void all_progress_is_averaged()
        {
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            var promiseC = new Promise<int>();
            var promiseD = new Promise<int>();

            int currentStep = 0;
            float[] expectedProgress =
            {
                0.25f, 0.50f,
                0.75f, 1f
            };

            Promise<int>.All(promiseA, promiseB, promiseC, promiseD)
                .Progress(progress =>
                {
                    AssertUtil.InRange(currentStep, 0, expectedProgress.Length - 1);
                    Assert.AreEqual(expectedProgress[currentStep], progress);
                    ++currentStep;
                });

            promiseA.ReportProgress(1f);
            promiseC.ReportProgress(1f);
            promiseB.ReportProgress(1f);
            promiseD.ReportProgress(1f);

            Assert.AreEqual(expectedProgress.Length, currentStep);
        }

        [Test]
        public void race_progress_is_maxed()
        {
            var promiseA = new Promise<int>();
            var promiseB = new Promise<int>();
            int reportCount = 0;

            Promise<int>.Race(promiseA, promiseB)
                .Progress(progress =>
                {
                    Assert.AreEqual(progress, 0.5f);
                    ++reportCount;
                });

            promiseA.ReportProgress(0.5f);
            promiseB.ReportProgress(0.1f);
            promiseB.ReportProgress(0.2f);
            promiseB.ReportProgress(0.3f);
            promiseB.ReportProgress(0.4f);
            promiseB.ReportProgress(0.5f);

            Assert.AreEqual(6, reportCount);
        }

        [Test]
        public void all_progress_with_resolved()
        {
            var promiseA = new Promise<int>();
            var promiseB = Promise<int>.Resolved(17);
            int reportedCount = 0;

            Promise<int>.All(promiseA, promiseB)
                .Progress(progress =>
                {
                    ++reportedCount;
                    Assert.AreEqual(0.75f, progress);
                });

            promiseA.ReportProgress(0.5f);

            Assert.AreEqual(1, reportedCount);
        }
    }
}
