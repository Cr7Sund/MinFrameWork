/**
 * @interface Cr7Sund.Framework.Api.IDispatcher
 *
 * A Dispatcher sends notification to any registered listener.
 * It represents the subject in a standard Observer pattern.
 *
 * In MVCSContext the dispatched notification is an IEvent.
 */

namespace Cr7Sund.Framework.Api
{
    public interface IDispatcher
    {
        /// Send a notification of type eventType. No data.
        /// In MVCSContext this dispatches an IEvent.
        void Dispatch(object eventType);

        /// Send a notification of type eventType and the provided data payload.
        /// In MVCSContext this dispatches an IEvent.
        void Dispatch(object eventType, object data);
    }
}
