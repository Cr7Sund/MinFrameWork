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
}
