using System.Dynamic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class CrossContext : Context, ICrossContext
    {
        /// A specific instance of EventDispatcher that communicates 
        /// across multiple contexts. An event sent across this 
        /// dispatcher will be re-dispatched by the various context-wide 
        /// dispatchers. So a dispatch to other contexts is simply 
        /// 
        /// `crossContextDispatcher.Dispatch(MY_EVENT, payload)`;
        /// 
        /// Other contexts don't need to listen to the cross-context dispatcher
        /// as such, just map the necessary event to your local context
        /// dispatcher and you'll receive it.
        protected IEventDispatcher _crossContextDispatcher;

        private ICrossContextInjectionBinder _injectionBinder;
  
        public CrossContext(object view) : base(view)
        { }

        public CrossContext(object view, ContextStartupFlags flags) : base(view, flags)
        { }

        public CrossContext(object view, bool autoMapping) : base(view, autoMapping)
        { }

        #region  IContext Implementation
        protected override void AddCoreComponents()
        {
            base.AddCoreComponents();

            if (InjectionBinder.CrossContextBinder == null) // Only null if it could not find a parent context/ firstContext
            {
                InjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
            }

            if (FirstContext == this)
            {
                InjectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>().AsSingleton().ToName(ContextKeys.CROSS_CONTEXT_DISPATCHER).CrossContext();
            }
        }

        protected override void InstantiateCoreComponents()
        {
            base.InstantiateCoreComponents();

            var dispatcherBinding = InjectionBinder.GetBinding<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER);

            if (dispatcherBinding != null)
            {
                var dispatcher = InjectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER);

                if (dispatcher != null)
                {
                    CrossContextDispatcher = InjectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CROSS_CONTEXT_DISPATCHER);
                    CrossContextDispatcher.AddTriggerable(dispatcher);
                }
            }
        }

        public override IContext AddContext(IContext context)
        {
            base.AddContext(context);
            if (context is ICrossContext crossContext)
            {
                AssignCrossContext(crossContext);
            }

            return this;
        }

        public override IContext RemoveContext(IContext context)
        {
            if (context is ICrossContext crossContext)
            {
                RemoveCrossContext(crossContext);
            }

            return base.RemoveContext(context);
        }

        #endregion

        #region  ICrossContext Implementation

        public IEventDispatcher CrossContextDispatcher
        {
            get => _crossContextDispatcher;
            set => _crossContextDispatcher = value;
        }

        /// A Binder that handles dependency injection binding and instantiation
        public ICrossContextInjectionBinder InjectionBinder
        {
            get => _injectionBinder ?? (_injectionBinder = new CrossContextInjectionBinder());
            set => _injectionBinder = value;
        }

        public void AssignCrossContext(ICrossContext childContext)
        {
            childContext.CrossContextDispatcher = CrossContextDispatcher;
            childContext.InjectionBinder.CrossContextBinder = InjectionBinder.CrossContextBinder;
        }

        public void RemoveCrossContext(ICrossContext childContext)
        {
            if (childContext.CrossContextDispatcher != null)
            {
                childContext.CrossContextDispatcher.RemoveTriggerable(childContext.GetComponent<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as ITriggerable);
                childContext.CrossContextDispatcher = null;
            }
        }

        public virtual object GetComponent<T>()
        {
            return null;
        }

        public virtual object GetComponent<T>(object name)
        {
            return null;
        }

        #endregion
    }
}