/**
* @class Cr7Sund.Framework.Api.EventDispatcher
* 
* A Dispatcher that uses IEvent to send messages.
* 
* Whenever the Dispatcher executes a `Dispatch()`, observers will be 
* notified of any event (Key) for which they have registered.
* 
* EventDispatcher dispatches TmEvent : IEvent.
* 
* The EventDispatcher is the only Dispatcher currently released with Strange
* (though by separating EventDispatcher from Dispatcher I'm obviously
	* signalling that I don't think it's the only possible one).
* 
* EventDispatcher is both an ITriggerProvider and an ITriggerable.
* 
* @see Cr7Sund.Framework.Api.IEvent
* @see Cr7Sund.Framework.Api.ITriggerProvider
* @see Cr7Sund.Framework.Api
*/
using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using PlasticPipe.PlasticProtocol.Client;

namespace Cr7Sund.Framework.Impl
{
    public class EventDispatcher : Binder, IEventDispatcher
    {

        /// The list of clients that will be triggered as a consequence of an Event firing.
		protected HashSet<ITriggerable> triggerClients;
        protected HashSet<ITriggerable> triggerClientRemovals;
        protected bool isTriggeringClients;

        // The eventPool is shared across all EventDispatchers for efficiency
        public static IPool<TmEvent> eventPool;

        public EventDispatcher()
        {
            if (eventPool == null)
            {
                eventPool = new Pool<TmEvent>();
                eventPool.InstanceProvider = new EventInstanceProvider();
            }
        }

        #region IBinder Implementation 
        
        protected override IBinding GetRawBinding()
        {
            return new EventBinding(resolver);
        }

        public new IEventBinding Bind(object key)
        {
            return base.Bind(key) as IEventBinding;
        }

        #endregion

        #region ITriggerProvider Implementation

        public int TriggerableCount
        {
            get
            {
                if (triggerClients == null)
                    return 0;
                return triggerClients.Count;
            }
        }

        public void AddTriggerable(ITriggerable target)
        {
            if (triggerClients == null)
            {
                triggerClients = new HashSet<ITriggerable>();
            }
            triggerClients.Add(target);
        }

        public void RemoveTriggerable(ITriggerable target)
        {
            if (triggerClients.Contains(target))
            {
                if (triggerClientRemovals == null)
                {
                    triggerClientRemovals = new HashSet<ITriggerable>();
                }
                triggerClientRemovals.Add(target);
                if (!isTriggeringClients)
                {
                    FlushRemoval();
                }
            }
        }

        #endregion

        #region IDispatcher Implementation

        public void Dispatch(object eventType)
        {
            Dispatch(eventType, null);
        }

        public void Dispatch(object eventType, object data)
        {
            // Scrub the data to make eventType and data conform is possible
            var evt = ConformDataToEvent(eventType, data);

            if (evt is IPoolable poolable)
            {
                poolable.Retain();
            }

            bool continueDispatch = true;
            if (triggerClients != null)
            {
                isTriggeringClients = true;
                foreach (ITriggerable trigger in triggerClients)
                {
                    if (!trigger.Trigger(eventType, evt))
                    {
                        continueDispatch = false;
                        break;
                    }
                }

                if (triggerClientRemovals != null)
                {
                    FlushRemoval();
                }
                isTriggeringClients = false;
            }

            if (!continueDispatch)
            {
                InternalReleaseEvent(evt);
                return;
            }

            var binding = GetBinding(eventType) as IEventBinding;
            if (binding == null)
            {
                InternalReleaseEvent(evt);
                return;
            }

            var callbacks = (binding.Value as object[]).Clone() as object[];
            if (callbacks == null)
            {
                InternalReleaseEvent(evt);
                return;
            }

            for (int i = 0; i < callbacks.Length; i++)
            {
                var callback = callbacks[i];
                if (callback == null) continue;

                callbacks[i] = null;

                var curCallback = binding.Value as object[];
                if (Array.IndexOf(curCallback, callback) == -1)
                    continue;
                if (callback is EventCallback evtCallback)
                {
                    InvokeEventCallback(evt, evtCallback);
                }
                else if (callback is EmptyCallback emptyCallback)
                {
                    emptyCallback();
                }
            }

            InternalReleaseEvent(evt);
        }

        #endregion

        #region ITriggerable Implementation

        public bool Trigger<T>(object data)
        {
            return Trigger(typeof(T), data);
        }

