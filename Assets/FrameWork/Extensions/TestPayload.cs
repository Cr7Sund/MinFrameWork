using System;
using System.Diagnostics;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using UnityEngine;

namespace TestMono{
    public class SimplePromise
    {
        public static int result = 0;
        public static float floatResult = 0;
        public static IPromise simulatePromise;
        public static IPromise<int> simulatePromiseOne;
        public static IPromise<int> simulatePromiseSecond;
        public static string exceptionStr;
        public static float currentProgress;
    }

    public class TestBasePromiseCommand : PromiseCommand
    {
        public override void OnExecute()
        {

        }
    }

    public class ExceptionPromiseCommand : PromiseCommand
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

    public class SimplePromiseCommandOne : PromiseCommand
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

    public class SimplePromiseCommandTwo : PromiseCommand
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

    public class SimpleAsyncPromiseCommandOne : PromiseAsyncCommand
    {
        public override void OnCatch(Exception e)
        {

        }

        public override IPromise OnExecuteAsync()
        {

            SimplePromise.simulatePromise.Then(() =>
            {
                SimplePromise.result = (SimplePromise.result + 3) * 5;
            });
            return SimplePromise.simulatePromise;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimpleProgressCommand : PromiseCommand
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

