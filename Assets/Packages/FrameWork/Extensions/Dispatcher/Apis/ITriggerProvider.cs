/**
 * @interface Cr7Sund.Framework.Api.ITrigerProvider
 *
 * Interface for declaring a class capable of Trigering an ITrigerable class.
 *
 * Objects implementing a TrigerProvider declare themselves able to
 * provide Trigering to any ITrigerable. The contract specifies that
 * TrigerProvider will pass events on to the Trigerable class.
 * This allows notifications, such as IEvents, to pass through
 * the event bus and Trigger other binders.
 *
 * @see Cr7Sund.Framework.Api.ITrigerable
 */

namespace Cr7Sund.Framework.Api
{
    public interface ITriggerProvider
    {

        /// Count of the current number of Trigger clients.
        int TriggerableCount { get; }
        /// Register a Trigerable client with this provider.
        void AddTriggerable(ITriggerable target);

        /// Remove a previously registered Trigerable client from this provider.
        void RemoveTriggerable(ITriggerable target);
    }
}
