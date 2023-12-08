using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class CrossContext : Context, ICrossContextCapable
    {
        private List<IContext> _contexts;

        public new ICrossContextInjectionBinder InjectionBinder
        {
            get;
        }

        public IDispatcher CrossContextDispatcher { get; private set; }

        
        
        public CrossContext()
        {
            InjectionBinder = new CrossContextInjectionBinder();
        }
        
        
        
        public override void AddContext(IContext context)
        {
            base.AddContext(context);
            if (context is ICrossContextCapable crossContext)
            {
                AssignCrossContext(crossContext);
            }
        }
        public override void RemoveContext(IContext context)
        {
            if (context is ICrossContextCapable crossContext)
            {
                RemoveCrossContext(crossContext);
            }
            base.RemoveContext(context);
        }

        private void AssignCrossContext(ICrossContextCapable childContext)
        {
            var crossContext = (CrossContext)childContext;
            // crossContext.CrossContextDispatcher = CrossContextDispatcher;
            crossContext.InjectionBinder.CrossContextBinder = InjectionBinder.CrossContextBinder;
        }

        private void RemoveCrossContext(ICrossContextCapable childContext)
        {
            if (childContext.CrossContextDispatcher != null)
            {
                var crossContext = (CrossContext)childContext;
                // ((crossContext.CrossContextDispatcher) as ITriggerProvider)?.RemoveTriggerable(childContext.GetComponent<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER) as ITriggerable);
                crossContext.CrossContextDispatcher = null;
            }
        }
    }
}
