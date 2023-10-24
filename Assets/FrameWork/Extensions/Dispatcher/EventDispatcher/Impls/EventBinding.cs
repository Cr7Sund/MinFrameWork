using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using log4net.DateFormatter;

namespace Cr7Sund.Framework.Impl
{
    public class EventBinding : Binding, IEventBinding
    {
        private Dictionary<Delegate, EventCallbackType> callbackTypes;

        public EventBinding() : this(null)
        {
        }

        public EventBinding(Impl.Binder.BindingResolver resolver) : base(resolver)
        {
            ValueConstraint = BindingConstraintType.MANY;
            KeyConstraint = BindingConstraintType.ONE;

            callbackTypes = new Dictionary<Delegate, EventCallbackType>();
        }

        #region  IBinding Implementation
        IEventBinding IEventBinding.Bind(object key)
        {
            return base.Bind(key) as IEventBinding;
        }

        public IEventBinding ToValue(EventCallback value)
        {
            return ToDelegateValue(value);
        }

        public IEventBinding ToValue(EmptyCallback value)
        {
            return ToDelegateValue(value);
        }

        private IEventBinding ToDelegateValue(Delegate value)
        {
            base.ToValue(value);
            StoreMethodType(value as Delegate);
            return this;
        }

        public override void RemoveValue(object value)
        {
            base.RemoveValue(value);
            callbackTypes.Remove(value as Delegate);
        }

        private void StoreMethodType(Delegate value)
        {
            if (value == null)
            {
                throw new DispatcherException("EventDispatcher can't map something that isn't a delegate'", DispatcherExceptionType.ILLEGAL_CALLBACK_HANDLER);
            }

            var methodInfo = value.Method;
            int argsLen = methodInfo.GetParameters().Length;
            switch (argsLen)
            {
                case 0:
                    callbackTypes[value] = EventCallbackType.NO_ARGUMENTS;
                    break;
                case 1:
                    callbackTypes[value] = EventCallbackType.ONE_ARGUMENT;
                    break;
                default:
                    throw new DispatcherException("Event callbacks must have either one or no arguments", DispatcherExceptionType.ILLEGAL_CALLBACK_HANDLER);
            }
        }
       
        #endregion

        #region  IEventBinding Implementation

        public EventCallbackType TypeForCallback(EventCallback callback)
        {
            if (callbackTypes.ContainsKey(callback))
            {
                return callbackTypes[callback];
            }
            return EventCallbackType.NOT_FOUND;
        }

        public EventCallbackType TypeForCallback(EmptyCallback callback)
        {
            if (callbackTypes.ContainsKey(callback))
            {
                return callbackTypes[callback];
            }
            return EventCallbackType.NOT_FOUND;
        }


        #endregion
    }
}