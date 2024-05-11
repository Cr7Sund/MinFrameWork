using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cr7Sund
{
    public partial struct PromiseTask<T>
    {
        public static PromiseTask<T> FromException(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
               return FromCanceled(oce.CancellationToken);
            }

            return new PromiseTask<T>(new ExceptionResultSource<T>(ex), 0);
        }

        public static PromiseTask<T> FromCanceled(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledUniTaskCache<T>.Task;
            }
            else
            {
                return new PromiseTask<T>(new CanceledResultSource<T>(cancellationToken), 0);
            }
        }

        public static PromiseTask<T> FromResult(T value)
        {
            return new PromiseTask<T>(value);
        }

        public static PromiseTask<T[]> WhenAll(params PromiseTask<T>[] tasks)
        {
            return WhenAll((IEnumerable<PromiseTask<T>>)tasks.ToArray());
        }
        public static PromiseTask<T[]> WhenAll(IEnumerable<PromiseTask<T>> tasks)
        {
            PromiseTask<T>[] promiseTasks = tasks.ToArray();
            var promise = new WhenAllPromiseTaskSource<T>(promiseTasks, promiseTasks.Length); // consumed array in constructor.
            return new PromiseTask<T[]>(promise, 0);
        }
    }

    public partial struct PromiseTask
    {
        public static PromiseTask FromException(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled(oce.CancellationToken);
            }
            return new PromiseTask(new ExceptionResultSource(ex), 0);
        }

        public static PromiseTask FromCanceled(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledUniTaskCache.Task;
            }
            else
            {
                return new PromiseTask(new CanceledResultSource(cancellationToken), 0);
            }
        }

        public PromiseTask<bool> SuppressCancellationThrow()
        {
            var status = Status;
            if (status == PromiseTaskStatus.Succeeded) return CompletedTasks.False;
            if (status == PromiseTaskStatus.Canceled) return CompletedTasks.True;
            return new PromiseTask<bool>(new IsCanceledSource(source), token);
        }
        
        public static PromiseTask WhenAll(params PromiseTask[] tasks)
        {
            return WhenAll((IEnumerable<PromiseTask>)tasks.ToArray());
        }

        public static PromiseTask WhenAll(IEnumerable<PromiseTask> tasks)
        {
            PromiseTask[] promiseTasks = tasks.ToArray();
            var promise = new WhenAllPromiseTaskSource(promiseTasks, promiseTasks.Length); // consumed array in constructor.
            return new PromiseTask(promise, 0);
        }
    }

    internal static class CompletedTasks
    {
        public static readonly PromiseTask<bool> True = PromiseTask<bool>.FromResult(true);
        public static readonly PromiseTask<bool> False = PromiseTask<bool>.FromResult(false);
        public static readonly PromiseTask<int> Zero = PromiseTask<int>.FromResult(0);
        public static readonly PromiseTask<int> MinusOne = PromiseTask<int>.FromResult(-1);
        public static readonly PromiseTask<int> One = PromiseTask<int>.FromResult(1);
    }
}
