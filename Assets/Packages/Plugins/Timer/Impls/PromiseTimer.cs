using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    public class PromiseTimer : IPromiseTimer
    {
        /// <summary>
        ///     Currently pending promises
        /// </summary>
        private readonly LinkedList<PredicateWait> waitings = new LinkedList<PredicateWait>();

        /// <summary>
        ///     The current running total for the amount of frames the PromiseTimer has run for
        /// </summary>
        private long curFrame;
        /// <summary>
        ///     The current running total for time that this PromiseTimer has run for
        /// </summary>
        private long curTime;

        public bool Cancel(IPromise promise)
        {
            var node = FindInWaiting(promise);

            if (node == null)
            {
                return false;
            }

            node.Value.pendingPromise.Reject(
                new PromiseTimerException("Promise was cancelled by user.",
                    PromiseTimerExceptionType.CANCEL_TIMER));
            waitings.Remove(node);

            return true;
        }

        public void Update(long deltaTime)
        {
            curTime += deltaTime;
            curFrame += 1;

            var node = waitings.First;
            while (node != null)
            {
                var wait = node.Value;
                long newElapsedTime = curTime - wait.timeStarted;
                wait.timeData.deltaTime = newElapsedTime - wait.timeData.elapsedTime;
                wait.timeData.elapsedTime = newElapsedTime;
                long newElapsedUpdates = curFrame - wait.frameStarted;
                wait.timeData.elapsedUpdates = newElapsedUpdates;

                bool result = false;

                try
                {
                    wait.polling?.Invoke(wait.timeData);
                    result = wait.predicate.Invoke(wait.timeData);
                }
                catch (Exception e)
                {
                    wait.pendingPromise.Reject(e);
                    node = RemoveNode(node);
                    continue;
                }

                if (result)
                {
                    node = RemoveNode(node);
                    wait.pendingPromise.Resolve();
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        public IPromise WaitFor(long duration)
        {
            AssertUtil.LessOrEqual(0, duration, PromiseTimerExceptionType.INVALID_DURATION);

            return WaitUntil(t => t.elapsedTime > duration);
        }

        public IPromise WaitFor(long duration, Action onTimeAction)
        {
            AssertUtil.LessOrEqual(0, duration, PromiseTimerExceptionType.INVALID_DURATION);

            return WaitUntil(t =>
            {
                if (t.elapsedTime > duration)
                {
                    onTimeAction?.Invoke();
                    return true;
                }
                return false;
            });
        }

        public IPromise WaitUntil(Func<TimeData, bool> predicate)
        {
            var promise = new Promise();

            var wait = new PredicateWait
            {
                timeStarted = curTime,
                pendingPromise = promise,
                timeData = new TimeData(),
                predicate = predicate,
                frameStarted = curFrame,
                innerTask = promise
            };

            waitings.AddLast(wait);

            return promise;
        }

        public IPromise WaitWhile(Func<TimeData, bool> predicate)
        {
            return WaitUntil(t => !predicate(t));
        }

        public IPromise Schedule(Func<TimeData, bool> predicate, Action<TimeData> poll, UnsafeCancellationToken cancellation = default)
        {
            if (cancellation.IsCancellationRequested)
            {
                // PLAN: StackTrace()
                return Promise.RejectedWithoutDebug(new OperationCanceledException());
            }

            var promise = new Promise();

            var wait = new PredicateWait
            {
                timeStarted = curTime,
                pendingPromise = promise,
                timeData = new TimeData(),
                predicate = predicate,
                polling = poll,
                frameStarted = curFrame,
                innerTask = promise
            };

            waitings.AddLast(wait);

            if (cancellation.IsValid)
            {
                cancellation.Register(() =>
                         {
                             promise.Cancel();
                             waitings.Remove(wait);
                         });
            }
            return promise;
        }

        public IPromise Schedule(long duration, Action<TimeData> poll, UnsafeCancellationToken cancellation = default)
        {
            AssertUtil.LessOrEqual(0, duration, PromiseTimerExceptionType.INVALID_DURATION);

            return Schedule(t => t.elapsedTime > duration, t => poll(t), cancellation);
        }

        public IPromise Schedule(Action<TimeData> poll, UnsafeCancellationToken cancellation = default)
        {
            return Schedule(t => false, t => poll(t), cancellation);
        }

        /// <summary>
        ///     Removes the provided node and returns the next node in the list.
        /// </summary>
        private LinkedListNode<PredicateWait> RemoveNode(LinkedListNode<PredicateWait> node)
        {
            var currentNode = node.Next;
            waitings.Remove(node);
            return currentNode;
        }

        private LinkedListNode<PredicateWait> FindInWaiting(IPromise promise)
        {
            for (var node = waitings.First; node != null; node = node.Next)
            {
                if (node.Value.pendingPromise.Id.Equals(promise.Id))
                {
                    return node;
                }
            }

            return null;
        }

        public void Clear()
        {
            var node = waitings.Last;
            while (node != null)
            {
                node.Value.innerTask.Cancel();
                node = node.Previous;
            }
   
            waitings.Clear();
        }

        public void Dispose()
        {
            AssertUtil.LessOrEqual(waitings.Count, 0);
            curFrame = 0;
            curFrame = 0;
        }
    }

}
