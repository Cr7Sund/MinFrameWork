using System;
using System.Collections.Generic;
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
        private int curFrame;
        /// <summary>
        ///     The current running total for time that this PromiseTimer has run for
        /// </summary>
        private int curTime;

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

        public void Update(int deltaTime)
        {
            curTime += deltaTime;
            curFrame += 1;

            var node = waitings.First;
            while (node != null)
            {
                var wait = node.Value;
                int newElapsedTime = curTime - wait.timeStarted;
                wait.timeData.deltaTime = newElapsedTime - wait.timeData.elapsedTime;
                wait.timeData.elapsedTime = newElapsedTime;
                int newElapsedUpdates = curFrame - wait.frameStarted;
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
                    wait.pendingPromise.Resolve();
                    node = RemoveNode(node);
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        public IPromise WaitFor(int duration)
        {
            AssertUtil.LessOrEqual(0, duration,PromiseTimerExceptionType.INVALID_DURATION);

            return WaitUntil(t => t.elapsedTime > duration);
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
                frameStarted = curFrame
            };

            waitings.AddLast(wait);

            return promise;
        }

        public IPromise WaitWhile(Func<TimeData, bool> predicate)
        {
            return WaitUntil(t => !predicate(t));
        }

        public IPromise WaitUntil(Func<TimeData, bool> predicate, Action<TimeData> poll)
        {
            var promise = new Promise();

            var wait = new PredicateWait
            {
                timeStarted = curTime,
                pendingPromise = promise,
                timeData = new TimeData(),
                predicate = predicate,
                polling = poll,
                frameStarted = curFrame
            };

            waitings.AddLast(wait);

            return promise;
        }

        public IPromise WaitFor(int duration, Action<int> poll)
        {
            AssertUtil.LessOrEqual(0, duration, PromiseTimerExceptionType.INVALID_DURATION);

            return WaitUntil(t => t.elapsedTime > duration, t => poll(t.elapsedTime));
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
    }

}
