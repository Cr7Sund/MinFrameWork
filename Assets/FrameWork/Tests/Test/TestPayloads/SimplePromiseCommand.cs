using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Tests
{
    public class TestBasePromiseCommand<T> : PromiseCommand<int>
    {
        public override int OnExecute(int value)
        {
            return value;
        }
    }

    public class ExceptionPromiseCommand<T> : PromiseCommand<int>
    {
        public override int OnExecute(int value)
        {
            throw new NotImplementedException();
        }

        public override void OnCatch(Exception e)
        {
            SimplePromise.exceptionStr = e.Message;
            UnityEngine.Debug.Log("sdf");
        }
    }

    public class SimplePromiseCommandOne<T> : PromiseCommand<int>
    {
        public override void OnCatch(Exception e)
        {

        }

        public override int OnExecute(int value)
        {
            return (value + 1) * 2;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimplePromiseCommandTwo<T> : PromiseCommand<int>
    {
        public override void OnCatch(Exception e)
        {

        }

        public override int OnExecute(int value)
        {
            return (value + 2) * 3;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class ConvertPromiseCommand<PromisedT, ConvertedT> : PromiseCommand<int, float>
    {
        public override float OnConvert(int value)
        {
            return (value + 3) * 4.2f;
        }

    }

    public class SimpleAsyncPromiseCommandOne<T> : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            return SimplePromise.simulatePromiseT.Then((value) =>
              {
                  // only  when return value , the chain promise  will continue the value chaining
                  return (value + aggValue + 3) * 5;
              });

        }
    }

    public class SimpleProgressCommand<T> : PromiseCommand<int>
    {
        public const float expectedStep = 0.25f;
        public override int OnExecute(int value)
        {
            return value;
        }

        public override void OnProgress(float progress)
        {
            AssertExt.InRange(expectedStep - (progress - SimplePromise.currentProgress), -Math.E, Math.E);
            SimplePromise.currentProgress = progress;
        }
    }
}

