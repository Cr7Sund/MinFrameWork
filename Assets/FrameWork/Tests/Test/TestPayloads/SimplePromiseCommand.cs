using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Tests
{
    public class TestBasePromiseCommand_Generic : PromiseCommand<int>
    {
        public override int OnExecute(int value)
        {
            return value;
        }
    }

    public class ExceptionPromiseCommand_Generic : PromiseCommand<int>
    {
        public override int OnExecute(int value)
        {
            throw new NotImplementedException();
        }

        public override void OnCatch(Exception e)
        {
            SimplePromise.exceptionStr = e.Message;
        }
    }

    public class SimplePromiseCommandOne_Generic : PromiseCommand<int>
    {
        public override void OnCatch(Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw e;
        }

        public override int OnExecute(int value)
        {
            return (value + 1) * 2;
        }

        public override void OnProgress(float progress)
        {
        }
    }

    public class SimplePromiseCommandTwo_Generic : PromiseCommand<int>
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

    public class AnotherPromiseCommand : PromiseCommand<float>
    {
        public override float OnExecute(float value)
        {
            return (value + 1) * 2;
        }
    }

    public class ConvertPromiseCommand : ConvertCommand<int>
    {
        public override object OnConvert(int value)
        {
            return (value + 3) * 4.2f;
        }

        public override void OnCatch(Exception e)
        {
            base.OnCatch(e);
        }

    }

    public class SimpleAsyncPromiseCommandOne_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            SimplePromise.result += 1;
            return SimplePromise.simulatePromiseOne.Then((value) =>
              {
                  SimplePromise.result = 10;
                  UnityEngine.Debug.Log("First " + value + " :" + aggValue);
                  // only  when return value , the chain promise  will continue the value chaining
                  return (value + aggValue + 3) * 5;
              });

        }
    }

    public class SimpleAsyncPromiseCommandSecond_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            SimplePromise.result += 2;
            return SimplePromise.simulatePromiseSecond.Then((value) =>
              {
                  SimplePromise.result = 20;
                  UnityEngine.Debug.Log("Second " + value + " :" + aggValue);
                  // only  when return value , the chain promise  will continue the value chaining
                  return (value + aggValue + 1) * 2;
              });

        }
    }

    public class SimpleProgressCommand_Generic : PromiseCommand<int>
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

