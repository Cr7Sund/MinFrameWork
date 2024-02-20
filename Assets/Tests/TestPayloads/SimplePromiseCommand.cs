using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Cr7Sund.PackageTest.IOC
{
    public class TestBaseCommandGeneric : Command<int>
    {
        public override int OnExecute(int value)
        {
            return value;
        }
    }

    public class ExceptionCommandGeneric : Command<int>
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

    public class SimpleCommandOneGeneric : Command<int>
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

    public class SimpleCommandTwoGeneric : Command<int>
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

    public class AnotherCommand : Command<float>
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

    public class ConvertCommand : Command<int, float>
    {
        public override float OnExecute(int value)
        {

            SimplePromise.floatResult = (value + 3) * 4.2f;
            return SimplePromise.floatResult;
        }
    }

    public class SimpleAsyncCommandOneGeneric : AsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            // you need to pass the value which is from the prev promise 
            var promise = DownloadAsync().Then(value =>
            {
                SimplePromise.result = (value + aggValue + 3) * 5;
                return (value + aggValue + 3) * 5;
            });

            return promise;
        }
        private static IPromise<int> DownloadAsync()
        {
            SimplePromise.simulatePromiseOne?.Dispose();

            return SimplePromise.simulatePromiseOne;
        }
        public override void OnCatch(Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public class SimpleAsyncCommandSecondGeneric : AsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            SimplePromise.result += 2;
            return SimplePromise.simulatePromiseSecond.Then(value =>
            {
                SimplePromise.result = (value + aggValue + 1) * 2;
                ;
                return (value + aggValue + 1) * 2;
            });

        }
    }

    public class SimpleAsyncException_Generic : AsyncCommand<int>
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

    public class SimpleAsyncConvertCommand:AsyncCommand<int,float>
    {
        public override IPromise<float> OnExecuteAsync(int aggValue)
        {
            var promise = DownloadAsync().Then(value =>
            {
                SimplePromise.floatResult = (value + aggValue + 3) * 5;
                return (value + aggValue + 3) * 5;
            });

            return promise;
        }
        private static IPromise<float> DownloadAsync()
        {
            SimplePromise.simulatePromiseFloat?.Dispose();

            return SimplePromise.simulatePromiseFloat;
        }
    }
    public class SimpleAsyncCatch_Generic : AsyncCommand<int>
    {
        public override IPromise<int> OnExecuteAsync(int aggValue)
        {
            Func<int, int> transform = value =>
            {
                throw new Exception();
            };
            return SimplePromise.simulatePromiseSecond.Then(transform);
        }

        public override IPromise<int> OnCatchAsync(Exception ex)
        {
            return SimplePromise.simulatePromiseSecond.Then(value =>
            {
                return (value + 1) * 2;
            });
        }
    }

    public class SimpleProgressCommand_Generic : Command<int>
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

    public class ConvertEnumerableCommand : Command<IEnumerable<int>, int>
    {
        public override int OnExecute(IEnumerable<int> values)
        {
            int first = values.First();
            SimplePromise.result = (first + 1) * 20;
            return SimplePromise.result;
        }
    }


    public class TestInjectionCommand : Command
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
