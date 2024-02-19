using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using System;
namespace Cr7Sund.PackageTest.IOC
{
    public class SimplePromise
    {
        public static int result;
        public static float floatResult = 0;
        public static IPromise simulatePromise;
        public static IPromise<int> simulatePromiseOne;
        public static IPromise<int> simulatePromiseSecond;
        public static IPromise<float> simulatePromiseFloat;
        public static string exceptionStr;
        public static float currentProgress;
    }

    public class TestBaseCommand : Command
    {
        public override void OnExecute()
        {

        }
    }

    public class ExceptionCommand : Command
    {
        public override void OnExecute()
        {
            throw new NotImplementedException();
        }

        public override void OnCatch(Exception e)
        {
            SimplePromise.exceptionStr = e.Message;
        }
    }

    public class SimpleCommandOne : Command
    {
        public override void OnCatch(Exception e)
        {

        }

        public override void OnExecute()
        {
            SimplePromise.result = (SimplePromise.result + 1) * 2;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimpleCommandTwo : Command
    {
        public override void OnCatch(Exception e)
        {

        }

        public override void OnExecute()
        {
            SimplePromise.result = (SimplePromise.result + 2) * 3;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimpleAsyncCommandOne : AsyncCommand
    {
        public override void OnCatch(Exception e)
        {

        }

        public override IPromise OnExecuteAsync()
        {
            return DownloadAsync().Then(() =>
            {
                SimplePromise.result = (SimplePromise.result + 3) * 5;
            });
            // return SimplePromise.simulatePromise;
        }
        private static IPromise DownloadAsync()
        {
            SimplePromise.simulatePromise?.Dispose();
            return SimplePromise.simulatePromise;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimpleProgressCommand : Command
    {
        public const float expectedStep = 0.25f;
        public override void OnExecute()
        {

        }

        public override void OnProgress(float progress)
        {
            AssertUtil.InRange(expectedStep - (progress - SimplePromise.currentProgress), -Math.E, Math.E);
            SimplePromise.currentProgress = progress;
        }
    }
}
