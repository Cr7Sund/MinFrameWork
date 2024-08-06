using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.Package.Impl
{
    /// <summary>
    ///     A class that wraps a pending promise with it's predicate and time data
    /// </summary>
    internal class PredicateWait
    {

        /// <summary>
        ///     The frame the promise was started
        /// </summary>
        public long frameStarted;

        /// <summary>
        ///     The pending promise which is an interface for a promise that can be rejected or resolved.
        /// </summary>
        public IPendingPromise pendingPromise;
        /// <summary>
        ///     Predicate for resolving the promise
        /// </summary>
        public Func<TimeData, bool> predicate;
        /// <summary>
        ///     polling action each update
        /// </summary>
        public Action<TimeData> polling;
        /// <summary>
        ///     The time data specific to this pending promise. Includes elapsed time and delta time.
        /// </summary>
        public TimeData timeData;

        /// <summary>
        ///     The time the promise was started
        /// </summary>
        public long timeStarted;

        public IPromise innerTask;

    }

}
