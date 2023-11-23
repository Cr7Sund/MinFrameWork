/**
 * @interface Cr7Sund.Framework.Api.IEvent
 *
 * The interface for an event sent by the EventDispatcher
 */

namespace Cr7Sund.Framework.Api
{
    public interface IEvent
    {
        /// The Event key
        object Type { get; set; }

        /// The IEventDispatcher that sent the event
        IEventDispatcher Target { get; set; }

        /// An arbitrary data payload
        object Data { get; set; }
    }
}
