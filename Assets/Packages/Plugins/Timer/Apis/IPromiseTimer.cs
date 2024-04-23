using System;
namespace Cr7Sund.Package.Api
{
    public interface IPromiseTimer : IDisposable
    {
        /// <summary>
        ///     Resolve the returned promise once the time has elapsed
        /// </summary>
        IPromise WaitFor(int duration);
        /// <summary>
        /// Resolve the returned promise once the time has elapsed
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="onTimeAction">in time actin</param>
        /// <returns></returns>
        IPromise WaitFor(int duration, Action onTimeAction);
        /// <summary>
        ///     Resolve the returned promise once the predicate evaluates to true
        /// </summary>
        IPromise WaitUntil(Func<TimeData, bool> predicate);

        /// <summary>
        ///     Resolve the returned promise once the predicate evaluates to false
        /// </summary>
        IPromise WaitWhile(Func<TimeData, bool> predicate);
        /// <summary>
        ///     Resolve the returned promise once the predicate evaluates to true
        /// </summary>
        IPromise Schedule(Func<TimeData, bool> predicate, Action<TimeData> poll);
        /// <summary>
        ///     Resolve the returned promise once the predicate evaluates to true
        /// </summary>
        IPromise Schedule(int duration, Action<TimeData> poll);
        IPromise Schedule(Action<TimeData> poll);
        /// <summary>
        ///     Update all pending promises. Must be called for the promises to progress and resolve at all.
        /// </summary>
        void Update(int deltaTime);

        /// <summary>
        ///     Cancel a waiting promise and reject it immediately.
        /// </summary>
        bool Cancel(IPromise promise);

        // TODO 
        // Support wait for frame
        // https://github.com/jonagill/AsyncRoutines/tree/main
    }

    /// <summary>
    ///     Time data specific to a particular pending promise.
    /// </summary>
    public struct TimeData
    {
        /// <summary>
        ///     The amount of time that has elapsed since the pending promise started running
        /// </summary>
        public int elapsedTime;

        /// <summary>
        ///     The amount of time since the last time the pending promise was updated.
        /// </summary>
        public int deltaTime;

        /// <summary>
        ///     The amount of times that update has been called since the pending promise started running
        /// </summary>
        public int elapsedUpdates;

    }

}
