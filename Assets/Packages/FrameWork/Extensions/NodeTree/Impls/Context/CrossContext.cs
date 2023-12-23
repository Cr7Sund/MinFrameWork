using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class CrossContext : Context, ICrossContext
    {
        private List<IContext> _contexts;
        protected ICrossContextInjectionBinder _crossContextInjectionBinder;

        public override IInjectionBinder InjectionBinder
        {
            get
            {
                return _crossContextInjectionBinder;
            }
        }

        
        public CrossContext()
        {
            _contexts = new List<IContext>();
            _crossContextInjectionBinder = new CrossContextInjectionBinder();
        }

        public override void AddContext(IContext context)
        {
            base.AddContext(context);
            if (context is ICrossContext  crossContext)
            {
                AssignCrossContext(crossContext);
            }
        }
        public override void RemoveContext(IContext context)
        {
            if (context is ICrossContext crossContext)
            {
                RemoveCrossContext(crossContext);
            }
            base.RemoveContext(context);
        }

        private void AssignCrossContext(ICrossContext childContext)
        {
            AssertUtil.NotNull(_crossContextInjectionBinder.CrossContextBinder, NodeTreeExceptionType.EMPTY_CROSS_CONTEXT);
            if (childContext.InjectionBinder is CrossContextInjectionBinder crossContextInjectionBinder)
            {
                crossContextInjectionBinder.CrossContextBinder = _crossContextInjectionBinder.CrossContextBinder;
            }
        }

        private void RemoveCrossContext(ICrossContext childContext)
        {

        }


    }
}