        public bool Trigger(object key, object data)
        {
            bool allow = ((data is IEvent && System.Object.ReferenceEquals((data as IEvent).Target, this) == false) ||
                (key is IEvent && System.Object.ReferenceEquals((data as IEvent).Target, this) == false));

            if (allow)
            {
                Dispatch(key, data);
            }
            return true;
        }

        #endregion

        #region IEventDispatcher Implementation

        public void AddListener(object evt, EventCallback callback)
        {
            IBinding binding = GetBinding(evt);
            if (binding == null)
            {
                Bind(evt).ToValue(callback);
            }
            else
            {
                binding.ToValue(callback);
            }
        }

        public void AddListener(object evt, EmptyCallback callback)
        {
            IBinding binding = GetBinding(evt);
            if (binding == null)
            {
                Bind(evt).ToValue(callback);
            }
            else
            {
                binding.ToValue(callback);
            }
        }

        public bool HasListener(object evt, EventCallback callback)
        {
            var binding = GetBinding(evt) as IEventBinding;
            if (binding == null)
            {
                return false;
            }
            return binding.TypeForCallback(callback) != EventCallbackType.NOT_FOUND;
        }

        public bool HasListener(object evt, EmptyCallback callback)
        {
            var binding = GetBinding(evt) as IEventBinding;
            if (binding == null)
            {
                return false;
            }
            return binding.TypeForCallback(callback) != EventCallbackType.NOT_FOUND;
        }

        public void RemoveListener(object evt, EventCallback callback)
        {
            RemoveDelegateListener(evt, callback);
        }

        public void RemoveListener(object evt, EmptyCallback callback)
        {
            RemoveDelegateListener(evt, callback);
        }

        private void RemoveDelegateListener(object evt, Delegate callback)
        {
            IBinding binding = GetBinding(evt);
            RemoveValue(binding, callback);
        }

        public void ReleaseEvent(IEvent evt)
        {
            if (evt is IPoolable poolable)
            {
                if (!poolable.IsRetain)
                {
                    CleanEvent(evt);
                    eventPool.ReturnInstance(evt);
                }
            }
        }

        private void FlushRemoval()
        {
            if (triggerClientRemovals == null)
            {
                return;
            }
            foreach (var target in triggerClientRemovals)
            {
                if (triggerClients.Contains(target))
                {
                    triggerClients.Remove(target);
                }
            }
            triggerClientRemovals.Clear();
        }

        private void InvokeEventCallback(IEvent evt, EventCallback callback)
        {
            try
            {
                callback(evt);
            }
            catch (InvalidCastException)
            {
                object target = callback.Target;
                var methodName = callback.Method.Name;
                string message = "An EventCallback is attempting an illegal cast. One possible reason is not typing the payload to IEvent in your callback. Another is illegal casting of the data.\nTarget class: " + target + " method: " + methodName;
                throw new EventDispatcherException(message, EventDispatcherExceptionType.TARGET_INVOCATION);
            }
        }

        private void InternalReleaseEvent(IEvent evt)
        {
            if (evt is IPoolable poolable)
            {
                poolable.Release();
            }
        }

        private IEvent ConformDataToEvent(object eventType, object data)
        {
            IEvent retVal = null;
            if (eventType == null)
            {
                throw new EventDispatcherException("Attempt to Dispatch to null.\ndata: " + data, EventDispatcherExceptionType.EVENT_KEY_NULL);
            }
            else if (eventType is IEvent)
            {
                //Client provided a full-formed event
                retVal = (IEvent)eventType;
            }
            else if (data == null)
            {
                //Client provided just an event ID. Create an event for injection
                retVal = CreateEvent(eventType, null);
            }
            else if (data is IEvent)
            {
                //Client provided both an evertType and a full-formed IEvent
                retVal = (IEvent)data;
            }
            else
            {
                //Client provided an eventType and some data which is not a IEvent.
                retVal = CreateEvent(eventType, data);
            }
            return retVal;
        }

        private IEvent CreateEvent(object eventType, object value)
        {
            var retVal = eventPool.GetInstance();
            retVal.Type = eventType;
            retVal.Target = this;
            retVal.Data = value;
            return retVal;
        }

        private void CleanEvent(IEvent evt)
        {
            evt.Target = null;
            evt.Data = null;
            evt.Type = null;
        }

        #endregion

    }
}