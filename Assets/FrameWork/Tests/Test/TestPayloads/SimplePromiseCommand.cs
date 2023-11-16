using System;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System.Collections.Generic;
using System.Linq;

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
            throw e;
        }

        public override int OnExecute(int value)
        {
            SimplePromise.result = (value + 1) * 2;
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
            SimplePromise.result = -1;
        }

        public override int OnExecute(int value)
        {
            SimplePromise.result = (value + 2) * 3;
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
            SimplePromise.floatResult = (value + 1) * 2;
            return SimplePromise.floatResult;
        }

        public override void OnCatch(Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public class ConvertPromiseCommand : PromiseCommand<int, float>
    {
        public override float OnExecute(int value)
        {

            SimplePromise.floatResult = (value + 3) * 4.2f;
            return SimplePromise.floatResult;
        }

    }

    public class SimpleAsyncPromiseCommandOne_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            return SimplePromise.simulatePromiseOne.Then((value) =>
              {
                  SimplePromise.result = (value + aggValue + 3) * 5;
                  return (value + aggValue + 3) * 5;
              });

        }
        public override void OnCatch(Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public class SimpleAsyncPromiseCommandSecond_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            SimplePromise.result += 2;
            return SimplePromise.simulatePromiseSecond.Then((value) =>
              {
                  SimplePromise.result = (value + aggValue + 1) * 2; ;
                  return (value + aggValue + 1) * 2;
              });

        }
    }

    public class SimpleAsyncException_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            throw new NotImplementedException();
        }

        public override IPromise<int> OnCatchAsync(Exception ex)
        {
            return SimplePromise.simulatePromiseOne;
        }
    }


    public class SimpleAsyncCatch_Generic : PromiseAsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            Func<int, int> transform = (value) =>
                        {
                            throw new System.Exception();
                        };
            return SimplePromise.simulatePromiseSecond.Then(transform);
        }

        public override IPromise<int> OnCatchAsync(Exception ex)
        {
            return SimplePromise.simulatePromiseSecond.Then((value) =>
            {
                return (value + 1) * 2;
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
            AssertUtil.InRange(expectedStep - (progress - SimplePromise.currentProgress), -Math.E, Math.E);
            SimplePromise.currentProgress = progress;
        }
    }

    public class ConvertPromiseEnumerableCommand : PromiseCommand<IEnumerable<int>, int>
    {
        public override int OnExecute(IEnumerable<int> values)
        {
            var first = values.First();
            SimplePromise.result = (first + 1) * 2;
            return SimplePromise.result;
        }

    }


    public class TestInjectionCommand : PromiseCommand
    {
        [Inject]
        public ISimpleInterface classToBeInjected;
        public override void OnExecute()
        {
            if (classToBeInjected != null)
            {
                SimplePromise.result = 1;
            }
        }
    }
}

