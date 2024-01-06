using Cr7Sund.Framework.Api;
using System;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Impl
{
    public class PromiseTimer : IPromiseTimer
    {

        /// <summary>
        ///     Currently pending promises
        /// </summary>
        private readonly LinkedList<PredicateWait> waiting = new LinkedList<PredicateWait>();

        /// <summary>
        ///     The current running total for the amount of frames the PromiseTimer has run for
        /// </summary>
        private int curFrame;
        /// <summary>
        ///     The current running total for time that this PromiseTimer has run for
        /// </summary>
        private float curTime;

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
            waiting.Remove(node);

            return true;
        }

        public void Update(float deltaTime)
        {
            curTime += deltaTime;
            curFrame += 1;

            var node = waiting.First;
            while (node != null)
            {
                var wait = node.Value;
                float newElapsedTime = curTime - wait.timeStarted;
                wait.timeData.deltaTime = newElapsedTime - wait.timeData.elapsedTime;
                wait.timeData.elapsedTime = newElapsedTime;
                int newElapsedUpdates = curFrame - wait.frameStarted;
                wait.timeData.elapsedUpdates = newElapsedUpdates;

                bool result = false;

                try
                {
                    result = wait.predicate(wait.timeData);
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

        public IPromise WaitFor(float seconds)
        {
            return WaitUntil(t => t.elapsedTime > seconds);
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

            waiting.AddLast(wait);

            return promise;
        }

        public IPromise WaitWhile(Func<TimeData, bool> predicate)
        {
            return WaitUntil(t => !predicate(t));
        }

        /// <summary>
        ///     Removes the provided node and returns the next node in the list.
        /// </summary>
        private LinkedListNode<PredicateWait> RemoveNode(LinkedListNode<PredicateWait> node)
        {
            var currentNode = node.Next;
            waiting.Remove(node);
            return currentNode;
        }

        private LinkedListNode<PredicateWait> FindInWaiting(IPromise promise)
        {
            for (var node = waiting.First; node != null; node = node.Next)
            {
                if (node.Value.pendingPromise.Id.Equals(promise.Id))
                {
                    return node;
                }
            }

            return null;
        }
    }

    /// <summary>
    ///     A class that wraps a pending promise with it's predicate and time data
    /// </summary>
    internal class PredicateWait
    {

        /// <summary>
        ///     The frame the promise was started
        /// </summary>
        public int frameStarted;

        /// <summary>
        ///     The pending promise which is an interface for a promise that can be rejected or resolved.
        /// </summary>
        public IPendingPromise pendingPromise;
        /// <summary>
        ///     Predicate for resolving the promise
        /// </summary>
        public Func<TimeData, bool> predicate;

        /// <summary>
        ///     The time data specific to this pending promise. Includes elapsed time and delta time.
        /// </summary>
        public TimeData timeData;

        /// <summary>
        ///     The time the promise was started
        /// </summary>
        public float timeStarted;
    }

}
