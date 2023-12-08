using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
namespace Cr7Sund.Framework.Impl
{
    public class EventBinding : Binding, IEventBinding
    {
        private readonly Dictionary<Delegate, EventCallbackType> callbackTypes;


        public EventBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            ValueConstraint = BindingConstraintType.MANY;

            callbackTypes = new Dictionary<Delegate, EventCallbackType>();
        }

        // only for unit-test binding
        public EventBinding() : this(null)
        {

        }

        #region IBinding Implementation
        IEventBinding IEventBinding.Bind(object key)
        {
            return base.Bind(key) as IEventBinding;
        }

        public IEventBinding To(EventCallback value)
        {
            return ToDelegateValue(value);
        }

        public IEventBinding To(EmptyCallback value)
        {
            return ToDelegateValue(value);
        }

        private IEventBinding ToDelegateValue(Delegate value)
        {
            base.To(value);
            StoreMethodType(value);
            return this;
        }

        public override void RemoveValue(object value)
        {
            base.RemoveValue(value);
            callbackTypes.Remove(value as Delegate);
        }

        private void StoreMethodType(Delegate value)
        {
            AssertUtil.NotNull(value, DispatcherExceptionType.ILLEGAL_CALLBACK_HANDLER);

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
                    throw new MyException(DispatcherExceptionType.OUT_OF_ARGUMENT_EVENT);
            }
        }
        #endregion

        #region IEventBinding Implementation
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
